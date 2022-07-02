using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary;
using Solcery.DebugViewers.StateQueues.Binary.Game;
using Solcery.DebugViewers.StateQueues.Binary.Pause;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.DebugViewers.StateQueues
{
    public sealed class DebugUpdateStateQueue : IDebugUpdateStateQueue
    {
        private const string PathDirectoryPattern = "usb";
        private const string PathPattern = "update_state_{0}.usb";

        private readonly List<Tuple<ContextGameStateTypes, int>> _files;
        private readonly string _pathPattern;

        public static IDebugUpdateStateQueue Create()
        {
            return new DebugUpdateStateQueue();
        }

        private DebugUpdateStateQueue()
        {
            _files = new List<Tuple<ContextGameStateTypes, int>>();
            var directory = Path.Combine(Application.persistentDataPath, PathDirectoryPattern);
            _pathPattern = Path.Combine(directory, PathPattern);

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            Directory.CreateDirectory(directory);
        }

        DebugUpdateStateBinary IDebugUpdateStateQueue.FirstUpdateState()
        {
            return null;
        }

        DebugUpdateStateBinary IDebugUpdateStateQueue.LastUpdateState()
        {
            return null;
        }

        DebugUpdateStateBinary IDebugUpdateStateQueue.NextUpdateState()
        {
            return null;
        }

        DebugUpdateStateBinary IDebugUpdateStateQueue.PreviewUpdateState()
        {
            return null;
        }
        
        void IDebugUpdateStateQueue.AddUpdateStates(JObject gameStateJson)
        {
            var stateArray = gameStateJson.GetValue<JArray>("states");
            foreach (var stateToken in stateArray)
            {
                if (stateToken is JObject stateObject)
                {
                    var stateType = stateObject.GetEnum<ContextGameStateTypes>("state_type");
                    var stateValue = stateObject.GetValue<JObject>("value");
                    var fileIndex = _files.Count;

                    switch (stateType)
                    {
                        case ContextGameStateTypes.Delay:
                            {
                                var state = DebugUpdatePauseStateBinary.Get();
                                state.InitFromJson(stateValue);
                                state.WriteForFile(string.Format(_pathPattern, fileIndex));
                                DebugUpdatePauseStateBinary.Release(state);
                                _files.Add(new Tuple<ContextGameStateTypes, int>(ContextGameStateTypes.Delay, fileIndex));
                            }
                            break;

                        case ContextGameStateTypes.GameState:
                            {
                                var state = DebugUpdateGameStateBinary.Get();
                                _lastFullGameState = CalculateFullGameState(stateValue);
                                state.InitFromJson(_lastFullGameState);
                                state.WriteForFile(string.Format(_pathPattern, fileIndex));
                                DebugUpdateGameStateBinary.Release(state);
                                _files.Add(new Tuple<ContextGameStateTypes, int>(ContextGameStateTypes.GameState, fileIndex));
                            }
                            break;
                    }
                }
            }
        }

        private JObject _lastFullGameState;

        private JObject CalculateFullGameState(JObject gameState)
        {
            var result = new JObject();

            var attrs = _lastFullGameState != null && _lastFullGameState.TryGetValue("attrs", out JArray rattrs)
                ? rattrs
                : null;
            var objects = _lastFullGameState != null && _lastFullGameState.TryGetValue("objects", out JArray robjects)
                ? robjects
                : null;
            AddAttrs(result, attrs, gameState.GetValue<JArray>("attrs"));
            AddRemovedObjectId(result, gameState);
            AddObjects(result, objects, gameState.GetValue<JArray>("objects"));
            
            return result;
        }

        private void AddAttrs(JObject target, JArray preview, JArray current)
        {
            JArray attrs;

            if (!target.TryGetValue("attrs", out attrs))
            {
                attrs = new JArray();
                target.Add("attrs", attrs);
            }

            var index = 0;
            var maxIndex = Mathf.Max(preview?.Count ?? 0, current?.Count ?? 0);
            var cache = new Dictionary<string, JObject>(attrs.Count);

            foreach (var attrToken in attrs)
            {
                if (attrToken is JObject attrObject
                    && attrObject.TryGetValue("key", out string key))
                {
                    cache.Add(key, attrObject);
                }
            }
            
            while (true)
            {
                if (maxIndex <= index)
                {
                    break;
                }

                {
                    if (preview != null
                        && preview.Count > index
                        && preview[index] is JObject po
                        && po.TryGetValue("key", out string key)
                        && po.TryGetValue("current", out int value))
                    {
                        if (cache.TryGetValue(key, out var obj))
                        {
                            if (obj.ContainsKey("preview"))
                            {
                                obj["preview"] = new JValue(value);
                            }
                            else
                            {
                                obj.Add("preview", new JValue(value));
                            }
                        }
                        else
                        {
                            obj = new JObject
                            {
                                {"key", new JValue(key)},
                                {"current", new JValue(value)},
                                {"preview", new JValue(value)}
                            };
                            
                            cache.Add(key, obj);
                            attrs.Add(obj);
                        }
                    }
                }

                {
                    if (current != null
                        && current.Count > index
                        && current[index] is JObject po
                        && po.TryGetValue("key", out string key)
                        && po.TryGetValue("value", out int value))
                    {
                        if (cache.TryGetValue(key, out var obj))
                        {
                            if (obj.ContainsKey("current"))
                            {
                                obj["current"] = new JValue(value);
                            }
                            else
                            {
                                obj.Add("current", new JValue(value));
                            }
                        }
                        else
                        {
                            obj = new JObject
                            {
                                {"key", new JValue(key)},
                                {"current", new JValue(value)},
                                {"preview", new JValue(value)}
                            };

                            cache.Add(key, obj);
                            attrs.Add(obj);
                        }
                    }
                }

                ++index;
            }
        }

        private void AddRemovedObjectId(JObject target, JObject current)
        {
            if (current.TryGetValue("deleted_objects", out JArray roia))
            {
                var dobj = new JArray();
                target.Add("deleted_objects", dobj);
                foreach (var token in roia)
                {
                    dobj.Add(new JValue(token.Value<int>()));
                }
            }
        }

        private void AddObjects(JObject target, JArray preview, JArray current)
        {
            var objects = new JArray();
            target.Add("objects", objects);

            var index = 0;
            var maxIndex = Mathf.Max(preview?.Count ?? 0, current?.Count ?? 0);
            var cache = new Dictionary<int, JObject>();
            while (true)
            {
                if (maxIndex <= index)
                {
                    break;
                }

                // preview
                {
                    if (preview != null
                        && preview.Count > index
                        && preview[index] is JObject po
                        && po.TryGetValue("id", out int id)
                        && po.TryGetValue("tplId", out int tplId)
                        && po.TryGetValue("attrs", out JArray attrs))
                    {
                        if (!cache.TryGetValue(id, out var obj))
                        {
                            obj = new JObject
                            {
                                {"id", new JValue(id)},
                                {"tplId", new JValue(tplId)}
                            };
                            objects.Add(obj);
                            cache.Add(id, obj);
                        }
                        
                        AddAttrs(obj, attrs, null);
                    }
                }
                
                // current
                {
                    if (current != null
                        && current.Count > index
                        && current[index] is JObject po
                        && po.TryGetValue("id", out int id)
                        && po.TryGetValue("tplId", out int tplId)
                        && po.TryGetValue("attrs", out JArray attrs))
                    {
                        if (!cache.TryGetValue(id, out var obj))
                        {
                            obj = new JObject
                            {
                                {"id", new JValue(id)},
                                {"tplId", new JValue(tplId)}
                            };
                            objects.Add(obj);
                            cache.Add(id, obj);
                        }
                        
                        AddAttrs(obj, null, attrs);
                    }
                }

                ++index;
            }
        }

        void IDebugUpdateStateQueue.Cleanup()
        {
            
        }
    }
}