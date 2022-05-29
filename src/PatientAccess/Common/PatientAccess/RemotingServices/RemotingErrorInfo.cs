namespace PatientAccess.RemotingServices
{
	/// <summary>
	/// Summary description for FormatErrorInfo.
	/// </summary>
	public struct RemotingErrorInfo
	{
		public int Code;
		public string Description;
		public string Body;

		public override bool Equals(object obj)
		{
			if(obj is RemotingErrorInfo)
			{
				RemotingErrorInfo arg = (RemotingErrorInfo)obj;
				if(this.Code == arg.Code &&
					this.Body == arg.Body &&
					this.Description == arg.Description)
				{
					return true;
				} 
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

	}
}
