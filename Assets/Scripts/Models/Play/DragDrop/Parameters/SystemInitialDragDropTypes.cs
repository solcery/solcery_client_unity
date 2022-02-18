using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
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
            var tagPool = world.GetPool<Parameters.ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var destinationsPool = world.GetPool<ComponentDragDropParametersDestinations>();
            var destinationConditionPool = world.GetPool<ComponentDragDropParametersDestinationCondition>();
            var requiredEclipseCardTypePool = world.GetPool<ComponentDragDropParametersRequiredEclipseCardType>();
            
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

                        requiredEclipseCardTypePool.Add(entity).RequiredEclipseCardType =
                            dndObject.TryGetEnum("required_card_type", out EclipseCardTypes ect) 
                                ? ect 
                                : EclipseCardTypes.None;
                    }
                }
            }

            _gameContent = null;
        }
    }
}