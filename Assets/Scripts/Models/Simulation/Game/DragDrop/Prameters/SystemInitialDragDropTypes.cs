using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.DragDrop.Parameters;
using Solcery.Services.GameContent;
using Solcery.Utils;

namespace Solcery.Models.Simulation.Game.DragDrop.Prameters
{
    public interface ISystemInitialDragDropTypes : IEcsInitSystem
    {
    }
    
    public class SystemInitialDragDropTypes : ISystemInitialDragDropTypes
    {
        private IServiceGameContent _serviceGameContent;
        
        public static ISystemInitialDragDropTypes Create(IServiceGameContent serviceGameContent)
        {
            return new SystemInitialDragDropTypes(serviceGameContent);
        }
        
        private SystemInitialDragDropTypes(IServiceGameContent serviceGameContent)
        {
            _serviceGameContent = serviceGameContent;
        }
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var tagPool = world.GetPool<ComponentDragDropParametersTag>();
            var idPool = world.GetPool<ComponentDragDropParametersId>();
            var actionPool = world.GetPool<ComponentDragDropParametersAction>();
            
            foreach (var dndToken in _serviceGameContent.DragDrop)
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

            _serviceGameContent = null;
        }
    }
}