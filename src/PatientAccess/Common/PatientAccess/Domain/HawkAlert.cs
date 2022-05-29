using System;

namespace PatientAccess.Domain
{
	[Serializable]
	public class HawkAlert
	{

	
		#region Event Handlers
		#endregion

		#region Methods

		#endregion

		#region Properties

	    private string Message
		{
			get
			{
				return i_Message;
			}
			set
			{
				i_Message = value;
			}
		}

	    private string Code
		{
			get
			{
				return i_Code;
			}
			set
			{
				i_Code = value;
			}
		}

	    private HawkAlertRecommendation Recommendation
		{
			get
			{
				return i_Recommendation;
			}
			set
			{
				i_Recommendation = value;
			}
		}

		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public HawkAlert( string code, string message ) : this( code, message, null )
		{

		}

		public HawkAlert( string code, string message, HawkAlertRecommendation recommendation )
		{
			this.Code = code;
			this.Message = message;
			this.Recommendation = recommendation;
		}
		#endregion

		#region Data Elements

		string i_Message = string.Empty;
		string i_Code = string.Empty;
		HawkAlertRecommendation i_Recommendation = null;
		#endregion

		#region Constants
		#endregion
	}
}
