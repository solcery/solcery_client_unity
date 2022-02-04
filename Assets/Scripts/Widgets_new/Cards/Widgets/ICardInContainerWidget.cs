using System;
using Newtonsoft.Json.Linq;
using Solcery.Widgets_new.Cards.Pools;
using UnityEngine;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public interface ICardInContainerWidget : IPoolingWidget
    {
        Vector3 WorldPosition { get; }
        void SetLocalPosition(Vector3 localPosition);
        void UpdateSiblingIndex(int siblingIndex);
        void MoveLocal(Vector3 fromWorld, Vector3 toLocal, Action<ICardInContainerWidget> onMoveComplete);
        void UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation);
        void UpdateInteractable(bool interactable);
        void UpdateHighlighted(bool highlighted);
        void UpdateFromCardTypeData(int objectId, JObject data);
    }
}