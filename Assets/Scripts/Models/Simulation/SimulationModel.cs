using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation
{
    public sealed class SimulationModel : ISimulationModel
    {
        private EcsWorld _world;
        EcsWorld ISimulationModel.World => _world;

        public static SimulationModel Create()
        {
            return new SimulationModel();
        }

        private SimulationModel() { }

        void ISimulationModel.Init(IServiceEditorLocalSimulation simulation, JObject gameContentJson, JObject initialGameState)
        {
            
        }

        void ISimulationModel.Update(float dt) { }

        void ISimulationModel.Destroy() { }
    }
}