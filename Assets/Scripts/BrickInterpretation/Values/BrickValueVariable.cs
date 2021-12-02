using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueVariable : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueVariable(type, subType);
        }
        
        private BrickValueVariable(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string varName))
            {
                var filter = world.Filter<ComponentContextVars>().End();
                foreach (var entityId in filter)
                {
                    ref var contextVars = ref world.GetPool<ComponentContextVars>().Get(entityId);
                    if (contextVars.TryGet(varName, out var value))
                    {
                        return value;
                    }
                    break;
                }
            }

            throw new ArgumentException($"BrickValueVariable Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
        
    }
}