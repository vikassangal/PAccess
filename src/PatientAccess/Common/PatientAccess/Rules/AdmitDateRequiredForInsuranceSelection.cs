using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateRequiredForInsuranceSelection.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateRequiredForInsuranceSelection : LeafRule
    {
        #region Events
        public event EventHandler AdmitDateRequiredForInsuranceSelectionEvent;
        
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateRequiredForInsuranceSelectionEvent += eventHandler;
            
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateRequiredForInsuranceSelectionEvent -= eventHandler;
           
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateRequiredForInsuranceSelectionEvent = null;
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null
                || context.GetType() != typeof( Account ) 
                || this.AssociatedControl == null)
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null)
            {
                return false;
            }

            string strName = this.AssociatedControl as string;
            ContextActivity = anAccount.Activity;
            if( strName != string.Empty
                && strName == FIND_A_PLAN
                && anAccount.AdmitDate.Date.Equals( DateTime.MinValue.Date ) )
            {
                if( this.FireEvents && AdmitDateRequiredForInsuranceSelectionEvent != null )
                {

                    {
                        AdmitDateRequiredForInsuranceSelectionEvent(this, null);
                    }

                }
                return false;
            }

            return true;
        }
        #endregion

        #region Properties

        public  Activity ContextActivity;
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public AdmitDateRequiredForInsuranceSelection()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string FIND_A_PLAN = "findAPlanView";

        #endregion
    }


}
