using System.Collections.Generic;

namespace Solcery.Editor.CI.WebGl.Configuration.Dev
{
    public sealed class BuildConfigurationDev : BuildConfiguration
    {
        public static BuildConfiguration Create()
        {
            var addDefineSymbols = new[] {"DEV"};
            var removeDefineSymbols = new[] {"PROD", "LOCAL_SIMULATION"};
            return new BuildConfigurationDev(addDefineSymbols, removeDefineSymbols);
        }

        public static BuildConfiguration CreateWithLocalSimulation()
        {
            var addDefineSymbols = new[] {"DEV", "LOCAL_SIMULATION"};
            var removeDefineSymbols = new[] {"PROD"};
            return new BuildConfigurationDev(addDefineSymbols, removeDefineSymbols);
        }

        private BuildConfigurationDev(IEnumerable<string> addDefineSymbols, IEnumerable<string> removeDefineSymbols) 
            : base(addDefineSymbols, removeDefineSymbols) { }
    }
}