using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
//using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;
using Solcery.Games.Contexts.GameStates;
using Solcery.Models.Shared.Attributes.Place;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;

namespace Solcery.Games.Contexts
{
    internal class CurrentContext : IContext
    {
        public IContextGameStates GameStates { get; }
        public IContextObject Object { get; }
        public IContextObjectAttrs ObjectAttrs { get; }
        public IContextGameAttrs GameAttrs { get; }
        //public IContextGameArgs GameArgs { get; }
        public IContextGameVars GameVars { get; }
        public IContextGameObjects GameObjects { get; }

        public IContextLocalScopes LocalScopes { get; }

        public ILog Log { get; }

        private readonly EcsWorld _world;
        private readonly EcsFilter _filterComponentObjectIdHash;

        private IGame _game;

        public static IContext Create(IGame game, EcsWorld world)
        {
            DestroyPreviewContextEntity(world);
            CreateContextEntity(world);
            return new CurrentContext(game, world);
        }

        public static void Destroy(EcsWorld world, IContext context)
        {
            DestroyPreviewContextEntity(world);
        }

        private static void CreateContextEntity(EcsWorld world)
        {
            var contextObjectPool = world.GetPool<ComponentContextObject>();
            var contextArgsPool = world.GetPool<ComponentContextArgs>();
            var contextVarsPool = world.GetPool<ComponentContextVars>();
            var contextEntityId = world.NewEntity();
            contextObjectPool.Add(contextEntityId);
            contextArgsPool.Add(contextEntityId);
            contextVarsPool.Add(contextEntityId);
        }

        private static void DestroyPreviewContextEntity(EcsWorld world)
        {
            var filterContext = world.Filter<ComponentContextObject>()
                .Inc<ComponentContextArgs>()
                .Inc<ComponentContextVars>()
                .End();
            
            foreach (var entityId in filterContext)
            {
                world.DelEntity(entityId);
            }
        }
        

        private CurrentContext(IGame game, EcsWorld world)
        {
            _game = game;
            GameStates = ContextGameStates.Create(world);
            Object = CurrentContextObject.Create(world);
            ObjectAttrs = CurrentContextObjectAttrs.Create(world);
            GameAttrs = CurrentContextGameAttrs.Create(world);
            //GameArgs = CurrentContextGameArgs.Create(world);
            GameVars = ComponentContextGameVars.Create(world);
            GameObjects = CurrentContextGameObjects.Create(game, world);
            LocalScopes = CurrentContextLocalScopes.Create();
            LocalScopes.New();
            Log = CurrentLog.Create();

            _world = world;
            _filterComponentObjectIdHash = _world.Filter<ComponentObjectIdHash>().End();
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
                    Log.Print($"Mark destroyed object with entity id {entityId}");
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
                var objectIdHashes = _world.GetPool<ComponentObjectIdHash>().Get(objectIdHashEntityId).ObjectIdHashes;
                var objectId = objectIdHashes.GetId();
                _world.GetPool<ComponentObjectTag>().Add(entityId);
                _world.GetPool<ComponentObjectId>().Add(entityId).Id = objectId;
                _world.GetPool<ComponentObjectType>().Add(entityId).TplId = cardTypeId;

                ref var componentAttributes = ref _world.GetPool<ComponentObjectAttributes>().Add(entityId);
                var attributeList = _game.GameContentAttributes;
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
                
                //_world.GetPool<ComponentObjectAttributes>().Add(entityId).Attributes.Add("place", AttributeValue.Create(place));
                _world.GetPool<ComponentAttributePlace>().Add(entityId).Value = AttributeValue.Create(place);

                @object = entityId;
                
                Log.Print($"Create new object with entityId {entityId} and objectId {objectId}");
                
                return true;
            }

            @object = -1;
            return false;
        }

        bool IContext.TryResetObjectAttrs(object @object)
        {
            var entityId = (int) @object;
            var objectAttrsPool = _world.GetPool<ComponentObjectAttributes>();

            if (objectAttrsPool.Has(entityId))
            {
                return false;
            }

            var objectAttrs = objectAttrsPool.Get(entityId).Attributes;
            foreach (var attribute in objectAttrs)
            {
                attribute.Value.UpdateValue(0);
            }

            return true;
        }
    }
}