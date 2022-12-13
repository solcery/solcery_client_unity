using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.Actions.Sound;

namespace Solcery.Models.Play.Sound
{
    public interface ISystemSoundPlay : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemSoundPlay : ISystemSoundPlay
    {
        private EcsFilter _filter;
        
        public static ISystemSoundPlay Create()
        {
            return new SystemSoundPlay();
        }
        
        private SystemSoundPlay() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld()
                .Filter<ComponentSoundTag>()
                .Inc<ComponentSoundId>()
                .Inc<ComponentSoundVolume>()
                .End();
        }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var poolSoundId = world.GetPool<ComponentSoundId>();
            var poolSoundVolume = world.GetPool<ComponentSoundVolume>();
            foreach (var entityId in _filter)
            {
                game.ServiceSound.Play(poolSoundId.Get(entityId).SoundId, poolSoundVolume.Get(entityId).Volume);
                world.DelEntity(entityId);
            }
        }
    }
}