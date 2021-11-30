using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Attributes.Interactable;
using Solcery.Models.Play.GameState.StaticAttributes;
using Solcery.Utils;

namespace Solcery.Models.Play.Game.State.StaticAttributes.Interactable
{
    public sealed class StaticAttributeInteractable : IStaticAttribute
    {
        public static IStaticAttribute Create()
        {
            return new StaticAttributeInteractable();
        }
        
        private StaticAttributeInteractable() { }
        
        string IStaticAttribute.Key => "interactable";

        void IStaticAttribute.Apply(EcsWorld world, int entity, JObject attribute)
        {
            var pool = world.GetPool<ComponentAttributeInteractable>();
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }

            ref var component = ref pool.Get(entity);
            component.Value = attribute.GetValue<int>("value") == 1;
        }
    }
}