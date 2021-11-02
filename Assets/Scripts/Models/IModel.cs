using Leopotam.EcsLite;

namespace Solcery.Models
{
    public interface IModel
    {
        public EcsWorld World { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}