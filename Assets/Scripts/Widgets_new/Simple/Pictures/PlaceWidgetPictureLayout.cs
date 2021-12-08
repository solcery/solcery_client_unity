using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Simple.Pictures
{
    public sealed class PlaceWidgetPictureLayout : PlaceWidgetSimpleLayout
    {
        [SerializeField]
        private Image picture;

        public void UpdatePicture(Texture2D texture)
        {
            picture.material.mainTexture = texture;
        }
    }
}