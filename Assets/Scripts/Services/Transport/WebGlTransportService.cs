using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.Accessors.Cache;
using Solcery.Games;
using Solcery.React;
using Solcery.Services.LocalSimulation;
using Solcery.Utils;

namespace Solcery.Services.Transport
{
    public sealed class WebGlTransportService : ITransportService
    {
        private interface IJsonPackageData
        {
            bool Done { get; }
            JToken JsonData { get; }
            void Append(string data);
        }
        
        private class JsonPackageData : IJsonPackageData
        {
            bool IJsonPackageData.Done => _currentCount >= _fullCount;
            JToken IJsonPackageData.JsonData => JToken.Parse(_str.ToString());
        
            private readonly int _fullCount;
            private readonly StringBuilder _str;
        
            private int _currentCount;

            public static IJsonPackageData Create(string data)
            {
                return new JsonPackageData(data);
            }

            private JsonPackageData(string data)
            {
                _str = new StringBuilder();
                var obj = JObject.Parse(data);
                _fullCount = obj.GetValue<int>("count");
                _currentCount = 0;
                Append(data);
            }

            void IJsonPackageData.Append(string data)
            {
                Append(data);
            }

            private void Append(string data)
            {
                if (_currentCount >= _fullCount)
                {
                    return;
                }
            
                var obj = JObject.Parse(data);
                var fc = obj.GetValue<int>("count");

                if (_fullCount != fc)
                {
                    throw new Exception($"Packages not equals, start full count {_fullCount} package full count {fc}!");
                }

                _str.Append(obj.GetValue<string>("value"));
                _currentCount++;
            }
        }

        private IGame _game;
        private IServiceLocalSimulation _localSimulation;
        private IGameTransportCallbacks _gameTransportCallbacks;
        private IJsonPackageData _gameContentPackageData;
        private IJsonPackageData _gameContentOverridesPackageData;
        private IJsonPackageData _gameStatePackageData;
        private readonly ICacheAccessor _cacheAccessor;  
        
        public static ITransportService Create(IGame game, IGameTransportCallbacks gameTransportCallbacks, ICacheAccessor cacheAccessor)
        {
            return new WebGlTransportService(game, gameTransportCallbacks, cacheAccessor);
        }

        private WebGlTransportService(IGame game, IGameTransportCallbacks gameTransportCallbacks, ICacheAccessor cacheAccessor)
        {
            _game = game;
            _localSimulation = ServiceLocalSimulation.Create();
            _localSimulation.EventOnUpdateGameState += LocalSimulationOnEventOnUpdateGameState;
            _gameTransportCallbacks = gameTransportCallbacks;
            _cacheAccessor = cacheAccessor;
            ReactToUnity.AddCallback(ReactToUnity.EventOnUpdateGameContent, OnGameContentUpdate);
            ReactToUnity.AddCallback(ReactToUnity.EventOnUpdateGameContentOverrides, OnGameContentOverridesUpdate);
            ReactToUnity.AddCallback(ReactToUnity.EventOnUpdateGameState, OnGameStateUpdate);
        }

        private void LocalSimulationOnEventOnUpdateGameState(JObject obj)
        {
            _gameTransportCallbacks?.OnReceivingGameState(obj);
        }

        void ITransportService.CallUnityLoaded(JObject metadata)
        {
            UnityToReact.Instance.CallOnUnityLoaded(metadata.ToString());
        }

        private void OnGameContentUpdate(string obj)
        {
            const string key = "game_content";
            if (!string.IsNullOrEmpty(obj))
            {
                if (_gameContentPackageData == null)
                {
                    _gameContentPackageData = JsonPackageData.Create(obj);
                }
                else
                {
                    _gameContentPackageData.Append(obj);
                }

                if (!_gameContentPackageData.Done)
                {
                    return;
                }

                if (_gameContentPackageData.JsonData is JObject gameContent)
                {
                    _cacheAccessor.UpdateMetadataForKey(key, gameContent);
                    _gameTransportCallbacks?.OnReceivingGameContent(gameContent);
                }

                _gameContentPackageData = null;
            }
            else
            {
                _gameTransportCallbacks?.OnReceivingGameContent(_cacheAccessor.GetCacheForKey(key));
            }
        }
        
        private void OnGameContentOverridesUpdate(string obj)
        {
            const string key = "game_content_overrides";
            if (!string.IsNullOrEmpty(obj))
            {
                if (_gameContentOverridesPackageData == null)
                {
                    _gameContentOverridesPackageData = JsonPackageData.Create(obj);
                }
                else
                {
                    _gameContentOverridesPackageData.Append(obj);
                }

                if (!_gameContentOverridesPackageData.Done)
                {
                    return;
                }

                if (_gameContentOverridesPackageData.JsonData is JObject gameContent)
                {
                    _cacheAccessor.UpdateMetadataForKey(key, gameContent);
                    _gameTransportCallbacks?.OnReceivingGameContentOverrides(gameContent);
                }

                _gameContentOverridesPackageData = null;
            }
            else
            {
                _gameTransportCallbacks?.OnReceivingGameContentOverrides(_cacheAccessor.GetCacheForKey(key));
            }
        }
        
        private void OnGameStateUpdate(string obj)
        {
            if (_gameStatePackageData == null)
            {
                _gameStatePackageData = JsonPackageData.Create(obj);
            }
            else
            {
                _gameStatePackageData.Append(obj);
            }

            if (!_gameStatePackageData.Done)
            {
                return;
            }
            
            if (_gameStatePackageData.JsonData is JObject gameState)
            {
                if (gameState.TryGetValue("commands", out JArray commands))
                {
                    foreach (var commandToken in commands)
                    {
                        if (commandToken is JObject command)
                        {
                            _localSimulation.PushCommand(command);
                        }
                    }
                }
                
                if (gameState.ContainsKey("states"))
                {
                    _localSimulation.Init(_game, gameState);
                }
            }
            _gameStatePackageData = null;
        }

        void ITransportService.SendCommand(JObject command)
        {
            UnityToReact.Instance.CallSendCommand(command.ToString(Formatting.None));
            _localSimulation.PushCommand(command);
        }

        void ITransportService.Update(float dt)
        {
            _localSimulation.Update(dt);
        }

        private void Cleanup()
        {
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnUpdateGameContent, OnGameContentUpdate);
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnUpdateGameContentOverrides, OnGameContentOverridesUpdate);
            ReactToUnity.RemoveCallback(ReactToUnity.EventOnUpdateGameState, OnGameStateUpdate);
            
            _localSimulation.EventOnUpdateGameState -= LocalSimulationOnEventOnUpdateGameState;
            _localSimulation.Cleanup();
        }
        
        void ITransportService.Cleanup()
        {
            //Cleanup();
        }
        
        void ITransportService.Destroy()
        {
            Cleanup();
            _localSimulation.Destroy();
            _localSimulation = null;
            _gameTransportCallbacks = null;
        }
    }
}