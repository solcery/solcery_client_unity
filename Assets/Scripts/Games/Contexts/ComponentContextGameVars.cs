using System.Collections.Generic;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;

namespace Solcery.Games.Contexts
{
    internal class ComponentContextGameVars : IContextGameVars
    {
        private Dictionary<string, int> _vars;

        private void Set(string key, int value)
        {
            if (!_vars.ContainsKey(key))
            {
                _vars.Add(key, value);
                return;
            }
            
            _vars[key] = value;
        }

        private bool TryGet(string key, out int value)
        {
            if (!_vars.ContainsKey(key))
            {
                Set(key, 0);
            }
            
            return _vars.TryGetValue(key, out value);
        }

        public static IContextGameVars Create()
        {
            return new ComponentContextGameVars();
        }

        private ComponentContextGameVars()
        {
            _vars = new Dictionary<string, int>();
        }
        
        void IContextGameVars.Update(string varName, int varValue)
        {
            Set(varName, varValue);
        }

        bool IContextGameVars.TryGet(string varName, out int varValue)
        {
            return TryGet(varName, out varValue);
        }
    }
}