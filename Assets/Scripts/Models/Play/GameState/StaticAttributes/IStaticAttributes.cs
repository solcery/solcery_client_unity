using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.GameState.StaticAttributes
{
    public interface IStaticAttributes
    {
        void RegistrationStaticAttribute(IStaticAttribute staticAttribute);
        void Cleanup();
        void ApplyAndUpdateAttributes(EcsWorld world, int entity, JArray attributes);
    }
}