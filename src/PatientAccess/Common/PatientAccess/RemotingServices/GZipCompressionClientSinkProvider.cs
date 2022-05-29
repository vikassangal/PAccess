using System.Collections;
using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{
    public class GZipCompressionClientSinkProvider : IClientChannelSinkProvider
    {
		#region Event Handlers
		#endregion

		#region Methods

		public IClientChannelSink CreateSink( IChannelSender channel, string url, object remoteChannelData )
		{
			IClientChannelSink sink = i_next.CreateSink( channel, url, remoteChannelData );
			return new GZipCompressionClientSink( sink, i_enablecompression );
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

		public GZipCompressionClientSinkProvider( IDictionary properties, ICollection providerData )
		{
			if( properties.Contains( ENABLED_KEY ) )
			{
				i_enablecompression = ( ( ( string )properties[ENABLED_KEY] ).Equals( ENABLED_VALUE ) );
			}
		}

		#endregion

		#region Data Elements

		private bool i_enablecompression;
		private IClientChannelSinkProvider i_next;

		#endregion

		#region Constants

		private const string ENABLED_VALUE = "true";
		private const string ENABLED_KEY = "Enable";

		#endregion
    }
}
