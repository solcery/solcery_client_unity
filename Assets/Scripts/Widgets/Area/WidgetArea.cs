using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Attributes;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Deck;

namespace Solcery.Widgets.Area
{
    public class WidgetArea : Widget
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly WidgetPlaceViewData _viewData;
        private readonly List<Widget> _subWidgets;

        public override WidgetViewBase View { get; } = null;
        
        public WidgetArea(IWidgetCanvas widgetCanvas, WidgetPlaceViewData viewData)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
            _subWidgets = new List<Widget>();
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var button = WidgetButton.Create(data, _widgetCanvas);
            if (button != null)
            {
                button.View.SetParent(_widgetCanvas.GetUiCanvas());
                button.View.ApplyAnchor(_viewData.AnchorMin, _viewData.AnchorMax);
                if (button.View is IIntractable click)
                {
                    click.OnClick = () => { OnClick(world, entityId); };
                }
                button.ApplyAttributes(world, entityId);
                return button;
            }

            return null;
        }
    }
}