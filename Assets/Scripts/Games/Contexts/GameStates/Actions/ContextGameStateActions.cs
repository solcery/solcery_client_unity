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
        private readonly int _volume;

        public static ContextGameStateAction Create(int soundId, int volume)
        {
            return new ContextGameStateActionPlaySound(soundId, volume);
        }

        private ContextGameStateActionPlaySound(int soundId, int volume)
        {
            _soundId = soundId;
            _volume = volume;
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue((int)ContextGameStateActionTypes.PlaySound) }
            };

            var value = new JObject
            {
                { "sound_id", new JValue(_soundId) },
                { "volume", new JValue(_volume) }
            };
            result.Add("value", value);

            return result;
        }
    }

    public sealed class ContextGameStateActionPushAction : ContextGameStateAction
    {
        private readonly int _actionType;
        private readonly IReadOnlyDictionary<string, int> _values;
        
        public static ContextGameStateAction Create(int actionType, IReadOnlyDictionary<string, int> values)
        {
            return new ContextGameStateActionPushAction(actionType, values);
        }

        private ContextGameStateActionPushAction(int actionType, IReadOnlyDictionary<string, int> values)
        {
            _actionType = actionType;
            _values = new Dictionary<string, int>(values);
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue(_actionType) }
            };

            var value = new JObject();
            foreach (var val in _values)
            {
                value.Add(val.Key, new JValue(val.Value));
            }
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

            public static Actions CreateAction()
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
            _actions.Push(Actions.CreateAction());
        }

        void IContextGameStateActions.AddAction(ContextGameStateAction action)
        {
            if (_actions.Count <= 0)
            {
                _actions.Push(Actions.CreateAction());
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