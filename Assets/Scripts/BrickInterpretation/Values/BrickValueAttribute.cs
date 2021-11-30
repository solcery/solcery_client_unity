using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Entities;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueAttribute : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueAttribute(type, subType);
        }
        
        private BrickValueAttribute(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string attrName))
            {
                var filter = world.Filter<ComponentContextObject>().End();
                foreach (var uniqEntityId in filter)
                {
                    ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityId);
                    var attrsPool = world.GetPool<ComponentEntityAttributes>();
                    if (contextObject.TryPeek(out int entityId))
                    {
                        var attrs = attrsPool.Get(entityId).Attributes;
                        if (attrs != null && attrs.TryGetValue(attrName, out var attr))
                        {
                            return attr;
                        }
                    }
                    break;
                }
            }

            throw new ArgumentException($"BrickValueAttribute Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}