using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public sealed class PlaceWidgetEclipse : PlaceWidget<PlaceWidgetEclipseLayout>, IApplyDragWidget, IApplyDropWidget
    {
        private readonly Dictionary<int, IEclipseCardInContainerWidget> _cards;
        private readonly List<int> _tokensCache;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipse(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetEclipse(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, IEclipseCardInContainerWidget>();
            _tokensCache = new List<int>();
            Layout.UpdateVisible(true);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            RemoveCards(world, entityIds);
            
            if (entityIds.Length <= 0)
            {
                return;
            }

            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var cardTypes = new Dictionary<int, JObject>();

            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                cardTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                break;
            }

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;

                if (_cards.ContainsKey(objectId))
                {
                    continue;
                }

                if (world.GetPool<ComponentEclipseTokenTag>().Has(entityId))
                {
                    _tokensCache.Add(entityId);
                    continue;
                }

                if (Game.EclipseCardInContainerWidgetPool.TryPop(out var eclipseCard))
                {
                    if (objectTypePool.Has(entityId)
                        && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject)
                        && objectIdPool.Has(entityId))
                    {
                        eclipseCard.UpdateFromCardTypeData(objectIdPool.Get(entityId).Id, cardTypeDataObject);
                    }

                    var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
                    var showTimer = attributes.TryGetValue("show_duration", out var showDuration) &&
                                    showDuration.Current > 0;
                    var timerDuration = attributes.TryGetValue("duration", out var duration) ? duration.Current : 0;
                    eclipseCard.UpdateTimer(showTimer, timerDuration);

                    var eid = world.NewEntity();
                    world.GetPool<ComponentDragDropTag>().Add(eid);
                    world.GetPool<ComponentDragDropView>().Add(eid).View = eclipseCard;
                    world.GetPool<ComponentDragDropSourcePlaceEntityId>().Add(eid).SourcePlaceEntityId =
                        Layout.LinkedEntityId;
                    world.GetPool<ComponentDragDropEclipseCardType>().Add(eid).CardType =
                        world.GetPool<ComponentEclipseCardType>().Has(entityId)
                            ? world.GetPool<ComponentEclipseCardType>().Get(entityId).CardType
                            : EclipseCardTypes.None;
                    world.GetPool<ComponentDragDropObjectId>().Add(eid).ObjectId = objectId;
                    eclipseCard.UpdateAttachEntityId(eid);

                    Layout.AddCard(eclipseCard);
                    _cards.Add(objectId, eclipseCard);
                }
            }

            ApplyTokensForCard(world, cardTypes);
        }

        private void RemoveCards(EcsWorld world, int[] entityIds)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var keys = _cards.Keys.ToList();

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                keys.Remove(objectId);
            }

            foreach (var key in keys)
            {
                var eid = _cards[key].AttachEntityId;
                if (eid >= 0)
                {
                    world.DelEntity(eid);
                }

                _cards[key].UpdateAttachEntityId();
                Game.EclipseCardInContainerWidgetPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }

        private void ApplyTokensForCard(EcsWorld world, Dictionary<int, JObject> cardTypes)
        {
            var card = _cards.Values.FirstOrDefault();
            if (card != null)
            {
                var objectTypePool = world.GetPool<ComponentObjectType>();
                var counter = 0;
                card.ClearTokens();
                foreach (var entityId in _tokensCache)
                {
                    if (objectTypePool.Has(entityId)
                        && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject))
                    {
                        card.AttachToken(counter, cardTypeDataObject);
                        counter++;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Can't attach tokens for eclipse card!");
            }

            _tokensCache.Clear();
        }

        #region IApplyDropWidget

        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            //Debug.Log($"OnDrop Widget {dropWidget.ObjectId}");
            if (dropWidget is not IEclipseCardInContainerWidget ew)
            {
                return;
            }

            Layout.AddCard(ew);
            _cards.Add(dropWidget.ObjectId, ew);
        }

        #endregion

        #region IApplyDragWidget

        void IApplyDragWidget.OnDragWidget(IDraggableWidget dragWidget)
        {
            //Debug.Log($"OnDrag Widget {dragWidget.ObjectId}");
            _cards.Remove(dragWidget.ObjectId);
        }

        #endregion
    }
}