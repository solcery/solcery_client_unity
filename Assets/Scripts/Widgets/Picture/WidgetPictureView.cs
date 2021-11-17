using Solcery.Widgets.Deck;
using UnityEngine.UI;

namespace Solcery.Widgets.Picture
{
    public class WidgetPictureView : WidgetViewBase
    {
        public Image Image;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }
    }
}
