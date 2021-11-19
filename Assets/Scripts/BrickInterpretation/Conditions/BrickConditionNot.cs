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
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            return !serviceBricks.ExecuteConditionBrick(parameters[0], context);
        }

        public override void Reset() { }
    }
}