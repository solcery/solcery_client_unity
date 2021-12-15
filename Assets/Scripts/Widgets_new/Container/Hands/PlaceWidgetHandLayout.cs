using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public class PlaceWidgetHandLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private RectTransform fakeContent;

        public RectTransform Content => content;
        public RectTransform Transform => fakeContent;
    }
}