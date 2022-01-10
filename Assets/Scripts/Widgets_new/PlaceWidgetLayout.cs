using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class PlaceWidgetLayout : MonoBehaviour
    {
        public int OrderZ => _orderZ;
        
        public RectTransform Root => rectTransform;
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image background;
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        private int _orderZ;
        private GameObject _gameObject;

        private bool _firstUpdate;
        private int _firstUpdateIteration;
        private bool _firstUpdateVisible;
        private bool _visible;

        private void Awake()
        {
            _gameObject = gameObject;
        }

        public void UpdateFirstUpdate(bool firstUpdate, bool visible)
        {
            _firstUpdate = firstUpdate;
            _firstUpdateVisible = visible;
        }

        public void UpdateVisible(bool enable)
        {
            if (_firstUpdate)
            {
                _firstUpdateIteration = 0;
                _visible = enable;
                _gameObject.SetActive(_firstUpdateVisible);
                return;
            }

            _gameObject.SetActive(enable);
        }
        
        public void UpdateOrderZ(int orderZ)
        {
            _orderZ = orderZ;
        }

        public void UpdateSiblingIndex(int siblingIndex)
        {
            rectTransform.SetSiblingIndex(siblingIndex);
        }

        public void UpdateAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        public virtual void UpdateAlpha(int alpha) { }

        public void UpdateBackgroundColor(string backgroundColor)
        {
            if (backgroundColor != null)
            {
                if (ColorUtility.TryParseHtmlString(backgroundColor, out var bgColor))
                {
                    if (background != null)
                    {
                        background.gameObject.SetActive(true);
                        background.color = bgColor;
                        return;
                    }
                }
            }

            if (background != null)
            {
                background.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (_firstUpdateIteration > 2)
            {
                _firstUpdateIteration = -1;
                _gameObject.SetActive(_visible);
            }

            if (_firstUpdateIteration > 0)
            {
                _firstUpdateIteration++;
            }
        }
    }
}