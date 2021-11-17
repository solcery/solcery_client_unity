using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Creator;
using Solcery.Widgets.Deck;
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
            _creator.Register(WidgetCreatorButton.Create(widgetCanvas, serviceResource));
            _creator.Register(WidgetCreatorPicture.Create(widgetCanvas, serviceResource));
            _creator.Register(WidgetCreatorText.Create(widgetCanvas, serviceResource));
        }

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var widget = _creator.CreateWidget(data);
            if (widget != null)
            {
                widget.View.SetParent(WidgetCanvas.GetUiCanvas());
                widget.View.ApplyPlaceViewData(_viewData);
            }

            return null;
        }

        protected override void ClearInternalView()
        {
        }
    }
}