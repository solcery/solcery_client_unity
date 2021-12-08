using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStackLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private TMP_Text text;

        public void UpdateText(string newText)
        {
            text.text = newText;
        }

        public void UpdateTextVisible(bool visible)
        {
            text.gameObject.SetActive(visible);
        }
    }
}