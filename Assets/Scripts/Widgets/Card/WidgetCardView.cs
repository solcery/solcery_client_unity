using System;
using Leopotam.EcsLite;
using Solcery.Widgets.Attributes;
using TMPro;
using UnityEngine.UI;

namespace Solcery.Widgets.Card
{
    public class WidgetCardView : WidgetViewBase, IHighlighted, IIntractable
    {
        public TextMeshProUGUI Text;
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

        public void SetIntractable(bool value)
        {
            Button.interactable = value;
        }
    }
}
