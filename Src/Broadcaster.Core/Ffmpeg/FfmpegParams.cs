using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcaster.Core.Ffmpeg
{
    public enum ShowType
    {
        [Description("Камера")]
        WebCam,
        [Description("Камера (H264)")]
        WebCamH264,
        [Description("Камера (4:3)")]
        WebCam43,
        [Description("Экран")]
        Desktop,
        [Description("Захват окна")]
        Document
    }
    public class FfmpegParams
    {
        public string VideoDeviceName { get; set; }
        public string AudioDeviceName { get; set; }
        public FfmpegVideoSize Resolution { get; set; }
        public ShowType ShowType { get; set; }
        public string PathToStream { get; set; }
        public string WindowName { get; set; }
    }
}
