using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.DragDrop.Parameters
{
    public struct ComponentDragDropBrickOriginCondition : IEcsAutoReset<ComponentDragDropBrickOriginCondition>
    {
        public JObject ConditionBrick;

        public void AutoReset(ref ComponentDragDropBrickOriginCondition c)
        {
            c.ConditionBrick = null;
        }
    }
}