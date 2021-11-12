using Solcery.Widgets.Pool;
using UnityEngine;

namespace Solcery.Widgets.Canvas
{
    public interface IWidgetCanvas
    {
        Transform GetWorldCanvas();
        RectTransform GetUiCanvas();
        WidgetPool GetWidgetPool();
        void Destroy();
    }
}