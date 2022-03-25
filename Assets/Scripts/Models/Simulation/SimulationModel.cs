#if UNITY_EDITOR ||  LOCAL_SIMULATION
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Commands;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Models.Shared.Initial.Game.Content;
using Solcery.Models.Shared.Triggers.Apply;
using Solcery.Models.Simulation.Game;
using Solcery.Models.Simulation.Game.DragDrop.Prameters;
using Solcery.Models.Simulation.Game.State;
using Solcery.Services.Commands;
using Solcery.Services.LocalSimulation;

namespace Solcery.Models.Simulation
{
    public sealed class SimulationModel : ISimulationModel
    {
        EcsWorld ISimulationModel.World => _world;
        
        private EcsWorld _world;
        private EcsSystems _systems;

        public static SimulationModel Create()
        {
            return new SimulationModel();
        }

        private SimulationModel() { }

        void ISimulationModel.Init(IServiceLocalSimulationApplyGameState applyGameState, IGame game, IServiceCommands serviceCommands, JObject initialGameState)
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            
            // TODO: чистые инициализационные системы, вызываются один раз, по порядку (важно!)
            _systems.Add(SystemInitialComponentGame.Create(game));
            _systems.Add(SystemInitialDragDropTypes.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentPlaces.Create(game.GameContent));
            _systems.Add(SystemInitialGameContentEntityTypes.Create(game.GameContent));
            _systems.Add(SystemGameStateInitial.Create(initialGameState));
            _systems.Add(SystemInitialGameContentTooltips.Create(initialGameState));

            // Process commands
            _systems.Add(SystemProcessCommands.Create(serviceCommands));
            
            // Apply triggers
            // TODO: fix it!!!
            _systems.Add(SystemsTriggerApply.Create(game.ServiceBricks, applyGameState as IServiceLocalSimulationApplyGameStateNew));
            
            // Update static attributes
            _systems.Add(SystemStaticAttributesUpdate.Create());
            
            // Create game state
            _systems.Add(SystemGameStateCreate.Create(applyGameState));
            
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif

            _systems.Init();
        }

        void ISimulationModel.Update(float dt)
        {
            _systems?.Run();
        }

        void ISimulationModel.Destroy()
        {
            _systems?.Destroy();
            _systems = null;
            
            _world?.Destroy();
            _world = null;
        }
    }
}
#endif