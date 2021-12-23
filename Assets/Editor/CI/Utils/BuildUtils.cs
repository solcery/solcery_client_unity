#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Solcery.Editor.CI.WebGl.Configuration;
using UnityEditor;

namespace Solcery.Editor.CI.Utils
{
    internal static class BuildUtils
    {
        internal static bool TryGetArgOrDefault(string argName, out string argValue, string defaultArgValue = "")
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != argName)
                {
                    continue;
                }
                
                argValue = args.Length > i + 1 ? args[i + 1] : defaultArgValue;
                return true;
            }

            argValue = defaultArgValue;
            return false;
        }
        
        internal static string GetOutputPath(string defaultPath)
        {
            TryGetArgOrDefault(BuildArgs.OutputPath, out var outputPath, defaultPath);
            outputPath = Path.GetFullPath(outputPath);
            return outputPath;
        }

        internal static void PrepareOutputDirectory(string outputPath)
        {
            var outputDirectoryPath = Path.GetDirectoryName(outputPath);
            
            if (outputDirectoryPath != null && Directory.Exists(outputDirectoryPath))
            {
                Directory.Delete(outputDirectoryPath, true);
            }

            if (outputDirectoryPath != null && !Directory.Exists(outputDirectoryPath))
            {
                Directory.CreateDirectory(outputDirectoryPath);
            }
        }
        
        internal static string[] GetScenePaths()
        {
            var scenes = new string[EditorBuildSettings.scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }

        internal static void AddDefineSymbols(BuildConfiguration configuration)
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines = allDefines.Except(configuration.RemoveDefineSymbols).ToList();
            allDefines.AddRange(configuration.AddDefineSymbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }
    }
}
#endif