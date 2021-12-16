using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public interface ICardInContainerWidget
    {
        Vector3 WorldPosition { get; }
        int CardIndex { get; set; }

        void UpdateParent(Transform parent);
        void UpdateSiblingIndex(int siblingIndex);
        void Move(Vector3 from, Vector3 to, Action<ICardInContainerWidget> onMoveComplete);
        void UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation);
        void UpdateInteractable(bool interactable);
        void UpdateHighlighted(bool highlighted);
        void UpdateFromCardTypeData(int objectId, JObject data);
        void Cleanup();
        void Destroy();
    }
}