using UnityEngine;

namespace Solcery.Services.Renderer.Widgets
{
    public interface IWidgetRenderData
    {
        Vector2 UV { get; }
        RenderTexture RenderTexture { get; }

        void Release();
    }
}