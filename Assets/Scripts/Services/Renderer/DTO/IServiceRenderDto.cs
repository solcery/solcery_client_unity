using UnityEngine;

namespace Solcery.Services.Renderer.DTO
{
    public interface IServiceRenderDto
    {
        Transform Frame { get; }
        GameObject WidgetRenderPrefab { get; }
    }
}