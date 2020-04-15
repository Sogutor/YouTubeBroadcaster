using System;
using System.Collections.Generic;

namespace Broadcaster.Core.Ffmpeg
{
    public class FfmpegCommands : IFfmpegCommands
    {
        private const string localStreamOut = " -f mpegts tcp://127.0.0.1:2000";
        private readonly Dictionary<ShowType, Func<FfmpegParams, string>> _argumentsDictionary;
        private readonly Dictionary<ShowType, Func<FfmpegParams, string>> _argumentsStreamDictionary;
        public FfmpegCommands()
        {
            _argumentsDictionary = new Dictionary<ShowType, Func<FfmpegParams, string>>
            {
                [ShowType.WebCam] = ffmpegParams => $"ffmpeg -f dshow -i video=\"{ffmpegParams.VideoDeviceName}\" -movflags +faststart {localStreamOut}",
                [ShowType.WebCamH264] = ffmpegParams => $"ffmpeg -f dshow -i video=\"{ffmpegParams.VideoDeviceName}\" -movflags +faststart {localStreamOut}",
                [ShowType.WebCam43] = ffmpegParams => $"ffmpeg -f dshow -i video=\"{ffmpegParams.VideoDeviceName}\" -movflags +faststart {localStreamOut}",
                [ShowType.Desktop] = ffmpegParams => $"ffmpeg -f gdigrab -framerate 60 -i desktop -crf 0 -pix_fmt yuv444p -preset ultrafast -movflags +faststart {localStreamOut}",
                [ShowType.Document] = ffmpegParams => $"ffmpeg -f gdigrab -framerate 60 -i title=\"{ffmpegParams.WindowName}\" -crf 0 -pix_fmt yuv444p -preset ultrafast -movflags +faststart {localStreamOut}"
            };
            _argumentsStreamDictionary = new Dictionary<ShowType, Func<FfmpegParams, string>>
            {
                [ShowType.WebCamH264] = ffmpegParams => $"ffmpeg -rtbufsize 1500M -f dshow -framerate 30 -i video=\"{ffmpegParams.VideoDeviceName}\":audio=\"{ffmpegParams.AudioDeviceName}\" -pix_fmt yuv420p -s {ffmpegParams.Resolution.ToString()} " +
                                                    "-vcodec libx264  -threads 8 -thread_type slice  -preset superfast -b:v 4000k -tune zerolatency -crf 23  -g 60 " +
                                                    $"-acodec libmp3lame -b:a 128k -ar 44100 -ac 2 -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}",
                [ShowType.WebCam43]= ffmpegParams=> $"ffmpeg -rtbufsize 1500M -f dshow -framerate 30 -i video=\"{ffmpegParams.VideoDeviceName}\":audio=\"{ffmpegParams.AudioDeviceName}\"  -pix_fmt yuv420p -vf scale=1024:720,pad=1280:720:128:0  -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}",
                [ShowType.WebCam] = ffmpegParams => $"ffmpeg -rtbufsize 1500M -f dshow -framerate 30 -i video=\"{ffmpegParams.VideoDeviceName}\":audio=\"{ffmpegParams.AudioDeviceName}\"  -pix_fmt yuv420p -s {ffmpegParams.Resolution.ToString()}  -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}",
                [ShowType.Desktop] = ffmpegParams => $"ffmpeg -y -f dshow -i audio=\"{ffmpegParams.AudioDeviceName}\" -f gdigrab -framerate 60 -i desktop  -pix_fmt yuv420p -s {ffmpegParams.Resolution.ToString()} " +
                                                     $"-acodec libmp3lame -b:a 128k -ar 44100 -ac 2 -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}",
                [ShowType.Document] = ffmpegParams => $"ffmpeg -y -f dshow -i audio=\"{ffmpegParams.AudioDeviceName}\" -f gdigrab -framerate 60 -i title=\"{ffmpegParams.WindowName}\" -pix_fmt yuv420p -s {ffmpegParams.Resolution.ToString()} " +
                                                     $"-acodec libmp3lame -b:a 128k -ar 44100 -ac 2 -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}"
            };
        }
        //[ShowType.Desktop] = ffmpegParams => $"ffmpeg -y -f dshow -i audio=\"{ffmpegParams.AudioDeviceName}\" -f gdigrab -framerate 60 -i desktop  -pix_fmt yuv420p -s {ffmpegParams.Resolution.ToString()} " +
        //       "-vcodec libx264  -threads 8 -thread_type slice  -preset ultrafast  -movflags +faststart -b:v 4000k -tune zerolatency -crf 23  -g 60 " +
        //       $"-acodec libmp3lame -b:a 128k -ar 44100 -ac 2 -movflags +faststart -f flv {ffmpegParams.PathToStream}{localStreamOut}"
        private string GetArguments(Dictionary<ShowType, Func<FfmpegParams, string>> argumentsDictionary, FfmpegParams ffmpegParams)
        {
            Func<FfmpegParams, string> func;
            argumentsDictionary.TryGetValue(ffmpegParams.ShowType, out func);
            if (func == null) throw new NotSupportedException(ffmpegParams.ShowType.ToString());
            return func.Invoke(ffmpegParams);
        }

        public string GetCommand(FfmpegParams ffmpegParams) => GetArguments(_argumentsDictionary, ffmpegParams);

        public string GetArgumentsWithStream(FfmpegParams ffmpegParams) => GetArguments(_argumentsStreamDictionary, ffmpegParams);

    }
}