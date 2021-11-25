using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionSetVariable : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetVariable(type, subType);
        }

        private BrickActionSetVariable(int type, int subType) : base(type, subType)
        {
        }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string varName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, world, out var value))
            {
                var filter = world.Filter<ComponentContextVars>().End();
                foreach (var entityId in filter)
                {
                    ref var contextVars = ref world.GetPool<ComponentContextVars>().Get(entityId);
                    contextVars.Set(varName, value);
                    return;
                }
            }
            
            throw new Exception($"BrickActionSetVariable Run parameters {parameters}!");
        }
    }
}