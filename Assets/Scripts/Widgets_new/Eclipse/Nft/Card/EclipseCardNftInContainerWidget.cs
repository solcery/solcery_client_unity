using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
using Solcery.Services.GameContent.Items;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Nft.Card
{
    public sealed class EclipseCardNftInContainerWidget : IEclipseCardNftInContainerWidget
    {
        EclipseCardNftInContainerWidgetLayout IEclipseCardNftInContainerWidget.Layout => _layout;
        int IEclipseCardNftInContainerWidget.Order => _order;
        int IEclipseCardNftInContainerWidget.EntityId => _entityId;
        int IEclipseCardNftInContainerWidget.CardType => _cardType;
        int IDraggableWidget.ObjectId => _objectId;
        int IEntityId.AttachEntityId => _layout.AttachEntityId ?? -1;

        private IGame _game;
        private EclipseCardNftInContainerWidgetLayout _layout;
        private EclipseCardTypes _eclipseCardType;
        private int _entityId;
        private int _cardType;
        private int _objectId;
        private int _order;
        // D&D
        private RectTransform _dragDropCacheParent;
        private int _dragDropCacheSiblingIndex;
        private Vector2 _dragAnchorMin;
        private Vector2 _dragAnchorMax;
        
        public static IEclipseCardNftInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new EclipseCardNftInContainerWidget(game, prefab, poolTransform);
        }
        
        private EclipseCardNftInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<EclipseCardNftInContainerWidgetLayout>();
        }

        void IEclipseCardNftInContainerWidget.UpdateFromCardTypeData(int entityId, int objectId, int objectType, EclipseCardTypes type, IItemType itemType)
        {
            _entityId = entityId;
            _objectId = objectId;
            _eclipseCardType = type;
            _cardType = objectType;
            
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
            
            _layout.EntityId = entityId;
        }

        void IEclipseCardNftInContainerWidget.UpdateSiblingIndex(int siblingIndex)
        {
            _layout.UpdateSiblingIndex(siblingIndex);
        }

        void IEclipseCardNftInContainerWidget.SetOrder(int order)
        {
            _order = order;
        }

        #region Pooling widget
        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
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

        void IPoolingWidget.BackToPool()
        {
            _game.EclipseCardNftInContainerWidgetPool.Push(this);
        }
        #endregion
        
        private void Cleanup() { }

        #region Drag drop support
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
        #endregion

        #region Ecs support
        void IEntityId.UpdateAttachEntityId(int entityId)
        {
            _layout.AttachEntityId = entityId;
        }
        #endregion
    }
}