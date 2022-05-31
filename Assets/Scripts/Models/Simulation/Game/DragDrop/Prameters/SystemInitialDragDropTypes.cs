using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Utils;

namespace Solcery.Models.Simulation.Game.DragDrop.Prameters
{
    public interface ISystemInitialDragDropTypes : IEcsInitSystem
    {
    }
    
    public class SystemInitialDragDropTypes : ISystemInitialDragDropTypes
    {
        private JObject _gameContent;
        
        public static ISystemInitialDragDropTypes Create(JObject gameContent)
        {
            return new SystemInitialDragDropTypes(gameContent);
        }
        
        private SystemInitialDragDropTypes(JObject gameContent)
        {
            _gameContent = gameContent;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var tagPool = world.GetPool<ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var actionPool = world.GetPool<ComponentDragDropParametersAction>();
            
            if (_gameContent.TryGetValue("drag_n_drops", out JObject dndBaseObject)
                && dndBaseObject.TryGetValue("objects", out JArray dndArray))
            {
                foreach (var dndToken in dndArray)
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

            _gameContent = null;
        }
    }
}