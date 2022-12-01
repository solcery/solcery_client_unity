using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.CardsContainer;
using UnityEngine;

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

        protected override void UpdateCardsPosition()
        {
            foreach (var card in _cards)
            {
                var cardTransform = card.Value.Layout.transform;
                var cardIndex = cardTransform.GetSiblingIndex();
                var localPosition = cardTransform.localPosition;
                var offset = Layout.GetCardOffset(cardIndex);
                cardTransform.localPosition = new Vector3(offset.x, offset.y, localPosition.z);;
                card.Value.SetActive(cardIndex < Layout.Capacity || cardIndex == _cards.Count - 1);
            }
        }

        public override void Update(EcsWorld world, bool isVisible, bool isAvailable, int[] entityIds)
        {
            var showCount = CardFace == PlaceWidgetCardFace.Down && entityIds.Length >= Layout.Capacity;
            Layout.UpdateTextVisible(showCount);
            if (showCount)
            {
                Layout.UpdateText(entityIds.Length.ToString());
            }
            
            base.Update(world, isVisible, isAvailable, entityIds);
        }
    }
}