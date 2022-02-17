using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.EcsSupport;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class TokenInContainerWidget : ITokenInContainerWidget
    {
        private IGame _game;
        private TokenInContainerWidgetLayout _layout;
        private int _counter;
        private int _attachEntityId;

        public static ITokenInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new TokenInContainerWidget(game, prefab, poolTransform);
        }
        
        private TokenInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _counter = 0;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<TokenInContainerWidgetLayout>();
        }
        
        public void IncreaseCounter()
        {
            _layout.UpdateCount(++_counter);
        }

        public void ClearCounter()
        {
            _counter = 0;
        }

        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        public void UpdateFromCardTypeData(int objectId, JObject data)
        {
            if (data.TryGetValue("picture", out string picture)
                && _game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                _layout.UpdateSprite(texture);
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
        }

        #region Drag drop support
        
        private RectTransform _dragDropCacheParent;
        
        void IDraggableWidget.OnDrag(RectTransform parent, Vector3 position)
        {
            _dragDropCacheParent = (RectTransform)_layout.RectTransform.parent;
            
            _layout.RaycastOff();
            _layout.UpdateParent(parent);
            _layout.RectTransform.position = position;
        }

        void IDraggableWidget.OnMove(Vector3 position)
        {
            _layout.RectTransform.position = position;
        }

        void IDraggableWidget.OnDrop(Vector3 position, IApplyDropWidget target)
        {
            _layout.RaycastOn();
            
            if (target == null)
            {
                _layout.UpdateParent(_dragDropCacheParent);
                return;
            }
            
            target.OnDropWidget(this, position);
        }
        
        #endregion

        #region Ecs support
        
        int IEntityId.AttachEntityId => _layout.EntityId;

        void IEntityId.UpdateAttachEntityId(int entityId)
        {
            _layout.EntityId = entityId;
        }
        
        #endregion
    }
}