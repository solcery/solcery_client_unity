using UnityEngine;

namespace Solcery.Services.Renderer.Widget
{
    public sealed class WidgetRenderData : IWidgetRenderData
    {
        public Vector2 UV => _uv;
        public RenderTexture RenderTexture => _renderTexture;
        
        private readonly Vector2 _uv;
        private readonly RenderTexture _renderTexture;

        public static WidgetRenderData Create(Vector2 uv, RenderTexture renderTexture)
        {
            return new WidgetRenderData(uv, renderTexture);
        }

        private WidgetRenderData(Vector2 uv, RenderTexture renderTexture)
        {
            _uv = uv;
            _renderTexture = renderTexture;
        }
    }
}