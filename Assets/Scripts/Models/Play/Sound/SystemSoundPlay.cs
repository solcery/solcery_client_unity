using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.Actions.PlaySound;

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
            var stateId = updateState?.StateId ?? -1;
            if (game.UpdateStateQueue.TryGetActionForStateId(stateId, out var actions))
            {
                foreach (var action in actions)
                {
                    if (action is UpdateActionPlaySound actionPlaySound)
                    {
                        game.ServiceSound.Play(actionPlaySound.SoundId, actionPlaySound.Volume);
                    }
                }
            }
        }
    }
}