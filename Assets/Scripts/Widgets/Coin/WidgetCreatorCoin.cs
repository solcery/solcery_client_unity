using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Coin
{
    public class WidgetCreatorCoin : WidgetCreatorBase<WidgetCoinViewData>
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        
        public WidgetCreatorCoin(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        protected override Widget Create()
        {
            return new WidgetCoin(_widgetCanvas, _serviceResource, _viewData);;
        }        
    }
}