using Solcery.Services.Resources;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Picture;
using Solcery.Widgets.Text;

namespace Solcery.Widgets.Area
{
    public class WidgetAreaButtons : WidgetArea
    {
        public WidgetAreaButtons(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource, viewData)
        {
            Creator.Register(new WidgetCreatorButton(widgetCanvas, serviceResource));
            Creator.Register(new WidgetCreatorPicture(widgetCanvas, serviceResource));
            Creator.Register(new WidgetCreatorText(widgetCanvas, serviceResource));
        }
    }
}