using System.Collections;
using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{
    class GZipCompressionServerSinkProvider : IServerChannelSinkProvider
    {

		#region Event Handlers
		#endregion

		#region Methods

		public IServerChannelSink CreateSink( IChannelReceiver channel )
		{
			IServerChannelSink sink = i_next.CreateSink( channel );
			return new GZipCompressionServerSink( sink, i_enablecompression );
		}

		public void GetChannelData( IChannelDataStore channelData )
		{
			//throw new Exception( "The method or operation is not implemented." );
		}

		#endregion

		#region Properties
		
		public IServerChannelSinkProvider Next
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

		public GZipCompressionServerSinkProvider( IDictionary properties, ICollection providerData )
		{
			if( properties.Contains( ENABLED_KEY ) )
			{
				i_enablecompression = ( ( (string)properties[ENABLED_KEY] ).Equals( ENABLED_VALUE ) );
			}
		}

		#endregion

		#region Data Elements

		private bool i_enablecompression;
		private IServerChannelSinkProvider i_next;

		#endregion

		#region Constants

		private const string ENABLED_VALUE = "true";
		private const string ENABLED_KEY = "Enable";

		#endregion

    }
}
