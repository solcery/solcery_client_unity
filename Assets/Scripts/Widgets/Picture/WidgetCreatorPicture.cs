using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Picture
{
    public class WidgetCreatorPicture : WidgetCreatorBase<WidgetPictureViewData>
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;

        public WidgetCreatorPicture(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
        }

        protected override Widget Create()
        {
            return new WidgetPicture(_widgetCanvas, _serviceResource, _viewData);;
        }
    }
}