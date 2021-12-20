using UnityEngine;

namespace Solcery.Widgets.Canvas
{
    public interface IWidgetCanvas
    {
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        void Destroy();
    }
}