using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Solcery.Utils;

namespace Solcery.DebugViewers.States.Games.Attrs
{
    public sealed class AttrsValue : IAttrsValue
    {
        IReadOnlyList<IAttrValue> IAttrsValue.Attrs => _attrs;

        private readonly List<IAttrValue> _attrs;

        public static IAttrsValue Create(JArray currentAttrArray, JArray oldAttrArray)
        {
            return new AttrsValue(currentAttrArray, oldAttrArray);
        }

        private AttrsValue(JArray currentAttrArray, JArray oldAttrArray)
        {
            _attrs = new List<IAttrValue>();

            var attrsCount = Mathf.Max(currentAttrArray?.Count ?? 0, oldAttrArray?.Count ?? 0);
            var currentAttrs = new Dictionary<string, int>();
            var oldAttrs = new Dictionary<string, int>();
            var keys = new HashSet<string>();

            for (var attrIndex = 0; attrIndex < attrsCount; attrIndex++)
            {
                if (currentAttrArray != null 
                    && currentAttrArray.Count > attrIndex 
                    && currentAttrArray[attrIndex] is JObject cao
                    && cao.TryGetValue("key", out string cakey) 
                    && cao.TryGetValue("value", out int cavalue))
                {
                    if (!keys.Contains(cakey))
                    {
                        keys.Add(cakey);
                    }
                    
                    currentAttrs.Add(cakey, cavalue);
                }
                
                if (oldAttrArray != null 
                    && oldAttrArray.Count > attrIndex 
                    && oldAttrArray[attrIndex] is JObject oao
                    && oao.TryGetValue("key", out string oakey) 
                    && oao.TryGetValue("value", out int oavalue))
                {
                    if (!keys.Contains(oakey))
                    {
                        keys.Add(oakey);
                    }
                    
                    oldAttrs.Add(oakey, oavalue);
                }
            }

            foreach (var key in keys)
            {
                var currentValue = currentAttrs.TryGetValue(key, out var cav) ? cav : int.MinValue;
                var oldValue = oldAttrs.TryGetValue(key, out var oav) ? oav : int.MinValue;
                _attrs.Add(AttrValue.Create(key, currentValue, oldValue));
            }
        }
    }
}