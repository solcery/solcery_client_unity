using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public sealed class BrickActionUseCard : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionUseCard(type, subType);
        }
        
        private BrickActionUseCard(int type, int subType) : base(type, subType) { }

        public override void Reset() { }
        
        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (context.Object.TryPeek<object>(out var @object) 
                && context.GameObjects.TryGetCardTypeData(@object, out var data))
            {
                serviceBricks.ExecuteActionBrick(data["action"] as JObject, context, level + 1);
                return;
            }

            context.Log.Print("Call BrickActionUseCard!");
        }
    }
}