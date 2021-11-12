using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery.Widgets.Canvas
{
    public sealed class WidgetCanvas : IWidgetCanvas
    {
        private Transform _worldCanvas;
        private RectTransform _uiCanvas;
        private WidgetPool _widgetPool;
        
        public static IWidgetCanvas Create(Transform worldCanvas, RectTransform uiCanvas)
        {
            return new WidgetCanvas(worldCanvas, uiCanvas);
        }
        
        private WidgetCanvas(Transform worldCanvas, RectTransform uiCanvas)
        {
            _worldCanvas = worldCanvas;
            _uiCanvas = uiCanvas;
            _widgetPool = new WidgetPool("widget_pool");
        }
        
        Transform IWidgetCanvas.GetWorldCanvas()
        {
            return _worldCanvas;
        }

        RectTransform IWidgetCanvas.GetUiCanvas()
        {
            return _uiCanvas;
        }

        WidgetPool IWidgetCanvas.GetWidgetPool()
        {
            return _widgetPool;
        }
        
        void IWidgetCanvas.Destroy()
        {
            _worldCanvas = null;
            _uiCanvas = null;
        }
    }
}