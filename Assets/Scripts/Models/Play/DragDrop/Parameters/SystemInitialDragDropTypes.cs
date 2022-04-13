using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public interface ISystemInitialDragDropTypes : IEcsInitSystem
    {
    }

    public sealed class SystemInitialDragDropTypes : ISystemInitialDragDropTypes
    {
        private JObject _gameContent;
        
        public static ISystemInitialDragDropTypes Create(JObject gameContent)
        {
            return new SystemInitialDragDropTypes(gameContent);
        }
        
        private SystemInitialDragDropTypes(JObject gameContent)
        {
            _gameContent = gameContent;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var tagPool = world.GetPool<ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var destinationsPool = world.GetPool<ComponentDragDropParametersDestinations>();
            var destinationConditionPool = world.GetPool<ComponentDragDropParametersDestinationCondition>();
            var requiredEclipseCardTypesPool = world.GetPool<ComponentDragDropParametersRequiredEclipseCardTypes>();

            if (_gameContent.TryGetValue("drag_n_drops", out JObject dndBaseObject)
                && dndBaseObject.TryGetValue("objects", out JArray dndArray))
            {
                foreach (var dndToken in dndArray)
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
                                : DragDropParametersDestinationConditionTypes.None;

                        ref var componentCardTypes = ref requiredEclipseCardTypesPool.Add(entity);
                        foreach (var cardTypeToken in dndObject.GetValue<JArray>("required_card_types"))
                        {
                            var cardType = cardTypeToken.Value<int>();
                            componentCardTypes.RequiredEclipseCardTypes.Add((EclipseCardTypes)cardType);
                        }
                    }
                }
            }

            _gameContent = null;
        }
    }
}