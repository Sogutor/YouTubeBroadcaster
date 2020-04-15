using System.ComponentModel;

namespace Broadcaster.Interfaces.Enums
{
    public enum PrivacyEnum
    {
        [Description("Public")]
        Public,
        [Description("Private")]
        Private,
        [Description("Unlisted")]
        Unlisted
    }
}
