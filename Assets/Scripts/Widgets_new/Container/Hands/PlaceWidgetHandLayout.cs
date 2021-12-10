using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public sealed class PlaceWidgetHandLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private RectTransform content;
        
        public RectTransform Content => content;
    }
}