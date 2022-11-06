using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Services.Commands
{
    public sealed class ServiceCommands : IServiceCommands
    {
        private readonly Queue<JObject> _commands;
        private readonly HashSet<int> _consumableCommandIds;

        public static IServiceCommands Create()
        {
            return new ServiceCommands();
        }

        private ServiceCommands()
        {
            _commands = new Queue<JObject>();
            _consumableCommandIds = new HashSet<int>();
        }
        
        void IServiceCommands.PushCommand(JObject command)
        {
            if (command.TryGetValue("cid", out int cid) && _consumableCommandIds.Contains(cid))
            {
                return;
            }
            
            _commands.Enqueue(command);
        }

        bool IServiceCommands.TryPopCommand(out JObject command)
        {
            if (_commands.Count > 0)
            {
                command = _commands.Dequeue();
                
                if (command.TryGetValue("cid", out int cid) 
                    && !_consumableCommandIds.Contains(cid))
                {
                    _consumableCommandIds.Add(cid);
                }
                
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
            _consumableCommandIds.Clear();
        }
    }
}