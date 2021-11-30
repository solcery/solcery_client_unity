using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Entities;
using Solcery.Utils;

namespace Solcery.Models.Shared.Initial.Game.Content
{
    public interface ISystemGameContentInitEntityTypes : IEcsInitSystem { }

    public sealed class SystemInitialGameContentEntityTypes : ISystemGameContentInitEntityTypes
    {
        private JObject _gameContent;
        
        public static ISystemGameContentInitEntityTypes Create(JObject gameContent)
        {
            return new SystemInitialGameContentEntityTypes(gameContent);
        }

        private SystemInitialGameContentEntityTypes(JObject gameContent)
        {
            _gameContent = gameContent;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            if (_gameContent == null)
            {
                return;
            }
            
            var world = systems.GetWorld();
            
            // Только тут мы инитим entity types, так что можно спокойно удалить если что то есть еще
            var filter = world.Filter<ComponentEntityTypes>().End();
            var pool = world.GetPool<ComponentEntityTypes>();

            foreach (var entityId in filter)
            {
                pool.Del(entityId);
            }

            // Подготовим entity types, для ускорения сделаем хеш мапу
            if (_gameContent.TryGetValue("cardTypes", out JObject entityTypesObject) 
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
            
            _gameContent = null;
        }
    }
}