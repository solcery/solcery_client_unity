using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

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

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3)
            {
                if (serviceBricks.ExecuteConditionBrick(parameters[0], world, out var result))
                {
                    serviceBricks.ExecuteActionBrick(result ? parameters[1] : parameters[2], world);
                    return;
                }
            }
            
            throw new Exception($"BrickActionIfThenElse Run parameters {parameters}!");
        }        
    }
}