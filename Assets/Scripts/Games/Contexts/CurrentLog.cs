using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using UnityEngine;

namespace Solcery.Games.Contexts
{
    internal class CurrentLog : ILog
    {
        public static ILog Create()
        {
            return new CurrentLog();
        }
        
        private CurrentLog() { }
        
        public void Print(string message)
        {
            Debug.Log(message);
        }
    }
}