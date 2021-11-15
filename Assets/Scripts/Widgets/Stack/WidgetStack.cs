using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
using Solcery.Widgets.Deck;
using UnityEngine;

namespace Solcery.Widgets.Stack
{
    public class WidgetStack : Widget
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly WidgetPlaceViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetStackView _stackView;
        public override WidgetViewBase View => _stackView;
        
        public WidgetStack(IWidgetCanvas widgetCanvas, WidgetPlaceViewData viewData)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/stack");
            CreateView();
        }
        
        private void CreateView()
        {
            _stackView = _widgetCanvas.GetWidgetPool().GetFromPool<WidgetStackView>(_gameObject, _widgetCanvas.GetUiCanvas());
            _stackView.ApplyAnchor(_viewData.AnchorMin, _viewData.AnchorMax);
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var card = WidgetCard.Create(data, _widgetCanvas);
            if (card != null)
            {
                card.View.SetParent(_stackView.Content);
                return card;
            }
            
            return null;
        }

        protected override void ClearInternalView()
        {
            _stackView.Clear();
            _widgetCanvas.GetWidgetPool().ReturnToPool(_stackView);
            _stackView = null;
        }
    }
}