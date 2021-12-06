using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueCardId : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueCardId(type, subType);
        }

        private BrickValueCardId(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            var filter = world.Filter<ComponentContextObject>().End();
            foreach (var uniqEntityId in filter)
            {
                ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityId);
                if (contextObject.TryPeek(out int entityId))
                {
                    return world.GetPool<ComponentObjectId>().Get(entityId).Id;
                }
            }

            throw new Exception($"ValueBrickCardId Run parameters {parameters}!");
        }
    }
}