#if UNITY_EDITOR || LOCAL_SIMULATION
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.EditorSimulation;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;
//using UnityEngine;

namespace Solcery.Services.Transport
{
    public sealed class EditorTransportService : ITransportService
    {
        private IGame _game;
        private IServiceLocalSimulation _localSimulation;
        private IServiceEditorSimulation _editorSimulation;
        private IGameTransportCallbacks _gameTransportCallbacks;
        
        public static ITransportService Create(IGameTransportCallbacks gameTransportCallbacks, IGame game)
        {
            return new EditorTransportService(gameTransportCallbacks, game);
        }

        private EditorTransportService(IGameTransportCallbacks gameTransportCallbacks, IGame game)
        {
            _game = game;
            _localSimulation = ServiceLocalSimulation.Create();
            _editorSimulation = ServiceEditorSimulation.Create();
            _editorSimulation.EventOnSendGameState += OnCommandsReceive;
            _gameTransportCallbacks = gameTransportCallbacks;
        }

        private void OnCommandsReceive(JObject obj)
        {
            if (obj.TryGetValue("commands", out JArray commands))
            {
                foreach (var commandToken in commands)
                {
                    if (commandToken is JObject command)
                    {
                        _localSimulation.PushCommand(command);
                    }
                }
            }
        }

        void ITransportService.CallUnityLoaded(JObject metadata)
        {
            StreamingAssetsUtils.LoadText("LocalSimulationData/game_content.json", OnGameContentLoaded);
        }

        private void OnGameContentLoaded(string obj)
        {
            _gameTransportCallbacks.OnReceivingGameContent(obj != null ? JObject.Parse(obj) : null);
            StreamingAssetsUtils.LoadText("LocalSimulationData/game_content_overrides.json", OnGameContentOverridesLoaded);
        }

        private void OnGameContentOverridesLoaded(string obj)
        {
            _gameTransportCallbacks.OnReceivingGameContentOverrides(obj != null ? JObject.Parse(obj) : null);
            StreamingAssetsUtils.LoadText("LocalSimulationData/game_state.json", OnGameStateLoaded);
        }

        private void OnGameStateLoaded(string obj)
        {
            var gameState = JObject.Parse(obj);
            _localSimulation.EventOnUpdateGameState += OnUpdateGameState;
            _localSimulation.Init(_game, gameState);
            OnCommandsReceive(gameState);
        }

        private void OnUpdateGameState(JObject gameStateJson)
        {
            //Debug.Log($"OnUpdateGameState {gameStateJson}");
            _gameTransportCallbacks.OnReceivingGameState(gameStateJson);
        }

        void ITransportService.SendCommand(JObject command)
        {
            _editorSimulation.SendCommand(command);
            _localSimulation.PushCommand(command);
        }
        
        void ITransportService.Update(float dt)
        {
            _localSimulation.Update(dt);
            _editorSimulation.Update(dt);
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
            _gameTransportCallbacks = null;

            _editorSimulation.EventOnSendGameState -= OnCommandsReceive;
            _editorSimulation = null;
        }

        private void Cleanup()
        {
            _localSimulation.EventOnUpdateGameState -= OnUpdateGameState;
            _localSimulation.Cleanup();
        }
    }
}
#endif