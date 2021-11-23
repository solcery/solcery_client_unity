using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueDiv : BrickValue
    {
        public static BrickValueDiv Create(int type, int subType)
        {
            return new BrickValueDiv(type, subType);
        }
        
        private BrickValueDiv(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 &&
                serviceBricks.ExecuteValueBrick(parameters[0], world, out var v1) &&
                serviceBricks.ExecuteValueBrick(parameters[1], world, out var v2))
            {
                return v1 / v2;
            }

            throw new ArgumentException($"BrickValueDiv Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}