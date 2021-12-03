using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionOr : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionOr(type, subType);
        }

        private BrickConditionOr(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            foreach (var parameterToken in parameters)
            {
                if (!parameterToken.TryParseBrickParameter(out _, out JObject conditionBrick) 
                    || !serviceBricks.ExecuteConditionBrick(conditionBrick, world, level + 1, out var result))
                {
                    throw new Exception($"BrickConditionOr Run parameters {parameters}");
                }
                
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Reset() { }
    }
}