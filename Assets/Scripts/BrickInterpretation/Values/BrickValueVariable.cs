using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
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
            if (parameters.Count >= 1 
                && parameters[0] is JObject varNameObject
                && varNameObject.TryGetValue("value", out string varName))
            {
                ref var contextVars = ref world.GetPool<ComponentContextVars>().GetRawDenseItems()[0];
                if (contextVars.TryGet(varName, out int value))
                {
                    return value;
                }
            }

            throw new ArgumentException($"BrickValueVariable Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
        
    }
}