using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for DiagnosisClinicOneRequired.
	/// </summary>
	//TODO: Create XML summary comment for DiagnosisClinicOneRequired
	[Serializable]
	[UsedImplicitly]
    public class DiagnosisClinicOneRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler DiagnosisClinicOneRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.DiagnosisClinicOneRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.DiagnosisClinicOneRequiredEvent -= eventHandler;
			return true;
		}
                
        public override void UnregisterHandlers()
        {
            this.DiagnosisClinicOneRequiredEvent = null;   
        }

		public override bool CanBeAppliedTo(object context)
		{
			if( context == null || 
				context.GetType() != typeof( Account ) )
			{                
				return true;
			} 	

            Account acct = (Account)context;

            if( acct == null && acct.KindOfVisit == null )
            {
                return true;
            }

            if( acct.KindOfVisit != null
                && acct.KindOfVisit.Code != VisitType.INPATIENT )
            {
                if( ((Account)context).HospitalClinic == null 
                    || ((Account)context).HospitalClinic.Code == null
                    || ((Account)context).HospitalClinic.Code.Trim() == string.Empty  )
                {
                    if( this.FireEvents && DiagnosisClinicOneRequiredEvent != null )
                    {
                        this.DiagnosisClinicOneRequiredEvent(this, null);
                    }
            
                    return false;
                }
                else
                {
                    return true;
                } 			                   
                
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
		public DiagnosisClinicOneRequired()
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
