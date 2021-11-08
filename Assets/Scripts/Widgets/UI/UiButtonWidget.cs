using Leopotam.EcsLite;
using Solcery.Models.Components;
using Solcery.Widgets.Attributes;
using UnityEngine.UI;

namespace Solcery.Widgets.UI
{
    public class UiButtonWidget : UiBaseWidget, IIntractable
    {
        public Button Button;
        public Image Image;
        public Text Text;

        public override void Init(EcsWorld world)
        {
            base.Init(world);
            Button.onClick.AddListener(OnButtonClick);
        }
        
        public override void Clear()
        {
            Button.onClick.RemoveAllListeners();
            base.Clear();
        }

        private void OnButtonClick()
        {
            var uiWidgetComponent = EcsWorld.GetPool<UiWidgetComponent>();
            var uiClickComponents = EcsWorld.GetPool<UiClickComponent>();
            var filter = EcsWorld.Filter<UiWidgetComponent>().End();
            foreach (var entity in filter)
            {
                ref var weapon = ref uiWidgetComponent.Get(entity);
                if (weapon.Widget == this)
                {
                    uiClickComponents.Add(entity);
                }
            }
        }

        public void SetIntractable(bool value)
        {
            Button.interactable = value;
        }
    }
}



