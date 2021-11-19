using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Card;
using Solcery.Widgets.Creator;
using UnityEngine;

namespace Solcery.Widgets.Stack
{
    public class WidgetStack : Widget
    {
        private readonly WidgetPlaceViewData _viewData;
        private readonly WidgetsCreator _creator;
        private readonly GameObject _gameObject;

        private WidgetStackView _stackView;
        public override WidgetViewBase View => _stackView;
        
        public WidgetStack(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPlaceViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _creator = WidgetsCreator.Create();
            _creator.Register(new WidgetCreatorCard(widgetCanvas, serviceResource));
            _gameObject = (GameObject) Resources.Load("ui/stack");
        }
        
        protected override Widget AddSubWidget(JObject data)
        {
            var widget = _creator.CreateWidget(data);
            if (widget != null)
            {
                var view = widget.CreateView();
                view.SetParent(_stackView.Content);
                view.ApplyPlaceViewData(_viewData);
                return widget;
            }
            
            return null;
        }
        
        public override WidgetViewBase CreateView()
        {
            if (_stackView == null)
            {
                _stackView = WidgetCanvas.GetWidgetPool()
                    .GetFromPool<WidgetStackView>(_gameObject, WidgetCanvas.GetUiCanvas());
                _stackView.ApplyPlaceViewData(_viewData);
            }
            return _stackView;
        }

        public override void ClearView()
        {
            if (_stackView != null)
            {
                _stackView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_stackView);
                _stackView = null;
            }
        }
    }
}