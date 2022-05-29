using System;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactNameRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactInfo : CompositeRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {           
            return true;
        }
                        
        public override void UnregisterHandlers()
        {
                
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this method determines the base requirements for all contact rules.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            // if we've made it this far, we are in the Registration activity

            if( this.account == null )
            {
                return true;
            }

            HospitalService hs = this.account.HospitalService;

            EmergencyContact anEC = context as EmergencyContact;

            // if activity is registration, optional for these service codes

            if( ( this.account.Activity.GetType() == typeof( RegistrationActivity )
                    || this.account.Activity.GetType() == typeof( PreRegistrationActivity ) )
                && (!hs.Code.Equals("SP") && !hs.Code.Equals("CV") && !hs.Code.Equals("LB") && !hs.Code.Equals("AB")
                    && !this.account.KindOfVisit.Code.Equals("PT") ) )
            {
                return true;
            }
            else // required
            {                
                return false;             
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        private Account account
        {
            get
            {
                if( i_account == null )
                {
                    IAccountView accountView = ActionHelper.LoadAccountView();

                    if( accountView != null )
                    {                        
                        i_account = accountView.GetModel();
                    }
                }
               
                return i_account;
            }
        }
        #endregion

        #region Construction and Finalization
        public ContactInfo()
        {
        }
        #endregion

        #region Data Elements
        
        Account i_account;

        #endregion

        #region Constants
        #endregion
    }
}
