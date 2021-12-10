using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Buttons
{
    public sealed class PlaceWidgetButtonLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private TMP_Text buttonText;

        public void UpdateButtonText(string text)
        {
            buttonText.text = text ?? "";
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
    }
}