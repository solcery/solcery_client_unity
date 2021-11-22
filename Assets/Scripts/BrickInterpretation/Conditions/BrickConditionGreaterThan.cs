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
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            var v1 = serviceBricks.ExecuteValueBrick(parameters[0], context);
            var v2 = serviceBricks.ExecuteValueBrick(parameters[1], context);
            return v1 >= v2;
        }

        public override void Reset() { }
    }
}