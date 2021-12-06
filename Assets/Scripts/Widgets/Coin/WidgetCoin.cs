using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Coin
{
    public class WidgetCoin : Widget
    {
        private readonly WidgetCoinViewData _viewData;
        private readonly GameObject _gameObject;

        private WidgetCoinView _coinView;
        public override WidgetViewBase View => _coinView;
        
        public WidgetCoin(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetCoinViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/coin");
        }

        public override WidgetViewBase CreateView()
        {
            if (_coinView == null)
            {
                _coinView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetCoinView>(_gameObject);
                if (ServiceResource.TryGetTextureForKey(_viewData.Picture, out var texture))
                {
                    _coinView.Image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                _coinView.Init();
            }

            return _coinView;
        }

        public override void ClearView()
        {
            if (_coinView != null)
            {
                _coinView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_coinView);
                _coinView = null;
            }
        } 
    }
}
