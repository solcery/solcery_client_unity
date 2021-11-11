using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Models;
using Solcery.Models.Entities;
using Solcery.Models.Places;
using Solcery.Utils;
using Solcery.Widgets.Factory;
using UnityEngine;

namespace Solcery.Services.GameContent.PrepareData
{
    public sealed class GameContentPrepareDataService : IGameContentPrepareDataService
    {
        private IGameContentService _gameContentService;
        private IWidgetFactory _widgetFactory;
        private IModel _model;
        
        public static IGameContentPrepareDataService Create(IGameContentService gameContentService, IWidgetFactory widgetFactory, IModel model)
        {
            return new GameContentPrepareDataService(gameContentService, widgetFactory, model);
        }

        private GameContentPrepareDataService(IGameContentService gameContentService, IWidgetFactory widgetFactory, IModel model)
        {
            _gameContentService = gameContentService;
            _widgetFactory = widgetFactory;
            _model = model;
        }
        
        void IGameContentPrepareDataService.Init()
        {
            _gameContentService.EventOnReceivingGame += OnReceivingGame;
        }

        private void OnReceivingGame(JObject obj)
        {
            Debug.Log($"Game content->Game\n {obj}");
            
            // Подготовим place's
            if (obj.TryGetValue("places", out JObject placesObject) 
                && placesObject.TryGetValue("objects", out JArray placeArray))
            {
                foreach (var placeToken in placeArray)
                {
                    if (placeToken is JObject placeObject)
                    {
                        var entityIndex = _model.World.NewEntity();
                        _model.World.GetPool<ComponentPlaceTag>().Add(entityIndex);
                        _model.World.GetPool<ComponentPlaceId>().Add(entityIndex).Id =
                            placeObject.GetValue<int>("placeId");

                        // TODO: тут пробуем создать виджет, если нет то ок, не будет компонента
                        if (_widgetFactory.TryCreateWidget(placeObject, out var widget))
                        {
                            _model.World.GetPool<ComponentPlaceWidget>().Add(entityIndex).Widget = widget;
                        }
                    }
                }
            }
            
            // Подготовим entity types, для ускорения сделаем хеш мапу
            if (obj.TryGetValue("cardTypes", out JObject entityTypesObject) 
                && entityTypesObject.TryGetValue("objects", out JArray entityTypeArray))
            {
                var entityTypeMap = new Dictionary<int, JObject>(entityTypeArray.Count);
                foreach (var entityTypeToken in entityTypeArray)
                {
                    if (entityTypeToken is JObject entityTypeObject)
                    {
                        entityTypeMap.Add(entityTypeObject.GetValue<int>("id"), entityTypeObject);
                    }
                }
                
                var entityIndex = _model.World.NewEntity();
                _model.World.GetPool<ComponentEntityTypes>().Add(entityIndex).Types = entityTypeMap;
            }
        }

        void IGameContentPrepareDataService.Cleanup()
        {
            Cleanup();
        }

        void IGameContentPrepareDataService.Destroy()
        {
            Cleanup();
            _gameContentService = null;
            _widgetFactory = null;
            _model = null;
        }

        private void Cleanup()
        {
            _gameContentService.EventOnReceivingGame -= OnReceivingGame;
        }
    }
}