#if UNITY_EDITOR || LOCAL_SIMULATION
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Models.Simulation;
using Solcery.Services.Commands;
using UnityEngine;

namespace Solcery.Services.LocalSimulation
{
    public sealed class ServiceLocalSimulation : IServiceLocalSimulation, IServiceLocalSimulationApplyGameState, IServiceLocalSimulationApplyGameStateNew
    {
        event Action<JObject> IServiceLocalSimulation.EventOnUpdateGameState
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
        private ISimulationModel _simulationModel;
        private IServiceCommands _serviceCommands;
        private IServiceBricks _serviceBricks;

        private IContextGameStates _gameStates;

        public static IServiceLocalSimulation Create(IServiceBricks serviceBricks)
        {
            return new ServiceLocalSimulation(serviceBricks);
        }
        
        private ServiceLocalSimulation(IServiceBricks serviceBricks)
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _serviceCommands = ServiceCommands.Create();
            _serviceBricks = serviceBricks;
            _simulationModel = SimulationModel.Create();
        }

        void IServiceLocalSimulation.Init(JObject gameContent, JObject gameState)
        {
            _simulationModel.Init(this, _serviceCommands, _serviceBricks, gameContent, gameState);
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }

        void IServiceLocalSimulation.PushCommand(JObject command)
        {
            // TODO: fix it hack!!!
            if (_gameStates is {IsEmpty: false})
            {
                return;
            }
            
            Debug.Log(command.ToString(Formatting.Indented));
            _serviceCommands.PushCommand(command);
        }

        void IServiceLocalSimulationApplyGameState.ApplySimulatedGameState(JObject gameState)
        {
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }
        
        void IServiceLocalSimulationApplyGameStateNew.ApplySimulatedGameStates(IContextGameStates gameStates)
        {
            _gameStates = gameStates;
        }

        void IServiceLocalSimulation.Update(float dt)
        {
            if (_serviceCommands.IsEmpty())
            {
                return;
            }
            
            dt /= _serviceCommands.CountCommand();
            while (!_serviceCommands.IsEmpty())
            {
                _simulationModel?.Update(dt);

                if (_gameStates != null)
                {
                    while (!_gameStates.IsEmpty)
                    {
                        if (_gameStates.TryGetGameState((int)(Time.deltaTime * 1000), out var gsd))
                        {
                            CallAllActionWithParams(_listOnUpdateGameState, gsd);
                        }
                    }
                }
            }
        }

        void IServiceLocalSimulation.Cleanup()
        {
            Cleanup();
            _serviceCommands.Cleanup();
        }

        private void Cleanup()
        {
            _listOnUpdateGameState.Clear();
        }

        void IServiceLocalSimulation.Destroy()
        {
            Cleanup();
            _simulationModel = null;
            _serviceCommands.Destroy();
            _serviceCommands = null;
            _serviceBricks = null;
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