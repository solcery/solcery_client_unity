using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using UnityEngine;

namespace Solcery.Services.Transport
{
    public class EditorTransportService : ITransportService
    {
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
        
        public static ITransportService Create(IGameOnReceivingGameContent gameOnReceivingGameContent)
        {
            return new EditorTransportService(gameOnReceivingGameContent);
        }

        private EditorTransportService(IGameOnReceivingGameContent gameOnReceivingGameContent)
        {
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
            throw new NotImplementedException();
        }

        void ITransportService.Cleanup()
        {
            Cleanup();
        }
        
        void ITransportService.Destroy()
        {
            Cleanup();
            _gameOnReceivingGameContent = null;
        }

        private void Cleanup()
        {
            _listEventReceivingGameContent.Clear();
            _listEventReceivingGameState.Clear();
        }
    }
}