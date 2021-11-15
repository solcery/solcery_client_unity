using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
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

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var card = WidgetCard.Create(data, _widgetCanvas);
            if (card != null)
            {
                card.View.SetParent(_deckView.Content);
                return card;
            }
            
            return null;
        }

        protected override void ClearInternalView()
        {
            _deckView.Clear();
            _widgetCanvas.GetWidgetPool().ReturnToPool(_deckView);
            _deckView = null;
        }
    }
}
