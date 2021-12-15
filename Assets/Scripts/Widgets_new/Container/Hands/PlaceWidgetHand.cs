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
using Object = UnityEngine.Object;

namespace Solcery.Widgets_new.Container.Hands
{
    public sealed class PlaceWidgetHand : PlaceWidgetHand<PlaceWidgetHandLayout>
    {
        private class FakeCardWrapper
        {
            public bool Available => _available;
            public Vector3 WorldPosition => _go.transform.position;
            
            private bool _available;
            private GameObject _go;

            public static FakeCardWrapper Create(GameObject prefab, Transform parent)
            {
                return new FakeCardWrapper(prefab, parent);
            }

            private FakeCardWrapper(GameObject prefab, Transform parent)
            {
                _available = true;
                _go = Object.Instantiate(prefab, parent);
            }

            public void UpdateAvailable(bool available)
            {
                _available = available;
            }

            public void UpdateActive(bool active)
            {
                _go.SetActive(active);
            }

            public void Destroy()
            {
                if (_go != null)
                {
                    Object.Destroy(_go);
                    _go = null;
                }
            }
        }
        
        private readonly List<FakeCardWrapper> _fakeCards;

        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetHand(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _fakeCards = new List<FakeCardWrapper>(3);
            game.ServiceResource.TryGetWidgetPrefabForKey("ui/ui_fake_card", out var fakeCardPrefab);
            PreloadFakeCards(fakeCardPrefab, 3);
        }

        private void PreloadFakeCards(GameObject fakeCardPrefab, int count)
        {
            for (var i = 0; i < count; i++)
            {
                _fakeCards.Add(FakeCardWrapper.Create(fakeCardPrefab,Layout.Content));
            }
        }

        public override void Destroy()
        {
            while (_fakeCards.Count > 0)
            {
                var go = _fakeCards[0];
                _fakeCards.RemoveAt(0);
                go.Destroy();
            }
            
            base.Destroy();
        }

        protected override int GetFreeCardIndex()
        {
            foreach (var fakeCard in _fakeCards)
            {
                if (fakeCard.Available)
                {
                    return _fakeCards.IndexOf(fakeCard);
                }
            }

            return -1;
        }

        protected override Vector3 WorldPositionForCardIndex(int cardIndex)
        {
            if (cardIndex >= 0 && cardIndex < _fakeCards.Count)
            {
                return _fakeCards[cardIndex].WorldPosition;
            }
            
            return Vector3.zero;
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
            if (objectAttributesPool.Has(entityId)
                && objectAttributesPool.Get(entityId).Attributes.TryGetValue("place", out var place))
            {
                if (place.Changed)
                {
                    oldWidget = GetPlaceWidget(place.Old, poolPlaceId, poolPlaceWidgetNew);
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

                var freeCardIndex = GetFreeCardIndex();
                var to = WorldPositionForCardIndex(freeCardIndex);

                cardInContainerWidget.CardIndex = freeCardIndex;
                UpdateAvailableFakeCardForCardIndex(freeCardIndex, false);
                cardInContainerWidget.Move(from, to, widget =>
                {
                    UpdateActiveFakeCardForCardIndex(widget.CardIndex, false);
                    widget.UpdateParent(Layout.Content);
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

        protected virtual int GetFreeCardIndex()
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
            
            return Vector3.zero;
        }
    }
}