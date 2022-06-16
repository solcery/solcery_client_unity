using Solcery.Ui;
using Solcery.Ui.DragDrop;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Effects;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public sealed class WidgetCanvas : IWidgetCanvas
    {
        private Transform _worldRoot;
        private RootUiGame _uiRoot;
        private RootDragDropLayout _dragDropRoot;
        private IWidgetEffects _widgetEffects;
        
        public static IWidgetCanvas Create(Transform worldRoot, RootUiGame uiRoot, RootDragDropLayout dragDropRoot)
        {
            return new WidgetCanvas(worldRoot, uiRoot, dragDropRoot);
        }
        
        private WidgetCanvas(Transform worldRoot, RootUiGame uiRoot, RootDragDropLayout dragDropRoot)
        {
            _worldRoot = worldRoot;
            _uiRoot = uiRoot;
            _dragDropRoot = dragDropRoot;
            _widgetEffects = WidgetEffects.Create(uiRoot.Effects);
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