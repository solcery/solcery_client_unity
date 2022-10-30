using Leopotam.EcsLite;
using Solcery.Services.Commands;

namespace Solcery.Models.Shared.Commands.New
{
    public interface ISystemProcessCommandNew : IEcsRunSystem { }

    public sealed class SystemProcessCommandNew : ISystemProcessCommandNew
    {
        private readonly IServiceCommands _serviceCommands;

        public static ISystemProcessCommandNew Create(IServiceCommands serviceCommands)
        {
            return new SystemProcessCommandNew(serviceCommands);
        }
        
        private SystemProcessCommandNew(IServiceCommands serviceCommands)
        {
            _serviceCommands = serviceCommands;
        }
        
        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            if (_serviceCommands.IsEmpty() 
                || !_serviceCommands.TryPopCommand(out var command))
            {
                return;
            }

            var world = systems.GetWorld();
            CommandDataFactoryNew.CreateFromJson(command)?.ApplyCommandToWorld(world);
        }
    }
}