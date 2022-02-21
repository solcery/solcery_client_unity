using Solcery.Ui.DragDrop;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public interface IWidgetCanvas
    {
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        RootDragDropLayout GetDragDropCanvas();
        void Destroy();
    }
}