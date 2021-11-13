using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Entities;
using Solcery.Models.Triggers;
using Solcery.Widgets.Attributes;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
using Solcery.Widgets.Data;
using UnityEngine;

namespace Solcery.Widgets.Deck
{
    public class WidgetDeck : Widget
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly WidgetPlaceViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetDeckView _deckView;
        public override WidgetViewBase View => _deckView;
        
        public WidgetDeck(IWidgetCanvas widgetCanvas, WidgetPlaceViewData viewData)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/deck");
            CreateView();
        }

        private void CreateView()
        {
            _deckView = _widgetCanvas.GetWidgetPool().GetFromPool<WidgetDeckView>(_gameObject, _widgetCanvas.GetUiCanvas());
            _deckView.ApplyAnchor(_viewData.AnchorMin, _viewData.AnchorMax);
        }

        protected override void AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var card = WidgetCard.Create(data, _widgetCanvas);
            if (card != null)
            {
                card.View.SetParent(_deckView.Content);
                if (card.View is IIntractable click)
                {
                    click.OnClick = () => { OnClick(world, entityId); };
                }
                card.ApplyAttributes(world, entityId);
            }
        }
    }
}
