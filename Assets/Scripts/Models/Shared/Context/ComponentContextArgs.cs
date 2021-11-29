using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Shared.Context
{
    public struct ComponentContextArgs : IEcsAutoReset<ComponentContextArgs>
    {
        private Stack<Dictionary<string, JObject>> _argsStack;

        public Dictionary<string, JObject> Pop()
        {
            return _argsStack.Pop();
        }

        public void Push(Dictionary<string, JObject> args)
        {
            _argsStack.Push(args);
        }
        
        public void AutoReset(ref ComponentContextArgs c)
        {
            c._argsStack ??= new Stack<Dictionary<string, JObject>>();
            c._argsStack.Clear();
        }
    }
}