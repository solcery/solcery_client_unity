using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.Animation;
using Solcery.Games.States.New.Actions.Animation.Factory;
using Solcery.Games.States.New.Actions.Animation.Move;
using Solcery.Games.States.New.Actions.Animation.Rotate;
using Solcery.Games.States.New.Actions.Animation.Sequence;
using Solcery.Games.States.New.Actions.Animation.Visibility;
using Solcery.Games.States.New.Actions.PlaySound;
using Solcery.Games.States.New.Factory;
using Solcery.Games.States.New.States;
using Solcery.React;
using Solcery.Utils;

namespace Solcery.Games.States.New
{
    public sealed class UpdateStateQueue : IUpdateStateQueue
    {
        public bool IsPredictable { get; private set; }
        public UpdateState CurrentState { get; private set; }

        private readonly IUpdateStateFactory _updateStateFactory;
        private readonly Queue<UpdateState> _updateStates;

        private readonly IUpdateActionFactory _actionFactory;
        private readonly SortedDictionary<int, List<UpdateAction>> _actions;

        public static IUpdateStateQueue Create(IGame game)
        {
            return new UpdateStateQueue(game);
        }

        private UpdateStateQueue(IGame game)
        {
            _updateStates = new Queue<UpdateState>();
            _updateStateFactory = UpdateStateFactory.Create();
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.GameState, UpdateGameState.Create);
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.Delay, UpdatePauseState.Create);
            _updateStateFactory.RegistrationCreationFunc(ContextGameStateTypes.Timer, UpdateTimerState.Create);
            _updateStateFactory.Init();

            _actions = new SortedDictionary<int, List<UpdateAction>>();
            _actionFactory = UpdateActionFactory.Create();
            _actionFactory.RegistrationCreationFunc(ContextGameStateActionTypes.PlaySound, UpdateActionPlaySound.Create);
            _actionFactory.RegistrationCreationFunc(ContextGameStateActionTypes.Animation, UpdateActionAnimationFactory
                .Create(game)
                .RegistrationCreationFunc(UpdateActionAnimationTypes.Move, UpdateActionAnimationMove.Create)
                .RegistrationCreationFunc(UpdateActionAnimationTypes.Rotate, UpdateActionAnimationRotate.Create)
                .RegistrationCreationFunc(UpdateActionAnimationTypes.Visibility, UpdateActionAnimationVisibility.Create)
                .RegistrationCreationFunc(UpdateActionAnimationTypes.Sequence, UpdateActionAnimationSequence.Create)
                .CreateAction);
        }

        void IUpdateStateQueue.PushGameState(JObject gameState)
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

            if (gameState.TryGetValue("actions", out JArray actionArray))
            {
                foreach (var actionToken in actionArray)
                {
                    if (actionToken is JObject actionObject
                        && _actionFactory.TryGetActionFromJson(actionObject, out var action))
                    {
                        if (!_actions.ContainsKey(action.StateId))
                        {
                            _actions.Add(action.StateId, new List<UpdateAction>());
                        }
                        
                        _actions[action.StateId].Add(action);
                        //Debug.Log($"Add play sound {((UpdateActionPlaySound)action).SoundId} for state id {action.StateId}");
                    }
                }
            }
        }
        
        void IUpdateStateQueue.Update(int deltaTimeMsec)
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
                    if (_updateStates.Count == 0)
                    {
                        UnityToReact.Instance.CallOnGameStateConfirmed();
                    }
                }
            }
        }

        bool IUpdateStateQueue.TryGetActionForStateId(int stateId, out List<UpdateAction> actions)
        {
            return _actions.TryGetValue(stateId, out actions);
        }

        void IUpdateStateQueue.RemoveAllActionForStateId(int stateId)
        {
            _actions.Remove(stateId);
        }
    }
}