using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionIfThenElse : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionIfThenElse(type, subType);
        }
        
        private BrickActionIfThenElse(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 3 
                && parameters[0] is JObject ifObject
                && parameters[1] is JObject thenObject
                && parameters[2] is JObject elseObject
                && ifObject.TryGetValue("value", out JObject ifBrick)
                && thenObject.TryGetValue("value", out JObject thenBrick)
                && elseObject.TryGetValue("value", out JObject elseBrick))
            {
                if (serviceBricks.ExecuteConditionBrick(ifBrick, world, level + 1, out var result))
                {
                    serviceBricks.ExecuteActionBrick(result ? thenBrick : elseBrick, world, level + 1);
                    return;
                }
            }
            
            throw new Exception($"BrickActionIfThenElse Run parameters {parameters}!");
        }        
    }
}