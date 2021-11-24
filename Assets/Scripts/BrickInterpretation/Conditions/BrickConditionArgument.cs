using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionArgument : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionArgument(type, subType);
        }
        
        private BrickConditionArgument(int type, int subType) : base(type, subType) { }

        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject
                && valueObject.TryGetValue("value", out string argName))
            {
                ref var contextArgs = ref world.GetPool<ComponentContextArgs>().GetRawDenseItems()[0];
                if (contextArgs.TryGet(argName, out JObject brick))
                {
                    if (serviceBricks.ExecuteConditionBrick(brick, world, out var result))
                    {
                        return result;
                    }
                }
            }

            throw new ArgumentException($"BrickConditionArgument Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}