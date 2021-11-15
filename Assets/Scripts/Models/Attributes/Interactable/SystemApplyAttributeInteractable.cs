using Leopotam.EcsLite;
using Solcery.Models.Places;
using Solcery.Models.Triggers;
using Solcery.Widgets.Data;

namespace Solcery.Models.Attributes.Interactable
{
    internal interface ISystemApplyAttributeInteractable : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeInteractable : ISystemApplyAttributeInteractable
    {
        private EcsFilter _filterWidgetViewComponent;
        
        public static SystemApplyAttributeInteractable Create()
        {
            return new SystemApplyAttributeInteractable();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterWidgetViewComponent = systems.GetWorld().Filter<ComponentPlaceWidgetView>().Inc<ComponentAttributeInteractable>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var widgetViewComponents = systems.GetWorld().GetPool<ComponentPlaceWidgetView>();
            var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeInteractable>();
            foreach (var entity in _filterWidgetViewComponent)
            {
                var view = widgetViewComponents.Get(entity).View;
                if (view is IInteractable value)
                {
                    value.OnClick = () => { OnClick(systems.GetWorld(), entity); };
                    value.SetInteractable(attributeComponents.Get(entity).Value);
                }

                attributeComponents.Del(entity);
            }
        }

        private void OnClick(EcsWorld world, int entityId)
        {
            var triggerPool = world.GetPool<ComponentApplyTrigger>();
            if (!triggerPool.Has(entityId))
            {
                triggerPool.Add(entityId).Type = TriggerTypes.OnClick;
            }
        }
    }
}