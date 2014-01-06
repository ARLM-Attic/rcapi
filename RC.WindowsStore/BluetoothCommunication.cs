using System;
using System.Linq;
using System.Threading;
using RC.Core.Interfaces;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RC.WinRT
{
	/// <summary>
	/// Communicate with EV3 brick over Bluetooth.
	/// </summary>
	public sealed class BluetoothCommunication : ICommunication
	{
		private StreamSocket _socket;
		private DataReader _reader;
		private CancellationTokenSource _tokenSource;

		/// <summary>
		/// Connect to the RC Device.
		/// </summary>
		/// <returns></returns>
		public IAsyncAction ConnectAsync(string name, string remoteServiceName)
		{
			return ConnectAsyncInternal(name).AsAsyncAction();
		}

		private async Task ConnectAsyncInternal(string name)
		{
			_tokenSource = new CancellationTokenSource();

			string selector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
			DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
			DeviceInformation device = (from d in devices where d.Name == name select d).FirstOrDefault();
			if(device == null)
				throw new Exception(name + " not found.");

			RfcommDeviceService service = await RfcommDeviceService.FromIdAsync(device.Id);
			if(service == null)
				throw new Exception("Unable to connect to " + name + "...is the manifest set properly?");

			_socket = new StreamSocket();
			await _socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName,
				 SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

		}

		/// <summary>
		/// Disconnect from the EV3 brick.
		/// </summary>
		public void Disconnect()
		{
			_tokenSource.Cancel();
			if(_reader != null)
			{
				_reader.DetachStream();
				_reader = null;
			}

			if(_socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}
		}

		/// <summary>
		/// Write data to the EV3 brick.
		/// </summary>
		/// <param name="data">Byte array to write to the EV3 brick.</param>
		/// <returns></returns>
		public IAsyncAction WriteAsync([ReadOnlyArray]byte[] data)
		{
			return WriteAsyncInternal(data).AsAsyncAction();
		}

		private async Task WriteAsyncInternal(byte[] data)
		{
			if(_socket != null)
				await _socket.OutputStream.WriteAsync(data.AsBuffer());
		}
    }
}
