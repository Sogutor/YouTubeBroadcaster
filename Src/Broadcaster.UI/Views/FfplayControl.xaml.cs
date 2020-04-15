using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Broadcaster.UI.ViewModels;

namespace Broadcaster.UI.Views
{
    /// <summary>
    /// Interaction logic for FfplayControl.xaml
    /// </summary>
    public partial class FfplayControl : UserControl
    {
        public FfplayControl()
        {
            InitializeComponent();
            IntPtr handle = NativeMethods.FindWindow("SDL_app", "pipe:");
            while (handle == IntPtr.Zero)
            {
                handle = NativeMethods.FindWindow("SDL_app", "pipe:");
            }
            Thread.Sleep(400);
            IntPtr handle1 = NativeMethods.FindWindow("SDL_app", "pipe:");
            while (handle1 == IntPtr.Zero)
            {
                handle1 = NativeMethods.FindWindow("SDL_app", "pipe:");
            }
            if (handle1 != IntPtr.Zero)
            {
                handle1 = NativeMethods.FindWindow("SDL_app", "pipe:");

                var hostControl = new System.Windows.Forms.Integration.WindowsFormsHost();

                hostControl.Child = new PlayerControl(handle1);

                contentControl.Children.Add(hostControl);
            }
        }
    }
}
