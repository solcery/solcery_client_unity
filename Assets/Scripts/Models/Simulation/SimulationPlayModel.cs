using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation
{
    public sealed class SimulationPlayModel
    {
        private EcsWorld _world;

        public static SimulationPlayModel Create()
        {
            return new SimulationPlayModel();
        }

        private SimulationPlayModel() { }

        void Init(IServiceEditorLocalSimulation simulation, JObject gameContentJson, JObject initialGameState)
        {
            
        }

        void Update(float dt)
        {
            throw new System.NotImplementedException();
        }

        void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}