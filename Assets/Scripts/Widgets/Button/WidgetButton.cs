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
        
        public static WidgetButton Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, JObject jsonData)
        {
            var viewData = new WidgetButtonViewData();
            if (viewData.TryParse(jsonData))
            {
                return new WidgetButton(widgetCanvas, serviceResource, viewData);
            }
            
            return null;
        }
        
        private WidgetButton(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetButtonViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/button");
            CreateView();
        }
        
        private void CreateView()
        {
            _buttonView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetViewButton>(_gameObject);
            _buttonView.Text.text = _viewData.Name;
            _buttonView.Init();
        }

        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            return null;
        }

        protected override void ClearInternalView()
        {
            _buttonView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_buttonView);
            _buttonView = null;
        }
    }
}