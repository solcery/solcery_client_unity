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
        private const string PrefabPathKey = "ui/ui_tooltip";
        private float? _delaySec;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private TooltipLayout _tooltipLayout;
        private int _tooltipId = -1;

        private float _offsetX;
        private float _offsetY;
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
            _offsetX = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipOffsetX, out int offsetXValue) ? offsetXValue : 0;
            _offsetY = tooltipDataObject.TryGetValue(GameJsonKeys.TooltipOffsetY, out int offsetYValue) ? offsetYValue : 0;
        }

        private void UpdateTooltipDelay(JObject tooltipDataObject)
        {
            var delaySec = tooltipDataObject.GetValue<int>(GameJsonKeys.TooltipDelay).ToSec();
            if (delaySec > 0)
            {
                Hide();
            }
            
            _delaySec = delaySec;
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
            var scaleFactor = _widgetCanvas.GetScaleFactor();
            var tooltipSizeDelta = _tooltipLayout.RectTransform.sizeDelta;
            var halfTooltipWidth = tooltipSizeDelta.x * scaleFactor * 0.5f;
            var halfTooltipHeight = tooltipSizeDelta.y * scaleFactor * 0.5f;

            if (targetPosition.x <= Screen.safeArea.width / 2f)
            {
                targetPosition.x += halfTooltipWidth + _offsetX * scaleFactor;
            }
            else
            {
                targetPosition.x -= halfTooltipWidth - _offsetX * scaleFactor;
            }

            if (targetPosition.y <= Screen.safeArea.height / 2f)
            {
                targetPosition.y += halfTooltipHeight + _offsetY * scaleFactor;
            }
            else
            {
                targetPosition.y -= halfTooltipHeight - _offsetY * scaleFactor;
            }
            
            return targetPosition;
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