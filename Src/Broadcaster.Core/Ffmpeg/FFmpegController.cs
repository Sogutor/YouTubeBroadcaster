using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Core.Configuration;
using Broadcaster.Interfaces;

namespace Broadcaster.Core.Ffmpeg
{
    public class FFmpegController : IStreamingController
    {
        private readonly IFfmpegCommands _ffmpegCommands;
        private Process _process;


        public FFmpegController(IFfmpegCommands ffmpegCommands)
        {
            _ffmpegCommands = ffmpegCommands;
        }

        public void TestShow(FfmpegParams ffmpegParams) => StartFfmpeg(_ffmpegCommands.GetCommand(ffmpegParams));

        private void StartFfmpeg(string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = "/C" + arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                FileName = "cmd.exe"
            };
            _process = Process.Start(info);
        }

        public void StartStreaming(FfmpegParams ffmpegParams) => StartFfmpeg(_ffmpegCommands.GetArgumentsWithStream(ffmpegParams));

        public void StopStreaming()
        {
            if (_process != null)
            {
                try
                {
                 
                    _process.Kill();
                }
                catch (Exception ex)
                {


                }
                _process = null;
            }
            Process.GetProcessesByName("ffmpeg").ToList().ForEach(process => process.Kill());
        }
    }
}
