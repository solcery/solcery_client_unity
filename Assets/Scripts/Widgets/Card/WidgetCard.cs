using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Card
{
    public class WidgetCard : Widget
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly WidgetCartViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetCardView _cardView;
        public override WidgetViewBase View => _cardView;

        public static WidgetCard Create(JObject jsonData, IWidgetCanvas widgetCanvas)
        {
            var viewData = new WidgetCartViewData();
            if (viewData.TryParse(jsonData))
            {
                return new WidgetCard(viewData, widgetCanvas);
            }

            return null;
        }

        private WidgetCard(WidgetCartViewData viewData, IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/card");
            CreateView();
        }

        private void CreateView()
        {
            _cardView = _widgetCanvas.GetWidgetPool().GetFromPool<WidgetCardView>(_gameObject);
            _cardView.Name.text = _viewData.Name;
            _cardView.Description.text = _viewData.Description;
            _cardView.Init();
        }

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            return null;
        }

        protected override void ClearInternalView()
        {
            _cardView.Clear();
            _widgetCanvas.GetWidgetPool().ReturnToPool(_cardView);
            _cardView = null;
        }
    }
}
