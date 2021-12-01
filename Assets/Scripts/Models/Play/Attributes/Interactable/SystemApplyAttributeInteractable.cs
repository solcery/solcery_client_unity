using Leopotam.EcsLite;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Attributes.Interactable;

namespace Solcery.Models.Play.Attributes.Interactable
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

        private SystemApplyAttributeInteractable()
        {
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

            var world = systems.GetWorld();
            var subWidgetComponents = world.GetPool<ComponentPlaceSubWidget>();
            var attributeComponents = world.GetPool<ComponentAttributeInteractable>();
            foreach (var entityId in _filterSubWidgetComponent)
            {
                var view = subWidgetComponents.Get(entityId).Widget.View;
                if (view is IInteractable value)
                {
                    value.SetInteractable(attributeComponents.Get(entityId).Value);
                }
            }
        }
    }
}