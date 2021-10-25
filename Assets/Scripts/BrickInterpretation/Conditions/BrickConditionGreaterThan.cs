using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Values;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionGreaterThan : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            if (parameters.Count >= 2 
                && parameters[0] is JObject parameterObject1 
                && parameterObject1.TryGetValue("type", out string brickTypeName1) 
                && BrickService.GetInstance().Create(brickTypeName1) is BrickValue condition1 
                && parameterObject1.TryGetValue("params", out JArray @params1)
                && parameters[1] is JObject parameterObject2 
                && parameterObject2.TryGetValue("type", out string brickTypeName2) 
                && BrickService.GetInstance().Create(brickTypeName2) is BrickValue condition2 
                && parameterObject2.TryGetValue("params", out JArray @params2))
            {
                var c1 = condition1.Run(@params1, context);
                var c2 = condition2.Run(@params2, context);
                BrickService.GetInstance().Free(condition1);
                BrickService.GetInstance().Free(condition2);
                
                return c1 >= c2;
            }
            
            throw new Exception($"BrickConditionGreaterThan Run has error! Parameters {parameters}");
        }

        public override string BrickTypeName()
        {
            return "brick_condition_greater_than";
        }

        public override void Reset() { }
    }
}