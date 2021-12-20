using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public class PlaceWidgetHandLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private float spacing;

        public RectTransform Content => content;
        public float Spacing => spacing;
    }
}