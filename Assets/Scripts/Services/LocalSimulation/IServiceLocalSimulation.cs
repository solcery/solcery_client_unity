#if UNITY_EDITOR || LOCAL_SIMULATION
using System;
using Newtonsoft.Json.Linq;
using Solcery.Games;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceLocalSimulation
    {
        event Action<JObject> EventOnUpdateGameState; 

        void Init(IGame game, JObject gameState);
        void PushCommand(JObject command);
        void Update(float dt);
        void Cleanup();
        void Destroy();
    }
}
#endif