using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Attributes.Interactable;
using Solcery.Services.Transport;

namespace Solcery.Models.Play.Attributes.Interactable
{
    internal interface ISystemApplyAttributeInteractable : IEcsInitSystem, IEcsRunSystem
    {
    }
    
    internal sealed class SystemApplyAttributeInteractable : ISystemApplyAttributeInteractable
    {
        private ITransportService _transportService;
        private EcsFilter _filterSubWidgetComponent;
        private EcsFilter _filterGameStateUpdate;
        
        public static SystemApplyAttributeInteractable Create(ITransportService transportService)
        {
            return new SystemApplyAttributeInteractable(transportService);
        }

        private SystemApplyAttributeInteractable(ITransportService transportService)
        {
            _transportService = transportService;
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
                    value.OnClick = () => { OnClick(entity); };
                    value.SetInteractable(attributeComponents.Get(entity).Value);
                }
            }
        }

        private void OnClick(int entityId)
        {
            _transportService.SendCommand(new JObject
            {
                ["entity_id"] = new JValue(entityId),
                ["trigger_type"] = new JValue(1),
                ["trigger_target_entity_type"] = new JValue(1)
            });
        }
    }
}