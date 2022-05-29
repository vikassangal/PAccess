using System;
using System.Configuration;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for TimeoutBroker.
	/// </summary>
	[Serializable]
	public class TimeoutBroker : MarshalByRefObject, ITimeoutBroker
	{
		#region Event Handlers
		#endregion
	
		#region Methods
        public ActivityTimeout TimeoutFor()
        {
            ActivityTimeout activityTimeout = new ActivityTimeout();
            activityTimeout.FirstAlertTime = Convert.ToInt32( ConfigurationManager.AppSettings[ACTIVITY_TIMEOUT_FIRST_ALERT] );
            activityTimeout.SecondAlertTime = Convert.ToInt32( ConfigurationManager.AppSettings[ACTIVITY_TIMEOUT_SECOND_ALERT] );
            return activityTimeout;
        }
		#endregion
	
		#region Properties
		#endregion
	
		#region Private Methods
		#endregion
	
		#region Private Properties
		#endregion
	
		#region Construction and Finalization
        public TimeoutBroker()
            : base()
        {
        }
		#endregion
	
		#region Data Elements
		#endregion
	
		#region Constants
        private const string ACTIVITY_TIMEOUT_FIRST_ALERT     = "ACTIVITY_TIMEOUT_FIRST_ALERT",
                             ACTIVITY_TIMEOUT_SECOND_ALERT    = "ACTIVITY_TIMEOUT_SECOND_ALERT";
		#endregion

	}
}

