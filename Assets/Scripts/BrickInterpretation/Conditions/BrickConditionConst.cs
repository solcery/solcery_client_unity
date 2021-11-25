using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionConst : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionConst(type, subType);
        }

        private BrickConditionConst(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
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