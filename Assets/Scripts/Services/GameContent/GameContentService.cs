using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Services.Transport;
using Solcery.Utils;

namespace Solcery.Services.GameContent
{
    public class GameContentService : IGameContentService
    {
        event Action IGameContentService.EventOnReceivingGameContent
        {
            add
            {
                if (!_listEventOnReceivingGameContent.Contains(value))
                {
                    _listEventOnReceivingGameContent.Add(value);
                }
            }

            remove => _listEventOnReceivingGameContent.Remove(value);
        }
        
        event Action<JObject> IGameContentService.EventOnReceivingUi
        {
            add
            {
                if (!_listEventOnReceivingUi.Contains(value))
                {
                    _listEventOnReceivingUi.Add(value);
                }
            }

            remove => _listEventOnReceivingUi.Remove(value);
        }

        event Action<JObject> IGameContentService.EventOnReceivingGame
        {
            add
            {
                if (!_listEventOnReceivingGame.Contains(value))
                {
                    _listEventOnReceivingGame.Add(value);
                }
            }

            remove => _listEventOnReceivingGame.Remove(value);
        }
        
        private ITransportService _transportService;
        private readonly List<Action> _listEventOnReceivingGameContent;
        private readonly List<Action<JObject>> _listEventOnReceivingUi;
        private readonly List<Action<JObject>> _listEventOnReceivingGame;

        public static IGameContentService Create(ITransportService transportService)
        {
            return new GameContentService(transportService);
        }

        private GameContentService(ITransportService transportService)
        {
            _transportService = transportService;
            _listEventOnReceivingGameContent = new List<Action>();
            _listEventOnReceivingUi = new List<Action<JObject>>();
            _listEventOnReceivingGame = new List<Action<JObject>>();
        }

        void IGameContentService.Init()
        {
            _transportService.EventReceivingGameContent += OnEventReceivingGameContent;
        }

        private void OnEventReceivingGameContent(JObject obj)
        {
            CallAllAction(_listEventOnReceivingGameContent);

            var executeOrders = new[]
            {
                new Tuple<string, List<Action<JObject>>>("ui", _listEventOnReceivingUi),
                new Tuple<string, List<Action<JObject>>>("game", _listEventOnReceivingGame)
            };

            foreach (var (key, actions) in executeOrders)
            {
                if (!obj.TryGetValue(key, out JObject @params))
                {
                    continue;
                }

                CallAllActionWithParams(actions, @params);
            }
        }

        private void CallAllAction(List<Action> listOfActions)
        {
            foreach (var action in listOfActions)
            {
                action.Invoke();
            }
        }

        private void CallAllActionWithParams(List<Action<JObject>> listOfActions, JObject @params)
        {
            foreach (var action in listOfActions)
            {
                action.Invoke(@params);
            }
        }
        
        void IGameContentService.Cleanup()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _transportService.EventReceivingGameContent -= OnEventReceivingGameContent;
            
            _listEventOnReceivingGameContent.Clear();
            _listEventOnReceivingUi.Clear();
            _listEventOnReceivingGame.Clear();
        }

        void IGameContentService.Destroy()
        {
            Cleanup();
            _transportService = null;
        }
    }
}