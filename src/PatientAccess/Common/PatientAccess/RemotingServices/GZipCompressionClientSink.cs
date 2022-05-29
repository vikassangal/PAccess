using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace PatientAccess.RemotingServices
{
    public class GZipCompressionClientSink : GZipCompressionSink<IClientChannelSink>, IClientChannelSink
    {
		#region Event Handlers
		#endregion

		#region Methods

        /// <summary>
        /// Async process request handler
        /// </summary>
        /// <param name="sinkStack">The sink stack.</param>
        /// <param name="remotingMessage">The remoting message.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="stream">The stream.</param>
		public void AsyncProcessRequest( 
			IClientChannelSinkStack sinkStack, 
			IMessage remotingMessage, 
			ITransportHeaders headers, 
			Stream stream )
		{

			if( EnableCompression )
			{

				headers[ COMPRESSION_HEADER_NAME ] = COMPRESSION_HEADER_VALUE;
                stream = this.Compressor.CompressAndCopy( stream );

			}//if

			sinkStack.Push( this, null );

			this.NextChannelSink.AsyncProcessRequest( 
				sinkStack, 
				remotingMessage, 
				headers, 
				stream );

		}//method


        /// <summary>
        /// Requests asynchronous processing of a response to a method call on the current sink.
        /// </summary>
        /// <param name="sinkStack">A stack of sinks that called this sink.</param>
        /// <param name="state">Information generated on the request side that is associated with this sink.</param>
        /// <param name="headers">The headers retrieved from the server response stream.</param>
        /// <param name="stream">The stream coming back from the transport sink.</param>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
		public void AsyncProcessResponse( 
			IClientResponseChannelSinkStack sinkStack, 
			object state, 
			ITransportHeaders headers, 
			Stream stream )
		{

			if( !String.IsNullOrEmpty( headers[ COMPRESSION_HEADER_NAME ] as string ) )
			{

                stream = this.Compressor.DecompressAndCopy( stream );

			}//if			

			sinkStack.AsyncProcessResponse( headers, stream );

		}//method


        /// <summary>
        /// Gets the request stream.
        /// </summary>
        /// <param name="remotingMessage">The remoting message.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
		public Stream GetRequestStream( 
			IMessage remotingMessage, 
			ITransportHeaders headers )
		{

			return this.NextChannelSink.GetRequestStream( remotingMessage, headers );

		}//method


        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="remotingMessage">The remoting message.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="requestStream">The request stream.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <param name="responseStream">The response stream.</param>
		public void ProcessMessage( 
			IMessage remotingMessage, 
			ITransportHeaders requestHeaders, 
			Stream requestStream, 
			out ITransportHeaders responseHeaders, 
			out Stream responseStream )
		{

			if( EnableCompression )
			{

				requestHeaders[ COMPRESSION_HEADER_NAME ] = COMPRESSION_HEADER_VALUE;
                requestStream = this.Compressor.CompressAndCopy( requestStream );


			}//if

			this.NextChannelSink.ProcessMessage( 
				remotingMessage, 
				requestHeaders, 
				requestStream, 
				out responseHeaders, 
				out responseStream );

            if( !String.IsNullOrEmpty( responseHeaders[ COMPRESSION_HEADER_NAME ] as string ) )
			{

                responseStream = this.Compressor.DecompressAndCopy( responseStream );            
			
            }//if
            
		}//method

		#endregion

		#region Properties



		#endregion

		#region Private Methods

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipCompressionClientSink"/> class.
        /// </summary>
        /// <param name="nextSink">The next sink.</param>
        /// <param name="enableCompression">if set to <c>true</c> [enable compression].</param>
		public GZipCompressionClientSink( IClientChannelSink nextSink, bool enableCompression )
		{

			this.i_NextSink = nextSink;
            
			this.EnableCompression = enableCompression;

		}//mathod

		#endregion

		#region Data Elements

		#endregion

		#region Constants

		#endregion

    }
}
