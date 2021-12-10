using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidget : ICardInContainerWidget
    {
        private IServiceResource _serviceResource;
        private CardInContainerWidgetLayout _layout;

        public static ICardInContainerWidget Create(IServiceResource serviceResource, GameObject prefab, Transform poolTransform)
        {
            return new CardInContainerWidget(serviceResource, prefab, poolTransform);
        }
        
        private CardInContainerWidget(IServiceResource serviceResource, GameObject prefab, Transform poolTransform)
        {
            _serviceResource = serviceResource;
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

        void ICardInContainerWidget.UpdateFromCardTypeData(JObject data)
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
                && _serviceResource.TryGetTextureForKey(picture, out var texture))
            {
                _layout.UpdateSprite(texture);
            }
        }
        
        void ICardInContainerWidget.Cleanup()
        {
            _layout.Cleanup();
        }

        void ICardInContainerWidget.Destroy()
        {
            Object.Destroy(_layout.gameObject);
            _layout = null;

            _serviceResource = null;
        }
    }
}