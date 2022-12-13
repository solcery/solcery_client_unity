using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.Actions.Animation.Move;
using Solcery.Models.Play.Actions.Animation.Rotate;
using Solcery.Models.Play.Actions.Animation.Sequence;
using Solcery.Models.Play.Actions.Animation.Visibility;
using Solcery.Models.Play.Actions.Factory;
using Solcery.Models.Play.Actions.Sound;

namespace Solcery.Models.Play.Actions
{
    public interface ISystemActionApply : IEcsRunSystem { }

    public sealed class SystemActionApply : ISystemActionApply
    {
        private readonly IActionObjectFactory _factory;
        
        public static ISystemActionApply Create()
        {
            return new SystemActionApply();
        }

        private SystemActionApply()
        {
            _factory = ActionObjectFactory
                .Create()
                .RegistrationCreationMethod(CreatorSound.Create)
                .RegistrationCreationMethod(CreatorAnimationSequence.Create)
                .RegistrationCreationMethod(CreatorAnimationMove.Create)
                .RegistrationCreationMethod(CreatorAnimationRotate.Create)
                .RegistrationCreationMethod(CreatorAnimationVisibility.Create);
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var game = systems.GetShared<IGame>();
            var updateState = game.UpdateStateQueue.CurrentState;
            var stateId = updateState?.StateId ?? -1;
            if (game.UpdateStateQueue.TryGetActionForStateId(stateId, out var actions))
            {
                foreach (var action in actions)
                {
                    _factory.ApplyAction(_factory, world, action);
                }
            }
            game.UpdateStateQueue.RemoveAllActionForStateId(stateId);
        }
    }
}