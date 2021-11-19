using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionOr : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionOr(typeName);
        }

        private BrickConditionOr(string typeName)
        {
            TypeName = typeName;
        }
        
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