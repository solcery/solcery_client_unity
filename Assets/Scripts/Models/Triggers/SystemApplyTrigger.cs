using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using UnityEngine;

namespace Solcery.Models.Triggers
{
    internal interface ISystemApplyTrigger : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyTrigger : ISystemApplyTrigger
    {
        private EcsFilter _filterApplyTriggerComponent;
        private readonly IServiceBricks _serviceBricks;

        public static ISystemApplyTrigger Create(IServiceBricks serviceBricks)
        {
            return new SystemApplyTrigger(serviceBricks);
        }

        private SystemApplyTrigger(IServiceBricks serviceBricks)
        {
            _serviceBricks = serviceBricks;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterApplyTriggerComponent = systems.GetWorld().Filter<ComponentApplyTrigger>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var applyTriggerComponents = systems.GetWorld().GetPool<ComponentApplyTrigger>();
            var triggerComponents = systems.GetWorld().GetPool<ComponentTriggers>();
            foreach (var entity in _filterApplyTriggerComponent)
            {
                if (triggerComponents.Has(entity))
                {
                    ref var triggersComponent = ref triggerComponents.Get(entity);
                    ref var applyTriggerComponent = ref applyTriggerComponents.Get(entity);
                    if (triggersComponent.Triggers.TryGetValue(applyTriggerComponent.Type, out JObject brick))
                    {
                        _serviceBricks.ExecuteActionBrick(brick, null);
                        Debug.Log($"Brick \"{brick["name"]}\" was executed!");
                    }
                }

                applyTriggerComponents.Del(entity);
            }
        }
    }
}