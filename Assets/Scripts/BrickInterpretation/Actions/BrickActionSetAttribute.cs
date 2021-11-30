using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Entities;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionSetAttribute : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetAttribute(type, subType);
        }
        
        private BrickActionSetAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, world, out var value))
            {
                var filter = world.Filter<ComponentContextObject>().End();
                foreach (var uniqEntityId in filter)
                {
                    ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityId);
                    var attrsPool = world.GetPool<ComponentEntityAttributes>();
                    if (contextObject.TryPeek(out int entityId))
                    {
                        var attrs = attrsPool.Get(entityId).Attributes;
                        if (attrs.ContainsKey(attrName))
                        {
                            attrs[attrName] = value;
                            return;
                        }
                    }
                    break;
                }
            }

            throw new Exception($"BrickActionSetAttribute Run parameters {parameters}!");
        }        
    }
}