using UnityEngine;

namespace Solcery.Services.Renderer.DTO
{
    public sealed class ServiceRenderDto : IServiceRenderDto
    {
        public Transform Frame => _frame;
        public GameObject WidgetRenderPrefab => _widgetRenderPrefab;
        
        private readonly Transform _frame;
        private readonly GameObject _widgetRenderPrefab;

        public static IServiceRenderDto Create(Transform frame, GameObject widgetRenderPrefab)
        {
            return new ServiceRenderDto(frame, widgetRenderPrefab);
        }

        private ServiceRenderDto(Transform frame, GameObject widgetRenderPrefab)
        {
            _frame = frame;
            _widgetRenderPrefab = widgetRenderPrefab;
        }
    }
}