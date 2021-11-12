using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Entities;
using Solcery.Models.GameState;

namespace Solcery.Models.Places
{
    public interface ISystemPlaceWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemPlaceWidgetsUpdate : ISystemPlaceWidgetsUpdate
    {
        private EcsFilter _filterPlaceWithWidget;
        private EcsFilter _filterEntities;
        private EcsFilter _filterGameStateUpdate;

        public static ISystemPlaceWidgetsUpdate Create()
        {
            return new SystemPlaceWidgetsUpdate();
        }
        
        private SystemPlaceWidgetsUpdate() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterPlaceWithWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceWidget>().End();
            _filterEntities = systems.GetWorld().Filter<ComponentEntityTag>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            var entitiesInPlace = new Dictionary<int, List<int>>();
            // Подготовим набор entity в place
            foreach (var entityIndex in _filterEntities)
            {
                var entityPlaceId = systems.GetWorld().GetPool<ComponentEntityAttributes>().Get(entityIndex)
                    .Attributes["place"];

                if (!entitiesInPlace.ContainsKey(entityPlaceId))
                {
                    entitiesInPlace.Add(entityPlaceId, new List<int>());
                }
                
                entitiesInPlace[entityPlaceId].Add(entityIndex);
            }
            
            // Пробежим по place с widget
            foreach (var placeIndex in _filterPlaceWithWidget)
            {
                if (entitiesInPlace.TryGetValue(systems.GetWorld().GetPool<ComponentPlaceId>().Get(placeIndex).Id,
                    out var entityIds))
                {
                    systems.GetWorld().GetPool<ComponentPlaceWidget>().Get(placeIndex).Widget
                        .UpdateWidget(systems.GetWorld(), entityIds.ToArray());
                }
            }
            
            entitiesInPlace.Clear();
        }
    }
}