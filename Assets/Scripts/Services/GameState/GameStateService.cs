using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Models;
using Solcery.Services.Transport;
using UnityEngine;

namespace Solcery.Services.GameState
{
    public sealed class GameStateService : IGameStateService
    {
        private ITransportService _transportService;
        private IModel _model;
        private readonly Queue<JObject> _queueGameStates;

        public static IGameStateService Create(ITransportService transportService, IModel model)
        {
            return new GameStateService(transportService, model);
        }

        private GameStateService(ITransportService transportService, IModel model)
        {
            _transportService = transportService;
            _model = model;
            _queueGameStates = new Queue<JObject>();
        }

        void IGameStateService.Init()
        {
            _transportService.EventReceivingGameState += OnEventReceivingGameState;
        }

        void IGameStateService.Update()
        {
            while (_queueGameStates.Count > 0)
            {
                ApplyGameState(_queueGameStates.Dequeue());
            }
        }

        void IGameStateService.Cleanup()
        {
            Cleanup();
        }

        void IGameStateService.Destroy()
        {
            Cleanup();
            _model = null;
            _transportService = null;
        }

        private void Cleanup()
        {
            _transportService.EventReceivingGameState -= OnEventReceivingGameState;
            _queueGameStates.Clear();
        }
        
        private void OnEventReceivingGameState(JObject obj)
        {
            Debug.Log($"Game state\n {obj}");
            _queueGameStates.Enqueue(obj);
        }

        private void ApplyGameState(JObject obj)
        {
            // TODO: тут инитим гейм стейт, работаем с можелью и тд
        }
    }
}