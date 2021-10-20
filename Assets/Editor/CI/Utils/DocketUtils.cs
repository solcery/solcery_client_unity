using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Solcery.Editor.CI.Utils
{
    public static class DocketUtils
    {
        public static void DockerImageUp()
        {
            RunExecutable("docker_compose_build_and_up");
        }

        public static void DocketImageDown()
        {
            RunExecutable("docker_compose_build_down");
        }

        private static void RunExecutable(string name)
        {
            Debug.Log($"Execute \"{name}\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = BuildUtils.GetOutputPath(BuildSettings.DefaultPathDevelopCI) + name
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