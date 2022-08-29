using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Cards.Pools;
using UnityEngine;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public sealed class EclipseCardInContainerWidget : IEclipseCardInContainerWidget
    {
        EclipseCardInContainerWidgetLayout IEclipseCardInContainerWidget.Layout => _layout;
        
        private IGame _game;
        private EclipseCardInContainerWidgetLayout _layout;
        private int _entityId;
        private int _cardType;
        private int _objectId;
        private int _order;

        int IEclipseCardInContainerWidget.Order => _order;
        int IEclipseCardInContainerWidget.EntityId => _entityId;
        int IEclipseCardInContainerWidget.CardType => _cardType;
        
        public static IEclipseCardInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new EclipseCardInContainerWidget(game, prefab, poolTransform);
        }
        
        private EclipseCardInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<EclipseCardInContainerWidgetLayout>();
        }

        void IEclipseCardInContainerWidget.UpdateFromCardTypeData(int entityId, int objectId, int objectType, IItemType itemType)
        {
            _entityId = entityId;
            _objectId = objectId;
            _cardType = objectType;

            var displayedType = itemType.TryGetValue(out var valueDisplayedTypeToken, GameJsonKeys.CardDisplayedType, objectId)
                ? valueDisplayedTypeToken.GetValue<string>()
                : string.Empty;
            var typeFontSize = itemType.TryGetValue(out var valueFontSizeToken, GameJsonKeys.CardTypeFontSize, objectId)
                ? valueFontSizeToken.GetValue<int>()
                : 5f;
            _layout.UpdateType(displayedType, typeFontSize);

            if (itemType.TryGetValue(out var valueCardNameToken, GameJsonKeys.CardName, objectId))
            {
                var nameFontSize =
                    itemType.TryGetValue(out var valueNameFontSizeAttributeToken, GameJsonKeys.CardNameFontSize,
                        objectId)
                        ? valueNameFontSizeAttributeToken.GetValue<int>()
                        : 6f;
                _layout.UpdateName(valueCardNameToken.GetValue<string>(), nameFontSize);
            }
            
            if (itemType.TryGetValue(out var valueCardDescriptionToken, GameJsonKeys.CardDescription, objectId))
            {
                var descriptionFontSize =
                    itemType.TryGetValue(out var valueDescriptionFontSizeAttributeToken,
                        GameJsonKeys.CardDescriptionFontSize, objectId)
                        ? valueDescriptionFontSizeAttributeToken.GetValue<int>()
                        : 10f;
                _layout.UpdateDescription(valueCardDescriptionToken.GetValue<string>(), descriptionFontSize);
            }

            if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.Picture, objectId) 
                && _game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
            {
                _layout.UpdateSprite(texture);
            }

            if (itemType.TryGetValue(out var valueTimerTextToken, GameJsonKeys.CardTimerText, objectId))
            {
                _layout.TimerLayout.UpdateTimerTextActive(true);
                _layout.TimerLayout.UpdateTimerText(valueTimerTextToken.GetValue<string>());
            }
            else
            {
                _layout.TimerLayout.UpdateTimerTextActive(false);
            }

            _layout.EntityId = entityId;
        }
        
        EclipseCardTokenLayout GetToken(int slot)
        {
            return _layout.TokensLayout.GetTokenByIndex(slot - 1);
        }
        
        EclipseCardTokenLayout IEclipseCardInContainerWidget.AttachToken(int slot, int objectId, IItemType itemType)
        {
            var tokenLayout = GetToken(slot);
            if (tokenLayout != null)
            {
                if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.Picture, objectId)
                    && _game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
                {
                    tokenLayout.UpdateSprite(texture);
                }

                if (itemType.TryGetValue(out var valueTooltipIdToken, GameJsonKeys.CardTooltipId, objectId))
                {
                    // tooltips turned off on small eclipse card 
                    // tokenLayout.UpdateTooltip(valueTooltipIdToken.GetValue<int>());
                }
            }
            else
            {
                Debug.LogWarning("Can't set token for slot on the eclipse card!");
            }

            return tokenLayout;
        }

        Vector3 IEclipseCardInContainerWidget.GetTokenPosition(int slot)
        {
            var tokenLayout = GetToken(slot);
            if (tokenLayout != null)
            {
                return tokenLayout.transform.position;
            }

            return _layout.transform.position;
        }

        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        void IEclipseCardInContainerWidget.SetOrder(int order)
        {
            _order = order;
        }
        
        void IEclipseCardInContainerWidget.UpdateSiblingIndex(int siblingIndex)
        {
            _layout.UpdateSiblingIndex(siblingIndex);
        }

        void IPoolingWidget.Cleanup()
        {
            Cleanup();
            _layout.Cleanup();
        }

        void IPoolingWidget.Destroy()
        {
            Cleanup();
            _layout.Cleanup();
            Object.Destroy(_layout.gameObject);
            _layout = null;

            _game = null;
        }
        
        private void Cleanup()
        {
        }

        #region Drag drop support

        private RectTransform _dragDropCacheParent;
        private int _dragDropCacheSiblingIndex;
        private Vector2 _dragAnchorMin;
        private Vector2 _dragAnchorMax;

        int IDraggableWidget.ObjectId => _objectId;

        void IDraggableWidget.OnDrag(RectTransform parent, Vector3 position)
        {
            if (_layout.ParentPlaceWidget is IApplyDragWidget dragWidget)
            {
                dragWidget.OnDragWidget(this);
            }
            
            _dragDropCacheParent = (RectTransform)_layout.RectTransform.parent;
            _dragDropCacheSiblingIndex = _layout.RectTransform.GetSiblingIndex();
            
            _layout.RaycastOff();
            var size = _layout.RectTransform.rect.size;
            _layout.UpdateParent(parent, true);
            _dragAnchorMin = _layout.RectTransform.anchorMin;
            _dragAnchorMax = _layout.RectTransform.anchorMax;
            _layout.RectTransform.anchorMin = Vector2.zero;
            _layout.RectTransform.anchorMax = Vector2.zero;
            _layout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(position);
            _layout.RectTransform.sizeDelta = size;
        }

        void IDraggableWidget.OnMove(Vector3 position)
        {
            _layout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(position);
        }

        void IDraggableWidget.OnDrop(Vector3 position, IApplyDropWidget target)
        {
            _layout.RectTransform.anchorMin = _dragAnchorMin;
            _layout.RectTransform.anchorMax = _dragAnchorMax;
            _layout.RaycastOn();
            
            if (target == null)
            {
                if (_layout.ParentPlaceWidget is IApplyDropWidget dropWidget)
                {
                    dropWidget.OnDropWidget(this, position, true);
                }
                
                _layout.UpdateParent(_dragDropCacheParent);
                _layout.UpdateSiblingIndex(_dragDropCacheSiblingIndex);
                return;
            }
            
            target.OnDropWidget(this, position, false);
        }
        
        void IPoolingWidget.BackToPool()
        {
            _game.EclipseCardInContainerWidgetPool.Push(this);
        }

        #endregion

        #region Ecs support

        int IEntityId.AttachEntityId => _layout.AttachEntityId ?? -1;

        void IEntityId.UpdateAttachEntityId(int entityId)
        {
            _layout.AttachEntityId = entityId;
        }

        #endregion
    }
}