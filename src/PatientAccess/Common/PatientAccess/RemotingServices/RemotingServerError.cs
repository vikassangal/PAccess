using System;
using System.Text.RegularExpressions;

namespace PatientAccess.RemotingServices
{
	/// <summary>
	/// Summary description for RemotingServerError.
	/// </summary>
	public class RemotingServerError : Exception
	{
		public RemotingServerError(RemotingErrorInfo info)
		{
            i_message = string.Format("An unknown remoting exception occurred: {0}",
                Regex.Replace(info.Body,@"<(.|\n)*?>", string.Empty ) );
			i_info = info;

		}

		public int Code
		{
			get
			{
				return i_info.Code;
			}
		}

		private string Body
		{
			get
			{
				return i_info.Body;
			}
		}

		public string Description
		{
			get 
			{
				return i_info.Description;
			}
		}

		public override string Message
		{
			get
			{
				return this.i_message;
			}
		}

        public override string ToString()
        {
            return this.Body;
        }


		private string i_message;
		private RemotingErrorInfo i_info;
	}
}
