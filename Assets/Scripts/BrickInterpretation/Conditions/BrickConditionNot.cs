using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionNot : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionNot(type, subType);
        }

        private BrickConditionNot(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            return !serviceBricks.ExecuteConditionBrick(parameters[0], world);
        }

        public override void Reset() { }
    }
}