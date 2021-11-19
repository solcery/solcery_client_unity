using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionAnd : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionAnd(typeName);
        }

        private BrickConditionAnd(string typeName)
        {
            TypeName = typeName;
        }
        
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