using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Values
{
    public sealed class BrickValueRandom : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueRandom(type, subType);
        }
        
        public BrickValueRandom(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject brickFrom)
                && parameters[1].TryParseBrickParameter(out _, out JObject brickTo)
                && serviceBricks.ExecuteValueBrick(brickFrom, context, level + 1, out var from)
                && serviceBricks.ExecuteValueBrick(brickTo, context, level + 1, out var to))
            {
                var rnd = new Random();
                return rnd.Next(from, to);
            }
            
            throw new ArgumentException($"BrickValueRandom Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}