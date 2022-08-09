using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Objects;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using Solcery.Widgets_new.Eclipse.Nft.Card;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Nft.Selector
{
    public sealed class PlaceWidgetEclipseNftSelector : PlaceWidget<PlaceWidgetEclipseNftSelectorLayout>, IApplyDragWidget, IApplyDropWidget
    {
        private readonly HashSet<int> _dropObjectId;
        private readonly Dictionary<int, IEclipseCardNftInContainerWidget> _cards;
        private readonly bool _defaultBlockRaycasts;
        
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipseNftSelector(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseNftSelector(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
            _dropObjectId = new HashSet<int>();
            _cards = new Dictionary<int, IEclipseCardNftInContainerWidget>();
            Layout.UpdateVisible(true);
            Layout.SetAnchor(TextAnchor.MiddleLeft);
            _defaultBlockRaycasts = Layout.BlockRaycasts;
        }

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
        {
            RemoveCards(world, entityIds);
            Layout.UpdateBlocksRaycasts(_defaultBlockRaycasts);
            Layout.gameObject.SetActive(isVisible);

            if (entityIds.Length <= 0 || !isVisible)
            {
                return;
            }
        }
        
        private void RemoveCards(EcsWorld world, int[] entityIds)
        {
            var objectIdPool = world.GetPool<ComponentObjectId>();
            var keys = _cards.Keys.ToList();

            foreach (var entityId in entityIds)
            {
                var objectId = objectIdPool.Get(entityId).Id;
                keys.Remove(objectId);
            }

            foreach (var key in keys)
            {
                var eid = _cards[key].AttachEntityId;
                if (eid >= 0)
                {
                    world.DelEntity(eid);
                }

                _cards[key].UpdateAttachEntityId();
                Game.EclipseCardNftInContainerWidgetPool.Push(_cards[key]);
                _cards.Remove(key);
            }
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }

        void IApplyDragWidget.OnDragWidget(IDraggableWidget dragWidget)
        {
            _cards.Remove(dragWidget.ObjectId);
        }

        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            throw new System.NotImplementedException();
        }
    }
}