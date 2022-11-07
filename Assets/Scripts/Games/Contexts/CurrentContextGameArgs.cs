using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.Args;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextGameArgs : IContextGameArgs
    {
        private readonly Stack<Dictionary<string, JObject>> _argsStack;

        private Dictionary<string, JObject> Pop()
        {
            return _argsStack.Pop();
        }

        private void Push(Dictionary<string, JObject> args)
        {
            _argsStack.Push(args);
        }

        public static IContextGameArgs Create()
        {
            return new CurrentContextGameArgs();
        }
        
        private CurrentContextGameArgs()
        {
            _argsStack = new Stack<Dictionary<string, JObject>>();
        }
        
        Dictionary<string, JObject> IContextGameArgs.Pop()
        {
            return Pop();
        }

        void IContextGameArgs.Push(Dictionary<string, JObject> args)
        {
            Push(args);
        }
    }
}