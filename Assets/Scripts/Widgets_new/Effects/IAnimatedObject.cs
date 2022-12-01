using UnityEngine;

namespace Solcery.Widgets_new.Effects
{
    public interface IAnimatedObject
    {
        public RectTransform GetAnimatedRect(PlaceWidgetCardFace face);
        public void SetActive(bool active);
    }
}