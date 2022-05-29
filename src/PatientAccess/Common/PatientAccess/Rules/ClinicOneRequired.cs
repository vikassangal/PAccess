using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for ClinicOneRequired.
	/// </summary>
	//TODO: Create XML summary comment for ClinicOneRequired
	[Serializable]
	[UsedImplicitly]
    public class ClinicOneRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler ClinicOneRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.ClinicOneRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.ClinicOneRequiredEvent -= eventHandler;
			return true;
		}
                                
        public override void UnregisterHandlers()
        {
            this.ClinicOneRequiredEvent = null;  
        }

		public override bool CanBeAppliedTo(object context)
		{
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	
            
            Account acct = (Account)context;

            if( acct.KindOfVisit != null
                && acct.KindOfVisit.Code == VisitType.INPATIENT  )
            {
                return true;
            }

            if( acct.HospitalClinic == null 
                || acct.HospitalClinic.Code == null
                || acct.HospitalClinic.Code.Trim() == string.Empty)
            {
                if( this.FireEvents && ClinicOneRequiredEvent != null )
                {
                    this.ClinicOneRequiredEvent(this, null);
                }
            
                return false;
            }
            else
            {
                return true;
            } 			                   
		}

		public override void ApplyTo(object context)
		{
            
		}

		public override bool ShouldStopProcessing()
		{
			return false;
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ClinicOneRequired()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
