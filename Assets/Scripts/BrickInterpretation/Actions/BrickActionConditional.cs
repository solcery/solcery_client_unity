using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Conditions;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionConditional : BrickAction
    {
        public override string BrickTypeName()
        {
            return "brick_action_conditional";
        }

        public override void Reset() { }

        public override void Run(JArray parameters, IContext context)
        {
            if (parameters.Count > 2
                && BrickUtils.TryGetBrickTypeName(parameters[0], out var conditionName)
                && BrickUtils.TryGetBrickParameters(parameters[0], out var conditionParams)
                && BrickUtils.TryGetBrickTypeName(parameters[1], out var positiveActionName)
                && BrickUtils.TryGetBrickParameters(parameters[1], out var positiveActionParams)
                && BrickUtils.TryGetBrickTypeName(parameters[2], out var negativeActionName)
                && BrickUtils.TryGetBrickParameters(parameters[2], out var negativeActionParams))
            {
                BrickService.GetInstance().TryCreate(conditionName, out BrickCondition condition);
                BrickService.GetInstance().TryCreate(positiveActionName, out BrickAction positiveAction);
                BrickService.GetInstance().TryCreate(negativeActionName, out BrickAction negativeAction);

                if (condition != null && positiveAction != null && negativeAction != null)
                {
                    if (condition.Run(conditionParams, context))
                    {
                        positiveAction.Run(positiveActionParams, context);
                    }
                    else
                    {
                        negativeAction.Run(negativeActionParams, context);
                    }
                    
                    BrickService.GetInstance().Free(condition);
                    BrickService.GetInstance().Free(positiveAction);
                    BrickService.GetInstance().Free(negativeAction);
                    
                    return;
                }
                
                BrickService.GetInstance().Free(condition);
                BrickService.GetInstance().Free(positiveAction);
                BrickService.GetInstance().Free(negativeAction);
                
                throw new Exception($"BrickActionConditional Run has error! Parameters {parameters}");
            }
        }
    }
}