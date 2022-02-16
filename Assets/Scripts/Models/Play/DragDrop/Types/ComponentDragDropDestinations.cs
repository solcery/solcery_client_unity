using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Play.DragDrop.Types
{
    public struct ComponentDragDropDestinations : IEcsAutoReset<ComponentDragDropDestinations>
    {
        public HashSet<int> PlaceIds;
        
        public void AutoReset(ref ComponentDragDropDestinations c)
        {
            c.PlaceIds ??= new HashSet<int>();
            c.PlaceIds.Clear();
        }
    }
}