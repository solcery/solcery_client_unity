using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.Actions.Animation;
using UnityEngine;

namespace Solcery.Models.Play.Actions.Animations
{
    public interface ISystemCardMoveAnimation : IEcsRunSystem { }

    public sealed class SystemCardMoveAnimation : ISystemCardMoveAnimation
    {
        public static ISystemCardMoveAnimation Create()
        {
            return new SystemCardMoveAnimation();
        }
        
        private SystemCardMoveAnimation() { }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var updateState = game.UpdateStateQueue.CurrentState;
            var stateId = updateState?.StateId ?? -1;
            if (game.UpdateStateQueue.TryGetActionForStateId(stateId, out var actions))
            {
                foreach (var action in actions)
                {
                    if (action is UpdateActionAnimationCardMove actionAnimationCardMove)
                    {
                        // TODO надо проиграть анимашку
                        Debug.Log($"Play card move animation objId {actionAnimationCardMove.ObjectId} duration {actionAnimationCardMove.DurationMsec} face {actionAnimationCardMove.Face} from place {actionAnimationCardMove.FromPlaceId}");
                    }
                }
            }
        }
    }
}