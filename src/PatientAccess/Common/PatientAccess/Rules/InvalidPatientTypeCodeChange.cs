using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidPatientTypeCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidPatientTypeCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidPatientTypeCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidPatientTypeCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidPatientTypeCodeChangeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;
                                    
            if( account.DischargeDate != DateTime.MinValue )
            {
                return true;
            }

            if( ( account != null ) &&  (account.KindOfVisit != null )&&  (account.Activity != null ) &&  (account.FinancialClass != null ) )
            {
                if( typeof( PreRegistrationActivity ).Equals(account.Activity.GetType() ) || 
                    typeof( PostMSERegistrationActivity ).Equals( account.Activity.GetType() ) ||
                    typeof( AdmitNewbornActivity ).Equals( account.Activity.GetType() ) ||
                    ( typeof( MaintenanceActivity ).Equals( account.Activity.GetType() ) &&
                    ( account.KindOfVisit.Code == VisitType.PREREG_PATIENT || 
                    account.KindOfVisit.Code == VisitType.INPATIENT ||
                    account.KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                    account.KindOfVisit.Code == VisitType.NON_PATIENT ) ||
                    ( account.KindOfVisit.Code == VisitType.OUTPATIENT 
                    && account.Location != null
                    && account.Location.FormattedLocation != String.Empty )||
                    (  account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT 
                    &&  account.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) ) ) )
                {
                    return true;
                }
                if(!(account.KindOfVisit.IsValid))
                              
                {
                    if( this.FireEvents && InvalidPatientTypeCodeChangeEvent != null )
                    {
                        this.InvalidPatientTypeCodeChangeEvent(this, null);
                    }
                
                    return false ;
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
            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public InvalidPatientTypeCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

