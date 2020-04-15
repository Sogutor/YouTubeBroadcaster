using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Broadcaster.UI.ViewModels
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    }
    public partial class PlayerControl : UserControl
    {
        private IntPtr _handle;
        
        public PlayerControl(IntPtr handle)
        {
            InitializeComponent();

            _handle = handle;

            // Set WS_STYLE to WS_CHILD | WS_VISIBLE
            NativeMethods.SetWindowLong(handle, -16, (uint)(0x40000000L | 0x10000000L));
            NativeMethods.SetParent(handle, this.Handle);
            NativeMethods.MoveWindow(_handle, this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.ClientRectangle.Height, true);
        }

        private void PlayerControl_ClientSizeChanged(object sender, EventArgs e)
        {
            NativeMethods.MoveWindow(_handle, this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.ClientRectangle.Height, true);
        }
    }
}
