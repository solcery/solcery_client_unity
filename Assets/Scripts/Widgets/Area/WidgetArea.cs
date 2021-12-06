using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;

namespace Solcery.Widgets.Area
{
    public abstract class WidgetArea : Widget
    {
        private readonly WidgetPlaceViewData _viewData;
        protected readonly WidgetsCreator Creator;
        public override WidgetViewBase View { get; } = null;

        protected WidgetArea(IWidgetCanvas widgetCanvas, IServiceResource serviceResource,  WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            Creator = WidgetsCreator.Create();
        }

        protected override Widget AddSubWidget(JObject data)
        {
            var widget = Creator.CreateWidget(data);
            if (widget != null)
            {
                var view = widget.CreateView();
                view.SetParent(WidgetCanvas.GetUiCanvas());
                view.ApplyPlaceViewData(_viewData);
                return widget;
            }

            return null;
        }
    }
}