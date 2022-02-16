using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class TokenInContainerWidgetLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI count;
        [SerializeField]
        private List<Graphic> raycastObjects;
        
        private Sprite _sprite;
        
        [HideInInspector]
        public int EntityId;
        
        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new();
        
        public RectTransform RectTransform => rectTransform;

        public void UpdateCount(int value)
        {
            count.text = value.ToString();
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = _sprite;
        }
        
        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
        }
        
        public void RaycastOn()
        {
            foreach (var targetSetting in _raycastTargetSettings)
            {
                targetSetting.Key.raycastTarget = targetSetting.Value;
            }
        }

        public void RaycastOff()
        {
            _raycastTargetSettings.Clear();
            foreach (var raycastObject in raycastObjects)
            {
                _raycastTargetSettings.Add(raycastObject, raycastObject.raycastTarget);
                raycastObject.raycastTarget = false;
            }
        }

        public void Cleanup()
        {
        }
        
        private void OnDestroy()
        {
            DestroySprite();
        }

        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }
    }
}
