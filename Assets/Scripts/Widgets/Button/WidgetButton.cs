using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Button
{
    public class WidgetButton : Widget
    {
        private readonly WidgetButtonViewData _viewData;
        private readonly GameObject _gameObject;
        
        private WidgetViewButton _buttonView;
        public override WidgetViewBase View => _buttonView;
        
        public WidgetButton(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetButtonViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            ServiceResource.TryGetWidgetPrefabForKey("ui/button", out _gameObject);
        }
        
        public override WidgetViewBase CreateView()
        {
            if (_buttonView == null)
            {
                _buttonView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetViewButton>(_gameObject);
                _buttonView.Text.text = _viewData.Name;
                _buttonView.Init();
            }
            return _buttonView;
        }

        public override void ClearView()
        {
            if (_buttonView != null)
            {
                _buttonView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_buttonView);
                _buttonView = null;
            }
        }
    }
}