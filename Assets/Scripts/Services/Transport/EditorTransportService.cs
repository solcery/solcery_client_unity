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
        private IEditorLocalSimulationService _localSimulation;
        private IGameOnReceivingData _gameOnReceivingData;
        
        public static ITransportService Create(IGameOnReceivingData gameOnReceivingData, IBrickService brickService)
        {
            return new EditorTransportService(gameOnReceivingData, brickService);
        }

        private EditorTransportService(IGameOnReceivingData gameOnReceivingData, IBrickService brickService)
        {
            _localSimulation = EditorLocalSimulationService.Create(brickService);
            _gameOnReceivingData = gameOnReceivingData;
        }
        
        void ITransportService.CallUnityLoaded()
        {
            var pathToGameContent = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_content.json");
            _gameOnReceivingData.OnReceivingGameContent(JObject.Parse(File.ReadAllText(pathToGameContent)));
            
            var pathToGameState = Path.GetFullPath($"{Application.dataPath}/LocalSimulationData/game_state.json");
            var gameState = JObject.Parse(File.ReadAllText(pathToGameState));
            _localSimulation.EventOnUpdateGameState += OnUpdateGameState;
            _localSimulation.Init(gameState);
        }

        private void OnUpdateGameState(JObject gameStateJson)
        {
            _gameOnReceivingData.OnReceivingGameState(gameStateJson);
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
            _gameOnReceivingData = null;
        }

        private void Cleanup()
        {
            _localSimulation.EventOnUpdateGameState -= OnUpdateGameState;
            _localSimulation.Cleanup();
        }
    }
}
#endif