using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidget : ICardInContainerWidget
    {
        private IGame _game;
        private CardInContainerWidgetLayout _layout;

        public static ICardInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new CardInContainerWidget(game, prefab, poolTransform);
        }
        
        private CardInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<CardInContainerWidgetLayout>();
        }

        void ICardInContainerWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        void ICardInContainerWidget.UpdateCardFace(PlaceWidgetCardFace cardFace)
        {
            _layout.UpdateCardFace(cardFace);
        }

        void ICardInContainerWidget.UpdateInteractable(bool interactable)
        {
            _layout.UpdateInteractable(interactable);
        }

        void ICardInContainerWidget.UpdateFromCardTypeData(int objectId, JObject data)
        {
            if (data.TryGetValue("name", out string name))
            {
                _layout.UpdateName(name);
            }
            
            if (data.TryGetValue("description", out string description))
            {
                _layout.UpdateDescription(description);
            }

            if (data.TryGetValue("picture", out string picture) 
                && _game.ServiceResource.TryGetTextureForKey(picture, out var texture))
            {
                _layout.UpdateSprite(texture);
            }
            
            _layout.AddOnClickListener(() =>
            {
                var command = new JObject
                {
                    ["object_id"] = new JValue(objectId),
                    ["trigger_type"] = new JValue(TriggerTypes.OnClick),
                    ["trigger_target_entity_type"] = new JValue(TriggerTargetEntityTypes.Card)
                };
                _game.TransportService.SendCommand(command);
            });
        }
        
        void ICardInContainerWidget.Cleanup()
        {
            _layout.Cleanup();
        }

        void ICardInContainerWidget.Destroy()
        {
            _layout.Cleanup();
            Object.Destroy(_layout.gameObject);
            _layout = null;

            _game = null;
        }
    }
}