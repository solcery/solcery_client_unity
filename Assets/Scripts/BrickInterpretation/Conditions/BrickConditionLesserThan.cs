using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionLesserThan : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionLesserThan(type, subType);
        }

        private BrickConditionLesserThan(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            var v1 = serviceBricks.ExecuteValueBrick(parameters[0], world);
            var v2 = serviceBricks.ExecuteValueBrick(parameters[1], world);
            return v1 <= v2;
        }

        public override void Reset() { }
    }
}