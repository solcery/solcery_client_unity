using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using UnityEngine;

namespace Solcery.Tests.PlayMode.Contexts
{
    internal class TestLog : ILog
    {
        public static ILog Create()
        {
            return new TestLog();
        }
        
        private TestLog() { }
        
        public void Print(string message)
        {
            Debug.Log(message);
        }
    }
}