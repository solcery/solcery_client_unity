using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;

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

            // Attrs
            {
                var array = new JArray();
                if (fullState.TryGetValue("attrs", out JArray attrsArray))
                {
                    foreach (var attrToken in attrsArray)
                    {
                        if (attrToken is JObject attrObject
                            && attrObject.TryGetValue("key", out string key)
                            && attrObject.TryGetValue("value", out int value))
                        {
                            if (ds.TryGetStateAttribute(key, out var newValue))
                            {
                                value = newValue;
                            }
                            
                            array.Add(new JObject
                            {
                                ["key"] = new JValue(key),
                                ["value"] = new JValue(value)
                            });
                        }
                    }
                }
                
                result.Add("attrs", array);
            }

            // Deleted objects
            var deletedObject = new HashSet<int>();
            {
                if (deltaState.TryGetValue("deleted_objects", out JArray array))
                {
                    foreach (var objectIdToken in array)
                    {
                        deletedObject.Add(objectIdToken.Value<int>());
                    }
                }
            }

            // Objects
            {
                var array = new JArray();
                if (fullState.TryGetValue("objects", out JArray objectsArray))
                {
                    foreach (var objectToken in objectsArray)
                    {
                        if (objectToken is JObject objectObject
                            && objectObject.TryGetValue("id", out int id)
                            && objectObject.TryGetValue("tplId", out int tplId)
                            && objectObject.TryGetValue("attrs", out JArray attrsArray)
                            && !deletedObject.Contains(id))
                        {
                            var obj = new JObject
                            {
                                {"id", new JValue(id)},
                                {"tplId", new JValue(tplId)}
                            };

                            var attrsArr = new JArray();
                            foreach (var attrToken in attrsArray)
                            {
                                if (attrToken is JObject attrObject
                                    && attrObject.TryGetValue("key", out string key)
                                    && attrObject.TryGetValue("value", out int value))
                                {
                                    if (ds.TryGetObjectAttribute(id, key, out var newValue))
                                    {
                                        value = newValue;
                                    }
                                    
                                    attrsArr.Add(new JObject
                                    {
                                        ["key"] = new JValue(key),
                                        ["value"] = new JValue(value)
                                    });
                                }
                            }
                            obj.Add("attrs", attrsArr);
                            
                            array.Add(obj);
                        }
                    }
                }

                result.Add("objects", array);
            }
            
            return result;
        }

        private readonly Dictionary<string, int> _stateAttrs;
        private readonly Dictionary<int, Dictionary<string, int>> _stateObjects;

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
                        && @object.TryGetValue("attrs", out JArray objectAttrsArray))
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
                                _states.Enqueue(GameState.Create(fullState));
                                break;
                            
                            case ContextGameStateTypes.Delay:
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