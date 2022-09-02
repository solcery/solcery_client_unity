using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.States;

namespace Solcery.Models.Play.Sound
{
    public interface ISystemSoundPlay : IEcsRunSystem { }

    public sealed class SystemSoundPlay : ISystemSoundPlay
    {
        public static ISystemSoundPlay Create()
        {
            return new SystemSoundPlay();
        }
        
        private SystemSoundPlay() { }

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var updateState = game.UpdateStateQueue.CurrentState;
            if (updateState is UpdatePlaySoundState updatePlaySoundState)
            {
                game.ServiceSound.Play(updatePlaySoundState.SoundId);
            }
        }
    }
}