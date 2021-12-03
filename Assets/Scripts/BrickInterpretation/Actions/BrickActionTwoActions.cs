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

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 2 
                && parameters[0].TryParseBrickParameter(out _, out JObject actionBrick1)
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick2))
            {
                var brickExecutionResult = serviceBricks.ExecuteActionBrick(actionBrick1, world, level + 1);
                brickExecutionResult = brickExecutionResult && serviceBricks.ExecuteActionBrick(actionBrick2, world, level + 1);

                if (brickExecutionResult)
                {
                    return;
                }
            }
            
            throw new Exception($"BrickActionTwoActions Run parameters {parameters}!");
        }
    }
}