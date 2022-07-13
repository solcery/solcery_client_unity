using Solcery.Ui.DragDrop;
using Solcery.Widgets_new.Effects;
using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public interface IWidgetCanvas
    {
        TMP_Text GetTimer();
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        RectTransform GetTooltipsCanvas();
        RootDragDropLayout GetDragDropCanvas();
        IWidgetEffects GetEffects();
        float GetScaleFactor();
        void Destroy();
    }
}