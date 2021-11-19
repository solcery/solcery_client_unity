using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;
using Solcery.Widgets.Picture;
using Solcery.Widgets.Text;

namespace Solcery.Widgets.Area
{
    public class WidgetArea : Widget
    {
        private readonly WidgetPlaceViewData _viewData;
        private readonly WidgetsCreator _creator;
        public override WidgetViewBase View { get; } = null;

        public WidgetArea(IWidgetCanvas widgetCanvas, IServiceResource serviceResource,  WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _creator = WidgetsCreator.Create();
            _creator.Register(new WidgetCreatorButton(widgetCanvas, serviceResource));
            _creator.Register(new WidgetCreatorPicture(widgetCanvas, serviceResource));
            _creator.Register(new WidgetCreatorText(widgetCanvas, serviceResource));
        }

        protected override Widget AddSubWidget(JObject data)
        {
            var widget = _creator.CreateWidget(data);
            if (widget != null)
            {
                var view = widget.CreateView();
                view.SetParent(WidgetCanvas.GetUiCanvas());
                view.ApplyPlaceViewData(_viewData);
            }

            return null;
        }
    }
}