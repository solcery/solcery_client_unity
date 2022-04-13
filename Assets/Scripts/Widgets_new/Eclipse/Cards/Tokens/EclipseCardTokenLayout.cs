using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards.Tokens
{
    public sealed class EclipseCardTokenLayout : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private TooltipBehaviour _tooltipBehaviour;
        private RectTransform _rectTransform;
        
        public Image Icon => icon;
        public RectTransform RectTransform => _rectTransform;
        
        public void Awake()
        {
            _rectTransform = icon.GetComponent<RectTransform>();
        }

        public void UpdateSprite(Texture2D texture)
        {
            icon.gameObject.SetActive(true);
            icon.sprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
        }

        public void UpdateTooltip(int tooltipId)
        {
            if (_tooltipBehaviour == null)
            {
                _tooltipBehaviour = gameObject.AddComponent<RectTransformTooltipBehaviour>();
            }
            _tooltipBehaviour.SetTooltipId(tooltipId);
        }

        public void Cleanup()
        {
            if (icon.sprite != null)
            {
                Destroy(icon.sprite);
            }
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            
            if (_tooltipBehaviour != null)
            {
                Destroy(_tooltipBehaviour);
            }
            _tooltipBehaviour = null;
        }
    }
}