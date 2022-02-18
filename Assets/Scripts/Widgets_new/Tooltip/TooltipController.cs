using Solcery.Services.Resources;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipController
    {
        private const string PrefabPathKey = "ui/ui_tooltip";
        private TooltipLayout _tooltipLayout;
        private float? _delay;
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
            _delay = null;
        }

        public void Update(float dt)
        {
            if (_delay != null && _delay.Value >= 0)
            {
                _delay -= dt;
                if (_delay <= 0)
                {
                    _delay = null;
                    SetTooltipActive(true);
                }
            }
        }
        
        public void Show(string text, Vector2 targetPosition, float delay = 0)
        {
            if (_tooltipLayout == null)
            {
                Initialize();
            }
            
            _tooltipLayout.Text.text = text;
            _tooltipLayout.transform.position = GetPosition(targetPosition);
            if (delay >= 0)
            {
                Hide();
                _delay = delay;
            }
        }

        public void Hide()
        {
            _delay = null;
            SetTooltipActive(false);
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

        private void SetTooltipActive(bool value)
        {
            if (_tooltipLayout != null && _tooltipLayout.gameObject.activeSelf != value)
            {
                _tooltipLayout.gameObject.SetActive(value);
            }
        }
    }
}