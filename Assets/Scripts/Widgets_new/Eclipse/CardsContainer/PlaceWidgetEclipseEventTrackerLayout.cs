using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.CardsContainer
{
    public class PlaceWidgetEclipseEventTrackerLayout : PlaceWidgetEclipseLayoutBase
    {
        [SerializeField]
        protected ScrollRect scroll;
        [SerializeField]
        protected HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField]
        protected VerticalLayoutGroup verticalLayoutGroup;

        private EventTrackerLayout _eventTrackerLayout;


        public override void SetLayout(EventTrackerLayout layout, TextAnchor anchor)
        {
            _eventTrackerLayout = layout;
            horizontalLayoutGroup.gameObject.SetActive(_eventTrackerLayout == EventTrackerLayout.Horizontal);
            verticalLayoutGroup.gameObject.SetActive(_eventTrackerLayout == EventTrackerLayout.Vertical);
            scroll.horizontal = _eventTrackerLayout == EventTrackerLayout.Horizontal;
            scroll.vertical = _eventTrackerLayout == EventTrackerLayout.Vertical;
            switch (layout)
            {
                case EventTrackerLayout.Horizontal:
                    scroll.content = horizontalLayoutGroup.GetComponent<RectTransform>();
                    break;
                case EventTrackerLayout.Vertical:
                    scroll.content = verticalLayoutGroup.GetComponent<RectTransform>();
                    break;
            }

            SetAnchor(anchor);
        }

        private void SetAnchor(TextAnchor anchor)
        {
            horizontalLayoutGroup.childAlignment = anchor;
            switch (anchor)
            {
                case TextAnchor.MiddleLeft:
                    scroll.content.pivot = new Vector2(0f, 1f);
                    break;
                case TextAnchor.MiddleRight:
                    scroll.content.pivot = new Vector2(1f, 0f);
                    break;
                case TextAnchor.MiddleCenter:
                    scroll.content.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }

        public override void AddCard(IEclipseCardInContainerWidget eclipseCardInContainerWidget)
        {
            eclipseCardInContainerWidget.UpdateParent(scroll.content);
            eclipseCardInContainerWidget.UpdateSiblingIndex(eclipseCardInContainerWidget.Order);
            switch (_eventTrackerLayout)
            {
                case EventTrackerLayout.Horizontal:
                    scroll.horizontal = scroll.content.childCount > 1;
                    scroll.vertical = false;
                    eclipseCardInContainerWidget.Layout.AspectRatioFitter.aspectMode =
                        AspectRatioFitter.AspectMode.HeightControlsWidth;  
                    break;
                case EventTrackerLayout.Vertical:
                    scroll.horizontal = false;
                    scroll.vertical = scroll.content.childCount > 1;
                    eclipseCardInContainerWidget.Layout.AspectRatioFitter.aspectMode =
                        AspectRatioFitter.AspectMode.WidthControlsHeight;  
                    break;
            }
            Rebuild();
        }

        public override void Rebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        }
    }
}