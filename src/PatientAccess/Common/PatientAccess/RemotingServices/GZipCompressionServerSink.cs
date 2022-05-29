using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace PatientAccess.RemotingServices
{

    /// <summary>
    /// 
    /// </summary>
    class GZipCompressionServerSink : GZipCompressionSink<IServerChannelSink>, IServerChannelSink
    {

		#region Event Handlers
		#endregion

		#region Methods

        /// <summary>
        /// Requests processing from the current sink of the response from a method call sent asynchronously.
        /// </summary>
        /// <param name="sinkStack">A stack of sinks leading back to the server transport sink.</param>
        /// <param name="state">Information generated on the request side that is associated with this sink.</param>
        /// <param name="msg">The response message.</param>
        /// <param name="headers">The headers to add to the return message heading to the client.</param>
        /// <param name="stream">The stream heading back to the transport sink.</param>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
		public void AsyncProcessResponse( 
			IServerResponseChannelSinkStack sinkStack, 
			object state, 
			IMessage remotingMessage, 
			ITransportHeaders headers, 
			Stream stream )
		{

			bool hasBeenCompressed = (bool) state;

			if( this.EnableCompression && hasBeenCompressed )
			{

                headers[ COMPRESSION_HEADER_NAME ] = COMPRESSION_HEADER_VALUE;
                stream = this.Compressor.CompressAndCopy( stream );
					
			}//if

			sinkStack.AsyncProcessResponse( remotingMessage, headers, stream );

		}//method


        /// <summary>
        /// Returns the <see cref="T:System.IO.Stream"/> onto which the provided response message is to be serialized.
        /// </summary>
        /// <param name="sinkStack">A stack of sinks leading back to the server transport sink.</param>
        /// <param name="state">The state that has been pushed to the stack by this sink.</param>
        /// <param name="msg">The response message to serialize.</param>
        /// <param name="headers">The headers to put in the response stream to the client.</param>
        /// <returns>
        /// The <see cref="T:System.IO.Stream"/> onto which the provided response message is to be serialized.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
		public Stream GetResponseStream( 
			IServerResponseChannelSinkStack sinkStack, 
			object state, 
			IMessage msg, 
			ITransportHeaders headers )
		{

			return null;

		}//method


        /// <summary>
        /// Requests message processing from the current sink.
        /// </summary>
        /// <param name="sinkStack">A stack of channel sinks that called the current sink.</param>
        /// <param name="requestMsg">The message that contains the request.</param>
        /// <param name="requestHeaders">Headers retrieved from the incoming message from the client.</param>
        /// <param name="requestStream">The stream that needs to be to processed and passed on to the deserialization sink.</param>
        /// <param name="responseMsg">When this method returns, contains a <see cref="T:System.Runtime.Remoting.Messaging.IMessage"/> that holds the response message. This parameter is passed uninitialized.</param>
        /// <param name="responseHeaders">When this method returns, contains a <see cref="T:System.Runtime.Remoting.Channels.ITransportHeaders"/> that holds the headers that are to be added to return message heading to the client. This parameter is passed uninitialized.</param>
        /// <param name="responseStream">When this method returns, contains a <see cref="T:System.IO.Stream"/> that is heading back to the transport sink. This parameter is passed uninitialized.</param>
        /// <returns>
        /// A <see cref="T:System.Runtime.Remoting.Channels.ServerProcessing"/> status value that provides information about how message was processed.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
		public ServerProcessing ProcessMessage( 
			IServerChannelSinkStack sinkStack, 
			IMessage requestMsg, 
			ITransportHeaders requestHeaders, 
			Stream requestStream, 
			out IMessage responseMsg, 
			out ITransportHeaders responseHeaders, 
			out Stream responseStream )
		{

            bool isCompressed = false;

            if( !String.IsNullOrEmpty(requestHeaders[ COMPRESSION_HEADER_NAME ] as string ) )
			{

                requestStream = this.Compressor.DecompressAndCopy( requestStream );
                isCompressed = true;

			}//if

			sinkStack.Push( this, isCompressed );

			ServerProcessing serverProcessing = 
                this.NextChannelSink.ProcessMessage(
				    sinkStack,
				    requestMsg,
				    requestHeaders,
				    requestStream,
				    out responseMsg,
				    out responseHeaders,
				    out responseStream );

            if( ( ServerProcessing.Complete == serverProcessing ) && isCompressed )
			{

				responseHeaders[ COMPRESSION_HEADER_NAME ] = COMPRESSION_HEADER_VALUE;
				responseStream = this.Compressor.CompressAndCopy( responseStream );

			}//if

			return serverProcessing;
            
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
        /// Initializes a new instance of the <see cref="GZipCompressionServerSink"/> class.
        /// </summary>
        /// <param name="nextSink">The next sink.</param>
        /// <param name="enableCompression">if set to <c>true</c> [enable compression].</param>
		public GZipCompressionServerSink( IServerChannelSink nextSink, bool enableCompression )
		{

			this.i_NextSink = nextSink;
            this.EnableCompression = enableCompression;

		}//method

		#endregion

		#region Data Elements
		        
		#endregion

		#region Constants

		#endregion

    }//class

}//namespace
