using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
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

        public static WidgetText Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, JObject jsonData)
        {
            var viewData = new WidgetTextViewData();
            if (viewData.TryParse(jsonData))
            {
                return new WidgetText(widgetCanvas, serviceResource, viewData);
            }
            
            return null;
        }
        
        private WidgetText(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetTextViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/text");
            CreateView();
        }

        private void CreateView()
        {
            _textView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetTextView>(_gameObject);
            _textView.Description = _textView.Description;
            _textView.Init();
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            return null;
        }

        protected override void ClearInternalView()
        {
            _textView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_textView);
            _textView = null;
        }    
    }
}
