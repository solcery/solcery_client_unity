using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Games.States
{
    public sealed class DeltaState
    {
        public static JObject CreateFullState(JObject fullState, JObject deltaState)
        {
            if (fullState == null)
            {
                return deltaState;
            }
            
            var ds = new DeltaState(deltaState);
            var result = new JObject();

            var stateAttrs = new JArray();
            if (fullState.TryGetValue("attrs", out JArray fsAttrsArray))
            {
                foreach (var fsAttrToken in fsAttrsArray)
                {
                    if (fsAttrToken is JObject fsAttrObject
                        && fsAttrObject.TryGetValue("key", out string name)
                        && fsAttrObject.TryGetValue("value", out int value))
                    {
                        if (ds.TryGetStateAttribute(name, out var newValue))
                        {
                            value = newValue;
                        }
                        
                        stateAttrs.Add(new JObject
                        {
                            {"key", new JValue(name)},
                            {"value", new JValue(value)},
                        });
                    }
                }
            }
            result.Add("attrs", stateAttrs);

            var objectsArray = new JArray();
            if (fullState.TryGetValue("objects", out JArray fsObjectsArray))
            {
                foreach (var fsObjectToken in fsObjectsArray)
                {
                    if (fsObjectToken is JObject fsObject 
                        && fsObject.TryGetValue("id", out int id)
                        && fsObject.TryGetValue("tplId", out int tplId)
                        && fsObject.TryGetValue("attrs", out JArray fsObjectAttrsArray))
                    {
                        var objectAttrs = new JArray();

                        foreach (var fsObjectAttrToken in fsObjectAttrsArray)
                        {
                            if (fsObjectAttrToken is JObject fsObjectAttrObject
                                && fsObjectAttrObject.TryGetValue("key", out string name)
                                && fsObjectAttrObject.TryGetValue("value", out int value))
                            {
                                if (ds.TryGetObjectAttribute(id, name, out var newValue))
                                {
                                    value = newValue;
                                }
                                
                                objectAttrs.Add(new JObject
                                {
                                    {"key", new JValue(name)},
                                    {"value", new JValue(value)},
                                });
                            }
                        }
                        
                        objectsArray.Add(new JObject
                        {
                            {"id", new JValue(id)},
                            {"tplId", new JValue(tplId)},
                            {"attrs", objectAttrs}
                        });
                    }
                }
            }
            result.Add("objects", objectsArray);
            
            return result;
        }

        private Dictionary<string, int> _stateAttrs;
        private Dictionary<int, Dictionary<string, int>> _stateObjects;

        private DeltaState(JObject deltaState)
        {
            _stateAttrs = new Dictionary<string, int>();
            if (deltaState.TryGetValue("attrs", out JArray stateAttrsArray))
            {
                foreach (var stateAttrToken in stateAttrsArray)
                {
                    if (stateAttrToken is JObject stateAttrObject
                        && stateAttrObject.TryGetValue("key", out string name)
                        && stateAttrObject.TryGetValue("value", out int value))
                    {
                        _stateAttrs.Add(name, value);
                    }
                }
            }

            _stateObjects = new Dictionary<int, Dictionary<string, int>>();
            if (deltaState.TryGetValue("objects", out JArray objectsArray))
            {
                foreach (var objectToken in objectsArray)
                {
                    if (objectToken is JObject @object
                        && @object.TryGetValue("id", out int id)
                        && @object.TryGetValue("attrs", out JArray objectAttrsArray)
                        && objectAttrsArray.Count > 0)
                    {
                        var attrs = new Dictionary<string, int>(objectAttrsArray.Count);
                        foreach (var objectAttrToken in objectAttrsArray)
                        {
                            if (objectAttrToken is JObject objectAttrObject
                                && objectAttrObject.TryGetValue("key", out string name)
                                && objectAttrObject.TryGetValue("value", out int value))
                            {
                                attrs.Add(name, value);
                            }
                        }
                        _stateObjects.Add(id, attrs);
                    }
                }
            }
        }

        private bool TryGetStateAttribute(string name, out int value)
        {
            return _stateAttrs.TryGetValue(name, out value);
        }

        private bool TryGetObjectAttribute(int objId, string name, out int value)
        {
            value = 0;
            return _stateObjects.TryGetValue(objId, out var attrs) 
                   && attrs.TryGetValue(name, out value);
        }
    }
    
    public sealed class GameStatePackage
    {
        public bool IsCompleted => _states.Count <= 0;
        
        private readonly Queue<State> _states;

        public static GameStatePackage Create(JObject gameStatePackage)
        {
            return new GameStatePackage(gameStatePackage);
        }
        
        private GameStatePackage(JObject gameStatePackage)
        {
            if (gameStatePackage.TryGetValue("states", out JArray stateArray))
            {
                _states = new Queue<State>(stateArray.Count);
                JObject fullState = null;

                foreach (var stateToken in stateArray)
                {
                    if (stateToken is JObject stateObject)
                    {
                        var type = stateObject.GetEnum<ContextGameStateTypes>("state_type");
                        var value = stateObject.GetValue<JObject>("value");

                        switch (type)
                        {
                            case ContextGameStateTypes.GameState:
                                fullState = DeltaState.CreateFullState(fullState, value);
                                Debug.Log($"Add full state {fullState.ToString(Formatting.None)}");
                                Debug.Log($"Delta state {value.ToString(Formatting.None)}");
                                _states.Enqueue(GameState.Create(fullState));
                                break;
                            
                            case ContextGameStateTypes.Delay:
                                Debug.Log($"Add pause state {value}");
                                _states.Enqueue(PauseState.Create(GetDelay(value)));
                                break;
                        }
                    }
                }
            }
        }

        private int GetDelay(JObject value)
        {
            return value.GetValue<int>("delay");
        }

        public bool TryGetGameState(int deltaTimeMsec, out JObject gameState)
        {
            var state = _states.Peek();

            switch (state)
            {
                case GameState gs:
                    _states.Dequeue();
                    gameState = gs.GameStateObject;
                    return true;
                
                case PauseState ps when ps.IsCompleted(deltaTimeMsec):
                    _states.Dequeue();
                    break;
            }

            gameState = null;
            return false;
        }
    }
}