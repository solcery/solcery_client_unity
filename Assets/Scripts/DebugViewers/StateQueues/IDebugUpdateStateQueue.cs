using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary;

namespace Solcery.DebugViewers.StateQueues
{
    public interface IDebugUpdateStateQueue
    {
        DebugUpdateStateBinary CurrentUpdateState();
        DebugUpdateStateBinary FirstUpdateState();
        DebugUpdateStateBinary LastUpdateState();
        DebugUpdateStateBinary NextUpdateState();
        DebugUpdateStateBinary PreviewUpdateState();
        void AddUpdateStates(JObject gameStateJson);
        void Cleanup();
    }
}