#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.Models.Simulation;

namespace Solcery.Services.LocalSimulation
{
    public sealed class ServiceEditorLocalSimulation : IServiceEditorLocalSimulation
    {
        private ISimulationModel _simulationModel;
        private readonly Queue<JObject> _commands;

        event Action<JObject> IServiceEditorLocalSimulation.EventOnUpdateGameState
        {
            add
            {
                if (!_listOnUpdateGameState.Contains(value))
                {
                    _listOnUpdateGameState.Add(value);
                }
            }
            
            remove => _listOnUpdateGameState.Remove(value);
        }

        private readonly List<Action<JObject>> _listOnUpdateGameState;
        private IServiceBricks _serviceBricks;

        public static IServiceEditorLocalSimulation Create(IServiceBricks serviceBricks)
        {
            return new ServiceEditorLocalSimulation(serviceBricks);
        }
        
        private ServiceEditorLocalSimulation(IServiceBricks serviceBricks)
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _serviceBricks = serviceBricks;
            _commands = new Queue<JObject>();
            _simulationModel = SimulationModel.Create();
        }

        void IServiceEditorLocalSimulation.Init(JObject gameContent, JObject gameState)
        {
            _simulationModel.Init(this, gameContent, gameState);
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }

        void IServiceEditorLocalSimulation.ApplyCommand(JObject command)
        {
            _commands.Enqueue(command);
        }

        void IServiceEditorLocalSimulation.ApplySimulatedGameState(JObject gameState)
        {
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }

        void IServiceEditorLocalSimulation.Update(float dt)
        {
            if (_commands.Count <= 0)
            {
                return;
            }
            
            dt /= _commands.Count;
            while (_commands.Count > 0)
            {
                var command = _commands.Dequeue();
                _simulationModel?.Update(dt);
            }
        }

        void IServiceEditorLocalSimulation.Cleanup()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _listOnUpdateGameState.Clear();
            _commands.Clear();
        }

        void IServiceEditorLocalSimulation.Destroy()
        {
            Cleanup();
        }
        
        private void CallAllActionWithParams(List<Action<JObject>> listActions, JObject @params)
        {
            foreach (var action in listActions)
            {
                action.Invoke(@params);
            }
        }
    }
}
#endif