using Leopotam.EcsLite;
using Solcery.Models.Shared.Triggers;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types;
using Solcery.Services.Commands;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands
{
    public interface ISystemProcessCommands : IEcsRunSystem { }

    public sealed class SystemProcessCommands : ISystemProcessCommands
    {
        private IServiceCommands _serviceCommands;
        
        public static ISystemProcessCommands Create(IServiceCommands serviceCommands)
        {
            return new SystemProcessCommands(serviceCommands);
        }

        private SystemProcessCommands(IServiceCommands serviceCommands)
        {
            _serviceCommands = serviceCommands;
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_serviceCommands.IsEmpty() 
                || !_serviceCommands.TryPopCommand(out var command)
                || !command.TryGetValue("object_id", out int objectId)
                || !command.TryGetEnum("trigger_type", out TriggerTypes triggerType)
                || !command.TryGetEnum("trigger_target_entity_type", out TriggerTargetEntityTypes triggerTargetEntityTypes))
            {
                return;
            }

            var world = systems.GetWorld();
            var entityId = world.NewEntity();

            if (!TryAddTriggerType(entityId, triggerType, world)
                || !TryAddTriggerEntityType(entityId, triggerTargetEntityTypes, world))
            {
                world.DelEntity(entityId);
                return;
            }
            
            world.GetPool<ComponentTriggerTag>().Add(entityId);
            world.GetPool<ComponentTriggerTargetObjectId>().Add(entityId).TargetObjectId = objectId;
        }

        private bool TryAddTriggerType(int entityId, TriggerTypes triggerType, EcsWorld world)
        {
            switch (triggerType)
            {
                case TriggerTypes.OnClick:
                    world.GetPool<ComponentTriggerOnClickTag>().Add(entityId);
                    return true;
            }

            return false;
        }

        private bool TryAddTriggerEntityType(int entityId, TriggerTargetEntityTypes triggerTargetEntityType, EcsWorld world)
        {
            switch (triggerTargetEntityType)
            {
                case TriggerTargetEntityTypes.Card:
                    world.GetPool<ComponentTriggerEntityCardTag>().Add(entityId);
                    return true;
            }
            
            return false;
        }
    }
}