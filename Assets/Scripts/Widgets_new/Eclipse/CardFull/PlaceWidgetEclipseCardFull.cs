using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.Events;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFull : PlaceWidget<PlaceWidgetEclipseCardFullLayout>, IPlaceWidgetTokenCollection
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseCardFull(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseCardFull(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
        }

        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0 && isVisible);
            Layout.ClearAllOnClickListener();
            
            if (entityIds.Length <= 0 || !isVisible)
            {
                return;
            }
            
            var eclipseCartTypePool = world.GetPool<ComponentEclipseCardType>();
            var objectTypePool = world.GetPool<ComponentObjectType>();

            foreach (var entityId in entityIds)
            {
                var eclipseCardType = eclipseCartTypePool.Get(entityId).CardType;

                if (objectTypePool.Has(entityId) 
                    && Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, objectTypePool.Get(entityId).TplId))
                {
                    switch (eclipseCardType)
                    {
                        case EclipseCardTypes.Token:
                            UpdateToken(world, entityId, itemType);
                            break;
                        default:
                            AddClickListeners(entityId);
                            UpdateEclipseCard(world, entityId, itemType);
                            break;
                    }
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }

        private void AddClickListeners(int entityId)
        {
            Layout.AddOnLeftClickListener(() =>
            {
                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
            });
            Layout.AddOnRightClickListener(() =>
            {
                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
            });
        }

        private void UpdateEclipseCard(EcsWorld world, int entityId, IItemType itemType)
        {
            var poolObjectId = world.GetPool<ComponentObjectId>();
            var poolEclipseCardTag = world.GetPool<ComponentEclipseCardTag>();
            if (poolObjectId.Has(entityId)
                && poolEclipseCardTag.Has(entityId))
            {
                var objectId = poolObjectId.Get(entityId).Id;
                var attributesPool = world.GetPool<ComponentObjectAttributes>();
                var attributes = attributesPool.Get(entityId).Attributes;
                UpdateCardMainAttributes(attributes);
                Layout.UpdateCardType(Game, objectId, itemType);
                UpdateCardAnimation(world, attributes);
            }
        }

        private void UpdateCardMainAttributes(Dictionary<string, IAttributeValue> attributes)
        {
            var tokenSlots = attributes.TryGetValue(GameJsonKeys.CardTokenSlots, out var tokenSlotsAttribute) ? tokenSlotsAttribute.Current : 0;
            Layout.TokensLayout.UpdateTokenSlots(tokenSlots);
                
            var showTimer = attributes.TryGetValue(GameJsonKeys.CardShowDuration, out var showDurationAttribute) && showDurationAttribute.Current > 0;
            var timerDuration = attributes.TryGetValue(GameJsonKeys.CardDuration, out var durationAttribute) ? durationAttribute.Current : 0;
            Layout.TimerLayout.gameObject.SetActive(showTimer);
            Layout.TimerLayout.UpdateTimerValue(timerDuration);
        }

        private void UpdateCardAnimation(EcsWorld world, Dictionary<string, IAttributeValue> attributes)
        {
            if (attributes.TryGetValue(GameJsonKeys.CardAnimCardFly, out var animCardFlyAttribute) &&
                animCardFlyAttribute.Current > 0)
            {
                var fromPlaceId = attributes.TryGetValue(GameJsonKeys.CardAnimCardFlyFromPlace, out var fromPlaceAttribute)
                    ? fromPlaceAttribute.Current
                    : 0;
                var animCardFlyTimeSec = attributes.TryGetValue(GameJsonKeys.CardAnimCardFlyTime, out var  animCardFlyTimeAttribute)
                    ? animCardFlyTimeAttribute.Current.ToSec()
                    : 1f;
                var from = world.GetPlaceWidget(fromPlaceId).GetPosition();
                Layout.CardTransform.gameObject.SetActive(false);
                WidgetCanvas.GetEffects().MoveEclipseCard(Layout.CardTransform, animCardFlyTimeSec, from, () =>
                {
                    Layout.CardTransform.gameObject.SetActive(true);
                });
            }
        }
        
        private void UpdateToken(EcsWorld world, int entityId, IItemType itemType)
        {
            var poolObjectId = world.GetPool<ComponentObjectId>();
            var attributesPool = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
            if (poolObjectId.Has(entityId)
                && attributesPool.TryGetValue(GameJsonKeys.TokenSlot, out var tokenSlotAttribute))
            {
                var objectId = poolObjectId.Get(entityId).Id;
                var tokenLayout =  Layout.TokensLayout.GetTokenByIndex(tokenSlotAttribute.Current - 1);
                if (tokenLayout != null)
                {
                    if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.Picture, objectId)
                        && Game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
                    {
                        tokenLayout.UpdateSprite(texture);
                    }

                    if (itemType.TryGetValue(out var valueTooltipIdToken, GameJsonKeys.CardTooltipId, objectId))
                    {
                        tokenLayout.UpdateTooltip(valueTooltipIdToken.GetValue<int>());
                    }
                }
            }
        }

        private Vector3 GetTokenPosition(int slot)
        {
            var tokenLayout = Layout.TokensLayout.GetTokenByIndex(slot - 1);
            if (tokenLayout != null)
            {
                return tokenLayout.transform.position;
            }

            return Layout.transform.position;
        }
        
        public bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position)
        {
            position = GetTokenPosition(slotId);
            return true;
        }
    }
}
