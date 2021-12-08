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
            button.onClick.AddListener(onClick);
        }
    }
}