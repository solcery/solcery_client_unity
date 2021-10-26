using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionVoid : BrickAction
    {
        public override string BrickTypeName()
        {
            return "brick_action_void";
        }

        public override void Reset() { }

        public override void Run(JArray parameters, IContext context) { }
    }
}