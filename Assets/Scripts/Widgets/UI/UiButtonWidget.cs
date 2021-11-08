using Leopotam.EcsLite;
using Solcery.Models.Components;
using Solcery.Widgets.Attributes;
using Solcery.Services.Widget;
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
            var uiWidgetComponents = EcsWorld.GetPool<UiWidgetComponent>();
            var filter = EcsWorld.Filter<UiWidgetComponent>().End();
            foreach (var entity in filter)
            {
                ref var uiWidgetComponent = ref uiWidgetComponents.Get(entity);
                if (uiWidgetComponent.Widget == this)
                {
                    ref var applyComponent = ref EcsWorld.GetPool<ApplyTriggerComponent>().Add(entity);
                    applyComponent.Type = TriggerTypes.OnClick;
                }
            }
        }

        public void SetIntractable(bool value)
        {
            Button.interactable = value;
        }
    }
}



