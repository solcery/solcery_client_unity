using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionVoid : BrickAction
    {
        public static BrickAction Create(string typeName)
        {
            return new BrickActionVoid(typeName);
        }

        private BrickActionVoid(string typeName)
        {
            TypeName = typeName;
        }
        
        public override void Reset() { }
        public override void Run(IBrickService brickService, JArray parameters, IContext context) { }
    }
}