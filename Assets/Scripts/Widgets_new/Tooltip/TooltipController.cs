using Solcery.Services.Resources;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipController
    {
        private const string PrefabPathKey = "ui/ui_tooltip";
        private TooltipLayout _tooltipLayout;
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
        }

        public void Update(float dt)
        {
        }
        
        public void Show(string text, Vector2 targetPosition)
        {
            if (_tooltipLayout == null)
            {
                Initialize();
            }
            _tooltipLayout.gameObject.SetActive(true);
            _tooltipLayout.transform.position = GetPosition(targetPosition);
        }

        public void Hide()
        {
            if (_tooltipLayout != null)
            {
                _tooltipLayout.gameObject.SetActive(false);
            }
        }

        private void Initialize()
        {
            if (_serviceResource.TryGetWidgetPrefabForKey(PrefabPathKey, out var go)
                && Object.Instantiate(go, _widgetCanvas.GetUiCanvas()).TryGetComponent(typeof(TooltipLayout), out var component)
                && component is TooltipLayout layout)
            {
                _tooltipLayout = layout;
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
            var leftLimit = Screen.safeArea.x + halfTooltipWidth;
            var rightLimit = Screen.safeArea.width - halfTooltipWidth;
            var topLimit = Screen.safeArea.height - halfTooltipHeight;
            var bottomLimit = Screen.safeArea.y + halfTooltipHeight;

            var newX = targetPosition.x;
            var newY = targetPosition.y + halfTooltipHeight;
            
            if (newY > topLimit)
            {
                newY = targetPosition.y - halfTooltipHeight;
            }

            newX = Mathf.Clamp(newX, leftLimit, rightLimit);
            newY = Mathf.Clamp(newY, bottomLimit, topLimit);

            return new Vector2(newX, newY);
        }
    }
}