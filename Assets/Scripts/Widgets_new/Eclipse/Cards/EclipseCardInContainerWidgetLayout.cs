using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.Cards.Traits;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public sealed class EclipseCardInContainerWidgetLayout : MonoBehaviour
    {
        public RectTransform RectTransform => rectTransform;
        
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private EclipseCardTimerLayout timerLayout;
        [SerializeField]
        private EclipseCardTraitsLayout traitsLayout;
        [SerializeField]
        private EclipseCardTokensLayout tokensLayout;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;
        
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        
        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
        }

        public void UpdateParent(Transform parent)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchoredPosition = _anchoredPosition;
            rectTransform.offsetMax = Vector2.down;
            rectTransform.pivot = _pivot;
            rectTransform.anchorMin = _anchorMin;
            rectTransform.anchorMax = _anchorMax;
            rectTransform.offsetMin = _offsetMin;
            rectTransform.offsetMax = _offsetMax;
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
        }
        
        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }
        
        public void Cleanup()
        {
        }

        public void UpdateName(string newName)
        {
            nameText.text = newName;
        }
    }
}