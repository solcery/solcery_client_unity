using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionConditional : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionConditional(type, subType);
        }
        
        private BrickActionConditional(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count <= 2 
                || !serviceBricks.ExecuteConditionBrick(parameters[0], world, out var conditionResult))
            {
                throw new Exception($"BrickActionConditional Run has error! Parameters {parameters}");
            }

            serviceBricks.ExecuteActionBrick(conditionResult ? parameters[1] : parameters[2], world);
        }
    }
}