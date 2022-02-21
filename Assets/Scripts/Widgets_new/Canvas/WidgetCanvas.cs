using Solcery.Ui.DragDrop;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public sealed class WidgetCanvas : IWidgetCanvas
    {
        private Transform _worldCanvas;
        private RectTransform _uiCanvas;
        private RootDragDropLayout _dragDropCanvas;
        
        public static IWidgetCanvas Create(Transform worldCanvas, RectTransform uiCanvas, RootDragDropLayout dragDropCanvas)
        {
            return new WidgetCanvas(worldCanvas, uiCanvas, dragDropCanvas);
        }
        
        private WidgetCanvas(Transform worldCanvas, RectTransform uiCanvas, RootDragDropLayout dragDropCanvas)
        {
            _worldCanvas = worldCanvas;
            _uiCanvas = uiCanvas;
            _dragDropCanvas = dragDropCanvas;
        }
        
        Transform IWidgetCanvas.GetWorldCanvas()
        {
            return _worldCanvas;
        }

        RectTransform IWidgetCanvas.GetUiCanvas()
        {
            return _uiCanvas;
        }

        RootDragDropLayout IWidgetCanvas.GetDragDropCanvas()
        {
            return _dragDropCanvas;
        }
        
        void IWidgetCanvas.Destroy()
        {
            _worldCanvas = null;
            _uiCanvas = null;
        }
    }
}