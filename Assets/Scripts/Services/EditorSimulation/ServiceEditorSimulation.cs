using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.EditorSimulation
{
    public sealed class ServiceEditorSimulation : IServiceEditorSimulation
    {
        public event Action<JObject> EventOnSendGameState;

        private readonly List<JObject> _commands;

        public static IServiceEditorSimulation Create()
        {
            return new ServiceEditorSimulation();
        }

        private ServiceEditorSimulation()
        {
            _commands = new List<JObject>();
        }

        void IServiceEditorSimulation.SendCommand(JObject command)
        {
            _commands.Add(command);
        }

        void IServiceEditorSimulation.Update(float dt)
        {
            if (_commands.Count <= 0)
            {
                return;
            }
            
            var commands = new JArray();
            foreach (var command in _commands)
            {
                commands.Add(command);
            }
            
            _commands.Clear();
            var gameState = new JObject { { "commands", commands } };
            EventOnSendGameState?.Invoke(gameState);
        }

        void IServiceEditorSimulation.Cleanup()
        {
            _commands.Clear();
        }
    }
}