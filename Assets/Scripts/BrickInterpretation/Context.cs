using System.Collections.Generic;

namespace Solcery.BrickInterpretation
{
    public class Context : IContext
    {
        public object Object { get; set; }

        private readonly Dictionary<string, object> _vars;

        public static IContext Create()
        {
            return new Context();
        }

        private Context()
        {
            Object = null;
            _vars = new Dictionary<string, object>();
        }

        bool IContext.TryGetVar<T>(string key, out T result)
        {
            result = default;
            if (!_vars.TryGetValue(key, out var res) || !(res is T resT)) return false;
            
            result = resT;
            return true;

        }

        void IContext.SetVarForKey<T>(string key, T obj)
        {
            if (!_vars.ContainsKey(key))
            {
                _vars.Add(key, obj);
                return;
            }

            _vars[key] = obj;
        }
    }
}