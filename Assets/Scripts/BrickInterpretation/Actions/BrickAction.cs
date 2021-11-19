using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public abstract class BrickAction : Brick
    {
        public abstract void Run(IServiceBricks serviceBricks, JArray parameters, IContext context);
    }
}