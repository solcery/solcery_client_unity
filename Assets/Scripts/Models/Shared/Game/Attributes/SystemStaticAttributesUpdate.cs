using Leopotam.EcsLite;
using Solcery.Models.Shared.Game.StaticAttributes;
using Solcery.Models.Shared.Game.StaticAttributes.Highlighted;
using Solcery.Models.Shared.Game.StaticAttributes.Interactable;
using Solcery.Models.Shared.Game.StaticAttributes.Place;
using Solcery.Models.Shared.Objects;

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
            _filter = systems.GetWorld().Filter<ComponentObjectTag>().Inc<ComponentObjectId>()
                .Inc<ComponentObjectAttributes>().End();
            
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
                ref var entityAttributes = ref world.GetPool<ComponentObjectAttributes>().Get(entityId);
                _staticAttributes.ApplyAndUpdateAttributes(world, entityId, entityAttributes.Attributes);
            }
        }
    }
}