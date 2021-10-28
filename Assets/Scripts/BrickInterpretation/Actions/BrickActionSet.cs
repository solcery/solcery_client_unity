using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionSet : BrickAction
    {
        public static BrickAction Create(string typeName)
        {
            return new BrickActionSet(typeName);
        }

        private BrickActionSet(string typeName)
        {
            TypeName = typeName;
        }

        public override void Reset() { }

        public override void Run(IBrickService brickService, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                brickService.ExecuteActionBrick(parameterToken, context);
            }
        }
    }
}