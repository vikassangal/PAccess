using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.GZip;
using log4net;

namespace PatientAccess.RemotingServices
{

	public class GZipCompressor
	{

		#region Event Handlers
		#endregion

		#region Methods

		public Stream CompressAndCopy( Stream inStream )
		{

			if( null == inStream )
			{
                
                c_Logger.Warn("CompressAndCopy encountered a null input stream");
				
                return inStream;

			}//if
			
            MemoryStream compressedMemoryStream = 
                new MemoryStream();
			GZipOutputStream gzipOutputStream = 
                new GZipOutputStream( compressedMemoryStream, BUFFER_SIZE );
            byte[] privateBuffer = 
                new byte[ BUFFER_SIZE ];
            int bytesRead = 
                inStream.Read( privateBuffer, 0, BUFFER_SIZE );
			
			while( bytesRead > 0 )
			{

				gzipOutputStream.Write( privateBuffer, 0, bytesRead );
                bytesRead = inStream.Read( privateBuffer, 0, BUFFER_SIZE );

			}//while

			gzipOutputStream.Finish();
			gzipOutputStream.Flush();

            if( c_Logger.IsDebugEnabled )
            {

                c_Logger.DebugFormat(
                    "Compression level of remoting payload is {0:#0.##%} ({1}/{2})",
                    (float)compressedMemoryStream.Length / (float)inStream.Length,
                    compressedMemoryStream.Length,
                    inStream.Length);
            
            }//if

			return compressedMemoryStream;

		}//method


        /// <summary>
        /// Decompresses and copies the stream.
        /// </summary>
        /// <param name="inStream">The in stream.</param>
        /// <returns></returns>
		public Stream DecompressAndCopy( Stream inStream )
		{

            Stream gzipInputStream = inStream;

            if (null == inStream)
            {

                c_Logger.Warn("DecompressAndCopy encountered a null input stream");

            }//if
            else
            {

                 gzipInputStream = new GZipInputStream(inStream);

            }//else

            return gzipInputStream;

		}//method

		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		#endregion

		#region Data Elements

        private static ILog c_Logger = 
            LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );

		#endregion

		#region Constants

        private const int BUFFER_SIZE = 4096;        

		#endregion

	}
}
