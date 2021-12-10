using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Highlighted;

namespace Solcery.Models.Shared.Game.StaticAttributes.Highlighted
{
    public sealed class StaticAttributeHighlighted : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributeHighlighted();
        }
        
        private StaticAttributeHighlighted() { }
        
        string IStaticAttribute.Key => "highlighted";

        void IStaticAttribute.Apply(EcsWorld world, int entity, int value)
        {
            var pool = world.GetPool<ComponentAttributeHighlighted>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Update(value);
        }

        void IStaticAttribute.Destroy(EcsWorld world, int entity)
        {
            var pool = world.GetPool<ComponentAttributeHighlighted>();
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }
        }
    }
}