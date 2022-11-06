using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.EditorSimulation
{
    public interface IServiceEditorSimulation
    {
        public event Action<JObject> EventOnSendGameState;
        void SendCommand(JObject command);
        void Update(float dt);
        void Cleanup();
    }
}