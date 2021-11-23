using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionAnd : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionAnd(type, subType);
        }

        private BrickConditionAnd(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            foreach (var parameterToken in parameters)
            {
                if (!serviceBricks.ExecuteConditionBrick(parameterToken, world, out var result))
                {
                    throw new Exception($"BrickConditionAnd Run parameters {parameters}");
                }
                    
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Reset() { }
    }
}