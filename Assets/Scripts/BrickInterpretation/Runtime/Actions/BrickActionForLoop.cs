using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionForLoop : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionForLoop(type, subType);
        }

        private BrickActionForLoop(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 3
                && parameters[0].TryParseBrickParameter(out _, out string counterVarName)
                && parameters[1].TryParseBrickParameter(out _, out JObject counterBrick)
                && serviceBricks.ExecuteValueBrick(counterBrick, context, level + 1, out var counter) 
                && parameters[2].TryParseBrickParameter(out _, out JObject actionBrick))
            {
                var failIteration = 0;
                for (var i = 0; i < counter; i++)
                {
                    context.GameVars.Update(counterVarName, i);
                    if (!serviceBricks.ExecuteActionBrick(actionBrick, context, level + 1))
                    {
                        failIteration++;
                    }
                }

                if (failIteration <= 0)
                {
                    return;
                }
            }

            throw new Exception($"BrickActionForLoop Run parameters {parameters}!");
        }
    }
}