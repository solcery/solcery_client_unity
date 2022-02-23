using Leopotam.EcsLite;
using Solcery.Models.Shared.Commands.Datas;
using Solcery.Services.Commands;

namespace Solcery.Models.Shared.Commands
{
    public interface ISystemProcessCommands : IEcsRunSystem { }

    public sealed class SystemProcessCommands : ISystemProcessCommands
    {
        private readonly IServiceCommands _serviceCommands;
        
        public static ISystemProcessCommands Create(IServiceCommands serviceCommands)
        {
            return new SystemProcessCommands(serviceCommands);
        }

        private SystemProcessCommands(IServiceCommands serviceCommands)
        {
            _serviceCommands = serviceCommands;
        }
        
        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_serviceCommands.IsEmpty() 
                || !_serviceCommands.TryPopCommand(out var command))
            {
                return;
            }

            var world = systems.GetWorld();
            CommandDataFactory.CreateFromJson(command)?.ApplyCommandToWorld(world);
        }
    }
}