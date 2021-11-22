using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionSet : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSet(type, subType);
        }
        
        private BrickActionSet(int type, int subType) : base(type, subType) { }
        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            foreach (var parameterToken in parameters)
            {
                serviceBricks.ExecuteActionBrick(parameterToken, context);
            }
        }
    }
}