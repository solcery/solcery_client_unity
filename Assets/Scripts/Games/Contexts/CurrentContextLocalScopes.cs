using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.LocalScopes.Vars;

namespace Solcery.Games.Contexts
{
    sealed class CurrentContextLocalScopeVars : IContextLocalScopeVars
    {
        IReadOnlyList<string> IContextLocalScopeVars.AllVarName => _varsName;
        
        private readonly Dictionary<string, int> _variables;
        private readonly List<string> _varsName;

        public static IContextLocalScopeVars Create()
        {
            return new CurrentContextLocalScopeVars();
        }
        
        private CurrentContextLocalScopeVars()
        {
            _variables = new Dictionary<string, int>();
            _varsName = new List<string>();
        }
        
        void IContextLocalScopeVars.Update(string name, int value)
        {
            if (!_variables.ContainsKey(name))
            {
                _variables.Add(name, 0);
                _varsName.Add(name);
            }

            _variables[name] = value;
        }

        bool IContextLocalScopeVars.TryGet(string name, out int value)
        {
            if (!_variables.ContainsKey(name))
            {
                _variables.Add(name, 0);
                _varsName.Add(name);
            }

            return _variables.TryGetValue(name, out value);
        }
    }

    sealed class CurrentContextLocalScopeArgs : IContextLocalScopeArgs
    {
        private readonly Dictionary<string, JObject> _arguments;

        public static IContextLocalScopeArgs Create()
        {
            return new CurrentContextLocalScopeArgs();
        }

        private CurrentContextLocalScopeArgs()
        {
            _arguments = new Dictionary<string, JObject>();
        }

        bool IContextLocalScopeArgs.Contains(string name)
        {
            return _arguments.ContainsKey(name);
        }

        void IContextLocalScopeArgs.Update(string name, JObject value)
        {
            if (!_arguments.ContainsKey(name))
            {
                _arguments.Add(name, value);
                return;
            }

            _arguments[name] = value;
        }

        bool IContextLocalScopeArgs.TryGetValue(string name, out JObject value)
        {
            return _arguments.TryGetValue(name, out value);
        }
    }
    
    sealed class CurrentContextLocalScope : IContextLocalScope
    {
        IContextLocalScopeVars IContextLocalScope.Vars => _vars;
        IContextLocalScopeArgs IContextLocalScope.Args => _args;

        private readonly IContextLocalScopeVars _vars;
        private readonly IContextLocalScopeArgs _args;

        public static IContextLocalScope Create()
        {
            return new CurrentContextLocalScope();
        }

        private CurrentContextLocalScope()
        {
            _vars = CurrentContextLocalScopeVars.Create();
            _args = CurrentContextLocalScopeArgs.Create();
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
        
        IContextLocalScope IContextLocalScopes.New()
        {
            var localScope = CurrentContextLocalScope.Create();
            _localScopes.Push(localScope);
            return localScope;
        }

        void IContextLocalScopes.Push(IContextLocalScope localScope)
        {
            _localScopes.Push(localScope);
        }

        IContextLocalScope IContextLocalScopes.Pop()
        {
            return _localScopes.Pop();
        }
    }
}