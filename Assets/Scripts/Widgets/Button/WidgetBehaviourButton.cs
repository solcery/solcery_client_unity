using Leopotam.EcsLite;
using Solcery.Models.Triggers;
using Solcery.Widgets.Data;
using UnityEngine;

namespace Solcery.Widgets
{
    public class WidgetBehaviourButton : WidgetBehaviourBase
    {
        public RectTransform RectTransform;
        public UnityEngine.UI.Button Button;

        public override void Init(EcsWorld ecsWorld, int entityId)
        {
            base.Init(ecsWorld, entityId);
            Button.onClick.AddListener(OnButtonClick);
        }

        public override void ApplyAnchor(Vector2 min, Vector2 max)
        {
            RectTransform.anchorMin = min;
            RectTransform.anchorMax = max;
        }


        public override void Clear()
        {
            Button.onClick.RemoveAllListeners();
            base.Clear();
        }

        private void OnButtonClick()
        {
            var triggerPool = EcsWorld.GetPool<ComponentApplyTrigger>();
            if (!triggerPool.Has(EntityId))
            {
                triggerPool.Add(EntityId).Type = TriggerTypes.OnClick;
            }
        }
    }
}
