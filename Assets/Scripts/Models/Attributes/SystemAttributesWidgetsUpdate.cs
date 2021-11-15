using Leopotam.EcsLite;
using Solcery.Models.Attributes.Highlighted;
using Solcery.Models.Attributes.Interactable;
using Solcery.Models.Entities;
using Solcery.Models.GameState;
using Solcery.Models.Places;

namespace Solcery.Models.Attributes
{
    public interface ISystemAttributesWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    public class SystemAttributesWidgetsUpdate : ISystemAttributesWidgetsUpdate
    {
        private EcsFilter _filterEntities;
        private EcsFilter _filterGameStateUpdate;
        
        public static SystemAttributesWidgetsUpdate Create()
        {
            return new SystemAttributesWidgetsUpdate();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterEntities = systems.GetWorld().Filter<ComponentEntityTag>().Inc<ComponentEntityView>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var entityIndex in _filterEntities)
            {
                ref var attributes = ref systems.GetWorld().GetPool<ComponentEntityAttributes>().Get(entityIndex);
                if (attributes.Attributes.TryGetValue("highlighted", out var highlighted))
                {
                    systems.GetWorld().GetPool<ComponentAttributeHighlighted>().Add(entityIndex).Value = highlighted == 1;
                }
                if (attributes.Attributes.TryGetValue("interactable", out var interactable))
                {
                    systems.GetWorld().GetPool<ComponentAttributeInteractable>().Add(entityIndex).Value = interactable == 1;
                }
            }        
        }
    }
}