using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Attributes.Interactable;
using Solcery.Services.Resources;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Deck;

namespace Solcery.Widgets.Area
{
    public class WidgetArea : Widget
    {
        private readonly WidgetPlaceViewData _viewData;

        public override WidgetViewBase View { get; } = null;
        
        public WidgetArea(IWidgetCanvas widgetCanvas, IServiceResource serviceResource,  WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var button = WidgetButton.Create(WidgetCanvas, ServiceResource, data);
            if (button != null)
            {
                button.View.SetParent(WidgetCanvas.GetUiCanvas());
                button.View.ApplyPlaceViewData(_viewData);
                return button;
            }

            return null;
        }

        protected override void ClearInternalView()
        {
        }
    }
}