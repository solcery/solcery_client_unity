using System;
using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;
using Solcery.Models.Shared.Context;

namespace Solcery.Tests.PlayMode.Contexts
{
    internal class TestContextGameVars : IContextGameVars
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterContextGameVars;

        public static IContextGameVars Create(EcsWorld world)
        {
            return new TestContextGameVars(world);
        }

        private TestContextGameVars(EcsWorld world)
        {
            _world = world;
            _filterContextGameVars = _world.Filter<ComponentContextVars>().End();
        }
        
        void IContextGameVars.Update(string varName, int varValue)
        {
            var pool = _world.GetPool<ComponentContextVars>();
            foreach (var entityId in _filterContextGameVars)
            {
                ref var componentContextGameVars = ref pool.Get(entityId);
                componentContextGameVars.Set(varName, varValue);
                return;
            }
            
            throw new Exception("ComponentContextVars not found!");
        }

        bool IContextGameVars.TryGet(string varName, out int varValue)
        {
            var pool = _world.GetPool<ComponentContextVars>();
            foreach (var entityId in _filterContextGameVars)
            {
                ref var componentContextGameVars = ref pool.Get(entityId);
                return componentContextGameVars.TryGet(varName, out varValue);
            }
            
            throw new Exception("ComponentContextVars not found!");
        }
    }
}