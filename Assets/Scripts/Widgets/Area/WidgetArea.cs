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

        public override WidgetViewBase View { get; } = null;
        
        public WidgetArea(IWidgetCanvas widgetCanvas, WidgetPlaceViewData viewData)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
        }
        
        protected override void AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var card = WidgetButton.Create(data, _widgetCanvas);
            if (card != null)
            {
                card.View.SetParent(_widgetCanvas.GetUiCanvas());
                card.View.ApplyAnchor(_viewData.AnchorMin, _viewData.AnchorMax);
                if (card.View is IIntractable click)
                {
                    click.OnClick = () => { OnClick(world, entityId); };
                }
                card.ApplyAttributes(world, entityId);
            }
        }
    }
}