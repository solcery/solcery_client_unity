using Solcery.Ui;
using Solcery.Ui.DragDrop;
using Solcery.Widgets_new.Effects;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public sealed class WidgetCanvas : IWidgetCanvas
    {
        private TMP_Text _timer;
        private Transform _worldRoot;
        private RootUiGame _uiRoot;
        private RootDragDropLayout _dragDropRoot;
        private IWidgetEffects _widgetEffects;
        private UnityEngine.Canvas _uiCanvas;
        
        public static IWidgetCanvas Create(TMP_Text timer, Transform worldRoot, RootUiGame uiRoot, RootDragDropLayout dragDropRoot)
        {
            return new WidgetCanvas(timer, worldRoot, uiRoot, dragDropRoot);
        }
        
        private WidgetCanvas(TMP_Text timer, Transform worldRoot, RootUiGame uiRoot, RootDragDropLayout dragDropRoot)
        {
            _timer = timer;
            _worldRoot = worldRoot;
            _uiRoot = uiRoot;
            _dragDropRoot = dragDropRoot;
            _widgetEffects = WidgetEffects.Create(uiRoot.Effects);
        }

        TMP_Text IWidgetCanvas.GetTimer()
        {
            return _timer;
        }
        
        Transform IWidgetCanvas.GetWorldCanvas()
        {
            return _worldRoot;
        }

        RectTransform IWidgetCanvas.GetUiCanvas()
        {
            return _uiRoot.Game;
        }

        IWidgetEffects IWidgetCanvas.GetEffects()
        {
            return _widgetEffects;
        }

        public float GetScaleFactor()
        {
            return _uiRoot.UiCanvas.scaleFactor;
        }

        RectTransform IWidgetCanvas.GetTooltipsCanvas()
        {
            return _uiRoot.Tooltips;
        }
        
        RootDragDropLayout IWidgetCanvas.GetDragDropCanvas()
        {
            return _dragDropRoot;
        }
        
        void IWidgetCanvas.Destroy()
        {
            _worldRoot = null;
            _uiRoot = null;
            _dragDropRoot = null;
            _widgetEffects.Destroy();
            _widgetEffects = null;
        }
    }
}