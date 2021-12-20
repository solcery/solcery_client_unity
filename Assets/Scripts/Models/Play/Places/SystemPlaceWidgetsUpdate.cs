using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Games;
using Solcery.Models.Play.Game.State;
using Solcery.Models.Shared.Attributes.Place;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Places;
using Solcery.Widgets_new;

namespace Solcery.Models.Play.Places
{
    public interface ISystemPlaceWidgetsUpdate : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemPlaceWidgetsUpdate : ISystemPlaceWidgetsUpdate
    {
        private IGame _game;
        private EcsFilter _filterPlaceWithPlaceWidget;
        private EcsFilter _filterEntities;
        private EcsFilter _filterGameStateUpdate;

        public static ISystemPlaceWidgetsUpdate Create(IGame game)
        {
            return new SystemPlaceWidgetsUpdate(game);
        }

        private SystemPlaceWidgetsUpdate(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            _filterPlaceWithPlaceWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>()
                .Inc<ComponentPlaceWidgetNew>().End();
            _filterEntities = systems.GetWorld().Filter<ComponentObjectTag>().Inc<ComponentAttributePlace>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_filterGameStateUpdate.GetEntitiesCount() <= 0)
            {
                return;
            }

            var entitiesInPlace = new Dictionary<int, List<int>>();
            // Подготовим набор entity в place
            foreach (var entityIndex in _filterEntities)
            {
                var entityPlaceId = systems.GetWorld().GetPool<ComponentAttributePlace>().Get(entityIndex).Value;

                if (!entitiesInPlace.ContainsKey(entityPlaceId.Current))
                {
                    entitiesInPlace.Add(entityPlaceId.Current, new List<int>());
                }
                
                entitiesInPlace[entityPlaceId.Current].Add(entityIndex);
            }

            var world = systems.GetWorld();
            var poolPlaceId = world.GetPool<ComponentPlaceId>();
            var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
            // TODO: New place widget update
            foreach (var entityId in _filterPlaceWithPlaceWidget)
            {
                var placeId = poolPlaceId.Get(entityId).Id;
                var entityIds = entitiesInPlace.TryGetValue(placeId, out var eid) ? eid.ToArray() : new int[]{};
                var placeWidget = poolPlaceWidgetNew.Get(entityId).Widget;
                placeWidget.Update(world, entityIds);
            }
            
            PlaceWidget.RefreshPlaceWidgetOrderZ(_game.WidgetCanvas.GetUiCanvas());
            
            entitiesInPlace.Clear();
        }
    }
}