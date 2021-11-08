using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery.Models.Triggers
{
    internal interface ISystemApplyTrigger : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyTrigger : ISystemApplyTrigger
    {
        private EcsFilter _applyTriggerComponentFilter;

        public static ISystemApplyTrigger Create()
        {
            return new SystemApplyTrigger();
        }
        
        private SystemApplyTrigger() { }
        
        public void Init(EcsSystems systems)
        {
            _applyTriggerComponentFilter = systems.GetWorld().Filter<ComponentApplyTrigger>().End();
        }
        
        public void Run(EcsSystems systems)
        {
            var applyTriggerComponents = systems.GetWorld().GetPool<ComponentApplyTrigger>();
            var triggerComponents = systems.GetWorld().GetPool<ComponentTriggers>();
            foreach (var entity in _applyTriggerComponentFilter)
            {
                if (triggerComponents.Has(entity))
                {
                    ref var triggersComponent = ref triggerComponents.Get(entity);
                    ref var applyTriggerComponent = ref applyTriggerComponents.Get(entity);
                    if (triggersComponent.Triggers.TryGetValue(applyTriggerComponent.Type, out var triggerData))
                    {
                        Debug.Log($"Apply \"{triggerData.Action}\"!");
                    }
                }

                applyTriggerComponents.Del(entity);
            }
        }
    }
}