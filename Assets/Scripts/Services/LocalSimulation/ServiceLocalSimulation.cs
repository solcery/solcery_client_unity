#if UNITY_EDITOR || LOCAL_SIMULATION
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Games;
using Solcery.Games.Contexts.GameStates;
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

        private Queue<IContextGameStates> _gameStates;

        public static IServiceLocalSimulation Create()
        {
            return new ServiceLocalSimulation();
        }
        
        private ServiceLocalSimulation()
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _serviceCommands = ServiceCommands.Create();
            _simulationModel = SimulationModel.Create();
            _gameStates = new Queue<IContextGameStates>();
        }

        void IServiceLocalSimulation.Init(IGame game, JObject gameState)
        {
            _simulationModel.Init(this, game, _serviceCommands, gameState);
            
            // TODO: fix it
            var gs = new JObject();
            var stateArray = new JArray();
            gs.Add("states", stateArray);
            stateArray.Add(new JObject
            {
                {"id", new JValue(0)},
                {"state_type", new JValue((int)ContextGameStateTypes.GameState)},
                {"value", gameState}
            });

            CallAllActionWithParams(_listOnUpdateGameState, gs);
        }

        void IServiceLocalSimulation.PushCommand(JObject command)
        {
            Debug.Log(command.ToString(Formatting.Indented));
            _serviceCommands.PushCommand(command);
        }

        void IServiceLocalSimulationApplyGameState.ApplySimulatedGameState(JObject gameState)
        {
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }
        
        void IServiceLocalSimulationApplyGameStateNew.ApplySimulatedGameStates(IContextGameStates gameStates)
        {
            _gameStates.Enqueue(gameStates);
        }

        void IServiceLocalSimulation.Update(float dt)
        {
            if (!_serviceCommands.IsEmpty())
            {
                dt /= _serviceCommands.CountCommand();
                while (!_serviceCommands.IsEmpty())
                {
                    _simulationModel?.Update(dt);
                }

                while (_gameStates.Count > 0)
                {
                    var gss = _gameStates.Dequeue();
                    if (gss.TryGetGameState((int) (Time.deltaTime * 1000), out var gsd))
                    {
                        CallAllActionWithParams(_listOnUpdateGameState, gsd);
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