using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;
using Solcery.Games.Contexts.GameStates;
using Solcery.Models.Play.Initial.Game;
using Solcery.Models.Shared.Attributes.Place;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Simulation.Game;
using Solcery.Utils;
using Random = UnityEngine.Random;

namespace Solcery.Games.Contexts
{
    internal class CurrentContext : IContext
    {
        public IContextGameStates GameStates { get; }
        public IContextObject Object { get; }
        public IContextObjectAttrs ObjectAttrs { get; }
        public IContextGameAttrs GameAttrs { get; }
        public IContextGameArgs GameArgs { get; }
        public IContextGameVars GameVars { get; }
        public IContextGameObjects GameObjects { get; }
        public ILog Log { get; }

        private readonly EcsWorld _world;
        private readonly EcsFilter _filterComponentObjectIdHash;
        private readonly EcsFilter _filterComponentGame;

        public static IContext Create(EcsWorld world)
        {
            return new CurrentContext(world);
        }

        private CurrentContext(EcsWorld world)
        {
            GameStates = ContextGameStates.Create(world);
            Object = CurrentContextObject.Create(world);
            ObjectAttrs = CurrentContextObjectAttrs.Create(world);
            GameAttrs = CurrentContextGameAttrs.Create(world);
            GameArgs = CurrentContextGameArgs.Create(world);
            GameVars = ComponentContextGameVars.Create(world);
            GameObjects = CurrentContextGameObjects.Create(world);
            Log = CurrentLog.Create();

            _world = world;
            _filterComponentObjectIdHash = _world.Filter<ComponentObjectIdHash>().End();
            _filterComponentGame = _world.Filter<ComponentGame>().End();
        }
        
        bool IContext.DeleteObject(object @object)
        {
            var entityId = (int) @object;
            var objectsPool = _world.GetPool<ComponentObjectTag>();
            var objectsDeletedPool = _world.GetPool<ComponentObjectDeletedTag>();

            if (objectsPool.Has(entityId))
            {
                if (!objectsDeletedPool.Has(entityId))
                {
                    objectsDeletedPool.Add(entityId);
                }

                return true;
            }

            return false;
        }

        bool IContext.TryCreateObject(JObject parameters, out object @object)
        {
            
            
            var entityId = _world.NewEntity();
            var cardTypeId = parameters.GetValue<int>("card_type");
            var place = parameters.GetValue<int>("place");

            foreach (var objectIdHashEntityId in _filterComponentObjectIdHash)
            {
                var objectIdHash = _world.GetPool<ComponentObjectIdHash>().Get(objectIdHashEntityId).ObjectIdHash;

                int objectId;
                var iterator = 0;
                
                do
                {
                    iterator++;

                    if (iterator >= 10000)
                    {
                        throw new Exception("Can't generate object id!");
                    }
                    
                    objectId = Random.Range(0, 100000);
                } while (objectIdHash.Contains(objectId));
                
                _world.GetPool<ComponentObjectTag>().Add(entityId);
                _world.GetPool<ComponentObjectId>().Add(entityId).Id = objectId;
                _world.GetPool<ComponentObjectType>().Add(entityId).Type = cardTypeId;

                ref var componentAttributes = ref _world.GetPool<ComponentObjectAttributes>().Add(entityId);
                foreach (var gameEntityId in _filterComponentGame)
                {
                    var attributeList = _world.GetPool<ComponentGame>().Get(gameEntityId).Game.GameContentAttributes;
                    foreach (var attribute in attributeList.AttributeNameList)
                    {
                        if (!componentAttributes.Attributes.ContainsKey(attribute))
                        {
                            if (attribute == "place")
                            {
                                componentAttributes.Attributes.Add(attribute, AttributeValue.Create(place));
                            }
                            else
                            {
                                componentAttributes.Attributes.Add(attribute, AttributeValue.Create(0));
                            }
                        }
                    }
                    break;
                }
                
                //_world.GetPool<ComponentObjectAttributes>().Add(entityId).Attributes.Add("place", AttributeValue.Create(place));
                _world.GetPool<ComponentAttributePlace>().Add(entityId).Value = AttributeValue.Create(place);

                @object = entityId;
                
                Log.Print($"Create new object with entity id {entityId}");
                
                return true;
            }

            @object = -1;
            return false;
        }
    }
}