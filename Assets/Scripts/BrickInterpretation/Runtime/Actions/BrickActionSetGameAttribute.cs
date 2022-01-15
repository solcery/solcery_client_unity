using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionSetGameAttribute : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetGameAttribute(type, subType);
        }
        
        private BrickActionSetGameAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, context, level + 1, out var value))
            {
                if (context.GameAttrs.Contains(attrName))
                {
                    context.GameAttrs.Update(attrName, value);
                    return;
                }
            }

            throw new Exception($"BrickActionSetGameAttribute Run parameters {parameters}!");
        }
    }
}