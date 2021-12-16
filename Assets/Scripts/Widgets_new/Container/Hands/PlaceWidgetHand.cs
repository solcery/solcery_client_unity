using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Container.Stacks;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public sealed class PlaceWidgetHand : PlaceWidgetHand<PlaceWidgetHandLayout>
    {
        private class FakeCardWrapper
        {
            public bool Available => _available;
            public Vector3 WorldPosition => _fakeCard.transform.position;
            
            private bool _available;
            private GameObject _fakeCard;

            public static FakeCardWrapper Create(GameObject fakeCard)
            {
                return new FakeCardWrapper(fakeCard);
            }

            private FakeCardWrapper(GameObject fakeCard)
            {
                _available = true;
                _fakeCard = fakeCard;
            }

            public void UpdateAvailable(bool available)
            {
                _available = available;
            }

            public void UpdateActive(bool active)
            {
                _fakeCard.SetActive(active);
            }
        }
        
        private List<FakeCardWrapper> _fakeCards;
        private bool _firstUpdate;

        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetHand(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            PreloadFakeCards();
            _firstUpdate = true;
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateFirstUpdate(_firstUpdate, true);
            _firstUpdate = false;
            base.Update(world, entityIds);
        }

        private void PreloadFakeCards()
        {
            _fakeCards = new List<FakeCardWrapper>(Layout.FakeCardList.Count);
            foreach (var gameObject in Layout.FakeCardList)
            {
                _fakeCards.Add(FakeCardWrapper.Create(gameObject));
            }
        }

        public override void Destroy()
        {
            while (_fakeCards.Count > 0)
            {
                _fakeCards.RemoveAt(0);
            }
            
            base.Destroy();
        }

        protected override int PopFreeCardIndex()
        {
            foreach (var fakeCard in _fakeCards)
            {
                if (fakeCard.Available)
                {
                    fakeCard.UpdateAvailable(false);
                    return _fakeCards.IndexOf(fakeCard);
                }
            }

            return -1;
        }

        protected override Vector3 WorldPositionForCardIndex(int cardIndex)
        {
            if (cardIndex >= 0 && cardIndex < _fakeCards.Count)
            {
                Debug.Log($"WorldPositionForCardIndex {cardIndex} position {_fakeCards[cardIndex].WorldPosition}");
                return _fakeCards[cardIndex].WorldPosition;
            }
            
            return Layout.Transform.position;
        }

        protected override void UpdateAvailableFakeCardForCardIndex(int cardIndex, bool available)
        {
            if (cardIndex >= 0 && cardIndex < _fakeCards.Count)
            {
                _fakeCards[cardIndex].UpdateAvailable(available);
            }
        }

        protected override void UpdateActiveFakeCardForCardIndex(int cardIndex, bool active)
        {
            if (cardIndex >= 0 && cardIndex < _fakeCards.Count)
            {
                _fakeCards[cardIndex].UpdateActive(active);
            }
        }

        protected override void OnRemoveCard(ICardInContainerWidget cardInContainerWidget)
        {
            UpdateAvailableFakeCardForCardIndex(cardInContainerWidget.CardIndex, true);
            UpdateActiveFakeCardForCardIndex(cardInContainerWidget.CardIndex, true);
        }
    }

    public interface IPlaceWidgetCardPositionForObjectId
    {
        Vector3 WorldPositionForObjectId(int objectId);
    }

    public class PlaceWidgetHand<T> : PlaceWidget<T>, IPlaceWidgetCardPositionForObjectId where T : PlaceWidgetHandLayout
    {
        private Dictionary<int, ICardInContainerWidget> _cards;
        
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
                
                if (Game.PlaceWidgetFactory.CardInContainerPool.TryPop(out var cardInContainerWidget))
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
            bool highlighted = false;
            
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

            if (oldWidget is PlaceWidgetHand or PlaceWidgetStack)
            {
                cardInContainerWidget.UpdateParent(Layout.Transform);
                var from = oldWidget.GetPosition();

                if (objectIdPool.Has(entityId) 
                    && oldWidget is IPlaceWidgetCardPositionForObjectId placeWidgetCardPositionForObjectId)
                {
                    from = placeWidgetCardPositionForObjectId.WorldPositionForObjectId(objectIdPool.Get(entityId).Id);
                }

                var freeCardIndex = PopFreeCardIndex();
                var to = WorldPositionForCardIndex(freeCardIndex);

                cardInContainerWidget.CardIndex = freeCardIndex;
                cardInContainerWidget.Move(from, to, widget =>
                {
                    UpdateActiveFakeCardForCardIndex(widget.CardIndex, false);
                    widget.UpdateParent(Layout.Content);
                    widget.UpdateSiblingIndex(widget.CardIndex);
                    widget.UpdateHighlighted(highlighted);
                });
                
                cardInContainerWidget.UpdateCardFace(CardFace, oldWidget.GetPlaceWidgetCardFace() != CardFace);
            }
            else
            {
                cardInContainerWidget.UpdateParent(Layout.Content);
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
                OnRemoveCard(_cards[key]);
                Game.PlaceWidgetFactory.CardInContainerPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }

        protected virtual int PopFreeCardIndex()
        {
            return -1;
        }

        protected virtual Vector3 WorldPositionForCardIndex(int cardIndex)
        {
            return Layout.Content.position;
        }
        
        protected virtual void UpdateAvailableFakeCardForCardIndex(int cardIndex, bool available) { }

        protected virtual void UpdateActiveFakeCardForCardIndex(int cardIndex, bool active) { }
        
        protected virtual void OnRemoveCard(ICardInContainerWidget cardInContainerWidget) { }
        
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