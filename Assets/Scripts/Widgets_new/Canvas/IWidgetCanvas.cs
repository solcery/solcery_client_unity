using UnityEngine;

namespace Solcery.Widgets_new.Canvas
{
    public interface IWidgetCanvas
    {
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        void Destroy();
    }
}