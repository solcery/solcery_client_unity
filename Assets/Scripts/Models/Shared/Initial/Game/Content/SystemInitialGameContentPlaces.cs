using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Models.Shared.Places;
using Solcery.Utils;

namespace Solcery.Models.Shared.Initial.Game.Content
{
    public interface ISystemInitialGameContentPlaces : IEcsInitSystem { }

    public sealed class SystemInitialGameContentPlaces : ISystemInitialGameContentPlaces
    {
        public static ISystemInitialGameContentPlaces Create()
        {
            return new SystemInitialGameContentPlaces();
        }
        
        private SystemInitialGameContentPlaces() { }

        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var serviceGameContent = systems.GetShared<IGame>().ServiceGameContent;
            
            // Только тут мы инитим place, так что можно спокойно удалить если что то есть еще
            var filter = world.Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>().End();
            var dragDropFilter = world
                .Filter<ComponentDragDropParametersTag>()
                .Inc<ComponentDragDropParametersId>()
                .End();
            var dragDropIdPool = world.GetPool<ComponentDragDropParametersId>();

            foreach (var entityId in filter)
            {
                world.DelEntity(entityId);
            }

            foreach (var placeToken in serviceGameContent.Places)
            {
                if (placeToken is JObject placeObject)
                {
                    var entityIndex = world.NewEntity();
                    world.GetPool<ComponentPlaceTag>().Add(entityIndex);
                    world.GetPool<ComponentPlaceId>().Add(entityIndex).Id =
                        placeObject.GetValue<int>(GameJsonKeys.PlaceId);
                    if (placeObject.TryGetValue("drag_n_drops", out JArray dragDropIdArray))
                    {
                        var idHash = new HashSet<int>();
                        foreach (var dragDropIdToken in dragDropIdArray)
                        {
                            idHash.Add(dragDropIdToken.Value<int>());
                        }

                        ref var entityIdsComponent =
                            ref world.GetPool<ComponentPlaceDragDropEntityId>().Add(entityIndex);
                            
                        foreach (var dragDropEntityId in dragDropFilter)
                        {
                                
                            if (idHash.Contains(dragDropIdPool.Get(dragDropEntityId).Id))
                            {
                                entityIdsComponent.DragDropEntityIds.Add(dragDropEntityId);
                                idHash.Remove(dragDropIdPool.Get(dragDropEntityId).Id);
                            }
                        }
                            
                    }
                }
            }
        }
    }
}