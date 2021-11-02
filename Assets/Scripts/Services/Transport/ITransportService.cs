using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.Transport
{
    public interface ITransportService
    {
        event Action<JObject> EventReceivingGameContent;
        event Action<JObject> EventReceivingGameState;
        void CallUnityLoaded();
        void SendCommand(JObject command);
    }
}