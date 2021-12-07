using Solcery.Widgets.Deck;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets.Stack
{
    public class WidgetStackView : WidgetViewBase
    {
        public RectTransform Content;
        public TextMeshProUGUI Number;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }
    }
}