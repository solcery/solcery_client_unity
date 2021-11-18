using Leopotam.EcsLite;
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
            CreateView();
        }

        private void CreateView()
        {
            _deckView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetDeckView>(_gameObject, WidgetCanvas.GetUiCanvas());
            _deckView.ApplyPlaceViewData(_viewData);
        }

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var widget = _creator.CreateWidget(data);
            if (widget != null)
            {
                widget.View.SetParent(_deckView.Content);
                widget.View.ApplyPlaceViewData(_viewData);
                return widget;
            }
            
            return null;
        }

        protected override void ClearView()
        {
            _deckView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_deckView);
            _deckView = null;
        }
    }
}
