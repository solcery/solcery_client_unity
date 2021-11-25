using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Context
{
    public struct ComponentContextArgs : IEcsAutoReset<ComponentContextArgs>
    {
        private Stack<Dictionary<string, JToken>> _argsStack;

        public Dictionary<string, JToken> Pop()
        {
            return _argsStack.Pop();
        }

        public void Push(Dictionary<string, JToken> args)
        {
            _argsStack.Push(args);
        }
        
        public void AutoReset(ref ComponentContextArgs c)
        {
            c._argsStack ??= new Stack<Dictionary<string, JToken>>();
            c._argsStack.Clear();
        }
    }
}