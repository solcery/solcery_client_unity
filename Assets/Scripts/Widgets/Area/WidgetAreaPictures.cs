using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Picture;

namespace Solcery.Widgets.Area
{
    public class WidgetAreaPictures : WidgetArea
    {
        public WidgetAreaPictures(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource, viewData)
        {
            Creator.Register(new WidgetCreatorPicture(widgetCanvas, serviceResource));
        }
    }
}