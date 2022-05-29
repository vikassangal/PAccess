using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AccomodationRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidAccomodationCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidAccomodationCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidAccomodationCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidAccomodationCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidAccomodationCodeChangeEvent = null;   
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

            if( ( account != null ) &&  (account.Location  != null )
                &&  (account.Location.Bed  != null ) &&  (account.Location.Bed.Accomodation != null ))
            {
                if( account.Activity.GetType().Equals( typeof( PreRegistrationActivity ) ) ||
                    account.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) ||
                    account.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) ||
                    ( account.Activity.GetType().Equals( typeof( RegistrationActivity ) ) &&
                    account.KindOfVisit.Code != VisitType.INPATIENT ) ||
                    ( account.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) ) )
                {
                    return true;
                }

                if(!(account.Location.Bed.Accomodation.IsValid))
                          
                {
                    if( this.FireEvents && InvalidAccomodationCodeChangeEvent != null )
                    {
                        this.InvalidAccomodationCodeChangeEvent(this, null);
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

        public InvalidAccomodationCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

