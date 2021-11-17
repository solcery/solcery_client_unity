using Solcery.Widgets.Deck;
using TMPro;

namespace Solcery.Widgets.Text
{
    public class WidgetTextView : WidgetViewBase
    {
        public TextMeshProUGUI Description;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            gameObject.SetActive(viewData.Visible);
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }
    }
}
 