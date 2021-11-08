#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.Games;
using Solcery.Services.LocalSimulation;
using UnityEngine;

namespace Solcery.Services.Transport
{
    public sealed class EditorTransportService : ITransportService
    {
        private IEditorLocalSimulationService _localSimulation;
        private readonly List<Action<JObject>> _listEventReceivingGameContent;
        private readonly List<Action<JObject>> _listEventReceivingGameState;

        event Action<JObject> ITransportService.EventReceivingGameContent
        {
            add
            {
                if (!_listEventReceivingGameContent.Contains(value))
                {
                    _listEventReceivingGameContent.Add(value);
                }
            }
            
            remove => _listEventReceivingGameContent.Remove(value);
        }

        event Action<JObject> ITransportService.EventReceivingGameState
        {
            add
            {
                if (!_listEventReceivingGameState.Contains(value))
                {
                    _listEventReceivingGameState.Add(value);
                }
            }

            remove => _listEventReceivingGameState.Remove(value);
        }

        private IGameOnReceivingGameContent _gameOnReceivingGameContent;
        
        public static ITransportService Create(IGameOnReceivingGameContent gameOnReceivingGameContent, IBrickService brickService)
        {
            return new EditorTransportService(gameOnReceivingGameContent, brickService);
        }

        private EditorTransportService(IGameOnReceivingGameContent gameOnReceivingGameContent, IBrickService brickService)
        {
            _localSimulation = EditorLocalSimulationService.Create(brickService);
            _gameOnReceivingGameContent = gameOnReceivingGameContent;
            _listEventReceivingGameContent = new List<Action<JObject>>();
            _listEventReceivingGameState = new List<Action<JObject>>();
        }
        
        void ITransportService.CallUnityLoaded()
        {
            _gameOnReceivingGameContent.OnReceivingGameContent();

            var pathToGameContent = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_content.json");
            var dataContent = JObject.Parse(File.ReadAllText(pathToGameContent));
            CallAllActionWithParams(_listEventReceivingGameContent, dataContent);
            
            var pathToGameState = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_state.json");
            var gameState = JObject.Parse(File.ReadAllText(pathToGameState));
            _localSimulation.EventOnUpdateGameState += OnUpdateGameState;
            _localSimulation.Init(gameState);
        }

        private void OnUpdateGameState(JObject gameState)
        {
            CallAllActionWithParams(_listEventReceivingGameState, gameState);
        }

        private void CallAllActionWithParams(List<Action<JObject>> listActions, JObject @params)
        {
            foreach (var action in listActions)
            {
                action.Invoke(@params);
            }
        }

        void ITransportService.SendCommand(JObject command)
        {
            _localSimulation.ApplyCommand(command);
        }

        void ITransportService.Cleanup()
        {
            Cleanup();
        }
        
        void ITransportService.Destroy()
        {
            Cleanup();
            _localSimulation.Destroy();
            _localSimulation = null;
            _gameOnReceivingGameContent = null;
        }

        private void Cleanup()
        {
            _localSimulation.EventOnUpdateGameState -= OnUpdateGameState;
            _localSimulation.Cleanup();
            _listEventReceivingGameContent.Clear();
            _listEventReceivingGameState.Clear();
        }
    }
}
#endif