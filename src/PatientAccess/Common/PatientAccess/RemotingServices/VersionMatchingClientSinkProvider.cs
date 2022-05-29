using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{
    public class VersionMatchingClientSinkProvider : IClientChannelSinkProvider
    {
		#region Event Handlers
		#endregion

		#region Methods

		public IClientChannelSink CreateSink( IChannelSender channel, string url, object remoteChannelData )
		{
			IClientChannelSink sink = this.Next.CreateSink( channel, url, remoteChannelData );

            string clientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			return new VersionMatchingClientSink( sink, clientVersion );
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

		public VersionMatchingClientSinkProvider( IDictionary properties, ICollection providerData )
		{
		}

		#endregion

		#region Data Elements

		private IClientChannelSinkProvider i_next;

		#endregion
    }
}    