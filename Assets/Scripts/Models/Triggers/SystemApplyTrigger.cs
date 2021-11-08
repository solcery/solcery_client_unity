using Leopotam.EcsLite;
using Solcery.BrickInterpretation;

namespace Solcery.Models.Triggers
{
    internal interface ISystemApplyTrigger : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyTrigger : ISystemApplyTrigger
    {
        private EcsFilter _applyTriggerComponentFilter;
        private IBrickService _brickService;

        public static ISystemApplyTrigger Create(IBrickService brickService)
        {
            return new SystemApplyTrigger(brickService);
        }

        private SystemApplyTrigger(IBrickService brickService)
        {
            _brickService = brickService;
        }
        
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
                        foreach (var brick in triggerData.Actions)
                        {
                            _brickService.ExecuteActionBrick(brick, null);
                        }
                    }
                }

                applyTriggerComponents.Del(entity);
            }
        }
    }
}