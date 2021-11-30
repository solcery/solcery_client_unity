using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Triggers.Actions.Card
{
    public sealed class TriggerActionCardOnClick : ITriggerAction
    {
        void ITriggerAction.ApplyTrigger(JObject brick, int targetEntityId, EcsWorld world)
        {
        }
    }
}