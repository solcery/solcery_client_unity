using Leopotam.EcsLite;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Game.StaticAttributes;
using Solcery.Models.Shared.Game.StaticAttributes.Highlighted;
using Solcery.Models.Shared.Game.StaticAttributes.Interactable;
using Solcery.Models.Shared.Game.StaticAttributes.Place;

namespace Solcery.Models.Shared.Game.Attributes
{
    public interface ISystemStaticAttributesUpdate : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemStaticAttributesUpdate : ISystemStaticAttributesUpdate
    {
        private EcsFilter _filter;
        private IStaticAttributes _staticAttributes;
        
        public static ISystemStaticAttributesUpdate Create()
        {
            return new SystemStaticAttributesUpdate();
        }
        
        private SystemStaticAttributesUpdate() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filter = systems.GetWorld().Filter<ComponentEntityTag>().Inc<ComponentEntityId>()
                .Inc<ComponentEntityAttributes>().End();
            
            _staticAttributes = StaticAttributes.StaticAttributes.Create();
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeHighlighted.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributeInteractable.Create());
            _staticAttributes.RegistrationStaticAttribute(StaticAttributePlace.Create());
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            foreach (var entityId in _filter)
            {
                ref var entityAttributes = ref world.GetPool<ComponentEntityAttributes>().Get(entityId);
                _staticAttributes.ApplyAndUpdateAttributes(world, entityId, entityAttributes.Attributes);
            }
        }
    }
}