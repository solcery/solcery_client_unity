using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Utils;

namespace Solcery.Models.Simulation.Game.DragDrop.Prameters
{
    public interface ISystemInitialDragDropTypes : IEcsInitSystem
    {
    }
    
    public class SystemInitialDragDropTypes : ISystemInitialDragDropTypes
    {
        public static ISystemInitialDragDropTypes Create()
        {
            return new SystemInitialDragDropTypes();
        }
        
        private SystemInitialDragDropTypes() { }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var game = systems.GetShared<IGame>();
            var world = systems.GetWorld();
            var tagPool = world.GetPool<ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var actionPool = world.GetPool<ComponentDragDropParametersAction>();
            
            foreach (var dndToken in game.ServiceGameContent.DragDrop)
            {
                if (dndToken is JObject dndObject)
                {
                    var entity = world.NewEntity();
                    tagPool.Add(entity);
                    idPool.Add(entity).Id = dndObject.GetValue<int>("id");

                    actionPool.Add(entity).Action = 
                        dndObject.TryGetValue("action_on_drop", out JObject action)
                            ? action
                            : new JObject();
                }
            }
        }
    }
}