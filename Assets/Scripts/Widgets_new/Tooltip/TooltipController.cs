using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.Resources;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Tooltip
{
    public class TooltipController
    {
        private const float BorderOffset = 50f;
        private const string PrefabPathKey = "ui/ui_tooltip";
        private float? _delaySec;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private TooltipLayout _tooltipLayout;
        private int _tooltipId = -1;

        private float _verticalOffset;
        private Vector3 _targetPosition;
        
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

            if (_tooltipLayout != null && _tooltipLayout.gameObject.activeSelf)
            {
                _tooltipLayout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(GetPosition(_targetPosition));
            }
        }
        
        public void Show(int tooltipId, IGame game, JObject tooltipDataObject, Vector2 targetPosition)
        {
            if (_tooltipLayout == null)
            {
                Initialize();
            }

            if (_tooltipId != tooltipId)
            {
                UpdateTooltipSize(tooltipDataObject);
                UpdateTooltipContent(game, tooltipDataObject);
                UpdateTooltipDelay(tooltipDataObject);
                _tooltipId = tooltipId;
            }

            _targetPosition = targetPosition;
        }

        private void UpdateTooltipContent(IGame game, JObject tooltipDataObject)
        {
            _tooltipLayout.HideContent();
            if (tooltipDataObject.TryGetValue<int>(GameJsonKeys.TooltipCardTypeId, out var tplid) 
                && game.ServiceGameContent.ItemTypes.TryGetItemType(out var itemType, tplid))
            {
                _tooltipLayout.ShowEclipseCard(game, itemType);
            }
            else
            {
                _tooltipLayout.ShowSimpleText(tooltipDataObject);
            }
            
            _tooltipLayout.UpdateFillColor(tooltipDataObject);
        }

        private void UpdateTooltipSize(JObject tooltipDataObject)
        {
            var width = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipWidth, out int widthValue) ? widthValue : 200;
            var height = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipHeight, out int heightValue) ? heightValue : 100;
            _tooltipLayout.RectTransform.sizeDelta = new Vector2(width, height);
            _tooltipLayout.RectTransform.anchorMin = Vector2.zero;
            _tooltipLayout.RectTransform.anchorMax = Vector2.zero;
            _verticalOffset = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipOffset, out int offsetValue) ? offsetValue : 30;
        }

        private void UpdateTooltipDelay(JObject tooltipDataObject)
        {
            var delaySec = tooltipDataObject.GetValue<int>(GameJsonKeys.TooltipDelay).ToSec();
            if (delaySec > 0)
            {
                Hide();
                _delaySec = delaySec;
            }
        }

        public void Hide()
        {
            _tooltipId = -1;
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
            var scaleFactor = _widgetCanvas.GetScaleFactor();
            var halfTooltipWidth = sizeDelta.x * scaleFactor * 0.5f;
            var halfTooltipHeight = sizeDelta.y * scaleFactor * 0.5f;
            var leftLimit = Screen.safeArea.x + halfTooltipWidth + BorderOffset;
            var rightLimit = Screen.safeArea.width - halfTooltipWidth - BorderOffset;
            var topLimit = Screen.safeArea.height - halfTooltipHeight - BorderOffset;
            var bottomLimit = Screen.safeArea.y + halfTooltipHeight + BorderOffset;

            var newX = targetPosition.x;
            var newY = targetPosition.y + halfTooltipHeight + _verticalOffset * scaleFactor;
            
            if (newY > topLimit)
            {
                newY = targetPosition.y - halfTooltipHeight - _verticalOffset * scaleFactor;
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