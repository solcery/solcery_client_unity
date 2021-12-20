using Leopotam.EcsLite;
using Solcery.Models.Play.Game.State;

namespace Solcery.Models.Play.Attributes.Highlighted
{
    internal interface ISystemApplyAttributeHighlighted : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeHighlighted : ISystemApplyAttributeHighlighted
    {
        private EcsFilter _filterSubViewComponent;
        private EcsFilter _filterGameStateUpdate;
        
        public static ISystemApplyAttributeHighlighted Create()
        {
            return new SystemApplyAttributeHighlighted();
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }
            
            // var subWidgetComponents = systems.GetWorld().GetPool<ComponentPlaceSubWidget>();
            // var attributeComponents = systems.GetWorld().GetPool<ComponentAttributeHighlighted>();
            // foreach (var entity in _filterSubViewComponent)
            // {
            //     var view = subWidgetComponents.Get(entity).Widget.View;
            //     if (view is IHighlighted value)
            //     {
            //         value.SetHighlighted(attributeComponents.Get(entity).Value);
            //     }
            // }
        }
    }
}