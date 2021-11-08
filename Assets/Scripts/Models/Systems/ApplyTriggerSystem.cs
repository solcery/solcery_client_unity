using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery
{
    sealed class ApplyTriggerSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var applyTriggerComponents = systems.GetWorld().GetPool<ApplyTriggerComponent>();
            var triggerComponents = systems.GetWorld().GetPool<TriggersComponent>();
            foreach (var entity in systems.GetWorld().Filter<ApplyTriggerComponent>().End())
            {
                if (triggerComponents.Has(entity))
                {
                    var triggersComponent = triggerComponents.Get(entity);
                    var applyTriggerComponent = applyTriggerComponents.Get(entity);
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