using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.Models.Shared.Objects;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextGameObjects : IContextGameObjects
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameObjects;
        private readonly EcsFilter _filterObjectTypes;

        public static IContextGameObjects Create(EcsWorld world)
        {
            return new CurrentContextGameObjects(world);
        }

        private CurrentContextGameObjects(EcsWorld world)
        {
            _world = world;
            _filterGameObjects = _world.Filter<ComponentObjectId>().End();
            _filterObjectTypes = _world.Filter<ComponentObjectTypes>().End();
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

        bool IContextGameObjects.TryGetCardTypeData(object @object, out JObject cardTypeData)
        {
            var poolObjectType = _world.GetPool<ComponentObjectType>();
            var poolObjectTypes = _world.GetPool<ComponentObjectTypes>();
            if (@object is int entityId 
                && poolObjectType.Has(entityId))
            {
                ref var componentObjectType = ref poolObjectType.Get(entityId);
                foreach (var uniqObjectTypesEntityTypes in _filterObjectTypes)
                {
                    return poolObjectTypes.Get(uniqObjectTypesEntityTypes).Types
                        .TryGetValue(componentObjectType.Type, out cardTypeData);
                }
            }

            cardTypeData = null;
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
            var poolObjectId = _world.GetPool<ComponentObjectType>();
            if (@object is int entityId 
                && poolObjectId.Has(entityId))
            {
                cardTypeId = poolObjectId.Get(entityId).Type;
                return true;
            }

            cardTypeId = -1;
            return false;
        }
    }
}