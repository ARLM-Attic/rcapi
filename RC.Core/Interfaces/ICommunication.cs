using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
#if WINRT
using Windows.Foundation;
#endif

namespace RC.Core.Interfaces
{
	/// <summary>
	/// Interface for communicating with the RC Toys
    /// This class is a copy of the class used in EV3 API on Codeplex
	/// </summary>
	public interface ICommunication
	{
		
		/// <summary>
		/// Connect to the EV3 brick.
		/// </summary>
#if WINRT
		IAsyncAction
#else
		Task
#endif
 ConnectAsync(string name, string remoteServiceName);

		/// <summary>
		/// Disconnect from the EV3 brick.
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Write a report to the EV3 brick.
		/// </summary>
		/// <param name="data"></param>
#if WINRT
		IAsyncAction
#else
		Task
#endif
		WriteAsync([ReadOnlyArray]byte[] data);
	}
}
