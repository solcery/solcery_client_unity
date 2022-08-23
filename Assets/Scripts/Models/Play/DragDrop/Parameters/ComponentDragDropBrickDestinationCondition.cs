using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public struct ComponentDragDropBrickDestinationCondition : IEcsAutoReset<ComponentDragDropBrickDestinationCondition>
    {
        public JObject ConditionBrick;

        public void AutoReset(ref ComponentDragDropBrickDestinationCondition c)
        {
            c.ConditionBrick = null;
        }
    }
}