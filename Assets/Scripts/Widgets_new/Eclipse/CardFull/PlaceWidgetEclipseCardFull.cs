using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;

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

        public override void Update(EcsWorld world, int[] entityIds)
        {
            if (entityIds.Length <= 0)
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
                            UpdateCard(world, entityId, eclipseCardType, cardTypeDataObject);
                            break;
                    }
                }
            }
        }

        private void UpdateCard(EcsWorld world, int entityId, EclipseCardTypes type, JObject cardTypeDataObject)
        {
            var attributesPool = world.GetPool<ComponentObjectAttributes>();
            if (world.GetPool<ComponentEclipseCardTag>().Has(entityId))
            {
                var attributes = attributesPool.Get(entityId).Attributes;
                var tokenSlots = attributes.TryGetValue(GameJsonKeys.CardTokenSlots, out var tokenSlotsAttribute) ? tokenSlotsAttribute.Current : 0;
                Layout.TokensLayout.UpdateTokenSlots(tokenSlots);
                
                Layout.UpdateType(type.ToString());

                if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardName, out string name))
                {
                    Layout.UpdateName(name);
                }

                if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardDescription, out string description))
                {
                    Layout.UpdateDescription(description);
                }

                if (cardTypeDataObject.TryGetValue(GameJsonKeys.CardDescription, out string picture)
                    && Game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                {
                    Layout.UpdateSprite(texture);
                }
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
    }
}
