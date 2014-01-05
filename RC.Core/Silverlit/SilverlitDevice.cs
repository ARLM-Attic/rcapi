using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using RC.Core.Interfaces;

#if WINRT
using Windows.UI.Xaml;
using RC.WinRT;
using Windows.Foundation;
#else
using RC.WindowsPhone;
using System.Windows.Threading;
#endif


namespace RC.Core.Silverlit
{
    public abstract class SilverlitBluetoothDevice:IDisposable
    {
        DispatcherTimer timer = new DispatcherTimer();

        ICommunication communication = new BluetoothCommunication();
        public string DeviceName { get; set; }

     
        /// <summary>
        /// Name of the Sevice to connect to
        /// </summary>
        public string ServiceName { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        protected byte Match { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        protected byte Rotor { get; set; }
        
        /// <summary>
        /// For cars Pitch controls speed
        /// </summary>
        protected byte Pitch { get; set; }

        /// <summary>
        /// For cars pitch controls right and left
        /// </summary>
        protected byte Yaw { get; set; }

        /// <summary>
        /// For cars Trimmer sets the origin of the wheels
        /// </summary>
        protected byte Trimmer { get; set; }

        /// <summary>
        /// Controls the lights as binary
        /// 1=headlights
        /// 2=Left blinkers
        /// 4=RightBlinkers
        /// 8=Left BLinkers
        /// </summary>
        protected byte Lights { get; set; }

        protected virtual byte[] GetBytesToSend()
        {
            return new byte[0];
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public SilverlitBluetoothDevice(string deviceName, string serviceName,int sendDelay)
        {
            ServiceName = serviceName;
            DeviceName = deviceName;
            timer.Interval = TimeSpan.FromMilliseconds(sendDelay);
            timer.Tick += timer_Tick;

            Trimmer = 0;
            Lights = 0;
        }

        void timer_Tick(object sender, object e)
        {
            SendCommandToDevice();
        }

        /// <summary>
        /// Starts sending commands to the device
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Stops sending commands to the device
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        public async Task ConnectAsync()
        {
            await communication.ConnectAsync(this.DeviceName, this.ServiceName);
        }




        /// <summary>
        /// Converts a byte array to a IBuffer
        /// </summary>
        /// <param name="package">byte array to be converted to an IBuffer</param>
        /// <returns>Ibuffer version of the byte array</returns>
        protected IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (DataWriter dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }

        /// <summary>
        /// Sends command to the device
        /// </summary>
        protected async void SendCommandToDevice()
        {
            await communication.WriteAsync(GetBytesToSend());
        }


        /// <summary>
        /// Converts a byte to a lowercase hex string
        /// </summary>
        /// <param name="b">byte to be converted to hex</param>
        /// <returns>Lowercase hex string</returns>
        protected string GetHexString(byte b)
        {
            if ((0xFF & b) < 16)
                return "0" + b.ToString("X").ToLower();
            return b.ToString("X").ToLower();
        }




        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            communication.Disconnect();
        }
    }
}