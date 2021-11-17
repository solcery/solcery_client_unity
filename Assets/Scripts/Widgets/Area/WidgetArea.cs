using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Attributes.Interactable;
using Solcery.Services.Resources;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Deck;
using Solcery.Widgets.Picture;
using Solcery.Widgets.Text;

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

        // todo refactoring it
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var button = WidgetButton.Create(WidgetCanvas, ServiceResource, data);
            if (button != null)
            {
                button.View.SetParent(WidgetCanvas.GetUiCanvas());
                button.View.ApplyPlaceViewData(_viewData);
                return button;
            }
            var picture = WidgetPicture.Create(WidgetCanvas, ServiceResource, data);
            if (picture != null)
            {
                picture.View.SetParent(WidgetCanvas.GetUiCanvas());
                picture.View.ApplyPlaceViewData(_viewData);
                return picture;
            }
            var text = WidgetText.Create(WidgetCanvas, ServiceResource, data);
            if (text != null)
            {
                text.View.SetParent(WidgetCanvas.GetUiCanvas());
                text.View.ApplyPlaceViewData(_viewData);
                return text;
            }

            return null;
        }

        protected override void ClearInternalView()
        {
        }
    }
}