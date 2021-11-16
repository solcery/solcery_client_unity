using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
using Solcery.Widgets.Deck;
using UnityEngine;

namespace Solcery.Widgets.Stack
{
    public class WidgetStack : Widget
    {
        private readonly WidgetPlaceViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetStackView _stackView;
        public override WidgetViewBase View => _stackView;
        
        public WidgetStack(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/stack");
            CreateView();
        }
        
        private void CreateView()
        {
            _stackView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetStackView>(_gameObject, WidgetCanvas.GetUiCanvas());
            _stackView.ApplyPlaceViewData(_viewData);
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            var card = WidgetCard.Create(WidgetCanvas, ServiceResource, data);
            if (card != null)
            {
                card.View.SetParent(_stackView.Content);
                card.View.ApplyPlaceViewData(_viewData);
                return card;
            }
            
            return null;
        }

        protected override void ClearInternalView()
        {
            _stackView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_stackView);
            _stackView = null;
        }
    }
}