using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipController
    {
        private const float BorderOffset = 50f;
        private const float VerticalOffset = 30f;
        private const string PrefabPathKey = "ui/ui_tooltip";
        private TooltipLayout _tooltipLayout;
        private float? _delaySec;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        
        public static TooltipController Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            return new TooltipController(widgetCanvas, serviceResource);
        }
        
        private TooltipController(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            _widgetCanvas = widgetCanvas;
            _serviceResource = serviceResource;
            _delaySec = null;
        }

        public void Update(float dt)
        {
            if (_delaySec != null && _delaySec.Value >= 0)
            {
                _delaySec -= dt;
                if (_delaySec <= 0)
                {
                    _delaySec = null;
                    SetTooltipActive(true);
                }
            }
        }
        
        public void Show(JObject tooltipDataObject, Vector2 targetPosition)
        {
            if (_tooltipLayout == null)
            {
                Initialize();
            }
            
            _tooltipLayout.UpdateText(tooltipDataObject.GetValue<string>("text"));
            _tooltipLayout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(GetPosition(targetPosition));
            var fillColor = tooltipDataObject.TryGetValue("fill_color", out string fillColorAttribute) ? fillColorAttribute : null;
            _tooltipLayout.UpdateFillColor(fillColor);
            var fontSize = tooltipDataObject.TryGetValue("font_size", out int fontSizeAttribute) ? fontSizeAttribute : 36;
            _tooltipLayout.UpdateFontSize(fontSize);
            var delaySec = tooltipDataObject.GetValue<int>("delay").ToSec();
            if (delaySec >= 0)
            {
                Hide();
                _delaySec = delaySec;
            }
        }

        public void Hide()
        {
            _delaySec = null;
            SetTooltipActive(false);
        }

        private void Initialize()
        {
            if (_serviceResource.TryGetWidgetPrefabForKey(PrefabPathKey, out var go)
                && Object.Instantiate(go, _widgetCanvas.GetTooltipsCanvas()).TryGetComponent(typeof(TooltipLayout), out var component)
                && component is TooltipLayout layout)
            {
                _tooltipLayout = layout;
                _tooltipLayout.RectTransform.anchorMin = Vector2.zero; 
                _tooltipLayout.RectTransform.anchorMax = Vector2.zero;
                _tooltipLayout.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Can't initialize TooltipController!");
            }
        }
        
        private Vector2 GetPosition(Vector2 targetPosition)
        {
            var sizeDelta = _tooltipLayout.RectTransform.sizeDelta;
            var halfTooltipWidth = sizeDelta.x * 0.5f;
            var halfTooltipHeight = sizeDelta.y * 0.5f;
            var leftLimit = Screen.safeArea.x + halfTooltipWidth + BorderOffset;
            var rightLimit = Screen.safeArea.width - halfTooltipWidth - BorderOffset;
            var topLimit = Screen.safeArea.height - halfTooltipHeight - BorderOffset;
            var bottomLimit = Screen.safeArea.y + halfTooltipHeight + BorderOffset;

            var newX = targetPosition.x;
            var newY = targetPosition.y + halfTooltipHeight + VerticalOffset;
            
            if (newY > topLimit)
            {
                newY = targetPosition.y - halfTooltipHeight - VerticalOffset;
            }

            newX = Mathf.Clamp(newX, leftLimit, rightLimit);
            newY = Mathf.Clamp(newY, bottomLimit, topLimit);

            return new Vector2(newX, newY);
        }

        private void SetTooltipActive(bool value)
        {
            if (_tooltipLayout != null && _tooltipLayout.gameObject.activeSelf != value)
            {
                _tooltipLayout.gameObject.SetActive(value);
            }
        }
        
        public void Destroy()
        {
        }
    }
}