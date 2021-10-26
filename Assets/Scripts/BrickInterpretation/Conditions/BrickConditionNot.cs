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
                && BrickUtils.TryGetBrickTypeName(parameters[0], out var brickTypeName)
                && BrickUtils.TryGetBrickParameters(parameters[0], out var @params)
                && BrickService.GetInstance().TryCreate(brickTypeName, out BrickCondition condition))
            {
                var result = !condition.Run(@params, context);
                BrickService.GetInstance().Free(condition);
                
                return result;
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