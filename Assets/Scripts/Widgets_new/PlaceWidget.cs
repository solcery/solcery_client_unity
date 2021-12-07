using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Utils;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new
{
    public abstract class PlaceWidget
    {
        public abstract void Update(EcsWorld world, int[] entityIds);
    }

    public abstract class PlaceWidget<T> : PlaceWidget where T : PlaceWidgetLayout
    {
        private const float AnchorDivider = 10000.0f;
        
        protected T Layout;

        protected PlaceWidget(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, string prefabPathKey, JObject placeDataObject)
        {
            if (serviceResource.TryGetWidgetPrefabForKey(prefabPathKey, out var go) 
                && Object.Instantiate(go, widgetCanvas.GetUiCanvas()).TryGetComponent(typeof(T), out var component)
                && component is T layout)
            {
                Layout = layout;

                if (placeDataObject.TryGetValue("x1", out int x1) 
                    && placeDataObject.TryGetValue("x2", out int x2)
                    && placeDataObject.TryGetValue("y1", out int y1)
                    && placeDataObject.TryGetValue("y2", out int y2))
                {
                    Layout.UpdateAnchor(new Vector2(x1 / AnchorDivider, y1 / AnchorDivider),
                        new Vector2(x2 / AnchorDivider, y2 / AnchorDivider));
                }

                if (placeDataObject.TryGetValue("zOrder", out int orderZ))
                {
                    Layout.UpdateOrderZ(orderZ);
                }

                var alpha = placeDataObject.TryGetValue("alpha", out int a) ? a : 100;
                Layout.UpdateAlpha(alpha);

                var backgroundColor = placeDataObject.TryGetValue("bgColor", out string bgColor) ? bgColor : null;
                Layout.UpdateBackgroundColor(backgroundColor);

                Layout.UpdateVisible(placeDataObject.TryGetValue("visible", out bool visible) && visible);
            }
        }
    }
}