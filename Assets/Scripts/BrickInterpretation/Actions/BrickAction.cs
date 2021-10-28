using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public abstract class BrickAction : Brick
    {
        public abstract void Run(IBrickService brickService, JArray parameters, IContext context);
    }
}