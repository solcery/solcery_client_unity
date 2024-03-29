using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Simple.Titles
{
    public sealed class PlaceWidgetTitleLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private TMP_Text titleText;

        public void UpdateTitle(string title)
        {
            titleText.text = title ?? "";
        }
    }
}