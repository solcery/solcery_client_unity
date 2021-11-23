using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueVariable : BrickValue
    {
        public static BrickValueVariable Create(int type, int subType)
        {
            return new BrickValueVariable(type, subType);
        }
        
        private BrickValueVariable(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 1 
                && parameters[0] is JObject varName)
            {
                ref var contextVars = ref world.GetPool<ComponentContextVars>().GetRawDenseItems()[0];
                if (contextVars.TryGetVar(varName.Value<string>(), out var value))
                {
                    return (int)value;
                }

                return 0;
            }

            throw new ArgumentException($"BrickValueVariable Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
        
    }
}