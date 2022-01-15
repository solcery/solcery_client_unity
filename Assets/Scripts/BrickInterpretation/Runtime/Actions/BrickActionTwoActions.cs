using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public sealed class BrickActionTwoActions : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionTwoActions(type, subType);
        }
        
        private BrickActionTwoActions(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject actionBrick1)
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick2)
                && serviceBricks.ExecuteActionBrick(actionBrick1, context, level + 1)
                && serviceBricks.ExecuteActionBrick(actionBrick2, context, level + 1))
            {
                return;
            }
            
            throw new Exception($"BrickActionTwoActions Run parameters {parameters}!");
        }
    }
}