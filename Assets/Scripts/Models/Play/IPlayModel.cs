using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.GameContent;

namespace Solcery.Models.Play
{
    public interface IPlayModel
    {
        public EcsWorld World { get; }

        public void Init(IGame game);
        public void Update(float dt);
        public void Destroy();
    }
}