using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Simple.Titles
{
    public sealed class PlaceWidgetTextLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void UpdateTitle(string title)
        {
            text.text = title ?? "";
        }
    }
}