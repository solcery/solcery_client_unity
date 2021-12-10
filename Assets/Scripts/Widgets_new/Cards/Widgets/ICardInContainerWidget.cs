using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public interface ICardInContainerWidget
    {
        void UpdateParent(Transform parent);
        void UpdateCardFace(PlaceWidgetCardFace cardFace);
        void UpdateInteractable(bool interactable);
        void UpdateFromCardTypeData(int objectId, JObject data);
        void Cleanup();
        void Destroy();
    }
}