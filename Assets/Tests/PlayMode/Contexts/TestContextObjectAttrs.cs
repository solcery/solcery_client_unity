using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;

namespace Solcery.Tests.PlayMode.Contexts
{
    internal class TestContextObjectAttrs : IContextObjectAttrs
    {
        private readonly EcsWorld _world;

        public static IContextObjectAttrs Create(EcsWorld world)
        {
            return new TestContextObjectAttrs(world);
        }

        private TestContextObjectAttrs(EcsWorld world)
        {
            _world = world;
        }
        
        /// <summary>
        /// Check available attribute for attrName key in context object
        /// </summary>
        /// <param name="contextObject">Int as entityId context object</param>
        /// <param name="attrName">Attribute name key</param>
        /// <returns>Current context object has attribute for attrName key</returns>
        bool IContextObjectAttrs.Contains(object contextObject, string attrName)
        {
            var pool = _world.GetPool<ComponentObjectAttributes>();
            return contextObject is int entityId
                   && pool.Has(entityId)
                   && pool.Get(entityId).Attributes.ContainsKey(attrName);
        }

        /// <summary>
        /// Update or create and set attribute for attrName key value.
        /// </summary>
        /// <param name="contextObject">Int as entityId context object</param>
        /// <param name="attrName">Attribute name key</param>
        /// <param name="attrValue">Attribute value</param>
        void IContextObjectAttrs.Update(object contextObject, string attrName, int attrValue)
        {
            var pool = _world.GetPool<ComponentObjectAttributes>();
            if (contextObject is int entityId
                && pool.Has(entityId))
            {
                ref var componentAttributes = ref pool.Get(entityId);

                if (!componentAttributes.Attributes.ContainsKey(attrName))
                {
                    componentAttributes.Attributes.Add(attrName, AttributeValue.Create(attrValue));
                    return;
                }
                
                componentAttributes.Attributes[attrName].UpdateValue(attrValue);
            }
        }

        /// <summary>
        /// Try get attribute value for attribute name key from context object.
        /// </summary>
        /// <param name="contextObject">Int as entityId context object</param>
        /// <param name="attrName">Attribute name key</param>
        /// <param name="attrValue">Attribute value</param>
        /// <returns>Try get result, true - value is finded, false - other cases</returns>
        bool IContextObjectAttrs.TryGetValue(object contextObject, string attrName, out int attrValue)
        {
            attrValue = 0;
            var pool = _world.GetPool<ComponentObjectAttributes>();
            if (contextObject is int entityId
                && pool.Has(entityId))
            {
                ref var componentAttributes = ref pool.Get(entityId);

                if (componentAttributes.Attributes.TryGetValue(attrName, out var value))
                {
                    attrValue = value.Current;
                    return true;
                }
            }

            return false;
        }
    }
}