using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public sealed class BrickValueAttribute : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueAttribute(type, subType);
        }
        
        private BrickValueAttribute(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && context.Object.TryPeek(out object @object)
                && context.ObjectAttrs.TryGetValue(@object, attrName, out var attrValue))
            {
                return attrValue;
            }

            throw new ArgumentException($"BrickValueAttribute Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}