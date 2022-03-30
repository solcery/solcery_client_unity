using Solcery.Services.Renderer.Widget;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Solcery.Services.Renderer
{
    public sealed class ServiceRenderWidget : IServiceRenderWidget
    {
        private readonly Camera _renderCamera;
        private readonly RectTransform _canvasRectTransform;

        public static IServiceRenderWidget Create(RendererLayout rendererLayout)
        {
            return new ServiceRenderWidget(rendererLayout);
        }

        private ServiceRenderWidget(RendererLayout rendererLayout)
        {
            _renderCamera = rendererLayout.RenderCamera;
            _canvasRectTransform = rendererLayout.RenderRectTransform;
        }

        private GameObject _widget;

        public IWidgetRenderData AddWidget(RectTransform widget)
        {
            if (_widget != null)
            {
                return null;
            }

            var rect = widget.rect;
            var width = rect.width;
            var height = rect.height;
            var powerOf2 = NearestPowerOf2_4(Mathf.CeilToInt(Mathf.Max(width, height)));

            var rtt = new RenderTexture(powerOf2, powerOf2, 16, GraphicsFormat.R8G8B8A8_UNorm);

            var crtt = _renderCamera.targetTexture;
            if (crtt != null)
            {
                crtt.Release();
            }

            _renderCamera.targetTexture = rtt;
            _canvasRectTransform.ForceUpdateRectTransforms();
            
            _widget = Object.Instantiate(widget.gameObject);
            var widgetRectTransformRect = _widget.GetComponent<RectTransform>();
            widgetRectTransformRect.SetParent(_canvasRectTransform);
            widgetRectTransformRect.pivot = Vector2.up;
            widgetRectTransformRect.localScale = Vector3.one;
            widgetRectTransformRect.position = Vector3.zero;
            widgetRectTransformRect.ForceUpdateRectTransforms();

            return WidgetRenderData.Create(new Vector2(width / powerOf2, height / powerOf2), rtt);
        }
        
        private int NearestPowerOf2_4(int n)
        {
            unsafe
            {
                const int mantissaMask = (1<<23) - 1;  
                (*(float*)&n) = n;  
                n = n + mantissaMask & ~mantissaMask;  
                n = (int) *(float*)&n; 
            }
             
            return n;
        }
    }
}