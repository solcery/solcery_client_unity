using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Widgets;

namespace Solcery.Widgets_new.EclipseTokensStockpile
{
    public class PlaceWidgetEclipseTokens : PlaceWidget<PlaceWidgetEclipseTokensLayout>
    {
        private Dictionary<int, ICardInContainerWidget> _tokens;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseTokens(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseTokens(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _tokens = new Dictionary<int, ICardInContainerWidget>();
            Layout.UpdateVisible(true);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            RemoveTokens(world, entityIds);
            if (entityIds.Length <= 0)
            {
                return;
            }
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var eclipseTokenTagPool = world.GetPool<ComponentEclipseTokenTag>();
            var cardTypes = new Dictionary<int, JObject>();

            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                cardTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                break;
            }

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;

                if (eclipseTokenTagPool.Has(entityId) && objectTypePool.Has(entityId)))
                {
                    var typeId = objectTypePool.Get(entityId).Type;
                    if (cardTypes.TryGetValue(typeId, out var cardTypeDataObject))
                    {
                        if (!_tokens.TryGetValue(typeId, out var tokenLayout))
                        {
                            if (Game.PlaceWidgetFactory.CardInContainerPool.TryPop(out tokenLayout))
                            {
                                tokenLayout.UpdateFromCardTypeData(objectIdPool.Get(entityId).Id, cardTypeDataObject);
                                tokenLayout.UpdateParent(Layout.Content);
                                _tokens.Add(typeId, tokenLayout);
                            }
                        }
                        else
                        {
                            // increase counter
                        }
                    }
                }
            }
        }
        
        private void RemoveTokens(EcsWorld world, int[] entityIds)
        {
        }
        
        protected override void DestroyImpl()
        {
            foreach (var tokenInContainerWidget in _tokens)
            {
                tokenInContainerWidget.Value.Destroy();
            }
            _tokens.Clear();
            _tokens = null;
        }
    }
}
