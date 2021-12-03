using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionNot : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionNot(type, subType);
        }

        private BrickConditionNot(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count > 0
                && parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick)
                && serviceBricks.ExecuteConditionBrick(conditionBrick, world, level + 1, out var result))
            {
                return !result;
            }
            
            throw new Exception($"BrickConditionNot Run parameters {parameters}!");
        }

        public override void Reset() { }
    }
}