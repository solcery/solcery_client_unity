using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;

namespace Solcery.Models.Play
{
    public interface IPlayModel
    {
        public EcsWorld World { get; }

        public void Init(IGame game, JObject gameContentJson);
        public void Update(float dt);
        public void Destroy();
    }
}