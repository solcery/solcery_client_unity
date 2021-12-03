using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Place;

namespace Solcery.Models.Shared.Game.StaticAttributes.Place
{
    public sealed class StaticAttributePlace : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributePlace();
        }
        
        private StaticAttributePlace() { }
        
        string IStaticAttribute.Key => "place";

        void IStaticAttribute.Apply(EcsWorld world, int entity, int value)
        {
            var pool = world.GetPool<ComponentAttributePlace>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Value = value;
        }
        
        void IStaticAttribute.Destroy(EcsWorld world, int entity)
        {
            var pool = world.GetPool<ComponentAttributePlace>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }
        }
    }
}