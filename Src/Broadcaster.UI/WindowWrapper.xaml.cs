﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Broadcaster.UI
{
    /// <summary>
    /// Interaction logic for WindowWrapper.xaml
    /// </summary>
    public partial class WindowWrapper : UserControl
    {
        public WindowWrapper()
        {
            InitializeComponent();
            _panel = new System.Windows.Forms.Panel();
            windowsFormsHost1.Child = _panel;
            f();
        }
        private System.Windows.Forms.Panel _panel;
        private Process _process;

     

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            FindExecutable(path, string.Empty, executable);
            return executable.ToString();
        }

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        private static extern long FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);

        private string file = "C:\\Users\\Андрей\\Desktop\\GIT_Succinctly.pdf";
        private void f()
        {
          //  button1.Visibility = Visibility.Hidden;
         //   ProcessStartInfo psi = new ProcessStartInfo(FindExecutable(file), file) ;
            ProcessStartInfo psi = new ProcessStartInfo(FindExecutable(file)) ;
            _process = Process.Start(psi);
         
       return;
            _process.WaitForInputIdle();
            SetParent(_process.MainWindowHandle, _panel.Handle);

            // remove control box
            int style = GetWindowLong(_process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            SetWindowLong(_process.MainWindowHandle, GWL_STYLE, style);

            // resize embedded application & refresh
            ResizeEmbeddedApp();
        }

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    if (_process != null)
        //    {
        //        _process.Refresh();
        //        _process.Close();
        //    }
        //}

        private void ResizeEmbeddedApp()
        {
            if (_process == null)
                return;

            SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, 0, 0, (int)_panel.ClientSize.Width, (int)_panel.ClientSize.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            ResizeEmbeddedApp();
            return size;
        }
    }
}
