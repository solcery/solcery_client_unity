using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueCardTypeId : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueCardTypeId(type, subType);
        }

        private BrickValueCardTypeId(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            var filter = world.Filter<ComponentContextObject>().End();
            foreach (var uniqEntityType in filter)
            {
                ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityType);
                if (contextObject.TryPeek(out int entityId))
                {
                    return world.GetPool<ComponentObjectType>().Get(entityId).Type;
                }
            }

            throw new Exception($"BrickValueCardTypeId Run parameters {parameters}!");
        }

    }
}