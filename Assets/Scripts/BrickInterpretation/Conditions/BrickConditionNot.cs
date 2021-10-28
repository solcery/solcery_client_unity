using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionNot : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionNot(typeName);
        }

        private BrickConditionNot(string typeName)
        {
            TypeName = typeName;
        }
        
        public override bool Run(IBrickService brickService, JArray parameters, IContext context)
        {
            return !brickService.ExecuteConditionBrick(parameters[0], context);
        }

        public override void Reset() { }
    }
}