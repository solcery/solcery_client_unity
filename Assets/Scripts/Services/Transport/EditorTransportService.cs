#if UNITY_EDITOR
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
        private IServiceEditorLocalSimulation _localSimulation;
        private IGameTransportCallbacks _gameTransportCallbacks;
        
        public static ITransportService Create(IGameTransportCallbacks gameTransportCallbacks, IServiceBricks serviceBricks)
        {
            return new EditorTransportService(gameTransportCallbacks, serviceBricks);
        }

        private EditorTransportService(IGameTransportCallbacks gameTransportCallbacks, IServiceBricks serviceBricks)
        {
            _localSimulation = ServiceEditorLocalSimulation.Create(serviceBricks);
            _gameTransportCallbacks = gameTransportCallbacks;
        }
        
        void ITransportService.CallUnityLoaded()
        {
            var pathToGameContent = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_content.json");
            _gameTransportCallbacks.OnReceivingGameContent(JObject.Parse(File.ReadAllText(pathToGameContent)));
            
            var pathToGameState = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_state.json");
            var gameState = JObject.Parse(File.ReadAllText(pathToGameState));
            _localSimulation.EventOnUpdateGameState += OnUpdateGameState;
            _localSimulation.Init(gameState);
        }

        private void OnUpdateGameState(JObject gameStateJson)
        {
            _gameTransportCallbacks.OnReceivingGameState(gameStateJson);
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
            _gameTransportCallbacks = null;
        }

        private void Cleanup()
        {
            _localSimulation.EventOnUpdateGameState -= OnUpdateGameState;
            _localSimulation.Cleanup();
        }
    }
}
#endif