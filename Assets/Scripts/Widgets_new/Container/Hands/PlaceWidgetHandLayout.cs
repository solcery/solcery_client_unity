using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Hands
{
    public class PlaceWidgetHandLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private RectTransform fakeContent;
        [SerializeField]
        private List<GameObject> fakeCardList;

        public RectTransform Content => content;
        public RectTransform Transform => fakeContent;
        public IReadOnlyList<GameObject> FakeCardList => fakeCardList;
    }
}