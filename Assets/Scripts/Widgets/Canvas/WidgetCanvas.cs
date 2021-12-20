using UnityEngine;

namespace Solcery.Widgets.Canvas
{
    public sealed class WidgetCanvas : IWidgetCanvas
    {
        private Transform _worldCanvas;
        private RectTransform _uiCanvas;
        
        public static IWidgetCanvas Create(Transform worldCanvas, RectTransform uiCanvas)
        {
            return new WidgetCanvas(worldCanvas, uiCanvas);
        }
        
        private WidgetCanvas(Transform worldCanvas, RectTransform uiCanvas)
        {
            _worldCanvas = worldCanvas;
            _uiCanvas = uiCanvas;
        }
        
        Transform IWidgetCanvas.GetWorldCanvas()
        {
            return _worldCanvas;
        }

        RectTransform IWidgetCanvas.GetUiCanvas()
        {
            return _uiCanvas;
        }
        
        void IWidgetCanvas.Destroy()
        {
            _worldCanvas = null;
            _uiCanvas = null;
        }
    }
}