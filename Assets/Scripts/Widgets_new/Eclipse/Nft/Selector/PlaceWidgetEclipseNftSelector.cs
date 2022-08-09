using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
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
            //throw new System.NotImplementedException();
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