using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueIfThenElse : BrickValue
    {
        public static BrickValueIfThenElse Create(int type, int subType)
        {
            return new BrickValueIfThenElse(type, subType);
        }
        
        private BrickValueIfThenElse(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3)
            {
                if (serviceBricks.ExecuteConditionBrick(parameters[0], world, out var result))
                {
                    if (result)
                    {
                        if (serviceBricks.ExecuteValueBrick(parameters[1], world, out var value))
                        {
                            return value;
                        }
                    }
                    else
                    {
                        if (serviceBricks.ExecuteValueBrick(parameters[2], world, out var value))
                        {
                            return value;
                        }
                    }
                }
            }

            throw new ArgumentException($"BrickValueIfThenElse Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }        
    }
}