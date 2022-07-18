using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Solcery.Editor.CI.Utils
{
    public static class DocketUtils
    {
        public static void DockerImageWebGlUp()
        {
            var script = Application.platform == RuntimePlatform.OSXEditor
                ? "docker_compose_build_and_up"
                : "docker_compose_build_and_up.cmd";
            RunExecutable(BuildSettings.DefaultPathDevelopWebGl + script);
        }

        public static void DockerImageWebGlWithCmsUp(string branch)
        {
            var args = new[] {branch};
            RunExecutable(BuildSettings.DefaultPathDevelopWebGlWithCms + "docker_compose_build_and_up", args);
        }

        private static void RunExecutable(string name, IEnumerable<string> args = null)
        {
            Debug.Log($"Execute \"{name}\"");
            var argsStr = BuildUtils.GetOutputPath(BuildSettings.DefaultPathDevelopCI) + name;
            if (args != null)
            {
                foreach (var arg in args)
                {
                    argsStr += $" {arg}";
                }
            }

            var fileName = "/bin/sh";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                fileName = Path.GetFullPath(argsStr);
                argsStr = Path.Combine(Path.GetDirectoryName(argsStr) ?? string.Empty, "docker-compose.yaml");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = argsStr
                }
            };
            
            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += OutputHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        
        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) 
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                Debug.Log(outLine.Data);
            }
        }
    }
}