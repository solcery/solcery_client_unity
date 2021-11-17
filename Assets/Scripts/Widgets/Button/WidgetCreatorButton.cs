using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Button
{
    public class WidgetCreatorButton : IWidgetCreator
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private readonly WidgetButtonViewData _viewData;
        
        public static WidgetCreatorButton Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new WidgetCreatorButton(widgetCanvas, serviceResource);
        }

        private WidgetCreatorButton(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _viewData = new WidgetButtonViewData();
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        public bool TryCreate(JObject jsonData, out Widget widget)
        {
            if (_viewData.TryParse(jsonData))
            {
                widget = new WidgetButton(_widgetCanvas, _serviceResource, _viewData);
                return true;
            }

            widget = null;
            return false;
        }
    }
}