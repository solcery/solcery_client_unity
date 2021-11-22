using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueConst : BrickValue
    {
        public static BrickValueConst Create(int type, int subType)
        {
            return new BrickValueConst(type, subType);
        }
        
        private BrickValueConst(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject 
                && valueObject.TryGetValue("value", out int value))
            {
                return value;
            }

            throw new ArgumentException($"BrickValueConst Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}