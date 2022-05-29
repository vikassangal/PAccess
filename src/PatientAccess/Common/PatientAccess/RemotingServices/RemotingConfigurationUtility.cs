using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace PatientAccess.RemotingServices
{

    /// <summary>
    /// Summary description for RemotingConfigurationUtility
    /// </summary>
    public static class RemotingConfigurationUtility
    {
        private const int DEFAULT_CHANNEL_INDEX = 0;

        #region Constants & Enumerations

        #endregion Constants & Enumerations

        #region Events and Delegates

        #endregion Events and Delegates

        #region Fields

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Construction & Finalization

        #endregion Construction & Finalization

        #region Public Methods

        /// <summary>
        /// Initialzes the default remoting.
        /// </summary>
        public static void InitialzeDefaultRemoting()
        {

            RemotingConfiguration.Configure( AppDomain.CurrentDomain
                                                      .SetupInformation
                                                      .ConfigurationFile, 
                                             true );

        }

        /// <summary>
        /// Initializes the paramaterized remoting.
        /// </summary>
        /// <param name="numberOfRetries">The number of retries.</param>
        /// <param name="useCompression">The use compression.</param>
        /// <param name="remotingTimeout">The remoting timeout.</param>
        public static void InitializeParamaterizedRemoting( string numberOfRetries, string useCompression, int remotingTimeout )
        {

            ChannelServices.UnregisterChannel( 
                ChannelServices.RegisteredChannels[DEFAULT_CHANNEL_INDEX] );
            

            // Create the comrpession sink
            IDictionary compressionProps = new Hashtable();
            compressionProps.Add( "Enable", useCompression );
            GZipCompressionClientSinkProvider gzipProvider =
                new GZipCompressionClientSinkProvider( compressionProps, null );

            // Create the ErrorSinkProvider and chain the GZipCompression sink
            IDictionary errorProps = new Hashtable();
            errorProps.Add( "Retries", numberOfRetries );
            ErrorSinkProvider errorSinkProvider =
                new ErrorSinkProvider( errorProps, null );
            errorSinkProvider.Next =
                gzipProvider as IClientChannelSinkProvider;

            // Create the VersionMathcingProvider and chain the GZipCompression sink
            VersionMatchingClientSinkProvider versionMatchingClientSinkProvider =
                new VersionMatchingClientSinkProvider( null, null );
            versionMatchingClientSinkProvider.Next =
                errorSinkProvider as IClientChannelSinkProvider;

            // Create the binary formatter and chain the VersionMatchingSink to the BinaryFormatter sink
            IClientChannelSinkProvider binaryFormatterSink =
                new BinaryClientFormatterSinkProvider();
            binaryFormatterSink.Next =
                versionMatchingClientSinkProvider as IClientChannelSinkProvider;

            // Create the channel with the sink chain ( binary formatter -> version matching sink -> error sink -> compression sink )
            IDictionary channelProps = new Hashtable();
            channelProps.Add( "timeout", remotingTimeout );
            HttpClientChannel clientChannel =
                new HttpClientChannel( channelProps, binaryFormatterSink );

            // Register the channel.
            ChannelServices.RegisterChannel( clientChannel, true );

        }

        #endregion Public Methods

        #region Non-Public Methods

        #endregion Non-Public Methods

        #region Event Handlers

        #endregion Event Handlers

    }//class

}//namespace