using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{
    class VersionMatchingServerSinkProvider : IServerChannelSinkProvider
    {

        #region Event Handlers
        #endregion

        #region Methods

        public IServerChannelSink CreateSink( IChannelReceiver channel )
        {
            IServerChannelSink sink = i_next.CreateSink( channel );

            string serverVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            return new VersionMatchingServerSink( sink, serverVersion );
        }

        public void GetChannelData( IChannelDataStore channelData )
        {
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

        public VersionMatchingServerSinkProvider( IDictionary properties, ICollection providerData )
        {         
        }

        #endregion

        #region Data Elements

        private IServerChannelSinkProvider i_next;

        #endregion

        #region Constants

        #endregion

    }
}
