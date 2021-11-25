using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueIfThenElse : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueIfThenElse(type, subType);
        }
        
        private BrickValueIfThenElse(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3)
            {
                if (parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick) 
                    && serviceBricks.ExecuteConditionBrick(conditionBrick, world, out var result))
                {
                    if (result)
                    {
                        if (parameters[1].TryParseBrickParameter(out _, out JObject actionBrick) 
                            && serviceBricks.ExecuteValueBrick(actionBrick, world, out var value))
                        {
                            return value;
                        }
                    }
                    else
                    {
                        if (parameters[2].TryParseBrickParameter(out _, out JObject actionBrick) 
                            && serviceBricks.ExecuteValueBrick(actionBrick, world, out var value))
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