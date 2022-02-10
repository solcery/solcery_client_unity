using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public sealed class PlaceWidgetEclipse : PlaceWidget<PlaceWidgetEclipseLayout>
    {
        private Dictionary<int, IEclipseCardInContainerWidget> _cards;
        
        // TODO: Remove this
        private bool _isHand;

        public static PlaceWidget CreateHand(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipse(widgetCanvas, game, prefabPathKey, placeDataObject, true);
        }
        
        public static PlaceWidget CreateSingle(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipse(widgetCanvas, game, prefabPathKey, placeDataObject, false);
        }
        
        private PlaceWidgetEclipse(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject, bool isHand)
            : this(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _isHand = isHand;
        }

        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipse(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetEclipse(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, IEclipseCardInContainerWidget>();
            Layout.UpdateVisible(true);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            if (entityIds.Length <= 0)
            {
                return;
            }

            Debug.Log("PlaceWidgetEclipse");

            RemoveCards(world, entityIds);
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var cardTypes = new Dictionary<int, JObject>();
            
            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                cardTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                break;
            }

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;

                if (_cards.ContainsKey(objectId))
                {
                    continue;
                }
                
                if (Game.EclipseCardInContainerWidgetPool.TryPop(out var eclipseCard))
                {
                    if (objectTypePool.Has(entityId)
                        && cardTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var cardTypeDataObject)
                        && objectIdPool.Has(entityId))
                    {
                        eclipseCard.UpdateFromCardTypeData(objectIdPool.Get(entityId).Id, cardTypeDataObject);
                    }
                    
                    Layout.AddCard(eclipseCard);
                    _cards.Add(objectId, eclipseCard);
                }
            }
        }
        
        private void RemoveCards(EcsWorld world, int[] entityIds)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var keys = _cards.Keys.ToList();

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                keys.Remove(objectId);
            }

            foreach (var key in keys)
            {
                Game.EclipseCardInContainerWidgetPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }
    }
}