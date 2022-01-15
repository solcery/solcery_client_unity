using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.Models.Shared.Context;

namespace Solcery.Tests.PlayMode.Contexts
{
    internal class TestContextObject : IContextObject
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterContextObject;

        public static IContextObject Create(EcsWorld world)
        {
            return new TestContextObject(world);
        }
        
        private TestContextObject(EcsWorld world)
        {
            _world = world;
            _filterContextObject = _world.Filter<ComponentContextObject>().End();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        void IContextObject.Push(object @object)
        {
            var pool = _world.GetPool<ComponentContextObject>();
            foreach (var entityId in _filterContextObject)
            {
                pool.Get(entityId).Push(@object);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IContextObject.TryPop<T>(out T @object)
        {
            var pool = _world.GetPool<ComponentContextObject>();
            foreach (var entityId in _filterContextObject)
            {
                return pool.Get(entityId).TryPop(out @object);
            }

            @object = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IContextObject.TryPeek<T>(out T @object)
        {
            var pool = _world.GetPool<ComponentContextObject>();
            foreach (var entityId in _filterContextObject)
            {
                return pool.Get(entityId).TryPeek(out @object);
            }

            @object = default;
            return false;
        }
    }
}