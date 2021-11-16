using Solcery.Widgets.Deck;
using UnityEngine;

namespace Solcery.Widgets.Stack
{
    public class WidgetStackView : WidgetViewBase
    {
        public RectTransform Content;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            gameObject.SetActive(viewData.Visible);
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }
    }
}