#if UNITY_EDITOR
namespace Solcery.Editor.CI.Utils
{
    internal static class BuildArgs
    {
        internal const string OutputPath = "-outputPath";
        internal const string DevelopBuild = "-developBuild";
        internal const string DataCachingWebGL = "-dataCachingWebGL";
        internal const string CompressionWebGL = "-compressionWebGL";
        internal const string EmscriptenArgs = "-emscriptenArgs";
    }
}
#endif