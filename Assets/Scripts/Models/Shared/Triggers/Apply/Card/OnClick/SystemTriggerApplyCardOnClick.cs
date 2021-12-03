using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.BrickInterpretation;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types;
using UnityEngine;

namespace Solcery.Models.Shared.Triggers.Apply.Card.OnClick
{
    public interface ISystemTriggerApplyCardOnClick : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemTriggerApplyCardOnClick : ISystemTriggerApplyCardOnClick
    {
        private IServiceBricks _serviceBricks;
        private EcsFilter _filterTriggers;
        private EcsFilter _filterContext;
        private EcsFilter _filterEntityTypes;
        private EcsFilter _filterEntities;

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
            var world = systems.GetWorld();
            
            _filterTriggers = world.Filter<ComponentTriggerTag>().Inc<ComponentTriggerTargetEntityId>()
                .Inc<ComponentTriggerEntityCardTag>()
                .Inc<ComponentTriggerOnClickTag>().End();

            _filterContext = world.Filter<ComponentContextObject>().Inc<ComponentContextArgs>()
                .Inc<ComponentContextVars>().End();

            _filterEntityTypes = world.Filter<ComponentEntityTypes>().End();

            _filterEntities = world.Filter<ComponentEntityTag>().Inc<ComponentEntityId>().Inc<ComponentEntityType>()
                .End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var triggerTargetEntityIdPool = world.GetPool<ComponentTriggerTargetEntityId>();
            var entityIdPool = world.GetPool<ComponentEntityId>();
            var entityTypePool = world.GetPool<ComponentEntityType>();
            var entityTypesPool = world.GetPool<ComponentEntityTypes>();

            foreach (var triggerEntityId in _filterTriggers)
            {
                var targetObjectId = triggerTargetEntityIdPool.Get(triggerEntityId).TargetEntityId;

                foreach (var entityId in _filterEntities)
                {
                    if (entityIdPool.Get(entityId).Id != targetObjectId)
                    {
                        continue;
                    }

                    var entityType = entityTypePool.Get(entityId).Type;
                    var entityTypesId = -1;
                    foreach (var etid in _filterEntityTypes)
                    {
                        entityTypesId = etid;
                        break;
                    }
                
                    if (entityTypesId == -1 || 
                        !entityTypesPool.Get(entityTypesId).Types.TryGetValue(entityType, out var entityTypeData) 
                        || !entityTypeData.TryGetValue("action", out JObject brick))
                    {
                        continue;
                    }

                    InitContext(entityId, world);
                    Debug.Log($"Action brick execute status {_serviceBricks.ExecuteActionBrick(brick, world, 1)}");
                    DestroyContext(world);
                    
                    break;
                }
                
                world.GetPool<ComponentTriggerOnClickTag>().Del(triggerEntityId);
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