using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueAttribute : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueAttribute(type, subType);
        }
        
        private BrickValueAttribute(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 1 
                && parameters[0] is JObject attrNameObject
                && attrNameObject.TryGetValue("value", out string attrName))
            {
                ref var contextVars = ref world.GetPool<ComponentContextAttrs>().GetRawDenseItems()[0];
                if (contextVars.TryGet(attrName, out int attr))
                {
                    return attr;
                }
            }

            throw new ArgumentException($"BrickValueAttribute Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}