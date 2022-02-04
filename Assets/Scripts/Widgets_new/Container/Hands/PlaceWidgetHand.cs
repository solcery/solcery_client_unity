using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Container.Stacks;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public sealed class PlaceWidgetHand : PlaceWidgetHand<PlaceWidgetHandLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetHand(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
        }

        protected override Vector3 WorldLocalPositionForCardIndex(int cardIndex)
        {
            var width = Layout.Content.rect.width;
            var partWidth = width / CardsCount;
            var partX = partWidth / 2 + cardIndex * partWidth - width / 2;
            return new Vector3(partX, 0f, 0f);     
        }
    }

    public interface IPlaceWidgetCardPositionForObjectId
    {
        Vector3 WorldPositionForObjectId(int objectId);
    }

    public class PlaceWidgetHand<T> : PlaceWidget<T>, IPlaceWidgetCardPositionForObjectId where T : PlaceWidgetHandLayout
    {
        private Dictionary<int, ICardInContainerWidget> _cards;
        protected int CardsCount;
        
        protected PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, ICardInContainerWidget>();
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            RemoveCards(world, entityIds);
            if (entityIds.Length <= 0)
            {
                return;
            }

            CardsCount = entityIds.Length;
            var cardFaceVisible = CardFace != PlaceWidgetCardFace.Down;

            Action<int, 
                EcsPool<ComponentObjectType>, 
                EcsPool<ComponentObjectId>, 
                EcsPool<ComponentObjectAttributes>, 
                EcsPool<ComponentPlaceId>, 
                EcsPool<ComponentPlaceWidgetNew>, 
                Dictionary<int, JObject>,
                ICardInContainerWidget> cardUpdater;
            if (cardFaceVisible)
            {
                cardUpdater = CardFaceShowUpdater;
            }
            else
            {
                cardUpdater = CardFaceHideUpdater;
            }
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectAttributesPool = world.GetPool<ComponentObjectAttributes>();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
            var cardTypes = new Dictionary<int, JObject>();

            if (cardFaceVisible)
            {
                foreach (var objectTypesEntityId in objectTypesFilter)
                {
                    cardTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                    break;
                }
            }

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;

                if (_cards.ContainsKey(objectId))
                {
                    continue;
                }
                
                if (Game.CardInContainerWidgetPool.TryPop(out var cardInContainerWidget))
                {
                    cardUpdater.Invoke(entityId, objectTypePool, objectIdPool, objectAttributesPool, poolPlaceId, poolPlaceWidgetNew, cardTypes, cardInContainerWidget);
                    _cards.Add(objectId, cardInContainerWidget);
                }
            }
        }

        private PlaceWidget GetPlaceWidget(
            int placeId, 
            EcsPool<ComponentPlaceId> poolPlaceId,
            EcsPool<ComponentPlaceWidgetNew> poolPlaceWidgetNew)
        {
            PlaceWidget oldWidget = null; 
            var widgetFilter = poolPlaceId.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().Inc<ComponentPlaceWidgetNew>().End();
            foreach (var entityId in widgetFilter)
            {
                if (poolPlaceId.Get(entityId).Id == placeId)
                {
                    oldWidget = poolPlaceWidgetNew.Get(entityId).Widget;
                }
            }

            return oldWidget;
        }

        private void CardFaceHideUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            EcsPool<ComponentObjectId> objectIdPool,
            EcsPool<ComponentObjectAttributes> objectAttributesPool,
            EcsPool<ComponentPlaceId> poolPlaceId,
            EcsPool<ComponentPlaceWidgetNew> poolPlaceWidgetNew,
            Dictionary<int, JObject> cardTypes, 
            ICardInContainerWidget cardInContainerWidget)
        {
            PlaceWidget oldWidget = null;
            var highlighted = false;
            
            if (objectAttributesPool.Has(entityId))
            {
                var attributes = objectAttributesPool.Get(entityId).Attributes;
                
                if (attributes.TryGetValue("place", out var place) && place.Changed)
                {
                    oldWidget = GetPlaceWidget(place.Old, poolPlaceId, poolPlaceWidgetNew);
                }

                if (attributes.TryGetValue("highlighted", out var highlightedValue))
                {
                    highlighted = highlightedValue.Current == 1;
                }
            }

            cardInContainerWidget.UpdateParent(Layout.Content);
            var cardIndex = _cards.Count;
            var localPosition = WorldLocalPositionForCardIndex(cardIndex);
            if (oldWidget is PlaceWidgetHand or PlaceWidgetStack)
            {
                var fromWorld = oldWidget.GetPosition();

                if (objectIdPool.Has(entityId) 
                    && oldWidget is IPlaceWidgetCardPositionForObjectId placeWidgetCardPositionForObjectId)
                {
                    fromWorld = placeWidgetCardPositionForObjectId.WorldPositionForObjectId(objectIdPool.Get(entityId).Id);
                }

                cardInContainerWidget.MoveLocal(fromWorld, localPosition, widget =>
                {
                    widget.UpdateSiblingIndex(cardIndex);
                    widget.UpdateHighlighted(highlighted);
                });
                
                cardInContainerWidget.UpdateCardFace(CardFace, oldWidget.GetPlaceWidgetCardFace() != CardFace);
            }
            else
            {
                cardInContainerWidget.SetLocalPosition(localPosition);
                cardInContainerWidget.UpdateSiblingIndex(cardIndex);
                cardInContainerWidget.UpdateHighlighted(highlighted);
                cardInContainerWidget.UpdateCardFace(CardFace, false);
            }

            cardInContainerWidget.UpdateInteractable(InteractableForActiveLocalPlayer);
        }

        private void CardFaceShowUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            EcsPool<ComponentObjectId> objectIdPool,
            EcsPool<ComponentObjectAttributes> objectAttributesPool,
            EcsPool<ComponentPlaceId> poolPlaceId,
            EcsPool<ComponentPlaceWidgetNew> poolPlaceWidgetNew,
            Dictionary<int, JObject> cardTypes,
            ICardInContainerWidget cardInContainerWidget)
        {
            CardFaceHideUpdater(entityId, objectTypePool, objectIdPool, objectAttributesPool, poolPlaceId, poolPlaceWidgetNew, cardTypes, cardInContainerWidget);
            
            if (objectTypePool.Has(entityId)
                && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject)
                && objectIdPool.Has(entityId))
            {
                cardInContainerWidget.UpdateFromCardTypeData(objectIdPool.Get(entityId).Id, cardTypeDataObject);
            }
        }
        
        protected override void DestroyImpl()
        {
            foreach (var cardInContainerWidget in _cards)
            {
                cardInContainerWidget.Value.Destroy();
            }
            _cards.Clear();
            _cards = null;
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
                Game.CardInContainerWidgetPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }
        
        protected virtual Vector3 WorldLocalPositionForCardIndex( int cardIndex)
        {
            return Vector3.zero;
        }

        Vector3 IPlaceWidgetCardPositionForObjectId.WorldPositionForObjectId(int objectId)
        {
            if (_cards.TryGetValue(objectId, out var cardInContainerWidget))
            {
                return cardInContainerWidget.WorldPosition;
            }
            
            return Layout.Content.position;
        }
    }
}