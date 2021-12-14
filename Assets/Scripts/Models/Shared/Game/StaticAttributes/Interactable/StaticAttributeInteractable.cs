using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Interactable;

namespace Solcery.Models.Shared.Game.StaticAttributes.Interactable
{
    public sealed class StaticAttributeInteractable : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributeInteractable();
        }
        
        private StaticAttributeInteractable() { }
        
        string IStaticAttribute.Key => "interactable";

        void IStaticAttribute.Apply(EcsWorld world, int entity, int value)
        {
            var pool = world.GetPool<ComponentAttributeInteractable>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Update(value);
        }
        
        void IStaticAttribute.Destroy(EcsWorld world, int entity)
        {
            var pool = world.GetPool<ComponentAttributeInteractable>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }
        }
    }
}