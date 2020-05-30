using System.Diagnostics;
using System.Threading.Tasks;

namespace Extractt.Utils
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