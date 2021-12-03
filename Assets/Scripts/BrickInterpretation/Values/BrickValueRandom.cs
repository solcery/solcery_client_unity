using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Random = UnityEngine.Random;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueRandom : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueRandom(type, subType);
        }
        
        public BrickValueRandom(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject brickFrom)
                && parameters[1].TryParseBrickParameter(out _, out JObject brickTo))
            {
                var brickExecutionResult = serviceBricks.ExecuteValueBrick(brickFrom, world, level + 1, out var from);
                var to = 0;
                brickExecutionResult = brickExecutionResult &&
                                       serviceBricks.ExecuteValueBrick(brickTo, world, level + 1, out to);

                if (brickExecutionResult)
                {
                    return Random.Range(from, to);
                }
            }
            
            throw new ArgumentException($"BrickValueRandom Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}