using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Container.Hands;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStack : PlaceWidgetHand<PlaceWidgetStackLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetStack(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetStack(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
        {
            var cardFaceVisible = CardFace != PlaceWidgetCardFace.Down;
            Layout.UpdateTextVisible(!cardFaceVisible);
            if (!cardFaceVisible)
            {
                Layout.UpdateText(entityIds.Length.ToString());
            }
            
            base.Update(world, isVisible, entityIds);
        }
    }
}