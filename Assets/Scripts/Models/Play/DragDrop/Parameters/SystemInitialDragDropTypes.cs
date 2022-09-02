using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Utils;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public interface ISystemInitialDragDropTypes : IEcsInitSystem
    {
    }

    public sealed class SystemInitialDragDropTypes : ISystemInitialDragDropTypes
    {
        public static ISystemInitialDragDropTypes Create()
        {
            return new SystemInitialDragDropTypes();
        }
        
        private SystemInitialDragDropTypes() { }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            var serviceGameContent = systems.GetShared<IGame>().ServiceGameContent;
            var world = systems.GetWorld();
            var tagPool = world.GetPool<ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var destinationsPool = world.GetPool<ComponentDragDropParametersDestinations>();

            // New brick conditions
            var destinationConditionBrickPool = world.GetPool<ComponentDragDropBrickDestinationCondition>();
            var originConditionBrickPool = world.GetPool<ComponentDragDropBrickOriginCondition>();

            foreach (var dndToken in serviceGameContent.DragDrop)
            {
                if (dndToken is JObject dndObject)
                {
                    var entity = world.NewEntity();
                    tagPool.Add(entity);
                    idPool.Add(entity).Id = dndObject.GetValue<int>("id");
                        
                    ref var componentDestinations = ref destinationsPool.Add(entity);
                    foreach (var destinationIdToken in dndObject.GetValue<JArray>("destinations"))
                    {
                        componentDestinations.PlaceIds.Add(destinationIdToken.Value<int>());
                    }

                    ref var dcb = ref destinationConditionBrickPool.Add(entity);
                    if (!dndObject.TryGetValue("destination_cond", out dcb.ConditionBrick))
                    {
                        dcb.ConditionBrick = null;
                    }

                    ref var ocb = ref originConditionBrickPool.Add(entity);
                    if (!dndObject.TryGetValue("origin_cond", out ocb.ConditionBrick))
                    {
                        ocb.ConditionBrick = null;
                    }
                }
            }
        }
    }
}