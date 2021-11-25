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
                && parameters[0] is JObject varNameObject 
                && varNameObject.TryGetValue("value", out string varName)
                && serviceBricks.ExecuteValueBrick(parameters[1], world, out var v1))
            {
                var filter = world.Filter<ComponentContextVars>().End();
                foreach (var entityId in filter)
                {
                    ref var contextVars = ref world.GetPool<ComponentContextVars>().Get(entityId);
                    contextVars.Set(varName, v1);
                    return;
                }
            }
            
            throw new Exception($"BrickActionSetVariable Run parameters {parameters}!");
        }
    }
}