using System;
using System.Collections.Generic;

namespace Solcery.BrickInterpretation.Runtime.Utils
{
    internal static class ListUtils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rnd = new Random();
            var count = list.Count;
            for (var i = 0; i < count; i++) 
            {
                var randomIndex = rnd.Next(i, count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }        
        }

        public static T Pop<T>(this IList<T> list)
        {
            T result = list[0];
            list.RemoveAt(0);
            return result;
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list.Count <= 0;
        }
    }
}