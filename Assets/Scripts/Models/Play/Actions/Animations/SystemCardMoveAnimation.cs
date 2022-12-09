using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.Actions.Animation;
using Solcery.Games.States.New.Actions.Animation.Move;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;
using Solcery.Widgets_new;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Models.Play.Actions.Animations
{
    public interface ISystemCardMoveAnimation : IEcsRunSystem { }

    public sealed class SystemCardMoveAnimation : ISystemCardMoveAnimation
    {
        private readonly IWidgetCanvas _widgetCanvas;
        
        public static ISystemCardMoveAnimation Create(IWidgetCanvas widgetCanvas)
        {
            return new SystemCardMoveAnimation(widgetCanvas);
        }

        private SystemCardMoveAnimation(IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
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
                    if (action is UpdateActionAnimationMove actionAnimationCardMove)
                    {
                        Debug.Log($"Play card move animation objId {actionAnimationCardMove.ObjectId} duration {actionAnimationCardMove.DurationMsec} face {actionAnimationCardMove.Face} from place {actionAnimationCardMove.FromPlaceId}");
                        
                        var fromPosition = world.GetPlaceWidget(actionAnimationCardMove.FromPlaceId).GetPosition();
                        var currentPlace = world.GetPlaceWidgetForObjectId(actionAnimationCardMove.ObjectId);
                        var animatedObject = currentPlace.GetAnimatedObject(actionAnimationCardMove.ObjectId);
                        if (animatedObject != null)
                        {
                            var animatedRect = animatedObject.GetAnimatedRect(actionAnimationCardMove.Face);
                            animatedObject.SetActive(false);
                            _widgetCanvas.GetEffects().MoveEclipseCard(animatedRect,
                                actionAnimationCardMove.DurationMsec.ToSec(), fromPosition,
                                () => { animatedObject.SetActive(true); });
                        }
                        else
                        {
                            Debug.Log($"Can't find animation view for object_id = {actionAnimationCardMove.ObjectId}");
                        }
                    }
                }
            }
        }
    }
}