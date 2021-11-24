using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueArgument : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueArgument(type, subType);
        }
        
        private BrickValueArgument(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject
                && valueObject.TryGetValue("value", out string argName))
            {
                ref var contextArgs = ref world.GetPool<ComponentContextArgs>().GetRawDenseItems()[0];
                if (contextArgs.TryGet(argName, out JObject brick))
                {
                    if (serviceBricks.ExecuteValueBrick(brick, world, out var result))
                    {
                        return result;
                    }
                }
            }

            throw new ArgumentException($"BrickValueArgument Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}