using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Coin;

namespace Solcery.Widgets.Area
{
    public class WidgetAreaCoins : WidgetArea
    {
        public WidgetAreaCoins(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource, viewData)
        {
            Creator.Register(new WidgetCreatorCoin(widgetCanvas, serviceResource));
        }
    }
}