using System;
using Solcery.Models.Attributes.Highlighted;
using Solcery.Models.Attributes.Interactable;
using Solcery.Widgets.Deck;
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
            gameObject.SetActive(viewData.Visible);
            Back.SetActive(!viewData.Face);
        }
    }
}
