using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent;
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
            var destinationConditionPool = world.GetPool<ComponentDragDropParametersDestinationCondition>();
            var requiredEclipseCardTypesPool = world.GetPool<ComponentDragDropParametersRequiredEclipseCardTypes>();

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

                    destinationConditionPool.Add(entity).ParametersDestinationConditionType =
                        dndObject.TryGetEnum("destination_condition", out DragDropParametersDestinationConditionTypes dct)
                            ? dct
                            : DragDropParametersDestinationConditionTypes.Any;

                    ref var componentCardTypes = ref requiredEclipseCardTypesPool.Add(entity);
                    foreach (var cardTypeToken in dndObject.GetValue<JArray>("required_card_types"))
                    {
                        var cardType = cardTypeToken.Value<int>();
                        componentCardTypes.RequiredEclipseCardTypes.Add((EclipseCardTypes)cardType);
                    }
                }
            }
        }
    }
}