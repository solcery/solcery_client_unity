using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Models.Shared.Triggers.Apply.Card.OnClick;

namespace Solcery.Models.Shared.Triggers.Apply
{
    public interface ISystemsTriggerApply : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem { }

    public sealed class SystemsTriggerApply : ISystemsTriggerApply
    {
        private List<IEcsSystem> _systems;

        public static ISystemsTriggerApply Create(IServiceBricks serviceBricks)
        {
            return new SystemsTriggerApply(serviceBricks);
        }
        
        private SystemsTriggerApply(IServiceBricks serviceBricks)
        {
            _systems = new List<IEcsSystem>
            {
                SystemTriggerApplyCardOnClick.Create(serviceBricks)
            };
        }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            foreach (var system in _systems)
            {
                if (system is IEcsInitSystem initSystem)
                {
                    initSystem.Init(systems);
                }
            }
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            foreach (var system in _systems)
            {
                if (system is IEcsRunSystem runSystem)
                {
                    runSystem.Run(systems);
                }
            }
        }

        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            foreach (var system in _systems)
            {
                if (system is IEcsDestroySystem destroySystem)
                {
                    destroySystem.Destroy(systems);
                }
            }
            
            _systems.Clear();
            _systems = null;
        }
    }
}