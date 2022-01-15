using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public class BrickValueIfThenElse : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueIfThenElse(type, subType);
        }
        
        private BrickValueIfThenElse(int type, int subType) : base(type, subType) { }
        
        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 3)
            {
                if (parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick) 
                    && serviceBricks.ExecuteConditionBrick(conditionBrick, context, level + 1, out var result))
                {
                    if (result)
                    {
                        if (parameters[1].TryParseBrickParameter(out _, out JObject actionBrick) 
                            && serviceBricks.ExecuteValueBrick(actionBrick, context, level + 1, out var value))
                        {
                            return value;
                        }
                    }
                    else
                    {
                        if (parameters[2].TryParseBrickParameter(out _, out JObject actionBrick) 
                            && serviceBricks.ExecuteValueBrick(actionBrick, context, level + 1, out var value))
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