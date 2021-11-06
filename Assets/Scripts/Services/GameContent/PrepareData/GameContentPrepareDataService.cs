using Newtonsoft.Json.Linq;
using Solcery.Models;
using UnityEngine;

namespace Solcery.Services.GameContent.PrepareData
{
    public sealed class GameContentPrepareDataService : IGameContentPrepareDataService
    {
        private IGameContentService _gameContentService;
        private IModel _model;
        
        public static IGameContentPrepareDataService Create(IGameContentService gameContentService, IModel model)
        {
            return new GameContentPrepareDataService(gameContentService, model);
        }

        private GameContentPrepareDataService(IGameContentService gameContentService, IModel model)
        {
            _gameContentService = gameContentService;
            _model = model;
        }
        
        void IGameContentPrepareDataService.Init()
        {
            _gameContentService.EventOnReceivingGame += OnReceivingGame;
        }

        private void OnReceivingGame(JObject obj)
        {
            Debug.Log($"Game content->Game\n {obj}");
            
        }

        void IGameContentPrepareDataService.Cleanup()
        {
            Cleanup();
        }

        void IGameContentPrepareDataService.Destroy()
        {
            Cleanup();
            _gameContentService = null;
            _model = null;
        }

        private void Cleanup()
        {
            _gameContentService.EventOnReceivingGame -= OnReceivingGame;
        }
    }
}