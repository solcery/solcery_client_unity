using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Play.DragDrop;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.Tokens;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.TokensStockpile
{
    public class PlaceWidgetEclipseTokens : PlaceWidget<PlaceWidgetEclipseTokensLayout>, IApplyDragWidget, IApplyDropWidget, IPlaceWidgetTokenCollection
    {
        private Dictionary<int, IListTokensInContainerWidget> _tokensByType;
        private Dictionary<int, ITokenInContainerWidget> _tokens;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetEclipseTokens(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseTokens(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _tokensByType = new Dictionary<int, IListTokensInContainerWidget>();
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
                    var objectId = objectIdPool.Get(entityId).Id;
                    var attributes = world.GetPool<ComponentObjectAttributes>().Get(entityId).Attributes;

                    if (!_tokensByType.ContainsKey(typeId))
                    {
                        AttachListTokens(typeId);
                    }

                    if (!_tokens.TryGetValue(objectId, out var eclipseToken))
                    {
                        eclipseToken = AttachToken(world, entityId, objectId, typeId, cardTypes);
                        AttachDragAndDrop(world, entityId, objectId, eclipseToken);
                    }

                    UpdateToken(world, eclipseToken, attributes);
                    UpdateDragAndDrop(world, eclipseToken);
                }
            }
        }

        private void AttachListTokens(int typeId)
        {
            if (Game.ListTokensInContainerWidgetPool.TryPop(out var listTokens))
            {
                listTokens.UpdateParent(Layout.Content);
                listTokens.ClearCounter();
                _tokensByType.Add(typeId, listTokens);
                LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.Content);
            }
        }

        private ITokenInContainerWidget AttachToken(EcsWorld world,
            int entityId, 
            int objectId,
            int typeId,
            Dictionary<int, JObject> cardTypes)
        {
            if (world.GetPool<ComponentEclipseTokenTag>().Has(entityId))
            {
                var objectTypePool = world.GetPool<ComponentObjectType>();
                if (Game.TokenInContainerWidgetPool.TryPop(out var eclipseToken))
                {
                    if (objectTypePool.Has(entityId)
                        && cardTypes.TryGetValue(typeId, out var cardTypeDataObject))
                    {
                        eclipseToken.UpdateFromCardTypeData(objectId, typeId, cardTypeDataObject);
                    }
                    
                    if (_tokensByType.TryGetValue(typeId, out var tokenList))
                    {
                        tokenList.AddToken(eclipseToken);
                        _tokens.Add(objectId, eclipseToken);
                    }
                    else
                    {
                        Debug.LogWarning("Can't attach token to list of token!");
                    }
                }

                return eclipseToken;
            }

            return null;
        }

        private void UpdateToken(EcsWorld world, ITokenInContainerWidget eclipseToken, Dictionary<string, IAttributeValue> attributes)
        {
            eclipseToken.Active = true;
            if (attributes.TryGetValue("anim_token_fly", out var animTokenFlyAttribute) && animTokenFlyAttribute.Current > 0)
            {
                var fromPlaceId = attributes.TryGetValue("anim_token_fly_from_place", out var fromPlaceAttribute) ? fromPlaceAttribute.Current : 0;
                var formCardId = attributes.TryGetValue("anim_token_fly_from_card_id", out var fromCardAttribute) ? fromCardAttribute.Current : 0;
                var fromSlotId = attributes.TryGetValue("anim_token_fly_from_slot", out var fromSlotAttribute) ? fromSlotAttribute.Current : 0;
                if (WidgetExtensions.TryGetTokenFromPosition(world, fromPlaceId, formCardId, fromSlotId, out var from))
                {
                    TokenAnimFly(eclipseToken, from);
                }
                else
                {
                    Debug.LogWarning($"Can't run token animation: anim_token_fly_from_place = {fromPlaceId}: anim_token_fly_from_card_id = {formCardId} and anim_token_fly_from_slot = {fromSlotId}");
                }
            }
            else
            {
                TokenAnimFlyCompleted(eclipseToken);
            }
            
            if (attributes.TryGetValue("anim_destroy", out var animDestroyAttribute) &&
                animDestroyAttribute.Current > 0)
            {
                TokenAnimDestroy(eclipseToken);
            }
        }

        public bool TryGetTokenPosition(EcsWorld world, int cardId, int slotId, out Vector3 position)
        {
            if (_tokens.TryGetValue(cardId, out var widget))
            {
                position = widget.Layout.RectTransform.position;
                return true;
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
            if (_tokensByType.TryGetValue(tokenLayout.TypeId, out var listTokens))
            {
                listTokens.IncreaseCounter();
            }
        }

        private void TokenAnimDestroy(ITokenInContainerWidget tokenLayout)
        {
            // todo
        }
        
        private void RemoveTokens(EcsWorld world, int[] entityIds)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var keys = _tokens.Keys.ToList();
            
            // clear tokens
            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                keys.Remove(objectId);
            }
           
            foreach (var key in keys)
            {
                var eid = _tokens[key].AttachEntityId;
                if (eid >= 0)
                {
                    world.DelEntity(eid);
                }

                _tokens[key].UpdateAttachEntityId();
                Game.TokenInContainerWidgetPool.Push(_tokens[key]);
                _tokens.Remove(key);
            }
            
            keys.Clear();

            // clear token types
            foreach (var tokensList in _tokensByType)
            {
                tokensList.Value.ClearCounter();
                if (tokensList.Value.Layout.Content.childCount == 0)
                {
                    keys.Add(tokensList.Key);
                }
            }

            foreach (var key in keys)
            {
                Game.ListTokensInContainerWidgetPool.Push(_tokensByType[key]);
                _tokensByType.Remove(key);
            }
        }
        
        protected override void DestroyImpl()
        {
            foreach (var tokensList in _tokensByType)
            {
                tokensList.Value.Destroy();
            }
            _tokensByType.Clear();
            _tokensByType = null;
            
            foreach (var tokenInContainerWidget in _tokens)
            {
                tokenInContainerWidget.Value.Destroy();
            }
            _tokens.Clear();
            _tokens = null;
        }

        // todo подумать как сделать получше
        // функция, которая актуализирует SourcePlaceEntityId, т.к. при Drop этот параметр меняется на 
        // id плейса, в который юзер пытается переместить токен, но токен нельзя переместить в пустой плейс
        // и по факту он остается в текущем плейсе но с неверным SourcePlaceEntityId  
        void UpdateDragAndDrop(EcsWorld world, ITokenInContainerWidget eclipseToken)
        {
            world.GetPool<ComponentDragDropSourcePlaceEntityId>().Get(eclipseToken.AttachEntityId).SourcePlaceEntityId =
                Layout.LinkedEntityId;
        }

        void AttachDragAndDrop(EcsWorld world, int entityId, int objectId, ITokenInContainerWidget eclipseToken)
        {
            var eid = world.NewEntity();
            world.GetPool<ComponentDragDropTag>().Add(eid);
            world.GetPool<ComponentDragDropView>().Add(eid).View = eclipseToken;
            world.GetPool<ComponentDragDropSourcePlaceEntityId>().Add(eid).SourcePlaceEntityId =
                Layout.LinkedEntityId;
            world.GetPool<ComponentDragDropEclipseCardType>().Add(eid).CardType =
                world.GetPool<ComponentEclipseCardType>().Has(entityId)
                    ? world.GetPool<ComponentEclipseCardType>().Get(entityId).CardType
                    : EclipseCardTypes.None;
            world.GetPool<ComponentDragDropObjectId>().Add(eid).ObjectId = objectId;
            eclipseToken.UpdateAttachEntityId(eid);        
        }
        
        #region IApplyDropWidget
        
        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            if (dropWidget is ITokenInContainerWidget ew)
            {
                if (_tokensByType.TryGetValue(ew.TypeId, out var listTokens))
                {
                    listTokens.AddToken(ew);
                    if (ew.Active)
                    {
                        listTokens.IncreaseCounter();
                    }

                    _tokens.Add(dropWidget.ObjectId, ew);
                }
                else
                {
                    Debug.LogWarning("Something wrong with tokens DD!");
                }
            }
        }

        #endregion
        
        #region IApplyDragWidget

        void IApplyDragWidget.OnDragWidget(IDraggableWidget dragWidget)
        {
            if (dragWidget is ITokenInContainerWidget ew)
            {
                if (_tokensByType.TryGetValue(ew.TypeId, out var listTokens))
                {
                    listTokens.DecreaseCounter();
                    _tokens.Remove(dragWidget.ObjectId);
                }
                else
                {
                    Debug.LogWarning("Something wrong with tokens DD!");
                }
            }
        }

        #endregion
    }
}
