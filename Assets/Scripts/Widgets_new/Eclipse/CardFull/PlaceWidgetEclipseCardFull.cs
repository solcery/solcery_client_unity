using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
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
                            Layout.AddOnClickListener(() => CloseFullView(entityId));
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
                UpdateCardAttributes(world, entityId);
                UpdateCardType(type, cardTypeDataObject);
            }
        }

        private void UpdateCardAttributes(EcsWorld world, int entityId)
        {
            var attributesPool = world.GetPool<ComponentObjectAttributes>();
            var attributes = attributesPool.Get(entityId).Attributes;
            var tokenSlots = attributes.TryGetValue(GameJsonKeys.CardTokenSlots, out var tokenSlotsAttribute) ? tokenSlotsAttribute.Current : 0;
            Layout.TokensLayout.UpdateTokenSlots(tokenSlots);
                
            var showTimer = attributes.TryGetValue(GameJsonKeys.CardShowDuration, out var showDurationAttribute) && showDurationAttribute.Current > 0;
            var timerDuration = attributes.TryGetValue(GameJsonKeys.CardDuration, out var durationAttribute) ? durationAttribute.Current : 0;
            Layout.TimerLayout.gameObject.SetActive(showTimer);
            Layout.TimerLayout.UpdateTimerValue(timerDuration);
        }

        private void UpdateCardType(EclipseCardTypes type, JObject cardTypeDataObject)
        {
            Layout.UpdateType(type.ToString());

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardName, out string name))
            {
                Layout.UpdateName(name);
            }

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardDescription, out string description))
            {
                Layout.UpdateDescription(description);
            }

            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardPicture, out string picture)
                && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                Layout.UpdateSprite(texture);
            }
                
            if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardTimerText, out string timerText))
            {
                Layout.TimerLayout.UpdateTimerTextActive(true);
                Layout.TimerLayout.UpdateTimerText(timerText);
            }
            else
            {
                Layout.TimerLayout.UpdateTimerTextActive(false);
            }        
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

        private void CloseFullView(int entityId)
        {
            ServiceEvents.Current.BroadcastEvent(OnRightClickEventData.Create(entityId));
        }
    }
}
