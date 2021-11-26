using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Models.Entities;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionUseCard : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionUseCard(type, subType);
        }
        
        private BrickActionUseCard(int type, int subType) : base(type, subType) { }

        public override void Reset() { }
        
        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            var filterObjects = world.Filter<ComponentContextObject>().End();
            var filterEntityTypes = world.Filter<ComponentEntityTypes>().End();
            foreach (var uniqEntityType in filterObjects)
            {
                ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityType);
                if (contextObject.TryPeek(out int entityId))
                {
                    var type = world.GetPool<ComponentEntityType>().Get(entityId).Type;
                    foreach (var uniqEntityTypes in filterEntityTypes)
                    {
                        ref var types = ref world.GetPool<ComponentEntityTypes>().Get(uniqEntityTypes);
                        if (types.Types.TryGetValue(type, out var data))
                        {
                            serviceBricks.ExecuteActionBrick(data["action"] as JObject, world);
                        }
                    }
                }
            }

            Debug.Log("Call BrickActionUseCard!");
        }
    }
}