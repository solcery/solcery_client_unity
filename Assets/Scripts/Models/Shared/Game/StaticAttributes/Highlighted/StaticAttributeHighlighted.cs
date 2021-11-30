using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Attributes.Highlighted;
using Solcery.Utils;

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

        void IStaticAttribute.Apply(EcsWorld world, int entity, JObject attribute)
        {
            var pool = world.GetPool<ComponentAttributeHighlighted>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Value = attribute.GetValue<int>("value") == 1;
        }
    }
}