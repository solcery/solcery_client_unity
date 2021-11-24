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
                ref var contextObject = ref world.GetPool<ComponentContextObject>().GetRawDenseItems()[0];
                var attrsPool = world.GetPool<ComponentEntityAttributes>();
                if (contextObject.TryGet(out int entityId))
                {
                    var attrs = attrsPool.Get(entityId).Attributes;
                    if (attrs.ContainsKey(attrName))
                    {
                        attrs["attrName"] = v1;
                        return;
                    }
                }
            }

            throw new Exception($"BrickValueSetAttribute Run parameters {parameters}!");
        }        
    }
}