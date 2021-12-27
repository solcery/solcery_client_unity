using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.React;
using Solcery.Utils;

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
            //UnityEngine.Debug.Log($"OnGameStateUpdate {obj}");
            _gameTransportCallbacks?.OnReceivingGameState(JObject.Parse(obj));
        }

        private void OnGameContentUpdate(string obj)
        {
            //UnityEngine.Debug.Log($"OnGameContentUpdate {obj}");
            _gameTransportCallbacks?.OnReceivingGameContent(JObject.Parse(obj));
        }

        void ITransportService.SendCommand(JObject command)
        {
            if (command.TryGetValue("object_id", out int objId))
            {
                UnityToReact.Instance.CallCastCard(objId);
            }
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