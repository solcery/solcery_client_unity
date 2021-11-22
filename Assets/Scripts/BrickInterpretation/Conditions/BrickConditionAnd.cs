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
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                var result = !serviceBricks.ExecuteConditionBrick(parameterToken, context);
                    
                if (result)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Reset() { }
    }
}