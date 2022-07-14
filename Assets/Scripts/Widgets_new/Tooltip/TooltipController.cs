using Leopotam.EcsLite;
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
        private const float VerticalOffset = 30f;
        private const string PrefabPathKey = "ui/ui_tooltip";
        private float? _delaySec;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceResource _serviceResource;
        private TooltipLayout _tooltipLayout;
        private int _tooltipId = -1;
        
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
        
        public void Show(int tooltipId, IGame game, EcsWorld world, JObject tooltipDataObject, Vector2 targetPosition)
        {
            if (_tooltipLayout == null)
            {
                Initialize();
            }

            if (_tooltipId != tooltipId)
            {
                UpdateTooltipContent(game, world, tooltipDataObject);
                UpdateTooltipDelay(tooltipDataObject);
                _tooltipId = tooltipId;
                _tooltipLayout.ToDefaultAnchors();
            }

            UpdateTooltipPosition(tooltipDataObject, targetPosition);
        }

        private void UpdateTooltipContent(IGame game, EcsWorld world, JObject tooltipDataObject)
        {
            _tooltipLayout.HideContent();
            var cardTypes = world.GetCardTypes();
            if (tooltipDataObject.TryGetValue<int>(GameJsonKeys.TooltipCardTypeId, out var cardType) && 
                cardTypes.TryGetValue(cardType, out var cardTypeDataObject))
            {
                _tooltipLayout.ShowEclipseCard(game, cardTypeDataObject);
            }
            else
            {
                _tooltipLayout.ShowSimpleText(tooltipDataObject);
            }
            
            _tooltipLayout.UpdateFillColor(tooltipDataObject);
        }

        private void UpdateTooltipPosition(JObject tooltipDataObject, Vector2 targetPosition)
        {
            var x1 = tooltipDataObject.TryGetValue(GameJsonKeys.PlaceX1, out int xt1) ? xt1 / GameConsts.AnchorDivider : 0f;
            var x2 = tooltipDataObject.TryGetValue(GameJsonKeys.PlaceX2, out int xt2) ? xt2 / GameConsts.AnchorDivider : 0f;
            var y1 = tooltipDataObject.TryGetValue(GameJsonKeys.PlaceY1, out int yt1) ? yt1 / GameConsts.AnchorDivider : 0f;
            var y2 = tooltipDataObject.TryGetValue(GameJsonKeys.PlaceY2, out int yt2) ? yt2 / GameConsts.AnchorDivider : 0f;
            if (x1 == 0 && x2 == 0 && y1 == 0 && y2 == 0)
            {
                _tooltipLayout.RectTransform.anchoredPosition = GameApplication.Instance.WorldToCanvas(GetPosition(targetPosition));
            }
            else
            {
                _tooltipLayout.UpdateAnchor(new Vector2(x1, y1), new Vector2(x2, y2));
            }
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
            var newY = targetPosition.y + halfTooltipHeight + VerticalOffset * scaleFactor;
            
            if (newY > topLimit)
            {
                newY = targetPosition.y - halfTooltipHeight - VerticalOffset * scaleFactor;
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