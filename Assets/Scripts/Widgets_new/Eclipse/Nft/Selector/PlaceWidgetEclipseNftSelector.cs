using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Eclipse.DragDropSupport;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Nft.Selector
{
    public sealed class PlaceWidgetEclipseNftSelector : PlaceWidget<PlaceWidgetEclipseNftSelectorLayout>, IApplyDragWidget, IApplyDropWidget
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey,
            JObject placeDataObject)
        {
            return new PlaceWidgetEclipseNftSelector(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetEclipseNftSelector(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject)
        {
        }

        public override void Update(EcsWorld world, bool isVisible, int[] entityIds)
        {
            throw new System.NotImplementedException();
        }

        public override PlaceWidgetLayout LayoutForObjectId(int objectId)
        {
            throw new System.NotImplementedException();
        }

        void IApplyDragWidget.OnDragWidget(IDraggableWidget dragWidget)
        {
            throw new System.NotImplementedException();
        }

        void IApplyDropWidget.OnDropWidget(IDraggableWidget dropWidget, Vector3 position)
        {
            throw new System.NotImplementedException();
        }
    }
}