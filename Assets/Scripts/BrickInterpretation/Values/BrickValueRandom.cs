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

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 
                && parameters[0] is JObject brickFromObject 
                && brickFromObject.TryGetValue("from", out JObject brickFrom)
                && parameters[1] is JObject brickToObject 
                && brickToObject.TryGetValue("to", out JObject brickTo)
                && serviceBricks.ExecuteValueBrick(brickFrom, world, out var from)
                && serviceBricks.ExecuteValueBrick(brickTo, world, out var to))
            {
                return Random.Range(from, to);
            }
            
            throw new ArgumentException($"BrickValueRandom Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}