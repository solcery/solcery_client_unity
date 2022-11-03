using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.CardsContainer;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStack : PlaceWidgetEclipse<PlaceWidgetStackLayout>
    {
        public new static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetStack(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetStack(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            var cardFaceVisible = CardFace != PlaceWidgetCardFace.Down;
            Layout.UpdateTextVisible(!cardFaceVisible);
            if (!cardFaceVisible)
            {
                Layout.UpdateText(entityIds.Length.ToString());
            }
            
            base.Update(world, isVisible, isAvailable, entityIds);
        }
    }
}