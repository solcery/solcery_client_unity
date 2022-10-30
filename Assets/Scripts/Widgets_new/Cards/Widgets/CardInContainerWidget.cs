using System;
using Solcery.Games;
using Solcery.Models.Shared.Commands.New;
using Solcery.Models.Shared.Commands.New.OnClick;
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
            var displayedName = itemType.TryGetValue(out var valueNameToken, GameJsonKeys.CardDisplayedName, objectId)
                ? valueNameToken.GetValue<string>()
                : string.Empty;
            _layout.UpdateName(displayedName);

            var description = itemType.TryGetValue(out var valueDescriptionToken, GameJsonKeys.CardDescription, objectId)
                ? valueDescriptionToken.GetValue<string>()
                : string.Empty;
            _layout.UpdateDescription(description);

            if (itemType.TryGetValue(out var valuePictureToken, GameJsonKeys.Picture, objectId) 
                && _game.ServiceResource.TryGetTextureForKey(valuePictureToken.GetValue<string>(), out var texture))
            {
                _layout.UpdateSprite(texture);
            }
            
            _layout.AddOnClickListener(() =>
            {
                var command =
                    CommandOnLeftClickDataNew.Create(
                        _game.ServiceGameContent.CommandIdForType(CommandTypesNew.OnLeftClick), _game.PlayerIndex, objectId);
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