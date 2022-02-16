using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.DragDropSupport
{
    public interface IApplyDropWidget
    {
        void OnDropWidget(IDraggableWidget dropWidget, Vector3 position);
    }
}