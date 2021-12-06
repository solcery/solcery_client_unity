using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Text;

namespace Solcery.Widgets.Area
{
    public class WidgetAreaTitles : WidgetArea
    {
        public WidgetAreaTitles(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource, viewData)
        {
            Creator.Register(new WidgetCreatorText(widgetCanvas, serviceResource));
        }
    }
}