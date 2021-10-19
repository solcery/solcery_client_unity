using System.Diagnostics;

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
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = BuildUtils.GetOutputPath(BuildSettings.DefaultPathDevelopCI) + name
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}