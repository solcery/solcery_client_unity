using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Utils
{
    public static class ListUtils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            for (var i = 0; i < count; i++) 
            {
                var randomIndex = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = tmp;
            }        
        }
    }
}