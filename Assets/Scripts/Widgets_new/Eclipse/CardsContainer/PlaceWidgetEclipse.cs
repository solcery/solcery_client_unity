using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Models.Shared.Places;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Effects;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public enum EventTrackerLayout
    {
        Horizontal = 0,
        Vertical = 1
    }
    
    public class PlaceWidgetEclipse<T> : PlaceWidget<T>, IApplyDragWidget, IApplyDropWidget, IPlaceWidgetTokenCollection where T : PlaceWidgetEclipseLayoutBase
    {
        private readonly HashSet<int> _dropObjectId;
        protected readonly Dictionary<int, IEclipseCardInContainerWidget> _cards;
        private readonly Dictionary<int, List<int>> _tokensPerCardCache;
        private readonly bool _defaultBlockRaycasts;
        protected IWidgetPool<IEclipseCardInContainerWidget> CardsPool;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipse<T>(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        protected PlaceWidgetEclipse(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _dropObjectId = new HashSet<int>();
            _cards = new Dictionary<int, IEclipseCardInContainerWidget>();
            _tokensPerCardCache = new Dictionary<int, List<int>>();
            Layout.UpdateVisible(true);
            var eventTrackerLayout = placeDataObject.TryGetEnum("event_tracker_layout", out EventTrackerLayout res)
                ? res
                : EventTrackerLayout.Horizontal;
            Layout.SetLayout(eventTrackerLayout, TextAnchor.MiddleLeft);
            _defaultBlockRaycasts = Layout.BlockRaycasts;
            CardsPool = Game.EclipseCardInContainerWidgetPool;
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

            var objectAttributesPool = world.GetPool<ComponentObjectAttributes>();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var itemTypes = Game.ServiceGameContent.ItemTypes;

            foreach (var entityId in entityIds)
            {
                // TODO: удалить старый механизм блокировки рейкастов!
                {
                    var attributes = objectAttributesPool.Has(entityId)
                        ? objectAttributesPool.Get(entityId).Attributes
                        : new Dictionary<string, IAttributeValue>();

                    if (attributes.ContainsKey("disable_raycasts_on_place")
                        && attributes["disable_raycasts_on_place"].Current > 0)
                    {
                        Layout.UpdateBlocksRaycasts(false);
                        continue;
                    }
                }

                var objectId = objectIdPool.Get(entityId).Id;
                if (!eclipseCartTypePool.Has(entityId))
                {
                    continue;
                }

                var eclipseCardType = eclipseCartTypePool.Get(entityId).CardType;
                var tplId = world.GetPool<ComponentObjectType>().Get(entityId).TplId;

                switch (eclipseCardType)
                {
                    case EclipseCardTypes.Token:
                        PrepareToken(world, entityId);
                        break;
                    default:
                    {
                        if (itemTypes.TryGetItemType(out var itemType, tplId))
                        {
                            var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;

                            if (!_cards.TryGetValue(objectId, out var eclipseCard))
                            {
                                eclipseCard = AttachCard(world, entityId, tplId, objectId, itemType);
                                eclipseCard.Layout.UpdateAvailable(isAvailable);
                            }

                            // Update tplId
                            if (tplId != eclipseCard.CardType)
                            {
                                UpdateFromCardTypeData(entityId, tplId, objectId, itemType, eclipseCard);
                            }
                            
                            eclipseCard.SetActive(true);
                            eclipseCard.UpdateDescription(objectId, itemType, attributes);
                            UpdateCardAttributes(world, attributes, eclipseCard, CardFace);
                        }
                        break;
                    }
                }
            }
            
            AttachTokensForCard(world, itemTypes);
            UpdatedCardsOrder();
            UpdateCardsPosition();
            UpdateCardsAvailable(isAvailable);
            UpdateCardsAnimation(world);
            _dropObjectId.Clear();
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }

        private void UpdateCardsAvailable(bool isAvailable)
        {
            foreach (var eclipseCard in _cards.Values)
            {
                eclipseCard.Layout.UpdateAvailable(isAvailable);
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

        protected virtual void UpdateCardsPosition()
        {
        }

        public override IAnimatedObject GetAnimatedObject(int objectId)
        {
            if (_cards.TryGetValue(objectId, out var card))
            {
                return card;
            }

            return null;
        }

        private void UpdateCardsAnimation(EcsWorld world)
        {
            Layout.Rebuild();
            foreach (var eclipseCard in _cards.Values)
            {
                var attributes = world.GetPool<ComponentObjectAttributes>().Get(eclipseCard.EntityId).Attributes;

                var showDescription = attributes.TryGetValue(GameJsonKeys.CardShowDescription, out var showDescriptionAttribute) && showDescriptionAttribute.Current > 0;
                eclipseCard.Layout.ShowDescription(showDescription);

                if (GetCardMoveAnimation(attributes, out var fromPlaceId, out var animCardFlyTimeSec))
                {
                    var from = world.GetPlaceWidget(fromPlaceId).GetPosition();
                    eclipseCard.SetActive(false);
                    WidgetCanvas.GetEffects().MoveEclipseCard(GetOldPlaceWidgetCardFace(world, attributes) == PlaceWidgetCardFace.Up ? eclipseCard.Layout.FrontTransform : eclipseCard.Layout.BackTransform, animCardFlyTimeSec, from, () =>
                    {
                        eclipseCard.SetActive(true);
                    });
                }
                
                if (attributes.TryGetValue(GameJsonKeys.AnimDestroy, out var animDestroyAttribute) &&
                    animDestroyAttribute.Current > 0)
                {
                    var animCardDestroyTimeSec = attributes.TryGetValue(GameJsonKeys.AnimDestroyTime, out var  animCardDestroyTimeAttribute)
                        ? animCardDestroyTimeAttribute.Current.ToSec()
                        : 3f;
                    AnimEclipseCardDestroy(eclipseCard, animCardDestroyTimeSec);
                }
            }
        }

        private bool GetCardMoveAnimation(Dictionary<string, IAttributeValue> attributes, out int fromPlaceId, out float animCardFlyTimeSec)
        {
            if (attributes.TryGetValue(GameJsonKeys.CardAnimCardFly, out var animCardFlyAttribute) &&
                animCardFlyAttribute.Current > 0)
            {
                fromPlaceId =
                    attributes.TryGetValue(GameJsonKeys.CardAnimCardFlyFromPlace, out var fromPlaceAttribute)
                        ? fromPlaceAttribute.Current
                        : 0;
                animCardFlyTimeSec =
                    attributes.TryGetValue(GameJsonKeys.CardAnimCardFlyTime, out var animCardFlyTimeAttribute)
                        ? animCardFlyTimeAttribute.Current.ToSec()
                        : 1f;
                return true;
            }

            fromPlaceId = default;
            animCardFlyTimeSec = default;
            return false;
        }

        private PlaceWidgetCardFace GetOldPlaceWidgetCardFace(EcsWorld world, Dictionary<string, IAttributeValue> attributes)
        {
            if (attributes.TryGetValue(GameJsonKeys.Place, out var place) && place.Changed)
            {
                var poolPlaceId = world.GetPool<ComponentPlaceId>();
                var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
                var oldWidget = GetPlaceWidget(place.Old, poolPlaceId, poolPlaceWidgetNew);

                if (oldWidget != null)
                {
                    return oldWidget.GetPlaceWidgetCardFace();
                }
            }

            return PlaceWidgetCardFace.Up;
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
        
        private void UpdateFromCardTypeData(int entityId, int tplId, int objectId, IItemType itemType, IEclipseCardInContainerWidget eclipseCard)
        {
            eclipseCard.UpdateFromCardTypeData(entityId, objectId, tplId, itemType);
        }

        private void UpdateCardAttributes(EcsWorld world, Dictionary<string, IAttributeValue> attributes, IEclipseCardInContainerWidget eclipseCard, PlaceWidgetCardFace cardFace)
        {
            // timer
            var showTimer = attributes.TryGetValue(GameJsonKeys.CardShowDuration, out var showDurationAttribute) && showDurationAttribute.Current > 0;
            var timerDuration = attributes.TryGetValue(GameJsonKeys.CardDuration, out var durationAttribute) ? durationAttribute.Current : 0;
            eclipseCard.Layout.TimerLayout.gameObject.SetActive(showTimer);
            eclipseCard.Layout.TimerLayout.UpdateTimerValue(timerDuration);
            
            // tokens
            var tokenSlots = attributes.TryGetValue(GameJsonKeys.CardTokenSlots, out var tokenSlotsAttribute) ? tokenSlotsAttribute.Current : 0;
            eclipseCard.Layout.TokensLayout.UpdateTokenSlots(tokenSlots);
            
            // order
            var order = attributes.TryGetValue("order", out var orderAttributeY) ? orderAttributeY.Current : 0;
            eclipseCard.SetOrder(order);
            
            // face
            GetCardMoveAnimation(attributes, out _, out var animCardFlyTimeSec);
            eclipseCard.UpdateCardFace(cardFace, GetOldPlaceWidgetCardFace(world, attributes) != cardFace, animCardFlyTimeSec);
            
            // highlights
            eclipseCard.Layout.UpdateHighlights(attributes);
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

        private void AnimEclipseCardDestroy(IEclipseCardInContainerWidget eclipseCard, float timeSec)
        {
            var rttData = Game.ServiceRenderWidget.CreateWidgetRender(eclipseCard.Layout.RectTransform);
            if (rttData != null)
            {
                eclipseCard.SetActive(false);
                WidgetCanvas.GetEffects().DestroyEclipseCard(eclipseCard.Layout.EffectLayout,
                    rttData,
                    timeSec,
                    () => { Game.ServiceRenderWidget.ReleaseWidgetRender(eclipseCard.Layout.RectTransform); });
            }
        }

        private IEclipseCardInContainerWidget AttachCard(EcsWorld world, int entityId, int cardType, int objectId, IItemType itemType)
        {
            if (world.GetPool<ComponentEclipseCardTag>().Has(entityId))
            {
                if (CardsPool.TryPop(out var eclipseCard))
                {
                    PutCardToInPlace(objectId, eclipseCard);
                    UpdateFromCardTypeData(entityId, cardType, objectId, itemType, eclipseCard);
                    UpdateDragAndDrop(world, entityId, objectId, eclipseCard);
                    return eclipseCard;
                }
            }

            return null;
        }

        private void PutCardToInPlace(int objectId, IEclipseCardInContainerWidget eclipseCard)
        {
            Layout.AddCard(eclipseCard);
            _cards.Add(objectId, eclipseCard);
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
                CardsPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }

        private void AttachTokensForCard(EcsWorld world, IItemTypes itemTypes)
        {
            foreach (var tokensPerCard in _tokensPerCardCache)
            {
                if (_cards.TryGetValue(tokensPerCard.Key, out var cardWidget))
                {
                    var objectIdPool = world.GetPool<ComponentObjectId>();
                    var objectTypePool = world.GetPool<ComponentObjectType>();
                    foreach (var entityId in tokensPerCard.Value)
                    {
                        if (objectIdPool.Has(entityId)
                            && objectTypePool.Has(entityId)
                            && itemTypes.TryGetItemType(out var itemType, objectTypePool.Get(entityId).TplId))
                        {
                            var objectId = objectIdPool.Get(entityId).Id;
                            var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
                            if (attributes.TryGetValue(GameJsonKeys.TokenSlot, out var tokenSlotAttribute))
                            {
                                var tokenLayout = cardWidget.AttachToken(tokenSlotAttribute.Current, objectId, itemType);
                                if (tokenLayout != null)
                                {
                                    ProcessTokenAttributes(world, tokenLayout, objectId, itemType, attributes);
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

        private void ProcessTokenAttributes(EcsWorld world, EclipseCardTokenLayout tokenLayout, int objectId, IItemType itemType, Dictionary<string, IAttributeValue> attributes)
        {
            var color = Color.white;

            if (itemType.TryGetValue(out var valueColorToken, "effect_color", objectId)
                && ColorUtility.TryParseHtmlString(valueColorToken.GetValue<string>(), out var col))
            {
                color = col;
            }
            
            if (attributes.TryGetValue("anim_token_fly", out var animTokenFlyAttribute) && animTokenFlyAttribute.Current > 0)
            {
                var fromPlaceId = attributes.TryGetValue("anim_token_fly_from_place", out var fromPlaceAttribute) ? fromPlaceAttribute.Current : 0;
                var formCardId = attributes.TryGetValue("anim_token_fly_from_card_id", out var fromCardAttribute) ? fromCardAttribute.Current : 0;
                var fromSlotId = attributes.TryGetValue("anim_token_fly_from_slot", out var fromSlotAttribute) ? fromSlotAttribute.Current : 0;
                if (WidgetExtensions.TryGetTokenFromPosition(world, fromPlaceId, formCardId, fromSlotId, out var from))
                {
                    var animTokenFlyTimeSec = attributes.TryGetValue("anim_token_fly_time", out var  animTokenFlyTimeAttribute)
                        ? animTokenFlyTimeAttribute.Current.ToSec()
                        : 0f;
                    AnimTokenFly(tokenLayout, from, animTokenFlyTimeSec, color);
                }
                else
                {
                    Debug.LogWarning($"Can't run token animation: anim_token_fly_from_place = {fromPlaceId}: anim_token_fly_from_card_id = {formCardId} and anim_token_fly_from_slot = {fromSlotId}");
                }
            }

            if (attributes.TryGetValue("anim_destroy", out var animDestroyAttribute) &&
                animDestroyAttribute.Current > 0)
            {
                AnimTokenDestroy(tokenLayout, color);
            }
        }

        private void AnimTokenFly(EclipseCardTokenLayout tokenLayout, Vector3 from, float timeSec, Color color)
        {
            tokenLayout.SetIconVisible(false);
            WidgetCanvas.GetEffects().MoveToken(tokenLayout.RectTransform, 
                tokenLayout.Sprite,
                from,
                timeSec,
                color,
                () => { tokenLayout.SetIconVisible(true); });
        }

        private void AnimTokenDestroy(EclipseCardTokenLayout tokenLayout, Color color)
        {
            tokenLayout.SetIconVisible(false);
            WidgetCanvas.GetEffects().DestroyToken(tokenLayout.RectTransform,
                tokenLayout.Sprite,
                0.5f,
                color,
                () => {});
        }

        public bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position)
        {
            if (_cards.TryGetValue(cardId, out var eclipseCard))
            {
                position = eclipseCard.GetTokenPosition(slotId);
                return true;
            }
            
            position = Layout.transform.position;
            return true;
        }

        void UpdateDragAndDrop(EcsWorld world, int entityId, int objectId, IEclipseCardInContainerWidget eclipseCard)
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
        
        #region IApplyDropWidget
        
        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position, bool discard)
        {
            if (discard)
            {
                if (dropWidget is IEclipseCardInContainerWidget ew)
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