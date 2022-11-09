using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation.Commands
{
    public interface IServiceCommands
    {
        void PushCommand(JObject command);
        bool TryPopCommand(out JObject command);
        void ClearAllCommand();
        bool IsEmpty();
        int CountCommand();
        void Cleanup();
        void Destroy();
    }
}