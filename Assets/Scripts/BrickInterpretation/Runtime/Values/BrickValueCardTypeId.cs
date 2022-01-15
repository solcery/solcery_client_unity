using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public class BrickValueCardTypeId : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueCardTypeId(type, subType);
        }

        private BrickValueCardTypeId(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (context.Object.TryPeek<object>(out var @object) 
                && context.GameObjects.TryGetCardTypeId(@object, out var cardTypeId))
            {
                return cardTypeId;
            }

            throw new Exception($"BrickValueCardTypeId Run parameters {parameters}!");
        }

    }
}