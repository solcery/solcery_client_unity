using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionForLoop : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionForLoop(type, subType);
        }

        private BrickActionForLoop(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3
                && parameters[0].TryParseBrickParameter(out _, out JObject counterBrick)
                && serviceBricks.ExecuteValueBrick(counterBrick, world, out var counter) 
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick)
                && parameters[2].TryParseBrickParameter(out _, out string counterVarName))
            {
                var filter = world.Filter<ComponentContextVars>().End();
                foreach (var entityId in filter)
                {
                    var failIteration = 0;
                    ref var contextVars = ref world.GetPool<ComponentContextVars>().Get(entityId);
                    for (var i = 0; i < counter; i++)
                    {
                        contextVars.Set(counterVarName, i);
                        if (!serviceBricks.ExecuteActionBrick(actionBrick, world))
                        {
                            failIteration++;
                        }
                    }

                    if (failIteration < 0)
                    {
                        return;
                    }
                    
                    break;
                }
            }

            throw new Exception($"BrickActionForLoop Run parameters {parameters}!");
        }
    }
}