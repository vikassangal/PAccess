using System;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace PatientAccess.RemotingServices
{
	/// <summary>
	/// Summary description for ErrorSinkFormatChecker.
	/// </summary>

	//TODO: Create XML summary comment for ErrorSinkFormatChecker
	public class ErrorSinkFormatChecker  
	{
		#region Event Handlers
		#endregion

		#region Methods
		public bool Check(ITransportHeaders headers, Stream response)
		{
			string header = headers[CONTENT_TYPE_KEY] as string;
			
			if(String.Compare(header, OCTET_STREAM, true) != 0)
			{
				i_error = new RemotingErrorInfo();

                /* The status code and the status description are /supposed/ to be found in
                 * in the transport headers. Unfortunately, empirical evidence proves other wise.
                 * But just to be on the safe side, let's put the code in anyhow, because that 
                 * information could be useful.
                 */
				string code = (string)headers[STATUS_CODE];
				string description = (string)headers[STATUS_DESCRIPTION];
				double num;
                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[4096];
                int count = 0;

                while( ( count = response.Read( buf, 0, 4096 ) ) > 0 )
                {
                    for(int index = 0; index < count; index++ )
                    {
                        sb.Append( Convert.ToChar( buf[index] ) );
                        buf[index] = 0;
                    }
                }

                if( response.CanSeek ) response.Position = 0;

				i_error.Body = sb.ToString();
				
                //Make sure to initialize the member to zero in case we can't parse it.
                i_error.Code = Double.TryParse(code, 
					NumberStyles.Integer, 
					CultureInfo.CurrentCulture,
					out num)? Convert.ToInt32(num) : 0;
				i_error.Description = description;

				return false;
			}

			return true;
		}
		#endregion

		#region Properties
		public RemotingErrorInfo Error
		{
			get 
			{
				return i_error;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ErrorSinkFormatChecker()
		{
		}
		#endregion

		#region Data Elements
		private RemotingErrorInfo i_error;
		#endregion

		#region Constants
		private const string CONTENT_TYPE_KEY = "Content-Type";
		private const string OCTET_STREAM = "application/octet-stream";
		private const string STATUS_CODE = "__HttpStatusCode";
		private const string STATUS_DESCRIPTION = "__HttpReasonPhrase";
		#endregion
	}
}
