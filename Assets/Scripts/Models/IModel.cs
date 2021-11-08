using Leopotam.EcsLite;
using Solcery.Games;

namespace Solcery.Models
{
    public interface IModel
    {
        public EcsWorld World { get; }

        public void Init(IGame game);
        public void Update(float dt);
        public void Destroy();
    }
}