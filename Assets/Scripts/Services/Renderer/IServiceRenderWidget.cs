using Solcery.Services.Renderer.Widget;
using UnityEngine;

namespace Solcery.Services.Renderer
{
    public interface IServiceRenderWidget
    {
        IWidgetRenderData AddWidget(RectTransform widget);
    }
}