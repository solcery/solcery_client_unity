#if UNITY_EDITOR
using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceEditorLocalSimulation
    {
        public event Action<JObject> EventOnUpdateGameState; 

        public void Init(JObject gameContent, JObject gameState);
        public void ApplyCommand(JObject command);
        public void ApplySimulatedGameState(JObject gameState);
        public void Update(float dt);
        public void Cleanup();
        public void Destroy();
    }
}
#endif