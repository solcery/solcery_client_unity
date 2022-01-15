using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Conditions
{
    public sealed class BrickConditionConst : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionConst(type, subType);
        }

        private BrickConditionConst(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out bool value))
            {
                return value;
            }

            throw new ArgumentException($"BrickConditionConst Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}