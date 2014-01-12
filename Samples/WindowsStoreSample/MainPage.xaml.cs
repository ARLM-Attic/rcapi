using RC.Core.Bluetooth;
using RC.Core.Silverlit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsStoreSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Ferrari458Italia ferrari = new Ferrari458Italia();
        DispatcherTimer timer = new DispatcherTimer();
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            var joyState = JoystickControl.GetState();
            ferrari.Trimmer = (byte)TrimmerSlider.Value;
            ferrari.Steering = joyState.X;
            ferrari.Speed = joyState.Y;
        }



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                ferrari.ConnectionLost += ferrari_ConnectionLost;
                await ferrari.ConnectAsync();
                ferrari.Start();
                ferrari.HeadLightOn=true;
                //JoystickControl.StartJoystick();
            }
            catch(Exception ex)
            {
                MessageDialog md=new MessageDialog(ex.Message);
                md.ShowAsync();
            }
            base.OnNavigatedTo(e);
        }

        void ferrari_ConnectionLost(object sender, Exception e)
        {
            MessageDialog md = new MessageDialog(e.Message);
            md.ShowAsync();

        }

        private void PartyButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.HeadLightOn = false;
            ferrari.LightSequence = new byte[] { (byte)LightEnum.Head, (byte)LightEnum.RightBlinker, (byte)LightEnum.Break, (byte)LightEnum.LeftBlinker };
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = new byte[] { 0, 8 };
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = new byte[] { (byte)LightEnum.LeftBlinker, 0 };
        }

        private void BlinkersOffButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = null;
        }

        private void HeadlightButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.HeadLightOn = !ferrari.HeadLightOn;
        }
    }
}
