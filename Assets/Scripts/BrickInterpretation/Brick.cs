using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation
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
        public abstract T Run(IServiceBricks serviceBricks, JArray parameters, IContext context);

        protected Brick(int type, int subType) : base(type, subType) { }
    }
}