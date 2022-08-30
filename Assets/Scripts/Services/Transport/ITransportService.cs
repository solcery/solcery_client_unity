using Newtonsoft.Json.Linq;

namespace Solcery.Services.Transport
{
    public interface ITransportService
    {
        void CallUnityLoaded(JObject metadata);
        void SendCommand(JObject command);
        void Update(float dt);
        void Cleanup();
        void Destroy();
    }
}