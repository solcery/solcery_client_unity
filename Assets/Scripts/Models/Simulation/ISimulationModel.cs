using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.Services.Commands;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation
{
    public interface ISimulationModel
    {
        EcsWorld World { get; }

        void Init(IServiceLocalSimulationApplyGameState applyGameState, IServiceCommands serviceCommands,
            IServiceBricks serviceBricks, JObject gameContentJson, JObject initialGameState);
        void Update(float dt);
        void Destroy();
    }
}