using Leopotam.EcsLite;
using Solcery.Models.Systems;

namespace Solcery.Models
{
    public sealed class PlayModel : IModel
    {
        public EcsWorld World { get; private set; }
        
        private EcsSystems _systems;

        public static IModel Create()
        {
            return new PlayModel();
        }

        private PlayModel() { }
        
        void IModel.Init()
        {
            World = new EcsWorld();
            _systems = new EcsSystems(World);
            _systems.Add(new ApplyTriggerSystem());
            _systems.Add(new UiUpdateSystem());
            
            // TODO сюда добавляем новые системы и тд
            
            _systems.Init();
        }

        void IModel.Update(float dt)
        {
            _systems?.Run();
        }
        
        void IModel.Destroy()
        {
            _systems?.Destroy();
            _systems = null;
            
            World?.Destroy();
            World = null;
        }
    }
}