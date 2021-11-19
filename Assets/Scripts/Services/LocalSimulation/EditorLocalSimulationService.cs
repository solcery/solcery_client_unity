#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;

namespace Solcery.Services.LocalSimulation
{
    public sealed class EditorLocalSimulationService : IEditorLocalSimulationService
    {
        event Action<JObject> IEditorLocalSimulationService.EventOnUpdateGameState
        {
            add
            {
                if (!_listOnUpdateGameState.Contains(value))
                {
                    _listOnUpdateGameState.Add(value);
                }
            }
            
            remove => _listOnUpdateGameState.Remove(value);
        }

        private readonly List<Action<JObject>> _listOnUpdateGameState;
        private IServiceBricks _serviceBricks;

        public static IEditorLocalSimulationService Create(IServiceBricks serviceBricks)
        {
            return new EditorLocalSimulationService(serviceBricks);
        }
        
        private EditorLocalSimulationService(IServiceBricks serviceBricks)
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _serviceBricks = serviceBricks;
        }

        public void Init(JObject gameState)
        {
            CallAllActionWithParams(_listOnUpdateGameState, gameState);
        }

        public void ApplyCommand(JObject command)
        {
        }

        void IEditorLocalSimulationService.Cleanup()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _listOnUpdateGameState.Clear();
        }

        void IEditorLocalSimulationService.Destroy()
        {
            Cleanup();
        }
        
        private void CallAllActionWithParams(List<Action<JObject>> listActions, JObject @params)
        {
            foreach (var action in listActions)
            {
                action.Invoke(@params);
            }
        }
    }
}
#endif