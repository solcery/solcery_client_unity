using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport
{
    public interface IDraggableWidget
    {
        public int ObjectId { get; }

        void OnDrag(RectTransform parent, Vector3 position);
        void OnMove(Vector3 position);
        void OnDrop(Vector3 position, IApplyDropWidget target = null);
    }
}