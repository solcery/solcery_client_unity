using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Values;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public class BrickConditionEqual : BrickCondition
    {
        public override bool Run(JArray parameters, IContext context)
        {
            if (parameters.Count > 1 
                && BrickUtils.TryGetBrickTypeName(parameters[0], out var brickTypeName1)
                && BrickUtils.TryGetBrickParameters(parameters[0], out var @params1)
                && BrickUtils.TryGetBrickTypeName(parameters[1], out var brickTypeName2)
                && BrickUtils.TryGetBrickParameters(parameters[1], out var @params2))
            {
                BrickService.GetInstance().TryCreate(brickTypeName1, out BrickValue value1);
                BrickService.GetInstance().TryCreate(brickTypeName2, out BrickValue value2);

                if (value1 != null && value2 != null)
                {
                    var v1 = value1.Run(@params1, context);
                    BrickService.GetInstance().Free(value1);
                    var v2 = value2.Run(@params2, context);
                    BrickService.GetInstance().Free(value2);
                    return v1 == v2;
                }
                
                BrickService.GetInstance().Free(value1);
                BrickService.GetInstance().Free(value2);
            }
            
            throw new Exception($"BrickConditionEqual Run has error! Parameters {parameters}");
        }

        public override string BrickTypeName()
        {
            return "brick_condition_equal";
        }

        public override void Reset() { }
    }
}