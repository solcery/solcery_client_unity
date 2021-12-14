using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Game.StaticAttributes
{
    public interface IStaticAttributes
    {
        void RegistrationStaticAttribute(IStaticAttribute staticAttribute);
        void Cleanup();
        void ApplyAndUpdateAttributes(EcsWorld world, int entity, IReadOnlyDictionary<string, IAttributeValue> attributes);
    }
}