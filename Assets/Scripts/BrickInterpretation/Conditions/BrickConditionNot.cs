using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionNot : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject parameterObject 
                && parameterObject.TryGetValue("type", out string brickTypeName) 
                && BrickService.GetInstance().Create(brickTypeName) is BrickCondition condition 
                && parameterObject.TryGetValue("params", out JArray @params))
            {
                return !condition.Run(@params, context);
            }
            
            throw new Exception($"BrickConditionNot Run has error! Parameters {parameters}");
        }

        public override string BrickTypeName()
        {
            return "brick_condition_not";
        }

        public override void Reset() { }
    }
}