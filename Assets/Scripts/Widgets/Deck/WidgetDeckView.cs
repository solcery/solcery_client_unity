using UnityEngine;

namespace Solcery.Widgets.Deck
{
    public class WidgetDeckView : WidgetViewBase
    {
        public RectTransform Content;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }
    }
}
