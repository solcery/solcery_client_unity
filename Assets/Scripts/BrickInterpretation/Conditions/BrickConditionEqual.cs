using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionEqual : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionEqual(typeName);
        }

        private BrickConditionEqual(string typeName)
        {
            TypeName = typeName;
        }
        
        public override bool Run(IBrickService brickService, JArray parameters, IContext context)
        {
            var v1 = brickService.ExecuteValueBrick(parameters[0], context);
            var v2 = brickService.ExecuteValueBrick(parameters[1], context);
            return v1 == v2;
        }

        public override void Reset() { }
    }
}