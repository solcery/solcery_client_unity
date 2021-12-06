using Solcery.Models.Play.Attributes.Numberable;
using TMPro;
using UnityEngine.UI;

namespace Solcery.Widgets.Coin
{
    public class WidgetCoinView : WidgetViewBase, INumberable
    {
        public TextMeshProUGUI Number;
        public Image Image;
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
        }

        public void SetNumber(int number)
        {
            Number.text = number.ToString();
        }

        public override void Clear()
        {
            base.Clear();
            Destroy(Image.sprite);
        }
    }
}