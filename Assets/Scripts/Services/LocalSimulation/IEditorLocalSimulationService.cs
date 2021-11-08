#if UNITY_EDITOR
using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation
{
    public interface IEditorLocalSimulationService
    {
        public event Action<JObject> EventOnUpdateGameState; 

        public void Init(JObject gameState);
        public void ApplyCommand(JObject command);
        public void Cleanup();
        public void Destroy();
    }
}
#endif