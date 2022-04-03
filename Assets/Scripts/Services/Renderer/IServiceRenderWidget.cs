using Solcery.Services.Renderer.Widgets;
using UnityEngine;

namespace Solcery.Services.Renderer
{
    public interface IServiceRenderWidget
    {
        IWidgetRenderData CreateWidgetRender(RectTransform widget);
        void ReleaseWidgetRender(RectTransform widget);
    }
}