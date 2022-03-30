using UnityEngine;

namespace Solcery.Services.Renderer.Widget
{
    public interface IWidgetRenderData
    {
        Vector2 UV { get; }
        RenderTexture RenderTexture { get; }
    }
}