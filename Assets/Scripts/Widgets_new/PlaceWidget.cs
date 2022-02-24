using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.StaticOrderZ;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;

namespace Solcery.Widgets_new
{
    public abstract class PlaceWidget
    {

        public static void RefreshPlaceWidgetOrderZ(Transform widgetParentTransform)
        {
            var staticOrderZLayoutCount = StaticOrderZLayout.StaticOrderZCount;

            var placeWidgetLayoutArray = widgetParentTransform.GetComponentsInChildren<PlaceWidgetLayout>().ToList();
            placeWidgetLayoutArray = placeWidgetLayoutArray.OrderBy(o=>o.OrderZ).ToList();

            foreach (var placeWidgetLayout in placeWidgetLayoutArray)
            {
                placeWidgetLayout.UpdateSiblingIndex(staticOrderZLayoutCount +
                                                     placeWidgetLayoutArray.IndexOf(placeWidgetLayout));
            }
        }
        
        public abstract void Update(EcsWorld world, int[] entityIds);
        public abstract void Destroy();
        public abstract Vector2 GetPosition();
        public abstract PlaceWidgetCardFace GetPlaceWidgetCardFace();
        public abstract int GetDragDropId();
        public abstract void UpdatePlaceId(int placeId);
        public abstract void UpdateLinkedEntityId(int linkedEntityId);
    }

    public abstract class PlaceWidget<T> : PlaceWidget where T : PlaceWidgetLayout
    {
        private const float AnchorDivider = 10000.0f;
        
        protected T Layout;
        protected IGame Game;
        protected readonly PlaceWidgetCardFace CardFace;
        protected readonly bool InteractableForActiveLocalPlayer;
        protected readonly int PlaceId;
        protected readonly int DragDropId;

        protected PlaceWidget(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            Game = game;

            if (Game.ServiceResource.TryGetWidgetPrefabForKey(prefabPathKey, out var go)
                && Object.Instantiate(go, widgetCanvas.GetUiCanvas()).TryGetComponent(typeof(T), out var component)
                && component is T layout)
            {
                Layout = layout;

                PlaceId = placeDataObject.TryGetValue("placeId", out int pid) ? pid : -1;
                DragDropId = placeDataObject.TryGetValue("drag_n_drop", out int dnd) ? dnd : -1;

                var x1 = placeDataObject.TryGetValue("x1", out int xt1) ? xt1 / AnchorDivider : 0f;
                var x2 = placeDataObject.TryGetValue("x2", out int xt2) ? xt2 / AnchorDivider : 0f;
                var y1 = placeDataObject.TryGetValue("y1", out int yt1) ? yt1 / AnchorDivider : 0f;
                var y2 = placeDataObject.TryGetValue("y2", out int yt2) ? yt2 / AnchorDivider : 0f;
                Layout.UpdateAnchor(new Vector2(x1, y1), new Vector2(x2, y2));

                var orderZ = placeDataObject.TryGetValue("zOrder", out int ordZ) ? ordZ : 0;
                Layout.UpdateOrderZ(orderZ);

                Layout.name = $"{PlaceId}_{orderZ}_{Layout.name}";

                CardFace = placeDataObject.TryGetEnum("face", out PlaceWidgetCardFace res)
                    ? res
                    : PlaceWidgetCardFace.Up;

                InteractableForActiveLocalPlayer =
                    placeDataObject.TryGetValue("interactableForActiveLocalPlayer", out bool ifalp) && ifalp;

                var alpha = placeDataObject.TryGetValue("alpha", out int a) ? a : 100;
                Layout.UpdateAlpha(alpha);

                var backgroundColor = placeDataObject.TryGetValue("bgColor", out string bgColor) ? bgColor : null;
                Layout.UpdateBackgroundColor(backgroundColor);

                Layout.UpdateCaption(placeDataObject.TryGetValue("caption", out string caption) ? caption : null);
                Layout.UpdateOutOfBorder(placeDataObject.TryGetValue("frame", out bool withBorders) && withBorders);

                if (placeDataObject.TryGetValue("tooltip_id", out int tooltipId))
                {
                    var tooltipBehavior = Layout.gameObject.AddComponent<RectTransformTooltipBehaviour>();
                    tooltipBehavior.SetTooltipId(tooltipId);
                }

                Layout.UpdateVisible(false);
                Layout.UpdatePlaceWidget(this);
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

        public override Vector2 GetPosition()
        {
            return Layout.transform.position;
        }
        
        public override PlaceWidgetCardFace GetPlaceWidgetCardFace()
        {
            return CardFace;
        }

        public override int GetDragDropId()
        {
            return DragDropId;
        }

        public override void UpdatePlaceId(int placeId)
        {
            Layout.UpdatePlaceId(placeId);
        }

        public override void UpdateLinkedEntityId(int linkedEntityId)
        {
            Layout.UpdateLinkedEntityId(linkedEntityId);
        }
    }
}