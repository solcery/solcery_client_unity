using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Solcery.Editor.CI.Utils
{
    public static class DocketUtils
    {
        public static void DockerImageWebGlUp()
        {
            RunExecutable(BuildSettings.DefaultPathDevelopWebGl + "docker_compose_build_and_up");
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

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
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