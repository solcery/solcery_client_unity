using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public class BrickValueCardId : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueCardId(type, subType);
        }

        private BrickValueCardId(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (context.Object.TryPeek<object>(out var @object) 
                && context.GameObjects.TryGetCardId(@object, out var cardId))
            {
                return cardId;
            }

            throw new Exception($"ValueBrickCardId Run parameters {parameters}!");
        }
    }
}