using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.BrickInterpretation;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnClick
{
    public interface ISystemTriggerApplyCardOnClick : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemTriggerApplyCardOnClick : ISystemTriggerApplyCardOnClick
    {
        private IServiceBricks _serviceBricks;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterContext;

        public static ISystemTriggerApplyCardOnClick Create(IServiceBricks serviceBricks)
        {
            return new SystemTriggerApplyCardOnClick(serviceBricks);
        }
        
        private SystemTriggerApplyCardOnClick(IServiceBricks serviceBricks)
        {
            _serviceBricks = serviceBricks;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterTriggers = systems.GetWorld().Filter<ComponentTriggerTag>().Inc<ComponentTriggerTargetEntityId>()
                .Inc<ComponentTriggerEntityCardTag>()
                .Inc<ComponentTriggerOnClickTag>().End();

            _filterContext = systems.GetWorld().Filter<ComponentContextObject>().Inc<ComponentContextArgs>()
                .Inc<ComponentContextVars>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var triggerTargetEntityIdPool = world.GetPool<ComponentTriggerTargetEntityId>();
            var entityTypePool = world.GetPool<ComponentEntityType>();
            var entityTypesPool = world.GetPool<ComponentEntityTypes>();
            
            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetEntityId = triggerTargetEntityIdPool.Get(triggerEntityId).TargetEntityId;

                if (!entityTypePool.Has(targetEntityId))
                {
                    continue;
                }

                var entityType = entityTypePool.Get(targetEntityId).Type;
                if (!entityTypesPool.Get(entityType).Types.TryGetValue(entityType, out var entityTypeData) 
                    || !entityTypeData.TryGetValue("action", out JObject brick))
                {
                    continue;
                }

                InitContext(targetEntityId, world);
                _serviceBricks.ExecuteActionBrick(brick, world);
                DestroyContext(world);
            }
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _serviceBricks = null;
            _filterTriggers = null;
        }

        private void InitContext(int targetEntityId, EcsWorld world)
        {
            var contextObjectPool = world.GetPool<ComponentContextObject>();
            var contextArgsPool = world.GetPool<ComponentContextArgs>();
            var contextVarsPool = world.GetPool<ComponentContextVars>();

            foreach (var entityId in _filterContext)
            {
                world.DelEntity(entityId);
            }

            var contextEntityId = world.NewEntity();
            contextObjectPool.Add(contextEntityId).Push(targetEntityId);
            contextArgsPool.Add(contextEntityId);
            contextVarsPool.Add(contextEntityId);
        }

        private void DestroyContext(EcsWorld world)
        {
            foreach (var entityId in _filterContext)
            {
                world.DelEntity(entityId);
            }
        }
    }
}