using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Entities;
using Solcery.Models.Play.Places;
using Solcery.Utils;

namespace Solcery.Models.Play.GameContent
{
    public interface ISystemGameContentInit : IEcsInitSystem { }

    public sealed class SystemGameContentInit : ISystemGameContentInit
    {
        private IGame _game;
        
        public static ISystemGameContentInit Create(IGame game)
        {
            return new SystemGameContentInit(game);
        }
        
        private SystemGameContentInit(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            if (_game.GameContent != null)
            {
                var world = systems.GetWorld();
                // Подготовим place's
                if (_game.GameContent.TryGetValue("places", out JObject placesObject) 
                    && placesObject.TryGetValue("objects", out JArray placeArray))
                {
                    foreach (var placeToken in placeArray)
                    {
                        if (placeToken is JObject placeObject)
                        {
                            var entityIndex = world.NewEntity();
                            world.GetPool<ComponentPlaceTag>().Add(entityIndex);
                            world.GetPool<ComponentPlaceId>().Add(entityIndex).Id =
                                placeObject.GetValue<int>("placeId");
            
                            // TODO: тут пробуем создать виджет, если нет то ок, не будет компонента
                            if (_game.WidgetFactory.TryCreateWidget(placeObject, out var widget))
                            {
                                widget.CreateView();
                                world.GetPool<ComponentPlaceWidget>().Add(entityIndex).Widget = widget;
                            }
                        }
                    }
                }
            
                // Подготовим entity types, для ускорения сделаем хеш мапу
                if (_game.GameContent.TryGetValue("cardTypes", out JObject entityTypesObject) 
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
                
                    var entityIndex = world.NewEntity();
                    world.GetPool<ComponentEntityTypes>().Add(entityIndex).Types = entityTypeMap;
                }
            }

            _game = null;
        }
    }
}