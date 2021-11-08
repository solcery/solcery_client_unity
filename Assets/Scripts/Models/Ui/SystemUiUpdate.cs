using Leopotam.EcsLite;

namespace Solcery.Models.Ui
{
    internal interface ISystemUiUpdate : IEcsRunSystem
    {
    }

    internal sealed class SystemUiUpdate : ISystemUiUpdate
    {
        public static ISystemUiUpdate Create()
        {
            return new SystemUiUpdate();
        }
        
        private SystemUiUpdate() { }
        
        public void Run(EcsSystems systems)
        {
        }
    }
}