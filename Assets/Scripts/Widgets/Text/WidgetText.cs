using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Text
{
    public class WidgetText : Widget
    {
        private readonly WidgetTextViewData _viewData;
        private readonly GameObject _gameObject;
        
        private WidgetTextView _textView;
        public override WidgetViewBase View => _textView;

        public WidgetText(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetTextViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            ServiceResource.TryGetWidgetPrefabForKey("ui/text", out _gameObject);
        }

        public override WidgetViewBase CreateView()
        {
            if (_textView == null)
            {
                _textView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetTextView>(_gameObject);
                _textView.Description.text = _viewData.Description;
                _textView.Init();
            }

            return _textView;
        }

        public override void ClearView()
        {
            if (_textView != null)
            {
                _textView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_textView);
                _textView = null;
            }
        }    
    }
}
