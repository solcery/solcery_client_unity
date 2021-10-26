using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionAnd : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                if (BrickUtils.TryGetBrickTypeName(parameterToken, out var brickTypeName)
                    && BrickUtils.TryGetBrickParameters(parameterToken, out var @params)
                    && BrickService.GetInstance().TryCreate(brickTypeName, out BrickCondition condition))
                {
                    var result = !condition.Run(@params, context);
                    BrickService.GetInstance().Free(condition);
                    
                    if (result)
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception($"BrickConditionAnd Run has error! Parameters {parameterToken}");
                }
            }

            return true;
        }

        public override string BrickTypeName()
        {
            return "brick_condition_and";
        }

        public override void Reset() { }
    }
}