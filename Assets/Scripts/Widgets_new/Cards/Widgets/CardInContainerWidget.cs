using System;
using Solcery.Games;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;
using Solcery.Widgets_new.Cards.Pools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Solcery.Widgets_new.Cards.Widgets
{
    public sealed class CardInContainerWidget : ICardInContainerWidget
    {
        Vector3 ICardInContainerWidget.WorldPosition => _layout.RectTransform.position;
        
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

        void IPoolingWidget.UpdateParent(Transform parent)
        {
            _layout.UpdateParent(parent);
        }

        public void SetLocalPosition(Vector3 localPosition)
        {
            _layout.RectTransform.localPosition = localPosition;
        }

        void ICardInContainerWidget.UpdateSiblingIndex(int siblingIndex)
        {
            _layout.UpdateSiblingIndex(siblingIndex);
        }

        void ICardInContainerWidget.MoveLocal(Vector3 fromWorld, Vector3 toLocal, Action<ICardInContainerWidget> onMoveComplete)
        {
            _layout.MoveLocal(fromWorld, toLocal, () => onMoveComplete.Invoke(this));
        }
        
        void ICardInContainerWidget.UpdateCardFace(PlaceWidgetCardFace cardFace, bool withAnimation)
        {
            _layout.UpdateCardFace(cardFace, withAnimation);
        }

        void ICardInContainerWidget.UpdateInteractable(bool interactable)
        {
            _layout.UpdateInteractable(interactable);
        }

        void ICardInContainerWidget.UpdateHighlighted(bool highlighted)
        {
            _layout.UpdateHighlighted(highlighted);
        }

        void ICardInContainerWidget.UpdateFromCardTypeData(int objectId, IItemType itemType)
        {
            if (itemType.TryGetValue(out var valueNameToken, "name", objectId))
            {
                _layout.UpdateName(valueNameToken.GetValue<string>());
            }
            
            if (itemType.TryGetValue(out var valueDescriptionToken, "description", objectId))
            {
                _layout.UpdateDescription(valueDescriptionToken.GetValue<string>());
            }

            if (itemType.TryGetValue(out var valuePictureToken, "picture", objectId) 
                && _game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
            {
                _layout.UpdateSprite(texture);
            }
            
            _layout.AddOnClickListener(() =>
            {
                var command = CommandOnLeftClickData.CreateFromParameters(objectId, TriggerTargetEntityTypes.Card);
                _game.TransportService.SendCommand(command.ToJson());
            });
        }
        
        void IPoolingWidget.Cleanup()
        {
            Cleanup();
            _layout.Cleanup();
        }

        void IPoolingWidget.Destroy()
        {
            Cleanup();
            _layout.Cleanup();
            Object.Destroy(_layout.gameObject);
            _layout = null;

            _game = null;
        }
        
        void IPoolingWidget.BackToPool()
        {
            _game.CardInContainerWidgetPool.Push(this);
        }

        private void Cleanup()
        {
        }
    }
}