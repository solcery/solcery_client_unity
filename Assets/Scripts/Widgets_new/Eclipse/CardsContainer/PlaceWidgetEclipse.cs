using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public sealed class PlaceWidgetEclipse : PlaceWidget<PlaceWidgetEclipseLayout>, IApplyDragWidget, IApplyDropWidget, IPlaceWidgetTokenCollection
    {
        private readonly Dictionary<int, IEclipseCardInContainerWidget> _cards;
        private readonly Dictionary<int, List<int>> _tokensPerCardCache;
        
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
            _tokensPerCardCache = new Dictionary<int, List<int>>();
            Layout.UpdateVisible(true);
            Layout.SetAnchor(TextAnchor.MiddleLeft);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            RemoveCards(world, entityIds);

            if (entityIds.Length <= 0)
            {
                return;
            }

            var objectIdPool = world.GetPool<ComponentObjectId>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var cardTypes = world.GetCardTypes();

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                var eclipseCardType = eclipseCartTypePool.Get(entityId).CardType;
                
                switch (eclipseCardType)
                {
                    case EclipseCardTypes.Token:
                        PrepareToken(world, entityId);
                        break;
                    default:
                    {
                        if (!_cards.TryGetValue(objectId, out var eclipseCard))
                        {
                            eclipseCard = AttachCard(world, entityId, objectId, cardTypes);
                        }
                        UpdateCard(world, entityId, eclipseCard);
                        break;
                    }
                }
            }

            AttachTokensForCard(world, cardTypes);
            UpdatedCardsOrder(world);
            UpdateCardsAnimation(world);
        }

        private void UpdatedCardsOrder(EcsWorld world)
        {
            const string orderAttributeName = "order";
            var cardsSorted = _cards.Values.ToList();
            cardsSorted.Sort((x, y) => 
            {
                var attributesX = world.GetPool<ComponentObjectAttributes>().Get(x.EntityId).Attributes;
                var orderX = attributesX.TryGetValue(orderAttributeName, out var orderAttributeX) ? orderAttributeX.Current : 0;

                var attributesY = world.GetPool<ComponentObjectAttributes>().Get(x.EntityId).Attributes;
                var orderY = attributesY.TryGetValue(orderAttributeName, out var orderAttributeY) ? orderAttributeY.Current : 0;

                return orderX.CompareTo(orderY);
            });

            for (var i = 0; i < cardsSorted.Count; i++)
            {
                cardsSorted[i].UpdateSiblingIndex(i);
            }
        }

        private void UpdateCardsAnimation(EcsWorld world)
        {
            Layout.RebuildScroll();
            foreach (var eclipseCard in _cards.Values)
            {
                var attributes = world.GetPool<ComponentObjectAttributes>().Get(eclipseCard.EntityId).Attributes;

                if (attributes.TryGetValue("anim_card_fly", out var animTokenFlyAttribute) &&
                    animTokenFlyAttribute.Current > 0)
                {
                    var fromPlaceId = attributes.TryGetValue("anim_card_fly_from_place", out var fromPlaceAttribute)
                        ? fromPlaceAttribute.Current
                        : 0;
                    var from = world.GetPlaceWidget(fromPlaceId).GetPosition();
                    eclipseCard.Layout.SetActive(false);
                    WidgetCanvas.GetEffects().MoveEclipseCard(eclipseCard, 1f, from, () =>
                    {
                        eclipseCard.Layout.SetActive(true);
                    });
                }
                
                if (attributes.TryGetValue("anim_destroy", out var animDestroyAttribute) &&
                    animDestroyAttribute.Current > 0)
                {
                    AnimEclipseCardDestroy(eclipseCard);
                }
            }
        }

        private void PrepareToken(EcsWorld world, int entityId)
        {
            if (world.GetPool<ComponentEclipseTokenTag>().Has(entityId))
            {
                var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
                if (attributes.TryGetValue("linked_card_id", out var tokenCardIdAttribute) && tokenCardIdAttribute.Current > 0)
                {
                    var cardId = tokenCardIdAttribute.Current;
                    if (_tokensPerCardCache.TryGetValue(cardId, out var tokensEntities))
                    {
                        tokensEntities.Add(entityId);
                    }
                    else
                    {
                        _tokensPerCardCache.Add(cardId, new List<int> {entityId});
                    }
                }
            }
        }

        private void UpdateCard(EcsWorld world, int entityId, IEclipseCardInContainerWidget eclipseCard)
        {
            var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
            
            // timer
            var showTimer = attributes.TryGetValue("show_duration", out var showDurationAttribute) && showDurationAttribute.Current > 0;
            var timerDuration = attributes.TryGetValue("duration", out var durationAttribute) ? durationAttribute.Current : 0;
            eclipseCard.Layout.TimerLayout.gameObject.SetActive(showTimer);
            eclipseCard.Layout.TimerLayout.UpdateTimer(timerDuration);
            
            // tokens
            var tokenSlots = attributes.TryGetValue("token_slots", out var tokenSlotsAttribute) ? tokenSlotsAttribute.Current : 0;
            eclipseCard.Layout.TokensLayout.UpdateTokenSlots(tokenSlots);
            
            // anims
            var animHighlight = attributes.TryGetValue("anim_highlight", out var animHighlightAttribute) && animHighlightAttribute.Current > 0;
            eclipseCard.Layout.Highlight.SetActive(animHighlight);
        }

        private void AnimEclipseCardDestroy(IEclipseCardInContainerWidget eclipseCard)
        {
            var rttData = Game.ServiceRenderWidget.CreateWidgetRender(eclipseCard.Layout.RectTransform);
            if (rttData != null)
            {
                eclipseCard.Layout.SetActive(false);
                WidgetCanvas.GetEffects().DestroyEclipseCard(eclipseCard,
                    rttData,
                    0.5f,
                    () => { Game.ServiceRenderWidget.ReleaseWidgetRender(eclipseCard.Layout.RectTransform); });
            }
        }

        private IEclipseCardInContainerWidget AttachCard(EcsWorld world, int entityId, int objectId, Dictionary<int, JObject> cardTypes)
        {
            if (world.GetPool<ComponentEclipseCardTag>().Has(entityId))
            {
                var objectTypePool = world.GetPool<ComponentObjectType>();
                var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
                if (Game.EclipseCardInContainerWidgetPool.TryPop(out var eclipseCard))
                {
                    if (objectTypePool.Has(entityId)
                        && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject))
                    {
                        eclipseCard.UpdateFromCardTypeData(entityId, objectId, cardTypeDataObject);
                        eclipseCard.SetEclipseCardType(eclipseCartTypePool.Get(entityId).CardType);
                    }
                    
                    // drug and drop
                    AttachDragAndDrop(world, entityId, objectId, eclipseCard);

                    PutCardToInPlace(objectId, eclipseCard);

                    return eclipseCard;
                }
            }

            return null;
        }

        private void PutCardToInPlace(int objectId, IEclipseCardInContainerWidget eclipseCard)
        {
            Layout.AddCard(eclipseCard);
            _cards.Add(objectId, eclipseCard);
            eclipseCard.Layout.SetActive(true);
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

        private void AttachTokensForCard(EcsWorld world, Dictionary<int, JObject> cardTypes)
        {
            foreach (var tokensPerCard in _tokensPerCardCache)
            {
                if (_cards.TryGetValue(tokensPerCard.Key, out var cardWidget))
                {
                    var objectTypePool = world.GetPool<ComponentObjectType>();
                    foreach (var entityId in tokensPerCard.Value)
                    {
                        if (objectTypePool.Has(entityId)
                            && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject))
                        {
                            var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
                            if (attributes.TryGetValue("slot", out var tokenSlotAttribute))
                            {
                                var tokenLayout = cardWidget.AttachToken(tokenSlotAttribute.Current, cardTypeDataObject);
                                if (tokenLayout != null)
                                {
                                    ProcessTokenAttributes(world, tokenLayout, attributes);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Can't attach tokens card_id = {tokensPerCard.Key}, place = {PlaceId}!");
                }
            }

            _tokensPerCardCache.Clear();
        }

        private void ProcessTokenAttributes(EcsWorld world, EclipseCardTokenLayout tokenLayout, Dictionary<string, IAttributeValue> attributes)
        {
            if (attributes.TryGetValue("anim_token_fly", out var animTokenFlyAttribute) && animTokenFlyAttribute.Current > 0)
            {
                var fromPlaceId = attributes.TryGetValue("anim_token_fly_from_place", out var fromPlaceAttribute) ? fromPlaceAttribute.Current : 0;
                var formCardId = attributes.TryGetValue("anim_token_fly_from_card_id", out var fromCardAttribute) ? fromCardAttribute.Current : 0;
                var fromSlotId = attributes.TryGetValue("anim_token_fly_from_slot", out var fromSlotAttribute) ? fromSlotAttribute.Current : 0;
                if (WidgetExtensions.TryGetTokenFromPosition(world, fromPlaceId, formCardId, fromSlotId, out var from))
                {
                    AnimTokenFly(tokenLayout, from);
                }
                else
                {
                    Debug.LogWarning($"Can't run token animation: anim_token_fly_from_place = {fromPlaceId}: anim_token_fly_from_card_id = {formCardId} and anim_token_fly_from_slot = {fromSlotId}");
                }
            }

            if (attributes.TryGetValue("anim_destroy", out var animDestroyAttribute) &&
                animDestroyAttribute.Current > 0)
            {
                AnimTokenDestroy(tokenLayout);
            }
        }

        private void AnimTokenFly(EclipseCardTokenLayout tokenLayout, Vector3 from)
        {
            tokenLayout.Icon.gameObject.SetActive(false);
            WidgetCanvas.GetEffects().MoveToken(tokenLayout.RectTransform, 
                tokenLayout.Icon.sprite,
                from,
                0.5f,
                () => { tokenLayout.Icon.gameObject.SetActive(true); });
        }

        private void AnimTokenDestroy(EclipseCardTokenLayout tokenLayout)
        {
            tokenLayout.Icon.gameObject.SetActive(false);
            WidgetCanvas.GetEffects().DestroyToken(tokenLayout.RectTransform,
                tokenLayout.Icon.sprite,
                0.5f,
                () => {});
        }

        public bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position)
        {
            if (_cards.TryGetValue(cardId, out var eclipseCard))
            {
                position = eclipseCard.GetTokenPosition(slotId);
                return true;
            }
            
            position = Vector3.zero;
            return false;
        }
        
        void AttachDragAndDrop(EcsWorld world, int entityId, int objectId, IEclipseCardInContainerWidget eclipseCard)
        {
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
        
        #region IApplyDropWidget
        
        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            //Debug.Log($"OnDrop Widget {dropWidget.ObjectId}");
            if (dropWidget is IEclipseCardInContainerWidget ew)
            {
                Layout.AddCard(ew);
                _cards.Add(dropWidget.ObjectId, ew);
            }
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