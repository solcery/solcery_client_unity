using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceDragDropEntityId : IEcsAutoReset<ComponentPlaceDragDropEntityId>
    {
        public List<int> DragDropEntityIds;
        
        public void AutoReset(ref ComponentPlaceDragDropEntityId c)
        {
            c.DragDropEntityIds ??= new List<int>();
            c.DragDropEntityIds.Clear();
        }
    }
}