using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Places;
using Solcery.Models.Shared.Objects;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets
{
    public abstract class Widget : IWidget
    {
        protected readonly IWidgetCanvas WidgetCanvas;
        protected readonly IServiceResource ServiceResource;
        
        public abstract WidgetViewBase View { get; }

        protected Widget(IWidgetCanvas widgetCanvas, IServiceResource serviceResource)
        {
            WidgetCanvas = widgetCanvas;
            ServiceResource = serviceResource;
        }

        public void UpdateSubWidgets(EcsWorld world, int[] entityIds)
        {
            var filter = world.Filter<ComponentObjectTypes>().End();

            foreach (var uniqEntityId in filter)
            {
                ref var types = ref world.GetPool<ComponentObjectTypes>().Get(uniqEntityId);
                
                foreach (var entityId in entityIds)
                {
                    var typePool = world.GetPool<ComponentObjectType>();
                    if (typePool.Has(entityId))
                    {
                        if (types.Types.TryGetValue(typePool.Get(entityId).Type, out var data))
                        {
                            var subWidget = AddSubWidget(data);
                            if (subWidget != null)
                            {
                                world.GetPool<ComponentPlaceSubWidget>().Add(entityId).Widget = subWidget;
                            }
                        }
                    }
                }
                
                break;
            }
        }
        
        public void ClearSubWidgets(EcsWorld world, int[] entityIds)
        {
            var subWidgetPool = world.GetPool<ComponentPlaceSubWidget>();
            foreach (var entityId in entityIds)
            {
                if (subWidgetPool.Has(entityId))
                {
                    var subWidget = subWidgetPool.Get(entityId);
                    subWidget.Widget.ClearView();
                    subWidgetPool.Del(entityId);
                }
            }
        }
        
        protected virtual Widget AddSubWidget(JObject data)
        {
            return null;
        }

        public virtual WidgetViewBase CreateView()
        {
            return null;
        }

        public virtual void ClearView()
        {
        }
    }
}