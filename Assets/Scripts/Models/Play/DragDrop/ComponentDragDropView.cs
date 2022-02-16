using Leopotam.EcsLite;
using Solcery.Widgets_new.Eclipse.DragDropSupport;

namespace Solcery.Models.Play.DragDrop
{
    public struct ComponentDragDropView : IEcsAutoReset<ComponentDragDropView>
    {
        public IDraggableWidget View;
        
        public void AutoReset(ref ComponentDragDropView c)
        {
            c.View = null;
        }
    }
}