using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Shared.Attributes.Place;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemPlaceWidgetsUpdate : ISystemPlaceWidgetsUpdate
    {
        private EcsFilter _filterPlaceWithWidget;
        private EcsFilter _filterPlaceWithPlaceWidget;
        private EcsFilter _filterEntities;
        private EcsFilter _filterGameStateUpdate;
        private EcsFilter _filterSubWidgetComponent;

        public static ISystemPlaceWidgetsUpdate Create()
        {
            return new SystemPlaceWidgetsUpdate();
        }
        
        private SystemPlaceWidgetsUpdate() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterPlaceWithWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceWidget>().End();
            _filterPlaceWithPlaceWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>()
                .Inc<ComponentPlaceWidgetNew>().End();
            _filterEntities = systems.GetWorld().Filter<ComponentObjectTag>().Inc<ComponentAttributePlace>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
            _filterSubWidgetComponent = systems.GetWorld().Filter<ComponentPlaceSubWidget>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            // Чистим старые сабвиджеты
            var subWidgetsPool = systems.GetWorld().GetPool<ComponentPlaceSubWidget>();
            foreach (var entityId in _filterSubWidgetComponent)
            {
                var subWidget = subWidgetsPool.Get(entityId).Widget;
                subWidget.ClearView();
                subWidgetsPool.Del(entityId);
            }
            
            var entitiesInPlace = new Dictionary<int, List<int>>();
            // Подготовим набор entity в place
            foreach (var entityIndex in _filterEntities)
            {
                var entityPlaceId = systems.GetWorld().GetPool<ComponentAttributePlace>().Get(entityIndex).Value;

                if (!entitiesInPlace.ContainsKey(entityPlaceId))
                {
                    entitiesInPlace.Add(entityPlaceId, new List<int>());
                }
                
                entitiesInPlace[entityPlaceId].Add(entityIndex);
            }

            var world = systems.GetWorld();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
            // TODO: New place widget update
            foreach (var entityId in _filterPlaceWithPlaceWidget)
            {
                var placeId = poolPlaceId.Get(entityId).Id;
                if (entitiesInPlace.TryGetValue(placeId, out var entityIds))
                {
                    var placeWidget = poolPlaceWidgetNew.Get(entityId).Widget;
                    placeWidget.Update(world, entityIds.ToArray());
                }
            }
            
            
            // Пробежим по place с widget
            foreach (var placeIndex in _filterPlaceWithWidget)
            {
                if (entitiesInPlace.TryGetValue(systems.GetWorld().GetPool<ComponentPlaceId>().Get(placeIndex).Id,
                    out var entityIds))
                {
                    var widget = systems.GetWorld().GetPool<ComponentPlaceWidget>().Get(placeIndex).Widget;
                    widget.UpdateSubWidgets(systems.GetWorld(), entityIds.ToArray());
                }
            }
            
            entitiesInPlace.Clear();
        }
    }
}