#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

        public static IEditorLocalSimulationService Create()
        {
            return new EditorLocalSimulationService();
        }
        
        private EditorLocalSimulationService()
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
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