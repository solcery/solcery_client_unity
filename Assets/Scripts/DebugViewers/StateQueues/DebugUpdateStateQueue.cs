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
        private const string PathDirectoryPattern = "ugs";
        private const string PathPattern = "update_state_{0}.json";

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

        public DebugUpdateStateBinary FirstUpdateState()
        {
            return null;
        }

        public DebugUpdateStateBinary LastUpdateState()
        {
            return null;
        }

        public DebugUpdateStateBinary NextUpdateState()
        {
            return null;
        }

        public DebugUpdateStateBinary PreviewUpdateState()
        {
            return null;
        }
        
        public void AddUpdateStates(JObject gameStateJson)
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

            if (_lastFullGameState == null)
            {
                AddAttrs(result, null, gameState.GetValue<JArray>("attrs"));
            }
            
            return null;
        }

        private void AddAttrs(JObject target, JArray preview, JArray current)
        {
            var attrs = new JArray();
            target.Add("attrs", attrs);

            var index = 0;
            var maxIndex = Mathf.Max(preview?.Count ?? 0, current?.Count ?? 0);
            while (true)
            {
                if (maxIndex <= index)
                {
                    break;
                }
                
                
                
                ++index;
            }
        }
    }
}