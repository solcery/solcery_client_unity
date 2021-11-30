using Newtonsoft.Json.Linq;

namespace Solcery.Services.Commands
{
    public interface IServiceCommands
    {
        void PushCommand(JObject command);
        bool TryPopCommand(out JObject command);
        bool IsEmpty();
        int CountCommand();
        void Cleanup();
        void Destroy();
    }
}