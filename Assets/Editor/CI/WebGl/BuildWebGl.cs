using Solcery.Editor.CI.Utils;
using UnityEditor;

namespace Solcery.Editor.CI.WebGl
{
    public static class BuildWebGl
    {
        private static readonly string [] DevelopWebGlAddSymbols = 
        {
            "DEV"
        };
        private static readonly string [] ReleaseWebGlAddSymbols = { };
        
        private static readonly string [] DevelopWebGlRemoveSymbols = { };
        private static readonly string [] ReleaseWebGlRemoveSymbols =
        {
            "DEV"
        };
        
        
        [MenuItem("Build/WebGl/Develop")]
        public static void BuildDevelop()
        {
            BuildUtils.AddDefineSymbols(DevelopWebGlAddSymbols, DevelopWebGlRemoveSymbols);
            Build(BuildUtils.GetOutputPath(BuildSettings.DefaultOutputPathDevelopWebGl),
                BuildSettings.DevelopWebGlEmscriptenArgs, WebGLLinkerTarget.Wasm, WebGLCompressionFormat.Disabled,
                false, BuildOptions.Development);
        }

        [MenuItem("Build/WebGl/Release")]
        public static void BuildRelease()
        {
            BuildUtils.AddDefineSymbols(ReleaseWebGlAddSymbols, ReleaseWebGlRemoveSymbols);
            Build(BuildUtils.GetOutputPath(BuildSettings.DefaultOutputPathReleaseWebGl),
                BuildSettings.ReleaseWebGlEmscriptenArgs, WebGLLinkerTarget.Wasm, WebGLCompressionFormat.Brotli, false,
                BuildOptions.None);
        }
        
        public static string BuildWithCommandLine()
        {
            var isDevelopBuild = false;
            var buildOptions = BuildOptions.None;
            
            if (BuildUtils.TryGetArgOrDefault(BuildArgs.DevelopBuild, out _))
            {
                isDevelopBuild = true;
                buildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
            }

            var outputPath = BuildUtils.GetOutputPath(isDevelopBuild
                ? BuildSettings.DefaultOutputPathDevelopWebGl
                : BuildSettings.DefaultOutputPathReleaseWebGl);

            var linkerTarget = WebGLLinkerTarget.Wasm;

            var compressionFormat = BuildUtils.TryGetArgOrDefault(BuildArgs.CompressionWebGL, out _)
                ? WebGLCompressionFormat.Brotli
                : WebGLCompressionFormat.Disabled;

            var dataCaching = BuildUtils.TryGetArgOrDefault(BuildArgs.DataCachingWebGL, out _);

            if (isDevelopBuild)
            {
                BuildUtils.AddDefineSymbols(DevelopWebGlAddSymbols, DevelopWebGlRemoveSymbols);
            }
            else
            {
                BuildUtils.AddDefineSymbols(ReleaseWebGlAddSymbols, ReleaseWebGlRemoveSymbols);
            }

            BuildUtils.TryGetArgOrDefault(BuildArgs.EmscriptenArgs, out var emscriptenArgs,
                isDevelopBuild ? BuildSettings.DevelopWebGlEmscriptenArgs : BuildSettings.ReleaseWebGlEmscriptenArgs);
            
            return Build(outputPath, emscriptenArgs, linkerTarget, compressionFormat, dataCaching, buildOptions);
        }

        private static string Build(string outputPath, string emscriptenArgs, WebGLLinkerTarget linkerTarget, WebGLCompressionFormat compressionFormat, bool dataCaching, BuildOptions buildOptions)
        {
            BuildUtils.PrepareOutputDirectory(outputPath);
            PlayerSettings.WebGL.linkerTarget = linkerTarget;
            PlayerSettings.WebGL.compressionFormat = compressionFormat;
            PlayerSettings.WebGL.dataCaching = dataCaching;
            PlayerSettings.WebGL.emscriptenArgs = emscriptenArgs;
            AssetDatabase.Refresh();
            BuildPipeline.BuildPlayer(BuildUtils.GetScenePaths(), outputPath, BuildTarget.WebGL, buildOptions);
            return outputPath;
        }
    }
}