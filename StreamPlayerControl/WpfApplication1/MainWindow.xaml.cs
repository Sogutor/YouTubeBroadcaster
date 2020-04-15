using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebEye;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer timer = new Timer(100) { AutoReset = true, Enabled = true };
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModelTest();
            timer.Elapsed += (sender, args) =>
            {
              //  var tt = StreamPlayerControl1.Sel;
             //   Console.WriteLine(tt);
                //if (t==null)
                //{
                //    Console.WriteLine("sdsds");
                //}
            };
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

        }
    }

    public class ViewModelTest : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StreamPlayerProxy StreamPlayer { get; set; }
        public bool Test { get; set; }
        public int Tt { get; set; }
        
      
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        System.Timers.Timer timer = new Timer(100) { AutoReset = true, Enabled = true };
        public ViewModelTest()
        {
            timer.Elapsed += Timer_Elapsed;



        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
          
           ;
        //    Test = false;
         //   Console.WriteLine(Tt);
           // StreamPlayer= new PlayerMethods();
            if (StreamPlayer != null)
            {
                StreamPlayer.StartPlay("tcp://127.0.0.1:2000?listen", TimeSpan.FromSeconds(15));
                timer.Stop();
                //Application.Current.Dispatcher.Invoke(
                //    () => StreamPlayer.StartPlay("tcp://127.0.0.1:2000?listen", TimeSpan.FromSeconds(15)));
           //     Console.WriteLine("sdsfsdf");
            }

        }
    }
}
