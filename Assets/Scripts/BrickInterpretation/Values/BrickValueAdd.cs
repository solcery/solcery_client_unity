using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueAdd : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueAdd(type, subType);
        }
        
        private BrickValueAdd(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject valueBrick1)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick2)
                && serviceBricks.ExecuteValueBrick(valueBrick1, world, out var value1) 
                && serviceBricks.ExecuteValueBrick(valueBrick2, world, out var value2))
            {
                return value1 + value2;
            }

            throw new ArgumentException($"BrickValueAdd Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}