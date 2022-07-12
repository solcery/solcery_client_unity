using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.Events;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;

namespace Solcery.Widgets_new.Eclipse.CardFull
{
    public class PlaceWidgetEclipseCardFull : PlaceWidget<PlaceWidgetEclipseCardFullLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseCardFull(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseCardFull(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
        }

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
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
                var cardTypes = world.GetCardTypes();

                if (objectTypePool.Has(entityId) &&
                    cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject))
                {
                    switch (eclipseCardType)
                    {
                        case EclipseCardTypes.Token:
                            UpdateToken(world, entityId, cardTypeDataObject);
                            break;
                        default:
                            Layout.AddOnLeftClickListener(() =>
                            {
                                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
                            });
                            Layout.AddOnRightClickListener(() =>
                            {
                                ServiceEvents.Current.BroadcastEvent(OnLeftClickEventData.Create(entityId));
                            });
                            UpdateCard(world, entityId, eclipseCardType, cardTypeDataObject);
                            break;
                    }
                }
            }
        }

        private void UpdateCard(EcsWorld world, int entityId, EclipseCardTypes type, JObject cardTypeDataObject)
        {
            if (world.GetPool<ComponentEclipseCardTag>().Has(entityId))
            {
                var attributesPool = world.GetPool<ComponentObjectAttributes>();
                var attributes = attributesPool.Get(entityId).Attributes;
                UpdateCardMainAttributes(attributes);
                UpdateCardType(type, cardTypeDataObject);
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
        
        private void UpdateCardType(EclipseCardTypes type, JObject cardTypeDataObject)
        {
            Layout.UpdateCardType(Game, type, cardTypeDataObject);
        }

        private void UpdateToken(EcsWorld world, int entityId, JObject cardTypeDataObject)
        {
            var attributesPool = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
            if (attributesPool.TryGetValue(GameJsonKeys.TokenSlot, out var tokenSlotAttribute))
            {
                var tokenLayout =  Layout.TokensLayout.GetTokenByIndex(tokenSlotAttribute.Current - 1);
                if (tokenLayout != null)
                {
                    if (cardTypeDataObject.TryGetValue(GameJsonKeys.TokenPicture, out string picture)
                        && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                    {
                        tokenLayout.UpdateSprite(texture);
                    }

                    if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardTooltipId, out int tooltipId))
                    {
                        tokenLayout.UpdateTooltip(tooltipId);
                    }
                }
            }
        }
    }
}
