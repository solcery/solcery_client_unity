using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Widgets
{
    public sealed class PlaceWidgetWidgetLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TMP_Text text;

        public void UpdateImage(Texture2D texture)
        {
            image.material.mainTexture = texture;
        }

        public void UpdateText(string newText)
        {
            text.text = newText;
        }
    }
}