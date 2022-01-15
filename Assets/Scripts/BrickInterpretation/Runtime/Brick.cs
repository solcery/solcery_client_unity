using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime
{
    public abstract class Brick
    {
        public int Type => _type;
        public int SubType => _subType;
        
        private readonly int _type;
        private readonly int _subType;

        protected Brick(int type, int subType)
        {
            _type = type;
            _subType = subType;
        }
        
        public abstract void Reset();
    }

    public abstract class Brick<T> : Brick
    {
        public abstract T Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level);

        protected Brick(int type, int subType) : base(type, subType) { }
    }
}