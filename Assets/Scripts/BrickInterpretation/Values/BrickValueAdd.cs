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
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject valueBrick1)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick2))
            {
                var brickExecutionResult = serviceBricks.ExecuteValueBrick(valueBrick1, world, level + 1, out var value1);
                var value2 = 0;
                brickExecutionResult = brickExecutionResult &&
                                       serviceBricks.ExecuteValueBrick(valueBrick2, world, level + 1, out value2);

                if (brickExecutionResult)
                {
                    return value1 + value2;
                }
            }

            throw new ArgumentException($"BrickValueAdd Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}