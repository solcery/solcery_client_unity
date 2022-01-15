using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public abstract class BrickAction : Brick
    {
        public abstract void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level);

        protected BrickAction(int type, int subType) : base(type, subType) { }
    }
}