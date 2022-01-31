#if UNITY_EDITOR || LOCAL_SIMULATION
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;

namespace Solcery.Services.Transport
{
    public sealed class EditorTransportService : ITransportService
    {
        private IServiceLocalSimulation _localSimulation;
        private IGameTransportCallbacks _gameTransportCallbacks;
        
        public static ITransportService Create(IGameTransportCallbacks gameTransportCallbacks, IServiceBricks serviceBricks)
        {
            return new EditorTransportService(gameTransportCallbacks, serviceBricks);
        }

        private EditorTransportService(IGameTransportCallbacks gameTransportCallbacks, IServiceBricks serviceBricks)
        {
            _localSimulation = ServiceLocalSimulation.Create(serviceBricks);
            _gameTransportCallbacks = gameTransportCallbacks;
        }
        
        void ITransportService.CallUnityLoaded()
        {
            StreamingAssetsUtils.LoadText("LocalSimulationData/game_content.json", OnGameContentLoaded);
        }

        private JObject _gameContent;

        private void OnGameContentLoaded(string obj)
        {
            _gameContent = JObject.Parse(obj);
            _gameTransportCallbacks.OnReceivingGameContent(_gameContent);

            StreamingAssetsUtils.LoadText("LocalSimulationData/game_state.json", OnGameStateLoaded);
        }

        private void OnGameStateLoaded(string obj)
        {
            var gameState = JObject.Parse(obj);
            _localSimulation.EventOnUpdateGameState += OnUpdateGameState;
            _localSimulation.Init(_gameContent, gameState);
            _gameContent = null;
        }

        private void OnUpdateGameState(JObject gameStateJson)
        {
            _gameTransportCallbacks.OnReceivingGameState(gameStateJson);
        }

        void ITransportService.SendCommand(JObject command)
        {
            _localSimulation.PushCommand(command);
        }
        
        void ITransportService.Update(float dt)
        {
            _localSimulation.Update(dt);
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