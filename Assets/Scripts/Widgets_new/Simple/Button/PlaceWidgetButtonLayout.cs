using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Solcery.Widgets_new.Simple.Button
{
    public sealed class PlaceWidgetButtonLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private UnityEngine.UI.Button button;
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