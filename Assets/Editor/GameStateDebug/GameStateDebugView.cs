using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Solcery.GameStateDebug;
using Solcery.Utils;
using UnityEditor;

namespace Solcery.Editor.GameStateDebug
{
    [CustomEditor (typeof (GameStateDebugView))]
    public class GameStateDebugViewInspector : UnityEditor.Editor
    {
        public sealed class AttrDiff
        {
            public readonly string AttrName;
            public readonly int AttrValue;

            public static AttrDiff Create(string attrName, int attrValue)
            {
                return new AttrDiff(attrName, attrValue);
            }

            private AttrDiff(string attrName, int attrValue)
            {
                AttrName = attrName;
                AttrValue = attrValue;
            }
        }
        
        public sealed class ObjectDiff
        {
            public readonly int Id;
            public readonly List<AttrDiff> Attrs;

            public bool HasDiff => Attrs.Count > 0;

            public static ObjectDiff Create(int id)
            {
                return new ObjectDiff(id);
            }

            private ObjectDiff(int id)
            {
                Id = id;
                Attrs = new List<AttrDiff>();
            }

            public void AddAttr(string name, int value)
            {
                Attrs.Add(AttrDiff.Create(name, value));
            }
        }

        private int _hashCode = -1;
        private Dictionary<string, int> _attrs;
        private Dictionary<int, Dictionary<string, int>> _objects;
        private List<AttrDiff> _attrDiffs;
        private List<ObjectDiff> _objectDiffs;

        public override void OnInspectorGUI()
        {
            var gameState = ((GameStateDebugView) target).GameState;
            if (gameState == null)
            {
                DrawGameStateIsNull();
            }
            else
            {
                DrawGameState(gameState);
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawGameStateIsNull()
        {
            EditorGUILayout.LabelField("Game state is null!", EditorStyles.boldLabel);
        }

        private void DrawGameState(JObject gameState)
        {
            GetDiff(gameState);
            EditorGUILayout.LabelField("Game state attrs diff", EditorStyles.boldLabel);
            foreach (var attrDiff in _attrDiffs)
            {
                DrawAttr(attrDiff);
            }
            
            EditorGUILayout.LabelField("Game state objects diff", EditorStyles.boldLabel);
            foreach (var objectDiff in _objectDiffs)
            {
                DrawObject(objectDiff);
            }
        }

        private void DrawAttr(AttrDiff attrDiff)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{attrDiff.AttrName}: ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{attrDiff.AttrValue}");
            EditorGUILayout.EndHorizontal();
        }

        private void DrawObject(ObjectDiff objectDiff)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Id: {objectDiff.Id}", EditorStyles.boldLabel);
            foreach (var attr in objectDiff.Attrs)
            {
                DrawAttr(attr);
            }
            EditorGUILayout.EndVertical();
        }

        private void GetDiff(JObject gameState)
        {
            _attrDiffs ??= new List<AttrDiff>();
            _objectDiffs ??= new List<ObjectDiff>();
            
            var hashCode = gameState.ToString(Formatting.None).GetHashCode();
            if (_hashCode == -1)
            {
                PrepareHashes(gameState);
                _hashCode = hashCode;
                return;
            }

            if (_hashCode == hashCode)
            {
                return;
            }

            _hashCode = hashCode;
            _attrDiffs.Clear();
            var attrs = gameState.GetValue<JArray>("attrs");
            foreach (var attrToken in attrs)
            {
                if (attrToken is JObject attrObject 
                    && attrObject.TryGetValue("key", out string attrName)
                    && attrObject.TryGetValue("value", out int attrValue))
                {
                    if (_attrs.TryGetValue(attrName, out var oldAttrValue))
                    {
                        if (oldAttrValue != attrValue)
                        {
                            _attrDiffs.Add(AttrDiff.Create(attrName, attrValue));
                        }
                    }
                    else
                    {
                        _attrDiffs.Add(AttrDiff.Create(attrName, attrValue));
                    }
                }
            }
            
            _objectDiffs.Clear();
            var objects = gameState.GetValue<JArray>("objects");
            foreach (var objectToken in objects)
            {
                if (objectToken is JObject objectObject
                    && objectObject.TryGetValue("id", out int id))
                {
                    var objectDiff = ObjectDiff.Create(id);
                    var objectAttrArray = objectObject.GetValue<JArray>("attrs");
                    
                    if (_objects.TryGetValue(id, out var hashObject))
                    {
                        foreach (var objectAttrToken in objectAttrArray)
                        {
                            if (objectAttrToken is JObject objectAttrObject
                                && objectAttrObject.TryGetValue("key", out string attrName)
                                && objectAttrObject.TryGetValue("value", out int attrValue))
                            {
                                if (hashObject.TryGetValue(attrName, out var oldAttrValue))
                                {
                                    if (oldAttrValue != attrValue)
                                    {
                                        objectDiff.AddAttr(attrName, attrValue);
                                    }
                                }
                                else
                                {
                                    objectDiff.AddAttr(attrName, attrValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var objectAttrToken in objectAttrArray)
                        {
                            if (objectAttrToken is JObject objectAttrObject
                                && objectAttrObject.TryGetValue("key", out string attrName)
                                && objectAttrObject.TryGetValue("value", out int attrValue))
                            {
                                objectDiff.AddAttr(attrName, attrValue);
                            }
                        }
                    }

                    if (objectDiff.HasDiff)
                    {
                        _objectDiffs.Add(objectDiff);
                    }
                }
            }
            
            PrepareHashes(gameState);
        }

        private void PrepareHashes(JObject gameState)
        {
            _attrs ??= new Dictionary<string, int>();
            _attrs.Clear();
            
            var attrs = gameState.GetValue<JArray>("attrs");
            foreach (var attrToken in attrs)
            {
                if (attrToken is JObject attrObject
                    && attrObject.TryGetValue("key", out string attrName)
                    && attrObject.TryGetValue("value", out int attrValue))
                {
                    _attrs.Add(attrName, attrValue);
                }
            }

            _objects ??= new Dictionary<int, Dictionary<string, int>>();
            _objects.Clear();
            
            var objects = gameState.GetValue<JArray>("objects");
            foreach (var objectToken in objects)
            {
                if (objectToken is JObject objectObject
                    && objectObject.TryGetValue("id", out int id)
                    && objectObject.TryGetValue("attrs", out JArray objectAttrsArray))
                {
                    _objects.Add(id, new Dictionary<string, int>());
                    foreach (var objectAttrToken in objectAttrsArray)
                    {
                        if (objectAttrToken is JObject objectAttrObject
                            && objectAttrObject.TryGetValue("key", out string attrName)
                            && objectAttrObject.TryGetValue("value", out int attrValue))
                        {
                            _objects[id].Add(attrName, attrValue);
                        }
                    }
                }
            }
        }
    }
}