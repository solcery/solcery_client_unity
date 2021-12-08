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
        protected PlaceWidgetCardFace CardFace;

        protected PlaceWidget(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            Game = game;
            
            if (Game.ServiceResource.TryGetWidgetPrefabForKey(prefabPathKey, out var go) 
                && Object.Instantiate(go, widgetCanvas.GetUiCanvas()).TryGetComponent(typeof(T), out var component)
                && component is T layout)
            {
                Layout = layout;

                var placeId = placeDataObject.TryGetValue("placeId", out int pid) ? pid : -1;
                Layout.name = $"{placeId}_{Layout.name}";

                var x1 = placeDataObject.TryGetValue("x1", out int xt1) ? xt1 / AnchorDivider : 0f;
                var x2 = placeDataObject.TryGetValue("x2", out int xt2) ? xt2 / AnchorDivider : 0f;
                var y1 = placeDataObject.TryGetValue("y1", out int yt1) ? yt1 / AnchorDivider : 0f;
                var y2 = placeDataObject.TryGetValue("y2", out int yt2) ? yt2 / AnchorDivider : 0f;
                Layout.UpdateAnchor(new Vector2(x1, y1), new Vector2(x2, y2));

                var orderZ = placeDataObject.TryGetValue("zOrder", out int ordZ) ? ordZ : 0;
                Layout.UpdateOrderZ(orderZ);

                CardFace = placeDataObject.TryGetEnum("face", out PlaceWidgetCardFace res)
                    ? res
                    : PlaceWidgetCardFace.Up;

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