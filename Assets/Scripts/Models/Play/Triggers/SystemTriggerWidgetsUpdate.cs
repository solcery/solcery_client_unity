using Leopotam.EcsLite;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Places;

namespace Solcery.Models.Play.Triggers
{
    public interface ISystemTriggerWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }

    public class SystemTriggerWidgetsUpdate : ISystemTriggerWidgetsUpdate
    {
        private EcsFilter _filterSubWidgetComponent;
        private EcsFilter _filterGameStateUpdate;
        private EcsFilter _filterTriggerWidgetCollector;

        public static SystemTriggerWidgetsUpdate Create()
        {
            return new SystemTriggerWidgetsUpdate();
        }

        private SystemTriggerWidgetsUpdate() { }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterSubWidgetComponent = systems.GetWorld().Filter<ComponentPlaceSubWidget>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
            _filterTriggerWidgetCollector = systems.GetWorld().Filter<ComponentTriggerWidgetCollector>().End();
            var world = systems.GetWorld();
            var entity = world.NewEntity();
            ref var collector = ref world.GetPool<ComponentTriggerWidgetCollector>().Add(entity);
            collector.TriggerCollector = new TriggerWidgetCollector();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = systems.GetWorld();
            foreach (var uniqEntityId in _filterTriggerWidgetCollector)
            {
                ref var triggerCollector = ref world.GetPool<ComponentTriggerWidgetCollector>().Get(uniqEntityId)
                    .TriggerCollector;
                triggerCollector.Cleanup();
                var subWidgetsPool = world.GetPool<ComponentPlaceSubWidget>();
                foreach (var entityId in _filterSubWidgetComponent)
                {
                    var subWidget = subWidgetsPool.Get(entityId).Widget;
                    triggerCollector.Register(entityId, subWidget);
                }

                break;
            }
        }
    }
}