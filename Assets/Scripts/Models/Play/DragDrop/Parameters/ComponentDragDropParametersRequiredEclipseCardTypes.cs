using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Models.Shared.Objects.Eclipse;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public struct ComponentDragDropParametersRequiredEclipseCardTypes : IEcsAutoReset<ComponentDragDropParametersRequiredEclipseCardTypes>
    {
        public HashSet<EclipseCardTypes> RequiredEclipseCardTypes;
        
        public void AutoReset(ref ComponentDragDropParametersRequiredEclipseCardTypes c)
        {
            c.RequiredEclipseCardTypes ??= new HashSet<EclipseCardTypes>();             
            c.RequiredEclipseCardTypes.Clear();
        }
    }
}