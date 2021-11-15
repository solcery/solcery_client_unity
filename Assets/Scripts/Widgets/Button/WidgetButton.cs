using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Button
{
    public class WidgetButton : Widget
    {
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly WidgetButtonViewData _viewData;
        private readonly GameObject _gameObject;
        
        private WidgetViewButton _buttonView;
        public override WidgetViewBase View => _buttonView;
        
        public static WidgetButton Create(JObject jsonData, IWidgetCanvas widgetCanvas)
        {
            var viewData = new WidgetButtonViewData();
            if (viewData.TryParse(jsonData))
            {
                return new WidgetButton(viewData, widgetCanvas);
            }
            
            return null;
        }
        
        private WidgetButton(WidgetButtonViewData viewData, IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/button");
            CreateView();
        }
        
        private void CreateView()
        {
            _buttonView = _widgetCanvas.GetWidgetPool().GetFromPool<WidgetViewButton>(_gameObject, _widgetCanvas.GetUiCanvas());
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
            _widgetCanvas.GetWidgetPool().ReturnToPool(_buttonView);
            _buttonView = null;
        }
    }
}