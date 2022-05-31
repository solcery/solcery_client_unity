namespace Solcery.Editor.CI.Utils
{
    internal static class BuildSettings
    {
        internal const string DefaultPathDevelopCI = "CI/";
        internal const string DefaultPathDevelopWebGl = "WebGl_image/";
        internal const string DefaultOutputPathWebGl = DefaultPathDevelopCI + DefaultPathDevelopWebGl +  "WebGl/";
        internal const string DefaultPathDevelopWebGlWithCms = "WebGl_with_cms_image/";
        internal const string DefaultOutputPathWebGlWithCms = DefaultPathDevelopCI + DefaultPathDevelopWebGlWithCms + "WebGl/";
        internal const string DefaultOutputPathReleaseWebGl = "Builds/WebGl/Release/";
        internal const string DevelopWebGlEmscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1 -s WASM_MEM_MAX=512MB -s TOTAL_MEMORY=32MB";
        internal const string ReleaseWebGlEmscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1 -s WASM_MEM_MAX=512MB -s TOTAL_MEMORY=32MB";
    }
}