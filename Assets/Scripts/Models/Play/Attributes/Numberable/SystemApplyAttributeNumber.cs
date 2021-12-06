using Leopotam.EcsLite;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Attributes.Number;

namespace Solcery.Models.Play.Attributes.Numberable
{
    internal interface ISystemApplyAttributeNumber : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeNumber : ISystemApplyAttributeNumber
    {
        private EcsFilter _filterSubViewComponent;
        private EcsFilter _filterGameStateUpdate;
        
        public static ISystemApplyAttributeNumber Create()
        {
            return new SystemApplyAttributeNumber();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterSubViewComponent = systems.GetWorld().Filter<ComponentPlaceSubWidget>().Inc<ComponentAttributeNumber>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }
            
            var subWidgetComponents = systems.GetWorld().GetPool<ComponentPlaceSubWidget>();
            var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeNumber>();
            foreach (var entity in _filterSubViewComponent)
            {
                var view = subWidgetComponents.Get(entity).Widget.View;
                if (view is INumberable value)
                {
                    value.SetNumber(attributeComponents.Get(entity).Value);
                }
            }
        }
    }
}