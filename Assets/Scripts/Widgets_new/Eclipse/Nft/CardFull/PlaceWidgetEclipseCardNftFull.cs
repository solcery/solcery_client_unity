using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.Events;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Nft.CardFull
{
    public class PlaceWidgetEclipseCardNftFull : PlaceWidget<PlaceWidgetEclipseCardNftFullLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseCardNftFull(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseCardNftFull(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
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

                if (objectTypePool.Has(entityId) 
                    && Game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, objectTypePool.Get(entityId).TplId))
                {
                    switch (eclipseCardType)
                    {
                        case EclipseCardTypes.Nft:
                            AddClickListeners(entityId);
                            UpdateNftCard(world, entityId, itemType);
                            break;
                        default:
                            Debug.LogWarning($"Can't show card with type \"{eclipseCardType}\" in full view!");
                            break;
                    }
                }
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }
        
        private void UpdateNftCard(EcsWorld world, int entityId, IItemType itemType)
        {
            var poolObjectId = world.GetPool<ComponentObjectId>();
            var poolEclipseCardTag = world.GetPool<ComponentEclipseCardTag>();
            if (poolObjectId.Has(entityId)
                && poolEclipseCardTag.Has(entityId))
            {
                var objectId = poolObjectId.Get(entityId).Id;
                Layout.UpdateCardType(Game, objectId, itemType);
            }
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
    }
}