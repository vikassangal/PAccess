using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using Extensions.Exceptions;
using PatientAccess.Domain.Parties.Exceptions;

namespace PatientAccess.RemotingServices
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ErrorSink : BaseChannelSinkWithProperties, IClientChannelSink
	{
		#region Event Handlers
		#endregion

		#region Methods

		public void AsyncProcessRequest( 
			IClientChannelSinkStack sinkStack, 
			IMessage msg, 
			ITransportHeaders headers, 
			Stream stream )
		{

			//This is a noop. We are only concerned about responses

			sinkStack.Push(this, null);
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
			if(!this.i_checker.Check(headers, stream))
			{
				RemotingErrorInfo rei = this.i_checker.Error;
				throw new RemotingServerError(rei);
			}

            sinkStack.AsyncProcessResponse( headers, stream );
		}

		public Stream GetRequestStream( 
			IMessage msg, 
			ITransportHeaders headers )
		{
			return this.i_nextSink.GetRequestStream( msg, headers );
		}

		public void ProcessMessage( 
			IMessage msg, 
			ITransportHeaders requestHeaders, 
			Stream requestStream, 
			out ITransportHeaders responseHeaders, 
			out Stream responseStream )
		{
            byte retries = 0;            

            this.i_nextSink.ProcessMessage(
				msg,
				requestHeaders,
				Copy(requestStream),
				out responseHeaders,
				out responseStream);



			while( !this.i_checker.Check( responseHeaders, responseStream ) )
			{
                if( this.i_checker.Error.Body != null &&
                    this.i_checker.Error.Body.IndexOf( "PatientAccess.Domain.Exceptions.VersionNotMatchedException" ) >= 0 )
                {
                    throw new VersionNotMatchedException( Severity.Catastrophic );
                }
                else
                {
                    if( retries++ < this.i_numRetries )
                    {

                        this.i_nextSink.ProcessMessage(
                            msg,
                            requestHeaders,
                            Copy( requestStream ),
                            out responseHeaders,
                            out responseStream );
                    }
                    else
                    {
                        throw new RemotingServerError( this.i_checker.Error );
                    }
                }
			}
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

		#endregion

		#region Private Methods

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization

		public ErrorSink(IClientChannelSink nextSink, byte retries)
		{
            i_numRetries = retries;
			i_nextSink = nextSink;
			i_checker = new ErrorSinkFormatChecker();
		}

		#endregion

		#region Data Elements

		private IClientChannelSink i_nextSink;
		private ErrorSinkFormatChecker i_checker;
        private byte i_numRetries;

		#endregion

		#region Constants

		#endregion

        private static Stream Copy( Stream inStream )
        {
            MemoryStream copy = new MemoryStream();

            byte[] buf = new byte[4096];
            int count = 0;

            while( ( count = inStream.Read( buf, 0, 4096 ) ) > 0 )
            {
                copy.Write( buf, 0, count );
            }

            inStream.Position = 0;
            copy.Position = 0;

            return copy;  
        }

	}
}
