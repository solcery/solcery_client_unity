using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Card
{
    public class WidgetCard : Widget
    {
        private readonly WidgetCardViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetCardView _cardView;
        public override WidgetViewBase View => _cardView;
        
        public WidgetCard(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetCardViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/card");
            CreateView();
        }

        private void CreateView()
        {
            _cardView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetCardView>(_gameObject);
            _cardView.Name.text = _viewData.Name;
            _cardView.Description.text = _viewData.Description;
            if (ServiceResource.TryGetTextureForKey(_viewData.Picture, out var texture))
            {
                _cardView.Image.material.mainTexture = texture;
            }
            _cardView.Init();
        }

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            return null;
        }

        protected override void ClearView()
        {
            _cardView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_cardView);
            _cardView = null;
        }
    }
}
