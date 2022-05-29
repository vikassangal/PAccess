using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for Timeout.
	/// </summary>
	[Serializable]
	public class ActivityTimeout
	{
		#region Event Handlers
		#endregion
	
		#region Methods

		#endregion
	
		#region Properties
        /// <summary>
        /// Time for Timeout in milliseconds
        /// </summary>
        public int SecondAlertTime
        {
            get
            {
                return i_SecondAlertTime;
            }
            set
            {
                i_SecondAlertTime = value;
            }
        }
        /// <summary>
        /// FirstAlertTime for timeout in milliseconds
        /// </summary>
        public int FirstAlertTime
        {
            get
            {
                return i_FirstAlertTime;
            }
            set
            {
                i_FirstAlertTime = value;
            }
        }
		#endregion
	
		#region Private Methods
		#endregion
	
		#region Private Properties
		#endregion
	
		#region Construction and Finalization
		public ActivityTimeout()
		{
		}
		#endregion
	
		#region Data Elements
        private int i_FirstAlertTime;
        private int i_SecondAlertTime;
		#endregion
	
		#region Constants
		#endregion

	}
}

