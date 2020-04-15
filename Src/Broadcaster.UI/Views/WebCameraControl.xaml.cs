using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Broadcaster.UI.Views
{
    /// <summary>
    /// Interaction logic for WebCameraControl.xaml
    /// </summary>
    public partial class WebCameraControl : UserControl
    {
        private volatile bool isStrat;
        public WebCameraControl()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                HelperCameraPreview.WebCameraControl = this;
            });
        }

        public void Start(string deviceName)
        {
            isStrat = true;
            var webCameraId = webCameraControl.GetVideoCaptureDevices().FirstOrDefault(id => id.Name == deviceName);
            Application.Current.Dispatcher.Invoke(() =>
            {
                webCameraControl.StartCapture(webCameraId);
            });

        }

        public void Stop()
        {
            if (!isStrat) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                webCameraControl.StopCapture();
            });

            isStrat = false;
        }

    }
}
