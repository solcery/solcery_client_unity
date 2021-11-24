using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueSub : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueSub(type, subType);
        }
        
        private BrickValueSub(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 &&
                serviceBricks.ExecuteValueBrick(parameters[0], world, out var v1) &&
                serviceBricks.ExecuteValueBrick(parameters[1], world, out var v2))
            {
                return v1 - v2;
            }

            throw new ArgumentException($"BrickValueSub Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}