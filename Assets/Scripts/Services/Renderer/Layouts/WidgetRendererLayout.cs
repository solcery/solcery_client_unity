using UnityEngine;

namespace Solcery.Services.Renderer.Layouts
{
    public sealed class WidgetRendererLayout : MonoBehaviour
    {
        [SerializeField]
        private Camera renderCamera;
        [SerializeField]
        private RectTransform renderRectTransform;

        public Camera RenderCamera => renderCamera;
        public RectTransform RenderRectTransform => renderRectTransform;
    }
}