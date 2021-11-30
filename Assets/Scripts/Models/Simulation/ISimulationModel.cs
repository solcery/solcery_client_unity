using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation
{
    public interface ISimulationModel
    {
        EcsWorld World { get; }
        void Init(IServiceEditorLocalSimulation simulation, JObject gameContentJson, JObject initialGameState);
        void Update(float dt);
        void Destroy();
    }
}