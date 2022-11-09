#if UNITY_EDITOR || LOCAL_SIMULATION
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.Games;
using Solcery.Models.Simulation;
using Solcery.Services.LocalSimulation.Commands;
using Solcery.Services.LocalSimulation.GameStates;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Services.LocalSimulation
{
    public sealed class ServiceLocalSimulation : IServiceLocalSimulation, IServiceLocalSimulationApplyGameStateNew
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
        private IServiceGameState _serviceGameState;
        private IServiceCommands _serviceCommands;

        private Queue<IContextGameStates> _gameStates;

        public static IServiceLocalSimulation Create()
        {
            return new ServiceLocalSimulation();
        }
        
        private ServiceLocalSimulation()
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _serviceGameState = ServiceGameState.Create();
            _serviceCommands = ServiceCommands.Create();
            _simulationModel = SimulationModel.Create();
            _gameStates = new Queue<IContextGameStates>();
        }

        void IServiceLocalSimulation.Init(IGame game)
        {
            _simulationModel.Init(this, game, _serviceCommands, _serviceGameState);
        }

        void IServiceLocalSimulation.PushGameState(JObject gameState)
        {
            if (!gameState.TryGetValue("states", out JArray states) 
                || states.Count <= 0
                || states[0] is not JObject stateObject
                || !stateObject.TryGetValue("value", out JObject gs))
            {
                gs = new JObject
                {
                    {"attrs", new JArray()},
                    {"objects", new JArray()}
                };
                Debug.LogError("Invalid initial game state!");
            }

            _serviceCommands.ClearAllCommand();
            _serviceGameState.PushGameState(gs);
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }

        void IServiceLocalSimulation.PushCommand(JObject command)
        {
            Debug.Log(command.ToString(Formatting.Indented));
            _serviceCommands.PushCommand(command);
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