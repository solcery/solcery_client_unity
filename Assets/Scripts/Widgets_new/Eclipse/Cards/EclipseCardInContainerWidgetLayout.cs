using System.Collections.Generic;
using DG.Tweening;
using Solcery.Services.Events;
using Solcery.Services.Renderer.Widgets;
using Solcery.Utils;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public sealed class EclipseCardInContainerWidgetLayout : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        [HideInInspector]
        public int EntityId;
        public int? AttachEntityId = null;
        public PlaceWidget ParentPlaceWidget;
        
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private RectTransform frontTransform;
        [SerializeField]
        private RectTransform backTransform;
        [SerializeField]
        private EclipseCardEffectLayout effectLayout;
        [SerializeField]
        private EclipseCardTimerLayout timerLayout;
        [SerializeField]
        private EclipseCardTokensLayout tokensLayout;
        [SerializeField]
        private RectTransform iconFrame;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private RectTransform iconRectTransform;
        [SerializeField]
        private TMP_Text typeText;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private GameObject[] highlights;
        [SerializeField]
        private List<Graphic> raycastObjects;
        [SerializeField]
        private Texture2D defaultTexture;

        private Sprite _sprite;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        private CustomImages _images;

        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new Dictionary<Graphic, bool>();
        
        public RectTransform RectTransform => rectTransform;
        public RectTransform FrontTransform => frontTransform;
        public RectTransform BackTransform => backTransform;
        public EclipseCardEffectLayout EffectLayout => effectLayout;
        
        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;

        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
            effectLayout.gameObject.SetActive(false);
            if (iconImage.sprite == null)
            {
                UpdateSprite(defaultTexture);
            }
            _images = gameObject.AddComponent<CustomImages>();
        }

        public void UpdateParent(Transform parent, bool isDragDrop = false)
        {
            rectTransform.SetParent(parent, false);
            if (!isDragDrop)
            {
                rectTransform.anchoredPosition = _anchoredPosition;
                rectTransform.offsetMax = Vector2.down;
                rectTransform.pivot = _pivot;
                rectTransform.anchorMin = _anchorMin;
                rectTransform.anchorMax = _anchorMax;
                rectTransform.offsetMin = _offsetMin;
                rectTransform.offsetMax = _offsetMax;
            }

            rectTransform.localScale = Vector3.one;
        }
        
        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }
        
        public void Cleanup()
        {
            tokensLayout.Cleanup();
            timerLayout.Cleanup();
        }
        
        private void OnDestroy()
        {
            DestroySprite();
        }

        public void UpdateName(string newName, float fontSize)
        {
            nameText.text = newName;
            nameText.UpdateFontSize(fontSize);
        }

        public void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.UpdateFontSize(fontSize);
        }

        public void UpdateType(string displayedType, float fontSize)
        {
            typeText.text = displayedType;
            typeText.UpdateFontSize(fontSize);
        }

        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            if (texture == null)
            {
                texture = defaultTexture;
            }
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
            UpdateIconSize();
        }

        public void UpdateHighlight(bool active)
        {
            foreach (var highlight in highlights)
            {
                highlight.SetActive(active);
            }
        }

        private void DestroySprite()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }
        }

        public void SetActive(bool active)
        {
            FrontTransform.gameObject.SetActive(active);

            if (active)
            {
                RaycastOn();
            }
            else
            {
                RaycastOff();
            }
        }
        
        //private Material m_Material;

        public void PlayDestroyAnimation(IWidgetRenderData widgetRenderData)
        {
            // m_Material = destroyCardRenderer.material;
            // //m_Material.SetTexture(MainTexRtId, widgetRenderData.RenderTexture);
            // m_Material.mainTexture = widgetRenderData.RenderTexture;
            // //MainTex(RT)
            // destroyTransform.gameObject.SetActive(true);
        }

        public void StopDestroyAnimation()
        {
            // m_Material.SetTexture(MainTexRtId, null);
            // Destroy(m_Material);
            // destroyTransform.gameObject.SetActive(false);
        }

        public void RaycastOn()
        {
            foreach (var targetSetting in _raycastTargetSettings)
            {
                targetSetting.Key.raycastTarget = targetSetting.Value;
            }
        }

        public void RaycastOff()
        {
            _raycastTargetSettings.Clear();
            foreach (var raycastObject in raycastObjects)
            {
                _raycastTargetSettings.Add(raycastObject, raycastObject.raycastTarget);
                raycastObject.raycastTarget = false;
            }
        }

        public void UpdateAvailable(bool available)
        {
            _images.UpdateAvailable(available);
        }

        private Tweener _tween;
        private bool _isClick;
        private Vector2 _downPosition;
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _isClick = true;
            _downPosition = eventData.position;
            _tween = DOTween.To(value => { }, 0, 1, .5f);
            _tween.OnComplete(OnRightClick);
        }

        private void OnRightClick()
        {
            _tween?.OnComplete(null);
            _tween = null;
            _downPosition = Vector2.zero;
            _isClick = false;
            OnOnPointerRightButtonClick();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            if (_isClick && (_downPosition - eventData.position).magnitude >= EventSystem.current.pixelDragThreshold)
            {
                _downPosition = Vector2.zero;
                _isClick = false;
                _tween?.Kill();
                _tween = null;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = null;

            if (_isClick)
            {
                _downPosition = Vector2.zero;
                _isClick = false;
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        OnOnPointerLeftButtonClick(eventData);
                        break;
                    case PointerEventData.InputButton.Right:
                        OnOnPointerRightButtonClick();
                        break;
                }
            }
        }

        private void OnOnPointerLeftButtonClick(PointerEventData eventData)
        {
            if (AttachEntityId == null)
            {
                return;
            }
            
            ParentPlaceWidget = transform.parent.GetComponentInParent<PlaceWidgetLayout>().PlaceWidget;
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            ServiceEvents.Current.BroadcastEvent(OnUniversalEventData.Create(EntityId, AttachEntityId.Value, position, eventData));
        }
        
        private void OnOnPointerRightButtonClick()
        {
            ServiceEvents.Current.BroadcastEvent(OnRightClickEventData.Create(EntityId));
        }

        private void OnEnable()
        {
            UpdateIconSize();
        }

        private void UpdateIconSize()
        {
            if (iconImage.sprite == null 
                || iconImage.sprite.texture == null)
            {
                return;
            }
            
            var texture = iconImage.sprite.texture;
            var textureSize = new Vector2(texture.width, texture.height);
            var iconSize = iconFrame.rect.size;
            var iconAspect = textureSize.y / textureSize.x;
            var width = iconSize.x;
            var height = iconAspect * width;
            iconRectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}