using System.Collections.Generic;

namespace Solcery.Editor.CI.WebGl.Configuration.Prod
{
    public sealed class BuildConfigurationProd : BuildConfiguration
    {
        public static BuildConfiguration Create()
        {
            var addDefineSymbols = new[] {"PROD"};
            var removeDefineSymbols = new[] {"DEV", "LOCAL_SIMULATION"};
            return new BuildConfigurationProd(addDefineSymbols, removeDefineSymbols);
        }

        public static BuildConfiguration CreateWithLocalSimulation()
        {
            var addDefineSymbols = new[] {"PROD", "LOCAL_SIMULATION"};
            var removeDefineSymbols = new[] {"DEV"};
            return new BuildConfigurationProd(addDefineSymbols, removeDefineSymbols);
        }
        
        private BuildConfigurationProd(IEnumerable<string> addDefineSymbols, IEnumerable<string> removeDefineSymbols) 
            : base(addDefineSymbols, removeDefineSymbols) { }
    }
}