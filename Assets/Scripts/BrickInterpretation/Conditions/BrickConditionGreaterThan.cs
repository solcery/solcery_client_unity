using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionGreaterThan : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionGreaterThan(typeName);
        }

        private BrickConditionGreaterThan(string typeName)
        {
            TypeName = typeName;
        }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            var v1 = serviceBricks.ExecuteValueBrick(parameters[0], context);
            var v2 = serviceBricks.ExecuteValueBrick(parameters[1], context);
            return v1 >= v2;
        }

        public override void Reset() { }
    }
}