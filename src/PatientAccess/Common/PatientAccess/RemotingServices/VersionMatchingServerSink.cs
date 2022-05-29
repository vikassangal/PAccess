using System;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using Extensions.Exceptions;
using PatientAccess.Domain.Parties.Exceptions;
using log4net;

namespace PatientAccess.RemotingServices
{
    class VersionMatchingServerSink : BaseChannelSinkWithProperties, IServerChannelSink
    {
        #region Event Handlers
        #endregion

        #region Methods

        public void AsyncProcessResponse(
            IServerResponseChannelSinkStack sinkStack,
            object state,
            IMessage msg,
            ITransportHeaders headers,
            Stream stream )
        {
            this.checkVersion( headers );

            sinkStack.AsyncProcessResponse( msg, headers, stream );

        }

        public Stream GetResponseStream(
            IServerResponseChannelSinkStack sinkStack,
            object state,
            IMessage msg,
            ITransportHeaders headers )
        {
            return null;
        }

        public ServerProcessing ProcessMessage(
            IServerChannelSinkStack sinkStack,
            IMessage requestMsg,
            ITransportHeaders requestHeaders,
            Stream requestStream,
            out IMessage responseMsg,
            out ITransportHeaders responseHeaders,
            out Stream responseStream )
        {          
            this.checkVersion( requestHeaders );

            ServerProcessing proc = NextChannelSink.ProcessMessage(
                sinkStack,
                requestMsg,
                requestHeaders,
                requestStream,
                out responseMsg,
                out responseHeaders,
                out responseStream );         

            return proc;

        }

        #endregion

        #region Properties

        public IServerChannelSink NextChannelSink
        {
            get
            {
                return i_NextSink;
            }
        }

        private string ServerVersion
        {
            get
            {
                return i_ServerVersion;
            }
        }

        #endregion

        #region Private Methods     

        /// <exception cref="VersionNotMatchedException"><c>VersionNotMatchedException</c>.</exception>
        private void checkVersion( ITransportHeaders headers )
        {
            bool isSuccess = false;

            string clientVersion = headers[CLIENT_VERSION] as string;

            // check if we are a developer version
            if( !string.IsNullOrEmpty( clientVersion ) )
            {
                if( clientVersion.StartsWith( DEVELOPER_VERSION ) ||
                    clientVersion == this.ServerVersion )
                {

                    if( c_log.IsDebugEnabled )
                    {
                        c_log.DebugFormat( "Successful version match for client and server - Client:{0}, {1}", clientVersion, ServerVersion );
                    }

                    isSuccess = true;
                }
            }            

            if( !isSuccess )
            {
                c_log.WarnFormat( "VersionMatchingSink version mis-match - ClientVerision: {0}; ServerVersion: {1}", clientVersion, ServerVersion );

                if( this.i_IsMatchingEnforced )
                {
                    throw new VersionNotMatchedException( Severity.Catastrophic );
                }
            }
        }
       
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public VersionMatchingServerSink( IServerChannelSink nextSink, string ServerVersion )
        {
            
            i_NextSink = nextSink;
            i_ServerVersion = ServerVersion;

            Boolean.TryParse( ConfigurationManager.AppSettings[VERSION_MATCH_ENFORCED], 
                              out this.i_IsMatchingEnforced );

            if( c_log.IsDebugEnabled )
            {
                c_log.DebugFormat( "VersionMatchEnforced = '{0}'", this.i_IsMatchingEnforced );
            }

        }

        #endregion

        #region Data Elements

        private static readonly ILog c_log =
           LogManager.GetLogger( typeof( VersionMatchingServerSink ) );

        private IServerChannelSink i_NextSink;
        private string i_ServerVersion = string.Empty;
        private bool i_IsMatchingEnforced = false;

        #endregion

        #region Constants

        private const string CLIENT_VERSION = "ClientVersion";
        private const string DEVELOPER_VERSION = "0.0";
        private const string VERSION_MATCH_ENFORCED = "VersionMatchEnforced";

        #endregion
    }
}
