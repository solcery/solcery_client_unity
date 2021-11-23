using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueConst : BrickValue
    {
        public static BrickValueConst Create(int type, int subType)
        {
            return new BrickValueConst(type, subType);
        }
        
        private BrickValueConst(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject)
            {
                return serviceBricks.GetValueInt(valueObject, world);
            }

            throw new ArgumentException($"BrickValueConst Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}