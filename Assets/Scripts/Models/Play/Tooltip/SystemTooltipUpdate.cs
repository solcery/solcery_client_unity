using Leopotam.EcsLite;

namespace Solcery.Models.Play.Tooltip
{
    public interface ISystemTooltipUpdate : IEcsRunSystem
    {
    }
    
    public class SystemTooltipUpdate : ISystemTooltipUpdate
    {
        public static ISystemTooltipUpdate Create()
        {
            return new SystemTooltipUpdate();
        }
        
        private SystemTooltipUpdate() { }
        
        public void Run(EcsSystems systems)
        {
        }
    }
}