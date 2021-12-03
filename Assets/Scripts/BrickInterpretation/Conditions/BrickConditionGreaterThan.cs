using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionGreaterThan : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionGreaterThan(type, subType);
        }

        private BrickConditionGreaterThan(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out JObject valueBrick1)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick2))
            {
                var brickExecutionResult = serviceBricks.ExecuteValueBrick(valueBrick1, world, level + 1, out var value1);
                var value2 = 0;
                brickExecutionResult = brickExecutionResult &&
                                       serviceBricks.ExecuteValueBrick(valueBrick2, world, level + 1, out value2);

                if (brickExecutionResult)
                {
                    return value1 >= value2;
                }
            }

            throw new Exception($"BrickConditionGreaterThan Run parameters {parameters}!");
        }

        public override void Reset() { }
    }
}