using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.States;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Models.Shared.Triggers.EntityTypes;
using UnityEngine;

namespace Solcery.Models.Play.Timer
{
    public interface ISystemTimerUpdate : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemTimerUpdate : ISystemTimerUpdate
    {
        private IGame _game;
        private EcsFilter _filterTimer;
        private EcsFilter _filterObjects;
        private EcsFilter _filterPlaces;
        
        public static ISystemTimerUpdate Create(IGame game)
        {
            return new SystemTimerUpdate(game);
        }

        private SystemTimerUpdate(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterTimer = world.Filter<ComponentTimerTag>()
                .Inc<ComponentTimerFinishTime>()
                .Inc<ComponentTimerTargetObjectId>()
                .End();
            _filterObjects = world.Filter<ComponentObjectTag>()
                .Inc<ComponentObjectId>()
                .End();
            _filterPlaces = world.Filter<ComponentPlaceTag>()
                .Inc<ComponentPlaceId>()
                .Inc<ComponentPlaceWidgetNew>()
                .End();
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var poolTimerDuration = world.GetPool<ComponentTimerFinishTime>();
            var poolTimerTargetObjectId = world.GetPool<ComponentTimerTargetObjectId>();
            var isNewTimer = false;
            
            // обновим таймер если нужно
            var updateState = _game.UpdateStateQueue.CurrentState;
            if (updateState is UpdateTimerState updateTimerState)
            {
                _game.WidgetCanvas.GetTimer().text = "FINISH";
                // удалим все текущие таймеры
                foreach (var timerEntity in _filterTimer)
                {
                    StopTimer(world, timerEntity);
                    world.DelEntity(timerEntity);
                    //Debug.Log("Stop timer!");
                }

                // если требуется, запустим новый таймер
                if (updateTimerState.IsStart)
                {
                    var timerEntity = world.NewEntity();
                    world.GetPool<ComponentTimerTag>().Add(timerEntity);

                    var startTime = Time.unscaledTime;
                    var finishTime = startTime + updateTimerState.DurationMsec / 1000f;
                    
                    poolTimerDuration.Add(timerEntity).FinishTime = finishTime;
                    poolTimerTargetObjectId.Add(timerEntity).TargetObjectId = updateTimerState.TargetObjectId;
                    isNewTimer = true;
                    StartTimer(world, timerEntity, updateTimerState.DurationMsec);
                    //Debug.Log($"Start timer start time {startTime} and finish time {finishTime}!");
                }
            }

            if (!isNewTimer)
            {
                foreach (var timerEntity in _filterTimer)
                {
                    var t = Mathf.Max(poolTimerDuration.Get(timerEntity).FinishTime - Time.unscaledTime, 0f);
                    _game.WidgetCanvas.GetTimer().text = $"{t:F2} sec";
                    
                    UpdateTimer(world, timerEntity, (int)(t * 1000));
                    
                    // проверка на то что таймер еще жив
                    if (poolTimerDuration.Get(timerEntity).FinishTime > Time.unscaledTime)
                    {
                        return;
                    }
                    
                    // удалим таймер
                    var targetObjectId = poolTimerTargetObjectId.Get(timerEntity).TargetObjectId;
                    StopTimer(world, timerEntity);
                    world.DelEntity(timerEntity);
                    var command = CommandOnLeftClickData.CreateFromParameters(targetObjectId, TriggerTargetEntityTypes.Card);
                    _game.TransportService.SendCommand(command.ToJson());
                    _game.WidgetCanvas.GetTimer().text = "FINISH";
                }
            }
        }

        void IEcsDestroySystem.Destroy(IEcsSystems systems)
        {
            _game = null;
        }

        private void StartTimer(EcsWorld world, int timerEntityId, int durationMsec)
        {
            var poolTargetObjectId = world.GetPool<ComponentTimerTargetObjectId>();
            var targetObjectId = poolTargetObjectId.Get(timerEntityId).TargetObjectId;

            var poolObjectId = world.GetPool<ComponentObjectId>();
            var poolObjectAttrs = world.GetPool<ComponentObjectAttributes>();
            foreach (var objectEntityId in _filterObjects)
            {
                if (poolObjectId.Get(objectEntityId).Id != targetObjectId)
                {
                    continue;
                }

                var attrs = poolObjectAttrs.Get(objectEntityId).Attributes;
                var objectPlaceId = attrs["place"].Current;

                var poolPlaceId = world.GetPool<ComponentPlaceId>();
                var poolPlaceWidget = world.GetPool<ComponentPlaceWidgetNew>();
                foreach (var placeEntityId in _filterPlaces)
                {
                    if (poolPlaceId.Get(placeEntityId).Id != objectPlaceId)
                    {
                        continue;
                    }

                    var placeLayout = poolPlaceWidget.Get(placeEntityId).Widget.LayoutForObjectId(targetObjectId);
                    var poolTimerTargetObjectPlaceLayout = world.GetPool<ComponentTimerTargetObjectPlaceLayout>();
                    poolTimerTargetObjectPlaceLayout.Add(timerEntityId).Layout = placeLayout;
                    if(placeLayout != null)
                    {
                        placeLayout.StartTimer(durationMsec);
                    }

                    break;
                }
                
                break;
            }
        }

        private void UpdateTimer(EcsWorld world, int timerEntityId, int durationMsec)
        {
            var poolTimerTargetObjectPlaceLayout = world.GetPool<ComponentTimerTargetObjectPlaceLayout>();
            var placeLayout = poolTimerTargetObjectPlaceLayout.Get(timerEntityId).Layout;
            if (placeLayout != null)
            {
                placeLayout.UpdateTimer(durationMsec);
            }
        }

        private void StopTimer(EcsWorld world, int timerEntityId)
        {
            var poolTimerTargetObjectPlaceLayout = world.GetPool<ComponentTimerTargetObjectPlaceLayout>();
            var placeLayout = poolTimerTargetObjectPlaceLayout.Get(timerEntityId).Layout;
            if (placeLayout != null)
            {
                placeLayout.StopTimer();
            }
        }
    }
}