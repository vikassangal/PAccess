using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace PatientAccess.RemotingServices
{
    public class VersionMatchingClientSink : BaseChannelSinkWithProperties, IClientChannelSink
    {
        #region Event Handlers
        #endregion

        #region Methods

        public Stream GetRequestStream( IMessage msg, ITransportHeaders headers )
        {
            return this.i_nextSink.GetRequestStream( msg, headers );
        }

        public void AsyncProcessRequest(
            IClientChannelSinkStack sinkStack,
            IMessage msg,
            ITransportHeaders headers,
            Stream stream )
        {
            headers[CLIENT_VERSION] = this.ClientVersion;

            sinkStack.Push( this, null );

            this.i_nextSink.AsyncProcessRequest(
                sinkStack,
                msg,
                headers,
                stream );
        }

        public void AsyncProcessResponse(
            IClientResponseChannelSinkStack sinkStack,
            object state,
            ITransportHeaders headers,
            Stream stream )
        {
            sinkStack.AsyncProcessResponse( headers, stream );
        }

        public void ProcessMessage(
            IMessage msg,
            ITransportHeaders requestHeaders,
            Stream requestStream,
            out ITransportHeaders responseHeaders,
            out Stream responseStream )
        {

            requestHeaders[CLIENT_VERSION] = this.ClientVersion;

            this.i_nextSink.ProcessMessage(
                msg,
                requestHeaders,
                requestStream,
                out responseHeaders,
                out responseStream );
        }

        #endregion

        #region Properties

        public IClientChannelSink NextChannelSink
        {
            get
            {
                return this.i_nextSink;
            }
        }

        private string ClientVersion
        {
            get
            {
                return this.i_ClientVersion;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public VersionMatchingClientSink( IClientChannelSink nextSink, string clientVersion )
        {
            i_nextSink = nextSink;
            i_ClientVersion = clientVersion;
        }

        #endregion

        #region Data Elements

        private IClientChannelSink i_nextSink;
        private string i_ClientVersion = string.Empty;

        #endregion

        #region Constants

        private const string CLIENT_VERSION = "ClientVersion";

        #endregion

    }
}
