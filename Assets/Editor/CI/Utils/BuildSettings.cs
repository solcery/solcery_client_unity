namespace Solcery.Editor.CI.Utils
{
    internal static class BuildSettings
    {
        internal const string DefaultPathDevelopCI = "CI/";
        internal const string DefaultOutputPathDevelopWebGl = DefaultPathDevelopCI + "WebGl/";
        internal const string DefaultOutputPathReleaseWebGl = "Builds/WebGl/Release/";
        internal const string DevelopWebGlEmscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1 -s WASM_MEM_MAX=512MB -s TOTAL_MEMORY=32MB";
        internal const string ReleaseWebGlEmscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1 -s WASM_MEM_MAX=512MB -s TOTAL_MEMORY=32MB";
    }
}