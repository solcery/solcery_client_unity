using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Services.Transport;

namespace Solcery.Models.Play.Triggers
{
    public interface ISystemTriggerWidgetsAppy : IEcsInitSystem, IEcsRunSystem
    {
    }

    public class SystemTriggerWidgetsApply : ISystemTriggerWidgetsAppy
    {
        private readonly ITransportService _transportService;
        private EcsFilter _filterTriggerWidgetCollector;

        public static SystemTriggerWidgetsApply Create(ITransportService transportService)
        {
            return new SystemTriggerWidgetsApply(transportService);
        }

        private SystemTriggerWidgetsApply(ITransportService transportService)
        {
            _transportService = transportService;
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterTriggerWidgetCollector = systems.GetWorld().Filter<ComponentTriggerWidgetCollector>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var entityIdPool = world.GetPool<ComponentObjectId>();
            foreach (var uniqEntityId in _filterTriggerWidgetCollector)
            {
                ref var triggerCollector = ref world.GetPool<ComponentTriggerWidgetCollector>().Get(uniqEntityId)
                    .TriggerCollector;
                while (triggerCollector.TryGet(out var triggerData))
                {
                    _transportService.SendCommand(new JObject
                    {
                        ["object_id"] = new JValue(entityIdPool.Get(triggerData.EntityId).Id),
                        ["trigger_type"] = new JValue(triggerData.Type),
                        ["trigger_target_entity_type"] = new JValue(TriggerTargetEntityTypes.Card)
                    });
                }
            }
        }
    }
}