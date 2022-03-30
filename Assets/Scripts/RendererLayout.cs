using UnityEngine;

namespace Solcery
{
    public sealed class RendererLayout : MonoBehaviour
    {
        [SerializeField]
        private Camera renderCamera;
        [SerializeField]
        private RectTransform renderRectTransform;

        public Camera RenderCamera => renderCamera;
        public RectTransform RenderRectTransform => renderRectTransform;
    }
}