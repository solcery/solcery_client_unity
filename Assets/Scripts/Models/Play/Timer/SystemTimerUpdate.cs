using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Games.States.New.States;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Triggers.EntityTypes;
using UnityEngine;

namespace Solcery.Models.Play.Timer
{
    public interface ISystemTimerUpdate : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }

    public sealed class SystemTimerUpdate : ISystemTimerUpdate
    {
        private IGame _game;
        private EcsFilter _filterTimer;
        
        public static ISystemTimerUpdate Create(IGame game)
        {
            return new SystemTimerUpdate(game);
        }

        private SystemTimerUpdate(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _filterTimer = world.Filter<ComponentTimerTag>()
                .Inc<ComponentTimerFinishTime>()
                .Inc<ComponentTimerTargetObjectId>()
                .End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
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
                    
                    //Debug.Log($"Start timer start time {startTime} and finish time {finishTime}!");
                }
            }

            if (!isNewTimer)
            {
                foreach (var timerEntity in _filterTimer)
                {
                    var t = Mathf.Max(poolTimerDuration.Get(timerEntity).FinishTime - Time.unscaledTime, 0f);
                    _game.WidgetCanvas.GetTimer().text = $"{t:F2} sec";
                    
                    // проверка на то что таймер еще жив
                    if (poolTimerDuration.Get(timerEntity).FinishTime > Time.unscaledTime)
                    {
                        return;
                    }
                    
                    // удалим таймер
                    var targetObjectId = poolTimerTargetObjectId.Get(timerEntity).TargetObjectId;
                    world.DelEntity(timerEntity);
                    var command = CommandOnLeftClickData.CreateFromParameters(targetObjectId, TriggerTargetEntityTypes.Card);
                    _game.TransportService.SendCommand(command.ToJson());
                    _game.WidgetCanvas.GetTimer().text = "FINISH";
                }
            }
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _game = null;
        }
    }
}