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
                .Inc<ComponentTimerDuration>()
                .Inc<ComponentTimerTargetObjectId>()
                .End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var poolTimerDuration = world.GetPool<ComponentTimerDuration>();
            var poolTimerTargetObjectId = world.GetPool<ComponentTimerTargetObjectId>();
            var isNewTimer = false;
            
            // обновим таймер если нужно
            var updateState = _game.UpdateStateQueue.CurrentState;
            if (updateState is UpdateTimerState updateTimerState)
            {
                // удалим все текущие таймеры
                foreach (var timerEntity in _filterTimer)
                {
                    world.DelEntity(timerEntity);
                }

                // если требуется, запустим новый таймер
                if (updateTimerState.IsStart)
                {
                    var timerEntity = world.NewEntity();
                    world.GetPool<ComponentTimerTag>().Add(timerEntity);
                    poolTimerDuration.Add(timerEntity).DurationMsec = updateTimerState.DurationMsec;
                    poolTimerTargetObjectId.Add(timerEntity).TargetObjectId = updateTimerState.TargetObjectId;
                    isNewTimer = true;
                }
            }

            if (!isNewTimer)
            {
                foreach (var timerEntity in _filterTimer)
                {
                    var duration = poolTimerDuration.Get(timerEntity).DurationMsec - (int)(Time.deltaTime * 1000);

                    // проверка на то что таймер еще жив
                    if (duration > 0)
                    {
                        poolTimerDuration.Get(timerEntity).DurationMsec = duration;
                        return;
                    }
                    
                    // удалим таймер
                    world.DelEntity(timerEntity);
                    var targetObjectId = poolTimerTargetObjectId.Get(timerEntity).TargetObjectId;
                    var command = CommandOnLeftClickData.CreateFromParameters(targetObjectId, TriggerTargetEntityTypes.Card);
                    _game.TransportService.SendCommand(command.ToJson());
                }
            }
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            _game = null;
        }
    }
}