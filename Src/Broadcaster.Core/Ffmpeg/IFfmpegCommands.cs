namespace Broadcaster.Core.Ffmpeg
{
    public interface IFfmpegCommands
    {
        string GetCommand(FfmpegParams ffmpegParams);
        string GetArgumentsWithStream(FfmpegParams ffmpegParams);
    }
}