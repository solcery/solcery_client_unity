using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation
{
    public abstract class Brick
    {
        public string TypeName { get; protected set; }
        public abstract void Reset();
    }

    public abstract class Brick<T> : Brick
    {
        public abstract T Run(IBrickService brickService, JArray parameters, IContext context);
    }
}