using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Buttons
{
    public sealed class PlaceWidgetButtonLayout : PlaceWidgetSimpleLayout, IWidgetLayoutHighlight
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Image[] highlight;
        
        private CustomImages _images;

        public Image[] Highlights => highlight;
        
        private void Awake()
        {
            _images = gameObject.AddComponent<CustomImages>();
        }

        public void UpdateButtonText(string text, float fontSize)
        {
            buttonText.text = text;
            buttonText.UpdateFontSize(fontSize);
        }

        public void ClearAllOnClickListener()
        {
            button.onClick.RemoveAllListeners();
        }

        public void AddOnClickListener(UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }

        public override void UpdateAlpha(int alpha)
        {
            if (canvasGroup == null)
                return;

            var isAlphaZero = alpha == 0;
            canvasGroup.interactable = !isAlphaZero;
            canvasGroup.blocksRaycasts = !isAlphaZero;

            var a = alpha / 100f;
            canvasGroup.alpha = a;
        }

        public override void UpdateAvailable(bool available)
        {
            canvasGroup.alpha = available ? 1f : 0.4f;
            _images.UpdateAvailable(available);
            base.UpdateAvailable(available);
        }
    }
}