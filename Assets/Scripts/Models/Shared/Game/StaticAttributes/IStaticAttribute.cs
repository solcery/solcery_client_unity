using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Game.StaticAttributes
{
    public interface IStaticAttribute
    {
        string Key { get; }

        void Apply(EcsWorld world, int entity, int value);
        void Destroy(EcsWorld world, int entity);
    }
}