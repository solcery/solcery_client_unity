using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionSet : BrickAction
    {
        public override string BrickTypeName()
        {
            return "brick_action_set";
        }

        public override void Reset() { }

        public override void Run(JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                if (BrickUtils.TryGetBrickTypeName(parameterToken, out var brickTypeName)
                    && BrickUtils.TryGetBrickParameters(parameterToken, out var @params)
                    && BrickService.GetInstance().TryCreate(brickTypeName, out BrickAction action))
                {
                    action.Run(@params, context);
                    BrickService.GetInstance().Free(action);
                }
                else
                {
                    throw new Exception($"BrickActionSet Run has error! Parameters {parameterToken}");
                }
            }
        }
    }
}