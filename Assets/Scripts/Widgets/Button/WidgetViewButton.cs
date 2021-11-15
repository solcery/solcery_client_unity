using System;
using Solcery.Models.Attributes.Interactable;
using TMPro;

namespace Solcery.Widgets.Button
{
    public class WidgetViewButton : WidgetViewBase, IInteractable
    {
        public UnityEngine.UI.Button Button;
        public TextMeshProUGUI Text;
        
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
        
        public void SetInteractable(bool value)
        {
            Button.interactable = value;
        }
    }
}
