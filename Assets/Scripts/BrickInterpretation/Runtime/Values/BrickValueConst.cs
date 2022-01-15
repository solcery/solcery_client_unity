using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public sealed class BrickValueConst : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueConst(type, subType);
        }
        
        private BrickValueConst(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out int value))
            {
                return value;
            }

            throw new ArgumentException($"BrickValueConst Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}