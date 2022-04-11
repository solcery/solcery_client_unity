using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.React;

namespace Solcery.Services.Transport
{
    public sealed class WebGlTransportService : ITransportService
    {
        private IGameTransportCallbacks _gameTransportCallbacks;
        
        public static ITransportService Create(IGameTransportCallbacks gameTransportCallbacks)
        {
            return new WebGlTransportService(gameTransportCallbacks);
        }

        private WebGlTransportService(IGameTransportCallbacks gameTransportCallbacks)
        {
            _gameTransportCallbacks = gameTransportCallbacks;
            ReactToUnity.AddCallback(ReactToUnity.EventOnUpdateGameContent, OnGameContentUpdate);
            ReactToUnity.AddCallback(ReactToUnity.EventOnUpdateGameState, OnGameStateUpdate);
        }
        
        void ITransportService.CallUnityLoaded()
        {
            UnityToReact.Instance.CallOnUnityLoaded();
        }

        private void OnGameStateUpdate(string obj)
        {
            _gameTransportCallbacks?.OnReceivingGameState(JObject.Parse(obj));
        }

        private void OnGameContentUpdate(string obj)
        {
            _gameTransportCallbacks?.OnReceivingGameContent(JObject.Parse(obj));
        }

        void ITransportService.SendCommand(JObject command)
        {
            UnityToReact.Instance.CallSendCommand(command.ToString(Formatting.None));
        }

        void ITransportService.Update(float dt)
        {
            
        }

        private void Cleanup()
        {
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnUpdateGameContent, OnGameContentUpdate);
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnUpdateGameState, OnGameStateUpdate);
        }
        
        void ITransportService.Cleanup()
        {
            //Cleanup();
        }
        
        void ITransportService.Destroy()
        {
            Cleanup();

            _gameTransportCallbacks = null;
        }
    }
}