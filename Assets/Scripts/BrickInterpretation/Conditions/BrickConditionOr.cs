using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionOr : BrickCondition
    {
        public static BrickCondition Create(int type, int subType)
        {
            return new BrickConditionOr(type, subType);
        }

        private BrickConditionOr(int type, int subType) : base(type, subType) { }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                var result = serviceBricks.ExecuteConditionBrick(parameterToken, context);
                    
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Reset() { }
    }
}