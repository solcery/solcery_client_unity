using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.Nft.Card;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Nft.Selector
{
    public sealed class PlaceWidgetEclipseNftSelector : PlaceWidget<PlaceWidgetEclipseNftSelectorLayout>, IApplyDragWidget, IApplyDropWidget
    {
        private readonly HashSet<int> _dropObjectId;
        private readonly Dictionary<int, IEclipseCardNftInContainerWidget> _cards;
        private readonly bool _defaultBlockRaycasts;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipseNftSelector(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseNftSelector(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _dropObjectId = new HashSet<int>();
            _cards = new Dictionary<int, IEclipseCardNftInContainerWidget>();
            Layout.UpdateVisible(true);
            Layout.SetAnchor(TextAnchor.MiddleLeft);
            _defaultBlockRaycasts = Layout.BlockRaycasts;
        }

        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            Layout.Wait(false);
            RemoveCards(world, entityIds);
            Layout.UpdateBlocksRaycasts(_defaultBlockRaycasts);
            Layout.gameObject.SetActive(isVisible);

            if (entityIds.Length <= 0 || !isVisible)
            {
                return;
            }
            
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var itemTypes = Game.ServiceGameContent.ItemTypes;

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                var eclipseCardType = eclipseCartTypePool.Get(entityId).CardType;
                var tplId = world.GetPool<ComponentObjectType>().Get(entityId).TplId;

                if (eclipseCardType != EclipseCardTypes.Nft)
                {
                    continue;
                }
                
                if (!_cards.TryGetValue(objectId, out var eclipseCard))
                {
                    eclipseCard = AttachCard(world, entityId, tplId, objectId, itemTypes);
                    eclipseCard.Layout.UpdateAvailable(isAvailable);
                }

                // Update tplId
                if (tplId != eclipseCard.CardType)
                {
                    UpdateFromCardTypeData(world, entityId, tplId, objectId, itemTypes, eclipseCard);
                }
                        
                // // Update drag drop
                // if (_dropObjectId.Contains(objectId))
                // {
                //     _dropObjectId.Remove(objectId);
                //     //UpdateDragAndDrop(world, entityId, objectId, eclipseCard);
                // }
                // Layout.Wait(_dropObjectId.Count > 0);
                        
                UpdateCard(world, entityId, /*tplId, objectId, cardTypes,*/ eclipseCard);
            }
            
            UpdatedCardsOrder();
            _dropObjectId.Clear();
        }
        
        private void UpdateCard(EcsWorld world, int entityId, IEclipseCardNftInContainerWidget eclipseCard)
        {
            var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
            
            // order
            var order = attributes.TryGetValue("order", out var orderAttributeY) ? orderAttributeY.Current : 0;
            eclipseCard.SetOrder(order);
        }
        
        private IEclipseCardNftInContainerWidget AttachCard(EcsWorld world, int entityId, int cardType, int objectId, IItemTypes itemTypes)
        {
            if (world.GetPool<ComponentEclipseCardTag>().Has(entityId))
            {
                if (Game.EclipseCardNftInContainerWidgetPool.TryPop(out var eclipseCard))
                {
                    UpdateFromCardTypeData(world, entityId, cardType, objectId, itemTypes, eclipseCard);
                    UpdateDragAndDrop(world, entityId, objectId, eclipseCard);
                    PutCardToInPlace(objectId, eclipseCard);

                    return eclipseCard;
                }
            }

            return null;
        }
        
        private void PutCardToInPlace(int objectId, IEclipseCardNftInContainerWidget eclipseCard)
        {
            Layout.AddCard(eclipseCard);
            _cards.Add(objectId, eclipseCard);
            eclipseCard.Layout.SetActive(true);
        }

        void UpdateDragAndDrop(EcsWorld world, int entityId, int objectId, IEclipseCardNftInContainerWidget eclipseCard)
        {
            // Remove old attached entity
            if (eclipseCard.AttachEntityId != -1)
            {
                world.DelEntity(eclipseCard.AttachEntityId);
            }
            
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
        }
        
        private void UpdateFromCardTypeData(EcsWorld world, int entityId, int tplid, int objectId, IItemTypes itemTypes, IEclipseCardNftInContainerWidget eclipseCard)
        {
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            if (itemTypes.TryGetItemType(out var itemType, tplid))
            {
                eclipseCard.UpdateFromCardTypeData(entityId, objectId, tplid, eclipseCartTypePool.Get(entityId).CardType, itemType);
            }
        }
        
        private void UpdatedCardsOrder()
        {
            var cardsSorted = _cards.Values.OrderBy(card=>card.Order).ToList();
            for (var i = 0; i < cardsSorted.Count; i++)
            {
                cardsSorted[i].UpdateSiblingIndex(i);
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
                Game.EclipseCardNftInContainerWidgetPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }

        void IApplyDragWidget.OnDragWidget(IDraggableWidget dragWidget)
        {
            _cards.Remove(dragWidget.ObjectId);
        }

        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position, bool discard)
        {
            if (discard)
            {
                if (dropWidget is IEclipseCardNftInContainerWidget ew)
                {
                    Layout.AddCard(ew);
                    _cards.Add(dropWidget.ObjectId, ew);
                }
            }
            else
            {
                var isPredictable = Game.IsPredictable;
#if LOCAL_SIMULATION
                isPredictable = false;
#endif
                Layout.Wait(!isPredictable);

                if (dropWidget is IPoolingWidget pw)
                {
                    pw.BackToPool();
                }
            }
        }
    }
}