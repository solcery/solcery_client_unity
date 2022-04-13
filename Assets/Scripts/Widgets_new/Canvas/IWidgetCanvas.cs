using Solcery.Ui;
using Solcery.Ui.DragDrop;
using Solcery.Widgets_new.Effects;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public interface IWidgetCanvas
    {
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        RootDragDropLayout GetDragDropCanvas();
        IWidgetEffects GetEffects();
        void Destroy();
    }
}