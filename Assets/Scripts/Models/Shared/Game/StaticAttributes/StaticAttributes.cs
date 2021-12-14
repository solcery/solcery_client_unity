using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Game.StaticAttributes
{
    public sealed class StaticAttributes : IStaticAttributes
    {
        private readonly Dictionary<string, IStaticAttribute> _staticAttributes;

        public static IStaticAttributes Create()
        {
            return new StaticAttributes();
        }

        private StaticAttributes()
        {
            _staticAttributes = new Dictionary<string, IStaticAttribute>();
        }

        public void RegistrationStaticAttribute(IStaticAttribute staticAttribute)
        {
            if (!_staticAttributes.ContainsKey(staticAttribute.Key))
            {
                _staticAttributes.Add(staticAttribute.Key, staticAttribute);
                return;
            }

            throw new Exception(
                $"Double static attribute {staticAttribute.GetType().Name} for key {staticAttribute.Key}");
        }

        public void Cleanup()
        {
            _staticAttributes.Clear();
        }

        public void ApplyAndUpdateAttributes(EcsWorld world, int entity, IReadOnlyDictionary<string, IAttributeValue> attributes)
        {
            foreach (var staticAttribute in _staticAttributes)
            {
                if (attributes.TryGetValue(staticAttribute.Key, out var value))
                {
                    staticAttribute.Value.Apply(world, entity, value.Current);
                }
                else
                {
                    staticAttribute.Value.Destroy(world, entity);
                }
            }
        }
    }
}