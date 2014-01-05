using RC.Core;
using RC.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace RC.WindowsPhone
{
	/// <summary>
	/// Communicate with the RC Device over Bluetooth.
	/// </summary>
	public class BluetoothCommunication : ICommunication
	{
		/// <summary>
		/// Event fired when a complete report is received from the EV3 brick.
		/// </summary>
		public event EventHandler<ReportReceivedEventArgs> ReportReceived;

		private StreamSocket _socket;
		private DataReader _reader;
		private CancellationTokenSource _tokenSource;
        
		/// <summary>
		/// Connect to the RC Device.
		/// </summary>
		/// <returns></returns>
		public async Task ConnectAsync(string name,string remoteServiceName)
		{
			_tokenSource = new CancellationTokenSource();

			PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
			IReadOnlyList<PeerInformation> peers = await PeerFinder.FindAllPeersAsync();
			PeerInformation peer = (from p in peers where p.DisplayName == name select p).FirstOrDefault();
			if(peer == null)
				throw new Exception( name + " not found");

			_socket = new StreamSocket();
            await _socket.ConnectAsync(peer.HostName,remoteServiceName);

			//_reader = new DataReader(_socket.InputStream);
			//_reader.ByteOrder = ByteOrder.LittleEndian;

			//ThreadPool.QueueUserWorkItem(PollInput);
		}

		private async void PollInput(object state)
		{
			while(_socket != null)
			{
				try
				{
					DataReaderLoadOperation drlo = _reader.LoadAsync(2);
					await drlo.AsTask(_tokenSource.Token);
					short size = _reader.ReadInt16();

					byte[] data = new byte[size];
					drlo = _reader.LoadAsync((uint)size);
					await drlo.AsTask(_tokenSource.Token);
					_reader.ReadBytes(data);

					if(ReportReceived != null)
						ReportReceived(this, new ReportReceivedEventArgs { Report = data });
				}
				catch (TaskCanceledException)
				{
					return;
				}
			}
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
		public async Task WriteAsync(byte[] data)
		{
			if(_socket != null)
				await _socket.OutputStream.WriteAsync(data.AsBuffer());
		}
	}
}
