using Solcery.Services.Renderer.Widgets;
using UnityEngine;

namespace Solcery.Services.Renderer
{
    public interface IServiceRenderWidget
    {
        IWidgetRenderData CreateWidgetRender(RectTransform widget, int size = 512);
        void ReleaseWidgetRender(RectTransform widget);
    }
}