using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Attributes.Place;
using Solcery.Models.Play.GameState.StaticAttributes;
using Solcery.Utils;

namespace Solcery.Models.Play.Game.State.StaticAttributes.Place
{
    public sealed class StaticAttributePlace : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributePlace();
        }
        
        private StaticAttributePlace() { }
        
        string IStaticAttribute.Key => "place";

        void IStaticAttribute.Apply(EcsWorld world, int entity, JObject attribute)
        {
            var pool = world.GetPool<ComponentAttributePlace>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Value = attribute.GetValue<int>("value");
        }
    }
}