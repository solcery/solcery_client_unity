using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public struct ComponentDragDropParametersDestinations : IEcsAutoReset<ComponentDragDropParametersDestinations>
    {
        public HashSet<int> PlaceIds;
        
        public void AutoReset(ref ComponentDragDropParametersDestinations c)
        {
            c.PlaceIds ??= new HashSet<int>();
            c.PlaceIds.Clear();
        }
    }
}