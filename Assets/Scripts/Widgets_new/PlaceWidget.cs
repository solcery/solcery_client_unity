using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new
{
    public abstract class PlaceWidget
    {
        public abstract void Update(EcsWorld world, int[] entityIds);
        public abstract void Destroy();
    }

    public abstract class PlaceWidget<T> : PlaceWidget where T : PlaceWidgetLayout
    {
        private const float AnchorDivider = 10000.0f;
        
        protected T Layout;
        protected IGame Game;

        protected PlaceWidget(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            Game = game;
            
            if (Game.ServiceResource.TryGetWidgetPrefabForKey(prefabPathKey, out var go) 
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

                Layout.UpdateVisible(false);
            }
        }

        protected virtual void DestroyImpl() { }

        public override void Destroy()
        {
            DestroyImpl();
            
            if (Layout != null && Layout.gameObject != null)
            {
                Object.Destroy(Layout.gameObject);
            }

            Layout = null;
            Game = null;
        }
    }
}