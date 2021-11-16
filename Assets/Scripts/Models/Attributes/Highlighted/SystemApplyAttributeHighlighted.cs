using Leopotam.EcsLite;
using Solcery.Models.GameState;
using Solcery.Models.Places;

namespace Solcery.Models.Attributes.Highlighted
{
    internal interface ISystemApplyAttributeHighlighted : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeHighlighted : ISystemApplyAttributeHighlighted
    {
        private EcsFilter _filterViewComponent;
        private EcsFilter _filterGameStateUpdate;
        
        public static ISystemApplyAttributeHighlighted Create()
        {
            return new SystemApplyAttributeHighlighted();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterViewComponent = systems.GetWorld().Filter<ComponentEntityView>().Inc<ComponentAttributeHighlighted>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }
            
            var widgetViewComponents = systems.GetWorld().GetPool<ComponentEntityView>();
            var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeHighlighted>();
            foreach (var entity in _filterViewComponent)
            {
                var view = widgetViewComponents.Get(entity).View;
                if (view is IHighlighted value)
                {
                    value.SetHighlighted(attributeComponents.Get(entity).Value);
                }
            }
        }
    }
}