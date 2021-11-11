using Leopotam.EcsLite;
using Solcery.Models.Entities;

namespace Solcery.Models.Places
{
    public interface ISystemPlaceWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemPlaceWidgetsUpdate : ISystemPlaceWidgetsUpdate
    {
        private EcsFilter _filterPlaceWithWidget;
        private EcsFilter _filterEntities;

        public static ISystemPlaceWidgetsUpdate Create()
        {
            return new SystemPlaceWidgetsUpdate();
        }
        
        private SystemPlaceWidgetsUpdate() { }
        
        public void Init(EcsSystems systems)
        {
            _filterPlaceWithWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceWidget>().End();
            _filterEntities = systems.GetWorld().Filter<ComponentEntityTag>().End();
        }
        
        public void Run(EcsSystems systems)
        {
            foreach (var entityIndex in _filterEntities)
            {
                var entityPlaceId = systems.GetWorld().GetPool<ComponentEntityAttributes>().Get(entityIndex)
                    .Attributes["place"];

                foreach (var placeIndex in _filterPlaceWithWidget)
                {
                    var placeId = systems.GetWorld().GetPool<ComponentPlaceId>().Get(placeIndex).Id;

                    if (placeId == entityPlaceId)
                    {
                        systems.GetWorld().GetPool<ComponentPlaceWidget>().Get(placeIndex).Widget
                            .UpdateWidget(systems.GetWorld(), entityIndex);
                        break;
                    }
                }
            }
        }
    }
}