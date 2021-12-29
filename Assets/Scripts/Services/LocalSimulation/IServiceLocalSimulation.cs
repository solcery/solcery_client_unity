#if UNITY_EDITOR || LOCAL_SIMULATION
using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceLocalSimulation
    {
        event Action<JObject> EventOnUpdateGameState; 

        void Init(JObject gameContent, JObject gameState);
        void PushCommand(JObject command);
        void Update(float dt);
        void Cleanup();
        void Destroy();
    }
}
#endif