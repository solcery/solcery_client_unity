using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Models.Entities;
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
            if (parameters.Count >= 1
                && parameters[0] is JObject attrNameObject
                && attrNameObject.TryGetValue("value", out string attrName) 
                && serviceBricks.ExecuteValueBrick(parameters[1], world, out var v1))
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
                            attrs[attrName] = v1;
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