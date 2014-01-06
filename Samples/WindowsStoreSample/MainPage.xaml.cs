using RC.Core.Bluetooth;
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
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
    }
}
