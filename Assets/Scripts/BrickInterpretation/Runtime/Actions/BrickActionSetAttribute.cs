using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionSetAttribute : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetAttribute(type, subType);
        }
        
        private BrickActionSetAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, context, level + 1, out var value)
                && context.Object.TryPeek<object>(out var @object)
                && context.ObjectAttrs.Contains(@object, attrName))
            {
                context.ObjectAttrs.Update(@object, attrName, value);
                return;
            }

            throw new Exception($"BrickActionSetAttribute Run parameters {parameters}!");
        }        
    }
}