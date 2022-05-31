using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects.Eclipse;
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
        private EclipseCardTypes _eclipseCardType;
        private int _entityId;
        private int _objectId;
        private int _order;

        int IEclipseCardInContainerWidget.Order => _order;
        int IEclipseCardInContainerWidget.EntityId => _entityId;
        
        public static IEclipseCardInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new EclipseCardInContainerWidget(game, prefab, poolTransform);
        }
        
        private EclipseCardInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<EclipseCardInContainerWidgetLayout>();
        }

        void IEclipseCardInContainerWidget.UpdateFromCardTypeData(int entityId, int objectId, JObject data)
        {
            _entityId = entityId;
            _objectId = objectId;
            
            if (data.TryGetValue("name", out string name))
            {
                _layout.UpdateName(name);
            }
            
            if (data.TryGetValue("description", out string description))
            {
                _layout.UpdateDescription(description);
            }

            if (data.TryGetValue("picture", out string picture) 
                && _game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                _layout.UpdateSprite(texture);
            }

            if (data.TryGetValue("timer_text", out string timerText))
            {
                _layout.TimerLayout.UpdateTimerTextActive(true);
                _layout.TimerLayout.UpdateTimerText(timerText);
            }
            else
            {
                _layout.TimerLayout.UpdateTimerTextActive(false);
            }

            
        }
        
        EclipseCardTokenLayout GetToken(int slot)
        {
            return _layout.TokensLayout.GetTokenByIndex(slot - 1);
        }
        
        EclipseCardTokenLayout IEclipseCardInContainerWidget.AttachToken(int slot, JObject data)
        {
            var tokenLayout = GetToken(slot);
            if (tokenLayout != null)
            {
                if (data.TryGetValue("picture", out string picture)
                    && _game.ServiceResource.TryGetTextureForKey(picture, out var texture))
                {
                    tokenLayout.UpdateSprite(texture);
                }

                if (data.TryGetValue("tooltip", out int tooltipId))
                {
                    tokenLayout.UpdateTooltip(tooltipId);
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
        
        void IEclipseCardInContainerWidget.SetEclipseCardType(EclipseCardTypes eclipseCardType)
        {
            _eclipseCardType = eclipseCardType;
            _layout.UpdateType(eclipseCardType.ToString());
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
            _layout.UpdateParent(parent, true);
            _dragAnchorMin = _layout.RectTransform.anchorMin;
            _dragAnchorMax = _layout.RectTransform.anchorMax;
            _layout.RectTransform.anchorMin = Vector2.zero;
            _layout.RectTransform.anchorMax = Vector2.zero;
            _layout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(position);
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
                    dropWidget.OnDropWidget(this, position);
                }
                
                _layout.UpdateParent(_dragDropCacheParent);
                _layout.UpdateSiblingIndex(_dragDropCacheSiblingIndex);
                return;
            }
            
            target.OnDropWidget(this, position);
        }

        #endregion

        #region Ecs support

        int IEntityId.AttachEntityId => _layout.AttachEntityId;

        void IEntityId.UpdateAttachEntityId(int entityId)
        {
            _layout.AttachEntityId = entityId;
        }

        #endregion
    }
}