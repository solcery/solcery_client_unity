using Solcery.Widgets_new.Container.Hands;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStackLayout : PlaceWidgetHandLayout
    {
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