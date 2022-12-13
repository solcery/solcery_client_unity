using Leopotam.EcsLite;
using Solcery.Models.Play.Actions.Animation;
using Solcery.Models.Play.Actions.Animation.Move;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Models.Play.Animation.Move
{
    public interface ISystemCardMoveAnimation : IEcsInitSystem, IEcsRunSystem { }

    public sealed class SystemAnimationMove : ISystemCardMoveAnimation
    {
        private readonly IWidgetCanvas _widgetCanvas;

        private EcsFilter _filter;
        
        public static ISystemCardMoveAnimation Create(IWidgetCanvas widgetCanvas)
        {
            return new SystemAnimationMove(widgetCanvas);
        }

        private SystemAnimationMove(IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld()
                .Filter<ComponentAnimationMoveTag>()
                .Inc<ComponentAnimationObjectId>()
                .Inc<ComponentAnimationDuration>()
                .Inc<ComponentAnimationMoveFromPlaceId>()
                .Inc<ComponentAnimationMoveFace>()
                .Exc<ComponentAnimationDelay>()
                .End();
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var objectIdPool = world.GetPool<ComponentAnimationObjectId>();
            var durationPool = world.GetPool<ComponentAnimationDuration>();
            var fromPlaceIdPool = world.GetPool<ComponentAnimationMoveFromPlaceId>();
            var facePool = world.GetPool<ComponentAnimationMoveFace>();

            foreach (var entityId in _filter)
            {
                var objectId = objectIdPool.Get(entityId).ObjectId;
                var durationMSec = durationPool.Get(entityId).DurationMSec;
                var fromPlaceId = fromPlaceIdPool.Get(entityId).FromPlaceId;
                var face = facePool.Get(entityId).Face;
                
                Debug.Log($"Play move animation objId {objectId} duration {durationMSec} face {face} from place {fromPlaceId}");
                
                var fromPosition = world.GetPlaceWidget(fromPlaceId).GetPosition();
                var currentPlace = world.GetPlaceWidgetForObjectId(objectId);
                var animatedObject = currentPlace.GetAnimatedObject(objectId);
                if (animatedObject != null)
                {
                    var animatedRect = animatedObject.GetAnimatedRect(face);
                    animatedObject.SetActive(false);
                    _widgetCanvas.GetEffects().MoveEclipseCard(animatedRect,
                        durationMSec.ToSec(), fromPosition,
                        () => { animatedObject.SetActive(true); });
                }
                else
                {
                    Debug.Log($"Can't find animation view for object_id = {objectId}");
                }
                
                world.DelEntity(entityId);
            }
        }
    }
}