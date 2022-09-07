using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates.Actions
{
    public abstract class ContextGameStateAction
    {
        public abstract JObject ToJson(int id, int stateId);
    }

    public sealed class ContextGameStateActionPlaySound : ContextGameStateAction
    {
        private readonly int _soundId;

        public static ContextGameStateAction Create(int soundId)
        {
            return new ContextGameStateActionPlaySound(soundId);
        }

        private ContextGameStateActionPlaySound(int soundId)
        {
            _soundId = soundId;
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue((int)ContextGameStateActionTypes.PlaySound) }
            };

            var value = new JObject { { "sound_id", new JValue(_soundId) } };
            result.Add("value", value);

            return result;
        }
    }
    
    public sealed class ContextGameStateActions : IContextGameStateActions
    {
        private sealed class Actions
        {
            private int _stateId;
            private readonly List<ContextGameStateAction> _actions;

            public static Actions Create()
            {
                return new Actions();
            }
            
            private Actions()
            {
                _actions = new List<ContextGameStateAction>();
            }

            public void AddAction(ContextGameStateAction action)
            {
                _actions.Add(action);
            }

            public void SetTargetStateId(int stateId)
            {
                _stateId = stateId;
            }

            public void ToJson(ref int id, JArray root)
            {
                foreach (var action in _actions)
                {
                    root.Add(action.ToJson(id, _stateId));
                    id++;
                }
            }
        }

        private readonly Stack<Actions> _actions;

        public static IContextGameStateActions Create()
        {
            return new ContextGameStateActions();
        }

        private ContextGameStateActions()
        {
            _actions = new Stack<Actions>();
        }

        void IContextGameStateActions.Push()
        {
            _actions.Push(Actions.Create());
        }

        void IContextGameStateActions.AddAction(ContextGameStateAction action)
        {
            if (_actions.Count <= 0)
            {
                _actions.Push(Actions.Create());
            }
            
            _actions.Peek().AddAction(action);
        }
        
        public void SetTargetStateId(int stateId)
        {
            if (_actions.Count > 0)
            {
                _actions.Peek().SetTargetStateId(stateId);
            }
        }

        JArray IContextGameStateActions.ToJson()
        {
            var result = new JArray();
            var id = 0;
            while (_actions.Count > 0)
            {
                var action = _actions.Pop();
                action.ToJson(ref id, result);
            }
            
            return result;
        }

        void IContextGameStateActions.Cleanup()
        {
            _actions.Clear();
        }
    }
}