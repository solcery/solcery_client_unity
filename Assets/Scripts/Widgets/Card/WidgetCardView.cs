using System;
using Solcery.Models.Play.Attributes.Highlighted;
using Solcery.Models.Play.Attributes.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets.Card
{
    public class WidgetCardView : WidgetViewBase, IHighlighted, IInteractable
    {
        public GameObject Back;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public UnityEngine.UI.Button Button;
        public Image Image;
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
            Destroy(Image.sprite);
        }

        public void SetHighlighted(bool value)
        {
        }

        public void SetInteractable(bool value)
        {
            Button.interactable = value;
        }

        public override void ApplyPlaceViewData(WidgetPlaceViewData viewData)
        {
            switch (viewData.Face)
            {
                case CardFaceOption.Up:
                    Back.SetActive(false);
                    break;
                case CardFaceOption.Down:
                    Back.SetActive(true);
                    break;
            }
        }
    }
}
