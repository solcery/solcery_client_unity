using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Conditions
{
    public sealed class BrickConditionOr : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionOr(type, subType);
        }

        private BrickConditionOr(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            foreach (var parameterToken in parameters)
            {
                if (!parameterToken.TryParseBrickParameter(out _, out JObject conditionBrick) 
                    || !serviceBricks.ExecuteConditionBrick(conditionBrick, context, level + 1, out var result))
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