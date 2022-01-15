using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Conditions
{
    public sealed class BrickConditionArgument : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionArgument(type, subType);
        }
        
        private BrickConditionArgument(int type, int subType) : base(type, subType) { }

        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string argName))
            {
                var args = context.GameArgs.Pop();
                if (args.TryGetValue(argName, out var brickObject))
                {
                    if (serviceBricks.ExecuteConditionBrick(brickObject, context, level + 1, out var result))
                    {
                        context.GameArgs.Push(args);
                        return result;
                    }
                }
            }

            throw new ArgumentException($"BrickConditionArgument Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}