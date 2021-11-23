using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionGreaterThan : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionGreaterThan(type, subType);
        }

        private BrickConditionGreaterThan(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (serviceBricks.ExecuteValueBrick(parameters[0], world, out var v1) 
                && serviceBricks.ExecuteValueBrick(parameters[1], world, out var v2))
            {
                return v1 >= v2;
            }

            throw new Exception($"BrickConditionGreaterThan Run parameters {parameters}!");
        }

        public override void Reset() { }
    }
}