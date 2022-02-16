using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport
{
    public interface IDraggableWidget
    {
        void OnDrag(RectTransform parent, Vector3 position);
        void OnMove(Vector3 position);
        void OnDrop(Vector3 position, IApplyDropWidget target = null);
    }
}