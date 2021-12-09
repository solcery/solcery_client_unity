using System.Linq;
using Solcery.Widgets_new.StaticOrderZ;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class PlaceWidgetLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image background;
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        private int _orderZ;
        private GameObject _gameObject;

        private void Awake()
        {
            _gameObject = gameObject;
        }

        public void UpdateVisible(bool enable)
        {
            _gameObject.SetActive(enable);
        }
        
        public void UpdateOrderZ(int orderZ)
        {
            _orderZ = orderZ;

            var parent = rectTransform.parent;
            var staticOrderZLayoutCount = StaticOrderZLayout.StaticOrderZCount;
            
            var placeWidgetLayoutArray = parent.GetComponentsInChildren<PlaceWidgetLayout>()
                .OrderBy(o => o._orderZ).ToList();
            foreach (var placeWidgetLayout in placeWidgetLayoutArray)
            {
                rectTransform.SetSiblingIndex(staticOrderZLayoutCount + placeWidgetLayoutArray.IndexOf(placeWidgetLayout));
            }
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
    }
}