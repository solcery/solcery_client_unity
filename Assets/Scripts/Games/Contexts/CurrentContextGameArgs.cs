using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.Models.Shared.Context;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextGameArgs : IContextGameArgs
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _filterContextArgs;

        public static IContextGameArgs Create(EcsWorld world)
        {
            return new CurrentContextGameArgs(world);
        }
        
        private CurrentContextGameArgs(EcsWorld world)
        {
            _world = world;
            _filterContextArgs = _world.Filter<ComponentContextArgs>().End();
        }
        
        Dictionary<string, JObject> IContextGameArgs.Pop()
        {
            var pool = _world.GetPool<ComponentContextArgs>();
            foreach (var entityId in _filterContextArgs)
            {
                ref var componentContextArgs = ref pool.Get(entityId);
                return componentContextArgs.Pop();
            }

            throw new Exception("ComponentContextArgs not found!");
        }

        void IContextGameArgs.Push(Dictionary<string, JObject> args)
        {
            var pool = _world.GetPool<ComponentContextArgs>();
            foreach (var entityId in _filterContextArgs)
            {
                ref var componentContextArgs = ref pool.Get(entityId);
                componentContextArgs.Push(args);
                return;
            }
            
            throw new Exception("ComponentContextArgs not found!");
        }
    }
}