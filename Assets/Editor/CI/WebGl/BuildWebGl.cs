using Solcery.Editor.CI.Utils;
using Solcery.Editor.CI.WebGl.Configuration.Dev;
using Solcery.Editor.CI.WebGl.Configuration.Prod;
using UnityEditor;

namespace Solcery.Editor.CI.WebGl
{
    public static class BuildWebGl
    {
        public static void BuildDevelop()
        {
            BuildUtils.AddDefineSymbols(BuildConfigurationDev.Create());
            Build(BuildUtils.GetOutputPath(BuildSettings.DefaultOutputPathWebGl),
                BuildSettings.DevelopWebGlEmscriptenArgs, WebGLLinkerTarget.Wasm, WebGLCompressionFormat.Disabled,
                false, BuildOptions.Development);
            DocketUtils.DockerImageWebGlUp();
        }

        public static void BuildDevelopWithCms(string branch)
        {
            BuildUtils.AddDefineSymbols(BuildConfigurationDev.Create());
            Build(BuildUtils.GetOutputPath(BuildSettings.DefaultOutputPathWebGlWithCms),
                BuildSettings.DevelopWebGlEmscriptenArgs, WebGLLinkerTarget.Wasm, WebGLCompressionFormat.Disabled,
                false, BuildOptions.Development);
            DocketUtils.DockerImageWebGlWithCmsUp(branch);
        }

        public static void BuildDevelopWithLocalSimulation()
        {
            BuildUtils.AddDefineSymbols(BuildConfigurationDev.CreateWithLocalSimulation());
            Build(BuildUtils.GetOutputPath(BuildSettings.DefaultOutputPathWebGl),
                BuildSettings.DevelopWebGlEmscriptenArgs, WebGLLinkerTarget.Wasm, WebGLCompressionFormat.Disabled,
                false, BuildOptions.Development);
            DocketUtils.DockerImageWebGlUp();
        }

        public static void BuildRelease()
        {
            BuildUtils.AddDefineSymbols(BuildConfigurationProd.Create());
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
                ? BuildSettings.DefaultOutputPathWebGl
                : BuildSettings.DefaultOutputPathReleaseWebGl);

            var linkerTarget = WebGLLinkerTarget.Wasm;

            var compressionFormat = BuildUtils.TryGetArgOrDefault(BuildArgs.CompressionWebGL, out _)
                ? WebGLCompressionFormat.Brotli
                : WebGLCompressionFormat.Disabled;

            var dataCaching = BuildUtils.TryGetArgOrDefault(BuildArgs.DataCachingWebGL, out _);

            if (isDevelopBuild)
            {
                BuildUtils.AddDefineSymbols(BuildConfigurationDev.Create());
            }
            else
            {
                BuildUtils.AddDefineSymbols(BuildConfigurationProd.Create());
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