using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Solcery.Utils;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public sealed class ObjectsValue : IObjectsValue
    {
        public IReadOnlyList<IObjectValue> Objects => _objects;

        private readonly List<IObjectValue> _objects;

        public static IObjectsValue Create(JArray currentObjectsArray, JArray oldObjectsArray)
        {
            return new ObjectsValue(currentObjectsArray, oldObjectsArray);
        }

        private ObjectsValue(JArray currentObjectsArray, JArray oldObjectsArray)
        {
            _objects = new List<IObjectValue>();
            
            var objectsCount = Mathf.Max(currentObjectsArray?.Count ?? 0, oldObjectsArray?.Count ?? 0);
            var currentObjects = new Dictionary<int, JObject>();
            var oldObjects = new Dictionary<int, JObject>();
            var ids = new HashSet<int>();

            for (var objectIndex = 0; objectIndex < objectsCount; objectIndex++)
            {
                if (currentObjectsArray != null 
                    && currentObjectsArray.Count > objectIndex 
                    && currentObjectsArray[objectIndex] is JObject cao
                    && cao.TryGetValue("id", out int caid))
                {
                    if (!ids.Contains(caid))
                    {
                        ids.Add(caid);
                    }
                    
                    currentObjects.Add(caid, cao);
                }
                
                if (oldObjectsArray != null 
                    && oldObjectsArray.Count > objectIndex 
                    && oldObjectsArray[objectIndex] is JObject oao
                    && oao.TryGetValue("id", out int oaid))
                {
                    if (!ids.Contains(oaid))
                    {
                        ids.Add(oaid);
                    }
                    
                    oldObjects.Add(oaid, oao);
                }
            }

            foreach (var id in ids)
            {
                var currentObject = currentObjects.TryGetValue(id, out var co) ? co : null;
                var oldObject = oldObjects.TryGetValue(id, out var oo) ? oo : null;
                _objects.Add(ObjectValue.Create(currentObject, oldObject));
            }
        }
    }
}