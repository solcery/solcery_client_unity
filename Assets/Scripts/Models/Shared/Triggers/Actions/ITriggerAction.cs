using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Triggers.Actions
{
    public interface ITriggerAction
    {
        void ApplyTrigger(JObject brick, int targetEntityId, EcsWorld world);
    }
}