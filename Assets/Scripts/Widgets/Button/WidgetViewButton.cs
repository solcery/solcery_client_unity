using System;
using Solcery.Models.Play.Attributes.Highlighted;
using Solcery.Models.Play.Attributes.Interactable;
using TMPro;
using UnityEngine.UI;

namespace Solcery.Widgets.Button
{
    public class WidgetViewButton : WidgetViewBase, IInteractable, IHighlighted
    {
        public UnityEngine.UI.Button Button;
        public TextMeshProUGUI Text;
        public Image HighlightImage;
        
        public Action OnClick { get; set; }
        
        public override void Init()
        {
            base.Init();
            Button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
        }

        public override void Clear()
        {
            Button.onClick.RemoveAllListeners();
            HighlightImage.gameObject.SetActive(false);
        }
        
        public void SetInteractable(bool value)
        {
            Button.interactable = value;
        }

        public void SetHighlighted(bool value)
        {
            HighlightImage.gameObject.SetActive(value);
        }
        
        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            RectTransform.anchorMin = viewData.AnchorMin;
            RectTransform.anchorMax = viewData.AnchorMax;
            SetInteractable(viewData.Interactable);
        }
    }
}
