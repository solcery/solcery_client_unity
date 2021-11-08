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
        private IBrickService _brickService;

        public static IEditorLocalSimulationService Create(IBrickService brickService)
        {
            return new EditorLocalSimulationService(brickService);
        }
        
        private EditorLocalSimulationService(IBrickService brickService)
        {
            _listOnUpdateGameState = new List<Action<JObject>>();
            _brickService = brickService;
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