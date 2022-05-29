using System;
using System.Collections;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for HawkAlertRecommendation.
	/// </summary>
	/// 
	[Serializable]
	public class HawkAlertRecommendation
	{
		
		#region Event Handlers
		#endregion

		#region Methods

		#endregion

		#region Properties

	    private string Title
		{
			get
			{
				return i_Title;
			}
			set
			{
				i_Title = value;
			}
		}

		public ArrayList Steps
		{
			get
			{
				return i_Steps;
			}
			set
			{
				i_Steps = value;
			}
		}

		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public HawkAlertRecommendation( string title)
		{
			this.Title = title;
			i_Steps = new ArrayList( );
		}
		#endregion

		#region Data Elements

		string i_Title = string.Empty;
		ArrayList i_Steps = null;

		#endregion

		#region Constants
		#endregion
	}
}
