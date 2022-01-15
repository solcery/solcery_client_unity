using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Game.Attributes;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextGameAttrs : IContextGameAttrs
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterGameAttributes;

        public static IContextGameAttrs Create(EcsWorld world)
        {
            return new CurrentContextGameAttrs(world);
        }
        
        private CurrentContextGameAttrs(EcsWorld world)
        {
            _world = world;
            _filterGameAttributes = _world.Filter<ComponentGameAttributes>().End();
        }
        
        bool IContextGameAttrs.Contains(string attrName)
        {
            foreach (var entityId in _filterGameAttributes)
            {
                var pool = _world.GetPool<ComponentGameAttributes>();
                if (!pool.Has(entityId))
                {
                    continue;
                }

                ref var componentGameAttrs = ref pool.Get(entityId);
                return componentGameAttrs.Attributes.ContainsKey(attrName);
            }

            return false;
        }

        void IContextGameAttrs.Update(string attrName, int attrValue)
        {
            foreach (var entityId in _filterGameAttributes)
            {
                var pool = _world.GetPool<ComponentGameAttributes>();
                if (!pool.Has(entityId))
                {
                    continue;
                }

                ref var componentGameAttrs = ref pool.Get(entityId);
                if (!componentGameAttrs.Attributes.ContainsKey(attrName))
                {
                    componentGameAttrs.Attributes.Add(attrName, AttributeValue.Create(attrValue));
                    return;
                }
                
                componentGameAttrs.Attributes[attrName].UpdateValue(attrValue);
                return;
            }
        }

        bool IContextGameAttrs.TryGetValue(string attrName, out int attrValue)
        {
            attrValue = 0;
            
            foreach (var entityId in _filterGameAttributes)
            {
                var pool = _world.GetPool<ComponentGameAttributes>();
                if (!pool.Has(entityId))
                {
                    continue;
                }

                ref var componentGameAttrs = ref pool.Get(entityId);
                if (componentGameAttrs.Attributes.TryGetValue(attrName, out var value))
                {
                    attrValue = value.Current;
                    return true;
                }
                break;
            }

            return false;
        }
    }
}