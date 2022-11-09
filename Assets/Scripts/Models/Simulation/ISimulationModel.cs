#if UNITY_EDITOR || LOCAL_SIMULATION
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games;
using Solcery.Services.LocalSimulation;
using Solcery.Services.LocalSimulation.Commands;
using Solcery.Services.LocalSimulation.GameStates;

namespace Solcery.Models.Simulation
{
    public interface ISimulationModel
    {
        EcsWorld World { get; }

        void Init(IServiceLocalSimulationApplyGameStateNew applyGameState, IGame game, IServiceCommands serviceCommands, IServiceGameState serviceGameState);
        void Update(float dt);
        void Destroy();
    }
}
#endif