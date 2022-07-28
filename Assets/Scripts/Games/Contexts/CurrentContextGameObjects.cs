using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.Models.Shared.Objects;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextGameObjects : IContextGameObjects
    {
        private readonly IGame _game;
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameObjects;

        public static IContextGameObjects Create(IGame game, EcsWorld world)
        {
            return new CurrentContextGameObjects(game, world);
        }

        private CurrentContextGameObjects(IGame game, EcsWorld world)
        {
            _game = game;
            _world = world;
            _filterGameObjects = _world.Filter<ComponentObjectId>().End();
        }
        
        List<object> IContextGameObjects.GetAllCardTypeObject()
        {
            var result = new List<object>();
            foreach (var entityId in _filterGameObjects)
            {
                result.Add(entityId);
            }

            return result;
        }

        bool IContextGameObjects.TryGetCardTypeValue(object @object, string key, out JToken value)
        {
            var poolObjectId = _world.GetPool<ComponentObjectId>();
            var poolObjectType = _world.GetPool<ComponentObjectType>();
            if (@object is int entityId 
                && poolObjectId.Has(entityId)
                && poolObjectType.Has(entityId))
            {
                var id = poolObjectId.Get(entityId).Id;
                var tplid = poolObjectType.Get(entityId).TplId;
                if (_game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid)
                    && itemType.TryGetValue(out value, key, id))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }

        bool IContextGameObjects.TryGetCardId(object @object, out int cardId)
        {
            var poolObjectId = _world.GetPool<ComponentObjectId>();
            if (@object is int entityId 
                && poolObjectId.Has(entityId))
            {
                cardId = poolObjectId.Get(entityId).Id;
                return true;
            }

            cardId = -1;
            return false;
        }

        bool IContextGameObjects.TryGetCardTypeId(object @object, out int cardTypeId)
        {
            var poolObjectType = _world.GetPool<ComponentObjectType>();
            if (@object is int entityId 
                && poolObjectType.Has(entityId))
            {
                cardTypeId  = poolObjectType.Get(entityId).TplId;
                return true;
            }

            cardTypeId = -1;
            return false;
        }

        public bool SetCardTypeId(object @object, int cardTypeId)
        {
            var poolObjectId = _world.GetPool<ComponentObjectType>();
            if (@object is int entityId 
                && poolObjectId.Has(entityId))
            {
                poolObjectId.Get(entityId).TplId = cardTypeId;
                return true;
            }

            return false;
        }
    }
}