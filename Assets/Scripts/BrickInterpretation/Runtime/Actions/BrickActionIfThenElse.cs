using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionIfThenElse : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionIfThenElse(type, subType);
        }
        
        private BrickActionIfThenElse(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 3 
                && parameters[0] is JObject ifObject
                && parameters[1] is JObject thenObject
                && parameters[2] is JObject elseObject
                && ifObject.TryGetValue("value", out JObject ifBrick)
                && thenObject.TryGetValue("value", out JObject thenBrick)
                && elseObject.TryGetValue("value", out JObject elseBrick))
            {
                if (serviceBricks.ExecuteConditionBrick(ifBrick, context, level + 1, out var result))
                {
                    serviceBricks.ExecuteActionBrick(result ? thenBrick : elseBrick, context, level + 1);
                    return;
                }
            }
            
            throw new Exception($"BrickActionIfThenElse Run parameters {parameters}!");
        }        
    }
}