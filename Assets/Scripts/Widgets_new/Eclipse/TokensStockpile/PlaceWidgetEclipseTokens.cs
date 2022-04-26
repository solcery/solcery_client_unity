using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.Tokens;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.TokensStockpile
{
    public class PlaceWidgetEclipseTokens : PlaceWidget<PlaceWidgetEclipseTokensLayout>, IPlaceWidgetTokenCollection
    {
        private Dictionary<int, ITokenInContainerWidget> _tokens;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseTokens(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseTokens(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _tokens = new Dictionary<int, ITokenInContainerWidget>();
            Layout.UpdateVisible(true);
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            RemoveTokens(world, entityIds);
            if (entityIds.Length <= 0)
            {
                return;
            }
            
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var eclipseTokenTagPool = world.GetPool<ComponentEclipseTokenTag>();
            var cardTypes = world.GetCardTypes();

            foreach (var entityId in entityIds)
            {
                if (eclipseTokenTagPool.Has(entityId) && objectTypePool.Has(entityId))
                {
                    var typeId = objectTypePool.Get(entityId).Type;
                    if (cardTypes.TryGetValue(typeId, out var cardTypeDataObject))
                    {
                        var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;
                        
                        if (!_tokens.TryGetValue(typeId, out var tokenLayout))
                        {
                            if (Game.TokenInContainerWidgetPool.TryPop(out tokenLayout))
                            {
                                tokenLayout.UpdateFromCardTypeData(objectIdPool.Get(entityId).Id, cardTypeDataObject);
                                tokenLayout.UpdateParent(Layout.Content);
                                _tokens.Add(typeId, tokenLayout);
                            }
                        }
                        
                        ProcessTokenAttributes(world, tokenLayout, attributes);
                    }
                }
            }
        }

        private void ProcessTokenAttributes(EcsWorld world, ITokenInContainerWidget tokenLayout, Dictionary<string, IAttributeValue> attributes)
        {
            if (attributes.TryGetValue("anim_token_fly", out var animTokenFlyAttribute) && animTokenFlyAttribute.Current > 0)
            {
                var fromPlaceId = attributes.TryGetValue("anim_token_fly_from_place", out var fromPlaceAttribute) ? fromPlaceAttribute.Current : 0;
                var formCardId = attributes.TryGetValue("anim_token_fly_from_card_id", out var fromCardAttribute) ? fromCardAttribute.Current : 0;
                var fromSlotId = attributes.TryGetValue("anim_token_fly_from_slot", out var fromSlotAttribute) ? fromSlotAttribute.Current : 0;
                if (WidgetExtensions.TryGetTokenFromPosition(world, fromPlaceId, formCardId, fromSlotId, out var from))
                {
                    TokenAnimFly(tokenLayout, from);
                }
                else
                {
                    Debug.LogWarning($"Can't run token animation: anim_token_fly_from_place = {fromPlaceId}: anim_token_fly_from_card_id = {formCardId} and anim_token_fly_from_slot = {fromSlotId}");
                }
            }
            else
            {
                TokenAnimFlyCompleted(tokenLayout);
            }
            
            if (attributes.TryGetValue("anim_destroy", out var animDestroyAttribute) &&
                animDestroyAttribute.Current > 0)
            {
                TokenAnimDestroy(tokenLayout);
            }
        }

        public bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position)
        {
            if (world.TryGetCardTypeByCardId(cardId, out var type))
            {
                if (_tokens.TryGetValue(type, out var widget))
                {
                    position = widget.Layout.RectTransform.position;
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
        }
        
        private void TokenAnimFly(ITokenInContainerWidget tokenLayout, Vector3 from)
        {
            WidgetCanvas.GetEffects().MoveToken(tokenLayout.Layout.Icon.rectTransform, 
                tokenLayout.Layout.Icon.sprite,
                from,
                0.5f, () => { TokenAnimFlyCompleted(tokenLayout); });
        }

        private void TokenAnimFlyCompleted(ITokenInContainerWidget tokenLayout)
        {
            tokenLayout.IncreaseCounter();
        }

        private void TokenAnimDestroy(ITokenInContainerWidget tokenLayout)
        {
            // todo
        }
        
        private void RemoveTokens(EcsWorld world, int[] entityIds)
        {
            foreach (var token in _tokens)
            {
                token.Value.ClearCounter();
            }
            
            var objectTypePool = world.GetPool<ComponentObjectType>();
            var keys = _tokens.Keys.ToList();

            foreach (var entityId in entityIds)
            {
                var typeId = objectTypePool.Get(entityId).Type;
                keys.Remove(typeId);
            }
            
            foreach (var key in keys)
            {
                Game.TokenInContainerWidgetPool.Push(_tokens[key]);
                _tokens.Remove(key);
            }
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
