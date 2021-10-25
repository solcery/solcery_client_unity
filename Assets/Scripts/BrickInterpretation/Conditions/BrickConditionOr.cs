using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionOr : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                if (parameterToken is JObject parameterObject 
                    && parameterObject.TryGetValue("type", out string brickTypeName) 
                    && BrickService.GetInstance().Create(brickTypeName) is BrickCondition condition 
                    && parameterObject.TryGetValue("params", out JArray @params))
                {
                    if (condition.Run(@params, context))
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception($"BrickConditionAnd Run has error! Parameters {parameterToken}");
                }
            }

            return false;
        }

        public override string BrickTypeName()
        {
            return "brick_condition_or";
        }

        public override void Reset() { }
    }
}