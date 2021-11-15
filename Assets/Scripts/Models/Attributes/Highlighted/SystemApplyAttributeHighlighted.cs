using Leopotam.EcsLite;
using Solcery.Models.Places;

namespace Solcery.Models.Attributes.Highlighted
{
    internal interface ISystemApplyAttributeHighlighted : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeHighlighted : ISystemApplyAttributeHighlighted
    {
        private EcsFilter _filterViewComponent;
        
        public static ISystemApplyAttributeHighlighted Create()
        {
            return new SystemApplyAttributeHighlighted();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterViewComponent = systems.GetWorld().Filter<ComponentEntityView>().Inc<ComponentAttributeHighlighted>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var widgetViewComponents = systems.GetWorld().GetPool<ComponentEntityView>();
            var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeHighlighted>();
            foreach (var entity in _filterViewComponent)
            {
                var view = widgetViewComponents.Get(entity).View;
                if (view is IHighlighted value)
                {
                    value.SetHighlighted(attributeComponents.Get(entity).Value);
                }

                attributeComponents.Del(entity);
            }
        }
    }
}