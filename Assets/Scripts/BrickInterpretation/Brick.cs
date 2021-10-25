using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation
{
    public abstract class Brick
    {
        public abstract string BrickTypeName();
        public abstract void Reset();
    }

    public abstract class Brick<T> : Brick
    {
        public abstract T Run(JArray parameters, IContext context);
    }
}