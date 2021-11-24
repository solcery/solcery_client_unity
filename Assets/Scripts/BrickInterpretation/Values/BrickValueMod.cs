using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueMod : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueMod(type, subType);
        }
        
        private BrickValueMod(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 &&
                serviceBricks.ExecuteValueBrick(parameters[0], world, out var v1) &&
                serviceBricks.ExecuteValueBrick(parameters[1], world, out var v2))
            {
                return v1 % v2;
            }

            throw new ArgumentException($"BrickValueMod Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}