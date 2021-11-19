using System;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionConditional : BrickAction
    {
        public static BrickActionConditional Create(string typeName)
        {
            return new BrickActionConditional(typeName);
        }

        private BrickActionConditional(string typeName)
        {
            TypeName = typeName;
        }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            if (parameters.Count <= 2)
            {
                throw new Exception($"BrickActionConditional Run has error! Parameters {parameters}");
            }

            serviceBricks.ExecuteActionBrick(serviceBricks.ExecuteConditionBrick(parameters[0], context) 
                ? parameters[1] 
                : parameters[2], 
                context);
        }
    }
}