#if UNITY_EDITOR || LOCAL_SIMULATION
using Solcery.BrickInterpretation.Runtime.Contexts.GameStates;

namespace Solcery.Services.LocalSimulation
{
    public interface IServiceLocalSimulationApplyGameStateNew
    {
        void ApplySimulatedGameStates(IContextGameStates gameStates);
    }
}
#endif