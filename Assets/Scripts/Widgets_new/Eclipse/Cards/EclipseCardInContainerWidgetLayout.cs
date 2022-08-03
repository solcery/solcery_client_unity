using System.Collections.Generic;
using Solcery.Services.Events;
using Solcery.Services.Renderer.Widgets;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using Solcery.Widgets_new.Eclipse.Cards.Timers;
using Solcery.Widgets_new.Eclipse.Cards.Tokens;
using Solcery.Widgets_new.Eclipse.DragDropSupport.EventsData;
using Solcery.Widgets_new.Eclipse.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards
{
    public sealed class EclipseCardInContainerWidgetLayout : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector]
        public int EntityId;
        [HideInInspector]
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

        private Sprite _sprite;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _pivot;
        private Vector2 _anchoredPosition;
        private Vector2 _offsetMax;
        private Vector2 _offsetMin;
        
        private readonly Dictionary<Graphic, bool> _raycastTargetSettings = new Dictionary<Graphic, bool>();
        
        public RectTransform RectTransform => rectTransform;
        public RectTransform FrontTransform => frontTransform;
        public RectTransform BackTransform => backTransform;
        public EclipseCardEffectLayout EffectLayout => effectLayout;
        
        public EclipseCardTokensLayout TokensLayout => tokensLayout;
        public EclipseCardTimerLayout TimerLayout => timerLayout;

        private Texture2D _newTexture;
        private Vector2 _previewIconFrameSize;
        private Vector2 _textureSize;

        private void Awake()
        {
            _anchoredPosition = rectTransform.anchoredPosition;
            _anchorMin = rectTransform.anchorMin;
            _anchorMax = rectTransform.anchorMax;
            _pivot = rectTransform.pivot;
            _offsetMin = rectTransform.offsetMin;
            _offsetMax = rectTransform.offsetMax;
            effectLayout.gameObject.SetActive(false);
            _previewIconFrameSize = iconFrame.rect.size;
            _textureSize = _previewIconFrameSize;
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
            nameText.fontSize = fontSize;
            nameText.text = newName;
        }

        public void UpdateDescription(string newDescription, float fontSize)
        {
            descriptionText.text = newDescription;
            descriptionText.fontSize = fontSize;
        }

        public void UpdateType(string type, float fontSize)
        {
            typeText.text = type;
            typeText.fontSize = fontSize;
        }
        
        public void UpdateSprite(Texture2D texture)
        {
            _newTexture = texture;
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

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
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
            ServiceEvents.Current.BroadcastEvent(OnDragEventData.Create(AttachEntityId.Value, position, eventData));
        }
        
        private void OnOnPointerRightButtonClick()
        {
            ServiceEvents.Current.BroadcastEvent(OnRightClickEventData.Create(EntityId));
        }

        private void LateUpdate()
        {
            if (_newTexture == null && iconFrame.rect.size != _previewIconFrameSize)
            {
                return;
            }

            // Update sprite
            if (_newTexture != null)
            {
                DestroySprite();
                _sprite = Sprite.Create(_newTexture, new Rect(0.0f, 0.0f, _newTexture.width, _newTexture.height),
                    new Vector2(0.5f, 0.5f), 100.0f);
                iconImage.sprite = _sprite;
                _textureSize = new Vector2(_newTexture.width, _newTexture.height);
                _newTexture = null;
            }
            
            // Update size
            _previewIconFrameSize = iconFrame.rect.size;
            var iconAspect = _textureSize.y / _textureSize.x;
            var width = _previewIconFrameSize.x;
            var height = iconAspect * width;
            iconRectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}