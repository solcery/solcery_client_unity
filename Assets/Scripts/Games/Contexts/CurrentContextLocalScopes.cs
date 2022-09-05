using System.Collections.Generic;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes.Vars;

namespace Solcery.Games.Contexts
{
    sealed class CurrentContextLocalScopeVars : IContextLocalScopeVars
    {
        private readonly Dictionary<string, int> _variables;

        public static IContextLocalScopeVars Create()
        {
            return new CurrentContextLocalScopeVars();
        }
        
        private CurrentContextLocalScopeVars()
        {
            _variables = new Dictionary<string, int>();
        }
        
        void IContextLocalScopeVars.Update(string name, int value)
        {
            if (!_variables.ContainsKey(name))
            {
                _variables.Add(name, 0);
            }

            _variables[name] = value;
        }

        bool IContextLocalScopeVars.TryGet(string name, out int value)
        {
            if (!_variables.ContainsKey(name))
            {
                _variables.Add(name, 0);
            }

            return _variables.TryGetValue(name, out value);
        }
    }
    
    sealed class CurrentContextLocalScope : IContextLocalScope
    {
        public IContextLocalScopeVars Vars { get; }

        public static IContextLocalScope Create()
        {
            return new CurrentContextLocalScope();
        }

        private CurrentContextLocalScope()
        {
            Vars = CurrentContextLocalScopeVars.Create();
        }
    }
    
    public sealed class CurrentContextLocalScopes : IContextLocalScopes
    {
        private readonly Stack<IContextLocalScope> _localScopes;

        public static IContextLocalScopes Create()
        {
            return new CurrentContextLocalScopes();
        }
        
        private CurrentContextLocalScopes()
        {
            _localScopes = new Stack<IContextLocalScope>();
        }
        
        void IContextLocalScopes.Push()
        {
            _localScopes.Push(CurrentContextLocalScope.Create());
        }

        bool IContextLocalScopes.TryPeek(out IContextLocalScope localScope)
        {
            return _localScopes.TryPeek(out localScope);
        }

        IContextLocalScope IContextLocalScopes.Pop()
        {
            return _localScopes.Pop();
        }
    }
}