using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Button
{
    public class WidgetCreatorButton : WidgetCreatorBase<WidgetButtonViewData>
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        
        public WidgetCreatorButton(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }
        
        protected override Widget Create()
        {
            return new WidgetButton(_widgetCanvas, _serviceResource, _viewData);;
        }
    }
}