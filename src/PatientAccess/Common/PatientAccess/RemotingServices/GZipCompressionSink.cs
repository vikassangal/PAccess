using System.Runtime.Remoting.Channels;

namespace PatientAccess.RemotingServices
{

    /// <summary>
    /// Summary description for GZipCompressionSink
    /// </summary>
    public abstract class GZipCompressionSink<T> :  BaseChannelSinkWithProperties where T : IChannelSinkBase
    {

        #region Constants & Enumerations

        protected const string COMPRESSION_HEADER_NAME = "X-Compress";
        protected const string COMPRESSION_HEADER_VALUE = "gzip";

        #endregion Constants & Enumerations

        #region Events and Delegates

        #endregion Events and Delegates

        #region Fields

        private GZipCompressor i_Compressor = new GZipCompressor();
        private bool i_EnableCompression = false;
        protected T i_NextSink = default(T);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the next client channel sink in the client sink chain.
        /// </summary>
        /// <value></value>
        /// <returns>The next client channel sink in the client sink chain.</returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure"/>
        /// </PermissionSet>
        public T NextChannelSink
        {

            get
            {

                return this.i_NextSink;

            }//get

        }//property


        /// <summary>
        /// Gets the compressor.
        /// </summary>
        /// <value>The compressor.</value>
        protected GZipCompressor Compressor
        {
            get
            {

                return this.i_Compressor;

            }//get

        }//property


        /// <summary>
        /// Gets or sets a value indicating whether [enable compression].
        /// </summary>
        /// <value><c>true</c> if [enable compression]; otherwise, <c>false</c>.</value>
        protected bool EnableCompression
        {

            get
            {

                return this.i_EnableCompression;

            }//get
            set
            {

                this.i_EnableCompression = value;

            }//set
        
        }//property


        #endregion Properties

        #region Construction & Finalization

        #endregion Construction & Finalization

        #region Public Methods

        #endregion Public Methods

        #region Non-Public Methods

        #endregion Non-Public Methods

        #region Event Handlers

        #endregion Event Handlers

    }//class

}//namespace