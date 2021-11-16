using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.GameState.StaticAttributes
{
    public interface IStaticAttribute
    {
        string Key { get; }

        void Apply(EcsWorld world, int entity, JObject attribute);
    }
}