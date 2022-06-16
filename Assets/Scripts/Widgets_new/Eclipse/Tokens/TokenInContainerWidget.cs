using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class TokenInContainerWidget : ITokenInContainerWidget
    {
        private IGame _game;
        private TokenInContainerWidgetLayout _layout;
        private int _counter;
        private int _objectId;
        private int _typeId;
        private bool _active;
        
        public TokenInContainerWidgetLayout Layout => _layout;
        public int TypeId => _typeId;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                _layout.gameObject.SetActive(_active);
            }
        }

        public static ITokenInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new TokenInContainerWidget(game, prefab, poolTransform);
        }
        
        private TokenInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<TokenInContainerWidgetLayout>();
        }

        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        public void UpdateFromCardTypeData(int objectId, int typeId, JObject data)
        {
            _objectId = objectId;
            _typeId = typeId;
            
            if (data.TryGetValue("picture", out string picture)
                && _game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                _layout.UpdateSprite(texture);
            }
            
            if (data.TryGetValue(GameJsonKeys.CardTooltipId, out int tooltipId))
            {
                InitTooltip(tooltipId);
            }
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
            CleanupTooltip();
        }

        #region tooltip support

        private TooltipBehaviour _tooltipBehaviour;
        
        private void InitTooltip(int tooltipId)
        {
            if (_tooltipBehaviour == null)
            {
                _tooltipBehaviour = _layout.gameObject.AddComponent<RectTransformTooltipBehaviour>();
            }
            _tooltipBehaviour.SetTooltipId(tooltipId);
        }

        private void CleanupTooltip()
        {
            if (_tooltipBehaviour != null)
            {
                Object.Destroy(_tooltipBehaviour);
            }

            _tooltipBehaviour = null;
        }

        #endregion
        
        #region Drag drop support
        
        private Vector2 _dragAnchorMin;
        private Vector2 _dragAnchorMax;
        private Vector3 _size;
        
        int IDraggableWidget.ObjectId => _objectId;
        
        void IDraggableWidget.OnDrag(RectTransform parent, Vector3 position)
        {
            if (_layout.ParentPlaceWidget is IApplyDragWidget dragWidget)
            {
                dragWidget.OnDragWidget(this);
            }
            
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
            _layout.RectTransform.sizeDelta = _size;
            _layout.RaycastOn();

            if (target != null)
            {
                Active = false;
            }
            
            if (_layout.ParentPlaceWidget is IApplyDropWidget dropWidget)
            {
                dropWidget.OnDropWidget(this, position);
            }
        }

        #endregion

        #region Ecs support
        
        int IEntityId.AttachEntityId => _layout.entityId;

        void IEntityId.UpdateAttachEntityId(int entityId)
        {
            _layout.entityId = entityId;
        }
        
        #endregion
    }
}
