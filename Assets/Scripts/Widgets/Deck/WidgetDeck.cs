using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
using Solcery.Widgets.Creator;
using UnityEngine;

namespace Solcery.Widgets.Deck
{
    public class WidgetDeck : Widget
    {
        private readonly WidgetPlaceViewData _viewData;
        private readonly WidgetsCreator _creator;
        private readonly GameObject _gameObject;
    
        private WidgetDeckView _deckView;
        public override WidgetViewBase View => _deckView;
        
        public WidgetDeck(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _creator = WidgetsCreator.Create();
            _creator.Register(new WidgetCreatorCard(widgetCanvas, serviceResource));
            _gameObject = (GameObject) Resources.Load("ui/deck");
        }

        public override WidgetViewBase CreateView()
        {
            if (_deckView == null)
            {
                _deckView = WidgetCanvas.GetWidgetPool()
                    .GetFromPool<WidgetDeckView>(_gameObject, WidgetCanvas.GetUiCanvas());
                _deckView.ApplyPlaceViewData(_viewData);
            }

            return _deckView;
        }

        public override void ClearView()
        {
            if (_deckView != null)
            {
                _deckView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_deckView);
                _deckView = null;
            }
        }
        
        protected override Widget AddSubWidget(JObject data)
        {
            var widget = _creator.CreateWidget(data);
            if (widget != null)
            {
                var view = widget.CreateView();
                view.SetParent(_deckView.Content);
                view.ApplyPlaceViewData(_viewData);
                return widget;
            }
            
            return null;
        }
    }
}
