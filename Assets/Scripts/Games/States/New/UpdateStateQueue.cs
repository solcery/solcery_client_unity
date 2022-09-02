using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Games.States.New.Factory;
using Solcery.Games.States.New.States;
using Solcery.Utils;

namespace Solcery.Games.States.New
{
    public sealed class UpdateStateQueue : IUpdateStateQueue
    {
        public bool IsPredictable { get; private set; }
        public UpdateState CurrentState { get; private set; }

        private readonly IUpdateStateFactory _updateStateFactory;
        private readonly Queue<UpdateState> _updateStates;

        public static IUpdateStateQueue Create()
        {
            return new UpdateStateQueue();
        }

        private UpdateStateQueue()
        {
            _updateStates = new Queue<UpdateState>();
            _updateStateFactory = UpdateStateFactory.Create();
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.GameState, UpdateGameState.Create);
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.Delay, UpdatePauseState.Create);
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.Timer, UpdateTimerState.Create);
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.PlaySound, UpdatePlaySoundState.Create);
            _updateStateFactory.Init();
        }

        public void PushGameState(JObject gameState)
        {
            var isPredictable = gameState.TryGetValue("predict", out bool ip) && ip;
            if (gameState.TryGetValue("states", out JArray updateStateArray))
            {
                foreach (var updateStateToken in updateStateArray)
                {
                    if (updateStateToken is JObject updateStateData)
                    {
                        _updateStates.Enqueue(_updateStateFactory.ConstructFromJObject(updateStateData, isPredictable));
                    }
                }
            }
        }
        
        public void Update(int deltaTimeMsec)
        {
            CurrentState = null;

            if (_updateStates.Count > 0)
            {
                CurrentState = _updateStates.Peek();
                IsPredictable = CurrentState.IsPredictable;
                CurrentState.Update(deltaTimeMsec);
                if (CurrentState.CanRemove())
                {
                    _updateStates.Dequeue();
                }
            }
        }
    }
}