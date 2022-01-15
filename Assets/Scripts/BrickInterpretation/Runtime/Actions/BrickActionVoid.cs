using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public sealed class BrickActionVoid : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionVoid(type, subType);
        }
        
        private BrickActionVoid(int type, int subType) : base(type, subType) { }
        
        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            //Debug.Log($"BrickActionVoid Run level {level}!");
        }
    }
}