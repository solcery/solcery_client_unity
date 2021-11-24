using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionTwoActions : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionTwoActions(type, subType);
        }
        
        private BrickActionTwoActions(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2 
                && parameters[0] is JObject actionObject1
                && actionObject1.TryGetValue("value", out JObject actionBrick1)
                && parameters[1] is JObject actionObject2
                && actionObject2.TryGetValue("value", out JObject actionBrick2)
                && serviceBricks.ExecuteActionBrick(actionBrick1, world)
                && serviceBricks.ExecuteActionBrick(actionBrick2, world))
            {
                return;
            }
            
            throw new Exception($"BrickActionTwoActions Run parameters {parameters}!");
        }
    }
}