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
        
        public override bool Run(IBrickService brickService, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                var result = !brickService.ExecuteConditionBrick(parameterToken, context);
                    
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