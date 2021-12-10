using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets.Canvas;

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
    }

    public class PlaceWidgetHand<T> : PlaceWidget<T> where T : PlaceWidgetHandLayout
    {
        private List<ICardInContainerWidget> _cards;
        
        protected PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new List<ICardInContainerWidget>();
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            RemoveCards();
            
            if (entityIds.Length <= 0)
            {
                return;
            }

            var cardFaceVisible = CardFace != PlaceWidgetCardFace.Down;

            Action<int, EcsPool<ComponentObjectType>, EcsPool<ComponentObjectId>, Dictionary<int, JObject>,
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
                if (Game.PlaceWidgetFactory.CardInContainerPool.TryPop(out var cardInContainerWidget))
                {
                    cardUpdater.Invoke(entityId, objectTypePool, objectIdPool, cardTypes, cardInContainerWidget); 
                    _cards.Add(cardInContainerWidget);
                }
            }
        }
        
        private void CardFaceHideUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            EcsPool<ComponentObjectId> objectIdPool,
            Dictionary<int, JObject> cardTypes, 
            ICardInContainerWidget cardInContainerWidget)
        {
            cardInContainerWidget.UpdateParent(Layout.Content);
            cardInContainerWidget.UpdateCardFace(CardFace);
            cardInContainerWidget.UpdateInteractable(InteractableForActiveLocalPlayer);
        }

        private void CardFaceShowUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            EcsPool<ComponentObjectId> objectIdPool,
            Dictionary<int, JObject> cardTypes,
            ICardInContainerWidget cardInContainerWidget)
        {
            CardFaceHideUpdater(entityId, objectTypePool, objectIdPool, cardTypes, cardInContainerWidget);
            
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
                cardInContainerWidget.Destroy();
            }
            _cards.Clear();
            _cards = null;
        }

        private void RemoveCards()
        {
            while (_cards.Count > 0)
            {
                var cardInContainerWidget = _cards[0];
                _cards.Remove(cardInContainerWidget);
                Game.PlaceWidgetFactory.CardInContainerPool.Push(cardInContainerWidget);
            }
        }
    }
}