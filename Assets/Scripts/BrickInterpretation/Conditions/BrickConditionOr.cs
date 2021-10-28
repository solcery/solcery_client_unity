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
        
        public override bool Run(IBrickService brickService, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                var result = brickService.ExecuteConditionBrick(parameterToken, context);
                    
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