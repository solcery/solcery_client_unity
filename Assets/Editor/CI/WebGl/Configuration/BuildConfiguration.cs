using System.Collections.Generic;

namespace Solcery.Editor.CI.WebGl.Configuration
{
    public abstract class BuildConfiguration
    {
        public IReadOnlyList<string> AddDefineSymbols => _addDefineSymbols;
        public IReadOnlyList<string> RemoveDefineSymbols => _removeDefineSymbols;
        
        private readonly IReadOnlyList<string> _addDefineSymbols;
        private readonly IReadOnlyList<string> _removeDefineSymbols;

        protected BuildConfiguration(IEnumerable<string> addDefineSymbols, IEnumerable<string> removeDefineSymbols)
        {
            _addDefineSymbols = new List<string>(addDefineSymbols);
            _removeDefineSymbols = new List<string>(removeDefineSymbols);
        }
    }
}