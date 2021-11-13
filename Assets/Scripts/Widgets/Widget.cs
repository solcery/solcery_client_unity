using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Entities;
using Solcery.Models.Triggers;
using Solcery.Widgets.Attributes;
using Solcery.Widgets.Data;

namespace Solcery.Widgets
{
    public abstract class Widget
    {
        public abstract WidgetViewBase View { get; }

        public void UpdateWidget(EcsWorld world, int[] entityIds)
        {
            var typesFilter = world.Filter<ComponentEntityTypes>().End();
            ref var types = ref world.GetPool<ComponentEntityTypes>().Get(typesFilter.GetRawEntities()[0]);

            foreach (var entityId in entityIds)
            {
                var typePool = world.GetPool<ComponentEntityType>();
                if (typePool.Has(entityId))
                {
                    if (types.Types.TryGetValue(typePool.Get(entityId).Type, out var data))
                    {
                        AddInternalWidget(world, entityId, data);
                    }
                }
            }
        }

        public void ApplyAttributes(EcsWorld world, int entityId)
        {            
            var attributes = world.GetPool<ComponentEntityAttributes>();
            if (View == null || !attributes.Has(entityId))
            {
                return;
            }
            foreach (var attribute in attributes.Get(entityId).Attributes)
            {
                switch (attribute.Key)
                {
                    case "highlighted":
                        (View as IHighlighted)?.SetHighlighted(attribute.Value == 1);
                        break;
                }
            }        
        }

        protected abstract void AddInternalWidget(EcsWorld world, int entityId, JObject data);
        
        protected void OnClick(EcsWorld world, int entityId)
        {
            var triggerPool = world.GetPool<ComponentApplyTrigger>();
            if (!triggerPool.Has(entityId))
            {
                triggerPool.Add(entityId).Type = TriggerTypes.OnClick;
            }
        }
    }
}