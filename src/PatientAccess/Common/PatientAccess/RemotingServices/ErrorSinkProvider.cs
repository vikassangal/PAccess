using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{
	/// <summary>
	/// Summary description for ErrorSinkProvider.
	/// </summary>
	public class ErrorSinkProvider : IClientChannelSinkProvider
	{
		#region Event Handlers
		#endregion

		#region Methods

		public IClientChannelSink CreateSink( IChannelSender channel, string url, object remoteChannelData )
		{
			IClientChannelSink sink = i_next.CreateSink( channel, url, remoteChannelData );
			return new ErrorSink( sink, i_retries );
		}

		#endregion

		#region Properties

		public IClientChannelSinkProvider Next
		{
			get
			{
				return i_next;
			}
			set
			{
				i_next = value;
			}
		}

		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

		public ErrorSinkProvider( IDictionary properties, ICollection providerData )
		{
            if( properties.Contains( RETRIES_KEY ) )
            {
                this.i_retries = Convert.ToByte(properties[RETRIES_KEY]);
            }
		}

		#endregion

		#region Data Elements

		private IClientChannelSinkProvider i_next;
        private byte i_retries;

		#endregion

		#region Constants

        private const string RETRIES_KEY = "Retries";
		#endregion
	}
}
