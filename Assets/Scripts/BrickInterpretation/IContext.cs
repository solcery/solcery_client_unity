using System;

namespace Solcery.BrickInterpretation
{
    public interface IContext
    {
        public object Object { get; set; }
        public bool TryGetVar<T>(string key, out T result);
        public void SetVarForKey<T>(string key, T obj);
    }
}