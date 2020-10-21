using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Extractt.Web.Utils
{
    public static class BashHelper
    {
        public async static Task<string> Bash(this string cmd)
        {
            var process = GetProcess(cmd);
            process.Start();
            var resultado = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            process.WaitForExit();
            return resultado;
        }

        private static Process GetProcess(string cmd)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return ExecuteOnWindows(cmd);
            else
                return ExecuteOnLinux(cmd);
        }

        private static Process ExecuteOnWindows(string cmd)
        {
            var arguments = cmd.Replace('\\', '/');
            var windows = Path.Combine(Directory.GetCurrentDirectory(), "dependencies-win").Replace('\\', '/');
            var newCmd = $"cd {windows}; ./{arguments};";
            return new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = newCmd,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
        }

        private static Process ExecuteOnLinux(string cmd)
        {
            var arguments = cmd.Replace('\\', '/');
            return new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{arguments}\" | tr -d \" | tr -d \'",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
        }
    }
}