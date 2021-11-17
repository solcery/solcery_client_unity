using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Text
{
    public class WidgetCreatorText : IWidgetCreator
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private readonly WidgetTextViewData _viewData;
        
        public static WidgetCreatorText Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new WidgetCreatorText(widgetCanvas, serviceResource);
        }

        private WidgetCreatorText(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _viewData = new WidgetTextViewData();
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        public bool TryCreate(JObject jsonData, out Widget widget)
        {
            if (_viewData.TryParse(jsonData))
            {
                widget = new WidgetText(_widgetCanvas, _serviceResource, _viewData);
                return true;
            }

            widget = null;
            return false;
        }          
    }
}