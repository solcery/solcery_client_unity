using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Entities;
using Solcery.Models.Places;

namespace Solcery.Widgets
{
    public abstract class Widget
    {
        public abstract WidgetViewBase View { get; }
        private readonly List<Widget> _widgets = new List<Widget>();

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
                        var widget = AddInternalWidget(world, entityId, data);
                        if (widget != null)
                        {
                            _widgets.Add(widget);
                            world.GetPool<ComponentPlaceWidgetView>().Add(entityId).View = widget.View;
                        }
                    }
                }
            }
        }
        
        public void ClearInternalWidgets(EcsWorld world, int[] entityIds)
        {
            var widgetViewPool = world.GetPool<ComponentPlaceWidgetView>();
            foreach (var entityId in entityIds)
            {
                if (widgetViewPool.Has(entityId))
                {
                    widgetViewPool.Del(entityId);
                }
            }

            foreach (var widget in _widgets)
            {
                widget.ClearInternalView();
            }
            _widgets.Clear();
        }

        protected abstract Widget AddInternalWidget(EcsWorld world, int entityId, JObject data);
        protected abstract void ClearInternalView();
    }
}