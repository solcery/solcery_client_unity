using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Number;

namespace Solcery.Models.Shared.Game.StaticAttributes.Number
{
    public sealed class StaticAttributeNumber : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributeNumber();
        }
        
        private StaticAttributeNumber() { }
        
        string IStaticAttribute.Key => "number";

        void IStaticAttribute.Apply(EcsWorld world, int entity, int value)
        {
            var pool = world.GetPool<ComponentAttributeNumber>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Value = value;
        }
        
        void IStaticAttribute.Destroy(EcsWorld world, int entity)
        {
            var pool = world.GetPool<ComponentAttributeNumber>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }
        }
    }
}