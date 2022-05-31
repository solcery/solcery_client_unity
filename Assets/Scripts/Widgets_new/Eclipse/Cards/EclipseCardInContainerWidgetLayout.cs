using System.Collections.Generic;
using Solcery.Services.Events;
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
        public int AttachEntityId;
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
        private Image iconImage;
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
        private bool _fullMode;
        
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
            //rectTransform.rotation = Quaternion.identity;
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

        public void UpdateName(string newName)
        {
            nameText.text = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            descriptionText.text = newDescription;
        }

        public void UpdateType(string newType)
        {
            typeText.text = newType;
        }
        
        public void UpdateSprite(Texture2D texture)
        {
            DestroySprite();
            
            _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
            iconImage.sprite = _sprite;
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

        public void SetFullMode(float height, Vector3 position)
        {
            _fullMode = true;
            RectTransform.sizeDelta = new Vector2(0, height);
            RectTransform.anchorMin = new Vector2(.5f, .5f);
            RectTransform.anchorMax = new Vector2(.5f, .5f);
            transform.position = position;
            RaycastOff();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (_fullMode)
                return;

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
            ParentPlaceWidget = transform.parent.GetComponentInParent<PlaceWidgetLayout>().PlaceWidget;
            RectTransformUtility.ScreenPointToWorldPointInRectangle
            (
                rectTransform, 
                eventData.position, 
                Camera.current,
                out var position
            );
            ServiceEvents.Current.BroadcastEvent(OnDragEventData.Create(AttachEntityId, position, eventData));
        }
        
        private void OnOnPointerRightButtonClick()
        {
            ServiceEvents.Current.BroadcastEvent(OnEclipseCardFullEventData.Create(this));
        }
    }
}