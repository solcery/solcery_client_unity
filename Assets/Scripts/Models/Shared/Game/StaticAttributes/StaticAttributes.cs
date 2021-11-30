using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

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

        public void ApplyAndUpdateAttributes(EcsWorld world, int entity, JArray attributes)
        {
            foreach (var attributeToken in attributes)
            {
                if (attributeToken is JObject attributeObject
                    && attributeObject.TryGetValue("key", out string key)
                    && _staticAttributes.TryGetValue(key, out var staticAttribute))
                {
                    staticAttribute.Apply(world, entity, attributeObject);
                }
            }
        }
    }
}