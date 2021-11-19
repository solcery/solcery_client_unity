using Leopotam.EcsLite;
using Solcery.Models.GameState;
using Solcery.Models.Places;
using Solcery.Models.Triggers;

namespace Solcery.Models.Attributes.Interactable
{
    internal interface ISystemApplyAttributeInteractable : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeInteractable : ISystemApplyAttributeInteractable
    {
        private EcsFilter _filterSubWidgetComponent;
        private EcsFilter _filterGameStateUpdate;
        
        public static SystemApplyAttributeInteractable Create()
        {
            return new SystemApplyAttributeInteractable();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterSubWidgetComponent = systems.GetWorld().Filter<ComponentPlaceSubWidget>().Inc<ComponentAttributeInteractable>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }
            
            var subWidgetComponents = systems.GetWorld().GetPool<ComponentPlaceSubWidget>();
            var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeInteractable>();
            foreach (var entity in _filterSubWidgetComponent)
            {
                var view = subWidgetComponents.Get(entity).Widget.View;
                if (view is IInteractable value)
                {
                    value.OnClick = () => { OnClick(systems.GetWorld(), entity); };
                    value.SetInteractable(attributeComponents.Get(entity).Value);
                }
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