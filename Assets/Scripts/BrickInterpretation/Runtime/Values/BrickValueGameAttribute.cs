using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public class BrickValueGameAttribute : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueGameAttribute(type, subType);
        }
        
        private BrickValueGameAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 1
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && context.GameAttrs.TryGetValue(attrName, out var gameAttrValue))
            {
                return gameAttrValue;
            }

            throw new Exception($"BrickValueGameAttribute Run parameters {parameters}!");
        }        
    }
}