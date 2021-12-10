using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets_new.Container.Stacks
{
    public sealed class PlaceWidgetStack : PlaceWidget<PlaceWidgetStackLayout>
    {
        private Dictionary<int, ICardInContainerWidget> _cards;

        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetStack(widgetCanvas, game, prefabPathKey, placeDataObject);
        }

        private PlaceWidgetStack(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _cards = new Dictionary<int, ICardInContainerWidget>();
        }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);

            RemoveCards(entityIds);
            
            var textVisible = CardFace == PlaceWidgetCardFace.Down;
            Layout.UpdateTextVisible(textVisible);
            
            if (entityIds.Length <= 0)
            {
                return;
            }

            if (textVisible)
            {
                Layout.UpdateText(entityIds.Length.ToString());
            }

            foreach (var entityId in entityIds)
            {
                if (_cards.ContainsKey(entityId))
                {
                    continue;
                }
                
                if (Game.PlaceWidgetFactory.CardInContainerPool.TryPop(out var cardInContainerWidget))
                {
                    cardInContainerWidget.UpdateParent(Layout.Content);
                    //cardInContainerWidget.UpdatePosition(Vector3.zero);
                    _cards.Add(entityId, cardInContainerWidget);
                }
            }
        }

        protected override void DestroyImpl()
        {
            foreach (var cardInContainerWidget in _cards)
            {
                cardInContainerWidget.Value.Destroy();
            }
            _cards.Clear();
            _cards = null;
        }

        private void RemoveCards(int[] entityIds)
        {
            foreach (var entityId in entityIds)
            {
                if (_cards.TryGetValue(entityId, out var cardInContainerWidget))
                {
                    _cards.Remove(entityId);
                    Game.PlaceWidgetFactory.CardInContainerPool.Push(cardInContainerWidget);
                }
            }
        }
    }
}