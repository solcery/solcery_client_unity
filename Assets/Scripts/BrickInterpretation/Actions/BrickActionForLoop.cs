using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionForLoop : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionForLoop(type, subType);
        }

        private BrickActionForLoop(int type, int subType) : base(type, subType)
        {
        }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3 && 
                serviceBricks.ExecuteValueBrick(parameters[0], world, out var v1) &&
                parameters[2] is JObject counter)
            {
                for (var i = 0; i < v1; i++)
                {
                    ref var contextVars = ref world.GetPool<ComponentContextVars>().GetRawDenseItems()[0];
                    contextVars.SetVar(counter.Value<string>(), i);
                    serviceBricks.ExecuteActionBrick(parameters[1], world);
                }
            }

            throw new Exception($"BrickActionForLoop Run parameters {parameters}!");
        }
    }
}