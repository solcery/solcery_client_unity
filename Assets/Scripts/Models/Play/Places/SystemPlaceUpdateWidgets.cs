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
    public interface ISystemPlaceUpdateWidgets : IEcsInitSystem, IEcsRunSystem
    {
    }

    public sealed class SystemPlaceUpdateWidgets : ISystemPlaceUpdateWidgets
    {
        private IGame _game;
        private EcsFilter _filterPlaceWithPlaceWidget;
        private EcsFilter _filterEntities;
        private EcsFilter _filterGameStateUpdate;

        public static ISystemPlaceUpdateWidgets Create(IGame game)
        {
            return new SystemPlaceUpdateWidgets(game);
        }

        private SystemPlaceUpdateWidgets(IGame game)
        {
            _game = game;
        }
        
        void IEcsInitSystem.Init(IEcsSystems systems)
        {
            _filterPlaceWithPlaceWidget = systems.GetWorld().Filter<ComponentPlaceTag>().Inc<ComponentPlaceId>()
                .Inc<ComponentPlaceWidgetNew>().End();
            _filterEntities = systems.GetWorld().Filter<ComponentObjectTag>().Inc<ComponentAttributePlace>().End();
            _filterGameStateUpdate = systems.GetWorld().Filter<ComponentGameStateUpdateTag>().End();
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
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
            var poolPlaceVisible = world.GetPool<ComponentPlaceIsVisible>();
            var poolPlaceAvailable = world.GetPool<ComponentPlaceIsAvailable>();
            var poolPlaceWidgetNew = world.GetPool<ComponentPlaceWidgetNew>();
            // TODO: New place widget update
            foreach (var entityId in _filterPlaceWithPlaceWidget)
            {
                var placeId = poolPlaceId.Get(entityId).Id;
                var placeIsVisible = poolPlaceVisible.Get(entityId).IsVisible;
                var placeIsAvailable = poolPlaceAvailable.Get(entityId).IsAvailable;
                var entityIds = entitiesInPlace.TryGetValue(placeId, out var eid) ? eid.ToArray() : new int[]{};
                var placeWidget = poolPlaceWidgetNew.Get(entityId).Widget;
                placeWidget.Update(world, placeIsVisible, entityIds);
                placeWidget.UpdateAvailability(placeIsAvailable);
            }
            
            PlaceWidget.RefreshPlaceWidgetOrderZ(_game.WidgetCanvas.GetUiCanvas());
            
            entitiesInPlace.Clear();
        }
    }
}