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
    public sealed class PlaceWidgetEclipse : PlaceWidget<PlaceWidgetEclipseLayout>, IApplyDropWidget
    {
        private Dictionary<int, IEclipseCardInContainerWidget> _cards;

        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipse(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetEclipse(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, IEclipseCardInContainerWidget>();
            Layout.UpdateVisible(true);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            if (entityIds.Length <= 0)
            {
                Layout.UpdateOutOfBorder(true);
                return;
            }

            Debug.Log("PlaceWidgetEclipse");
            Layout.UpdateOutOfBorder(false);

            RemoveCards(world, entityIds);
            
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

                    var eid = world.NewEntity();
                    world.GetPool<ComponentDragDropTag>().Add(eid);
                    world.GetPool<ComponentDragDropView>().Add(eid).View = eclipseCard;
                    world.GetPool<ComponentDragDropSourcePlaceEntityId>().Add(eid).SourcePlaceEntityId = Layout.LinkedEntityId;
                    world.GetPool<ComponentDragDropEclipseCardType>().Add(eid).CardType = 
                        world.GetPool<ComponentEclipseCardType>().Has(entityId) 
                            ? world.GetPool<ComponentEclipseCardType>().Get(entityId).CardType
                            : EclipseCardTypes.None;
                    eclipseCard.UpdateAttachEntityId(eid);
                    
                    Layout.AddCard(eclipseCard);
                    _cards.Add(objectId, eclipseCard);
                }
            }
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
        
        #region IApplyDropWidget

        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            if (dropWidget is IEclipseCardInContainerWidget ew)
            {
                Layout.AddCard(ew);
                _cards.Add(-100, ew);
            }
            
            Debug.Log("OnDropWidget");
        }

        #endregion
    }
}