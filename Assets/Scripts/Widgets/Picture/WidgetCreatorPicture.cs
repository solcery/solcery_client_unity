using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Picture
{
    public class WidgetCreatorPicture : IWidgetCreator
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private readonly WidgetPictureViewData _viewData;
        
        public static WidgetCreatorPicture Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new WidgetCreatorPicture(widgetCanvas, serviceResource);
        }

        private WidgetCreatorPicture(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _viewData = new WidgetPictureViewData();
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        public bool TryCreate(JObject jsonData, out Widget widget)
        {
            if (_viewData.TryParse(jsonData))
            {
                widget = new WidgetPicture(_widgetCanvas, _serviceResource, _viewData);
                return true;
            }

            widget = null;
            return false;
        }        
    }
}