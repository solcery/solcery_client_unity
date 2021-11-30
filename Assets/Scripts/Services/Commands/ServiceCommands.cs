using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.Commands
{
    public sealed class ServiceCommands : IServiceCommands
    {
        private readonly Queue<JObject> _commands;
        
        public static IServiceCommands Create()
        {
            return new ServiceCommands();
        }

        private ServiceCommands()
        {
            _commands = new Queue<JObject>();
        }
        
        void IServiceCommands.PushCommand(JObject command)
        {
            _commands.Enqueue(command);
        }

        bool IServiceCommands.TryPopCommand(out JObject command)
        {
            if (_commands.Count > 0)
            {
                command = _commands.Dequeue();
                return true;
            }

            command = null;
            return false;
        }

        bool IServiceCommands.IsEmpty()
        {
            return _commands.Count <= 0;
        }

        int IServiceCommands.CountCommand()
        {
            return _commands.Count;
        }

        void IServiceCommands.Cleanup()
        {
            Cleanup();
        }

        void IServiceCommands.Destroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _commands.Clear();
        }
    }
}