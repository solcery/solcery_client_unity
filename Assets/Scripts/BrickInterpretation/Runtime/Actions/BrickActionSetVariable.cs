using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionSetVariable : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetVariable(type, subType);
        }

        private BrickActionSetVariable(int type, int subType) : base(type, subType)
        {
        }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string varName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, context, level + 1, out var value))
            {
                context.GameVars.Update(varName, value);
                return;
            }
            
            throw new Exception($"BrickActionSetVariable Run parameters {parameters}!");
        }
    }
}