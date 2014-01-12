using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using System.Windows.Threading;
using Windows.Networking.Proximity;
using System.Windows.Input;
using RC.Core.Bluetooth;
using RC.Core.Silverlit;

namespace WindowsPhoneSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer timer = new DispatcherTimer();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var joyState = JoystickControl.GetState();
            ferrari.Steering = joyState.X;
            ferrari.Speed =  joyState.Y;
            ferrari.Trimmer = ((byte)Trimmer.Value);
        }

        Ferrari458Italia ferrari = new Ferrari458Italia();



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                await ferrari.ConnectAsync();
                ferrari.ConnectionLost += ferrari_ConnectionLost;
                ferrari.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            base.OnNavigatedTo(e);
        }

        void ferrari_ConnectionLost(object sender, Exception e)
        {
            MessageBox.Show(e.Message);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ferrari.Stop();
        }

        byte value = 106;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            value++;
            ((Button)sender).Content = value.ToString();
        }


        protected virtual void Dispose(bool disposing)
        {
            ferrari.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void BlinkersOffButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = null;
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = new byte[] { 0, 8 };
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.LightSequence = new byte[] { 0, 4 };
        }

        private void PartyButton_Click(object sender, RoutedEventArgs e)
        {
            ferrari.HeadLightOn = false;
            ferrari.LightSequence = new byte[] { (byte)LightEnum.Head, (byte)LightEnum.RightBlinker, (byte)LightEnum.Break, (byte)LightEnum.LeftBlinker };
        }

        
    }
}