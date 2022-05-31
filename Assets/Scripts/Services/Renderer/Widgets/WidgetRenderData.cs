using Solcery.Services.Renderer.Layouts;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Solcery.Services.Renderer.Widgets
{
    public sealed class WidgetRenderData : IWidgetRenderData
    {
        public Vector2 UV => _uv;
        public RenderTexture RenderTexture => _renderTexture;
        
        private readonly Vector2 _uv;
        private readonly RenderTexture _renderTexture;
        private readonly GameObject _renderObject;

        private RectTransform _widgetRectTransform;

        public static IWidgetRenderData Create(RectTransform widget, GameObject renderObjectPrefab, Transform parent)
        {
            return new WidgetRenderData(widget, renderObjectPrefab, parent);
        }

        private WidgetRenderData(RectTransform widget, GameObject renderObjectPrefab, Transform parent)
        {
            _renderObject = Object.Instantiate(renderObjectPrefab, parent);
            _renderObject.transform.position = Vector3.zero;
            _renderObject.transform.localScale = Vector3.one;
            var renderObjectLayout = _renderObject.GetComponent<WidgetRendererLayout>();

            var rect = widget.rect;
            var width = rect.width;
            var height = rect.height;
            var powerOf2 = NearestPowerOf2_4(Mathf.CeilToInt(Mathf.Max(width, height)));
            _uv = new Vector2(width / powerOf2, height / powerOf2);
            
            _renderTexture = new RenderTexture(powerOf2, powerOf2, 16, GraphicsFormat.R8G8B8A8_UNorm);
            renderObjectLayout.RenderCamera.targetTexture = _renderTexture;
            renderObjectLayout.RenderRectTransform.ForceUpdateRectTransforms();
            
            var wid = Object.Instantiate(widget.gameObject);
            _widgetRectTransform = wid.GetComponent<RectTransform>();
            _widgetRectTransform.SetParent(renderObjectLayout.RenderRectTransform);
            _widgetRectTransform.pivot = Vector2.up;
            _widgetRectTransform.localScale = Vector3.one;
            _widgetRectTransform.anchoredPosition3D = Vector3.zero;
            _widgetRectTransform.anchorMin = Vector2.up;
            _widgetRectTransform.anchorMax = Vector2.up;
            _widgetRectTransform.sizeDelta = new Vector2(0, powerOf2);
            _widgetRectTransform.ForceUpdateRectTransforms();
        }
        
        void IWidgetRenderData.Release()
        {
            _widgetRectTransform = null;
            Object.Destroy(_renderObject);
            _renderTexture.Release();
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