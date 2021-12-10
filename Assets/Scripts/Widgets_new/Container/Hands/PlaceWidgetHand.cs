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
    public sealed class PlaceWidgetHand : PlaceWidget<PlaceWidgetHandLayout>
    {
        private Dictionary<int, ICardInContainerWidget> _cards;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetHand(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, ICardInContainerWidget>();
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            RemoveCards(entityIds);
            
            if (entityIds.Length <= 0)
            {
                return;
            }

            var cardFaceVisible = CardFace != PlaceWidgetCardFace.Down;
            
            Action<int, EcsPool<ComponentObjectType>, Dictionary<int, JObject>, ICardInContainerWidget> cardUpdater;
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
                if (_cards.ContainsKey(entityId))
                {
                    continue;
                }

                if (Game.PlaceWidgetFactory.CardInContainerPool.TryPop(out var cardInContainerWidget))
                {
                    cardUpdater.Invoke(entityId, objectTypePool, cardTypes, cardInContainerWidget); 
                    _cards.Add(entityId, cardInContainerWidget);
                }
            }
        }
        
        private void CardFaceHideUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            Dictionary<int, JObject> cardTypes, 
            ICardInContainerWidget cardInContainerWidget)
        {
            cardInContainerWidget.UpdateParent(Layout.Content);
            cardInContainerWidget.UpdateCardFace(CardFace);
            cardInContainerWidget.UpdateInteractable(InteractableForActiveLocalPlayer);
        }

        private void CardFaceShowUpdater(int entityId, 
            EcsPool<ComponentObjectType> objectTypePool, 
            Dictionary<int, JObject> cardTypes,
            ICardInContainerWidget cardInContainerWidget)
        {
            CardFaceHideUpdater(entityId, objectTypePool, cardTypes, cardInContainerWidget);
            
            if (objectTypePool.Has(entityId)
                && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject))
            {
                cardInContainerWidget.UpdateFromCardTypeData(cardTypeDataObject);
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

        private void RemoveCards(int[] entityIds)
        {
            foreach (var entityId in entityIds)
            {
                if (_cards.TryGetValue(entityId, out var cardInContainerWidget))
                {
                    _cards.Remove(entityId);
                    Game.PlaceWidgetFactory.CardInContainerPool.Push(cardInContainerWidget);
                }
            }
        }
    }
}