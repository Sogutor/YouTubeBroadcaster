using System.ComponentModel;

namespace Broadcaster.Interfaces.Enums
{
    public enum Resolution
    {
        [Description("240p")]
        Sd,
        [Description("360p")]
        Wide360P,
        [Description("480p")]
        Wide480P,
        [Description("720p")]
        Hd,
        [Description("1080p")]
        FullHd
    }
}