using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateEarlierThanOccurrenceDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateEarlierThanOccurrenceDate : LeafRule
    {
        #region Events

        public event EventHandler AdmitDateEarlierThanOccurrenceDateEvent ;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateEarlierThanOccurrenceDateEvent = eventHandler ;
            return true ;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.AdmitDateEarlierThanOccurrenceDateEvent -= eventHandler ;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.AdmitDateEarlierThanOccurrenceDateEvent = null ;
        }

        public override bool ShouldStopProcessing()
        {
            return true ;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || !( context is Account ) )
            {
                return true;
            }

            Acct = context as Account ;

            if ( AccountAndPropertiesValid() )
            {
                return true ;
            }

            return true ;
        }

        #endregion

        #region Properties

        private Account Acct
        {
            get
            {
                return i_Acct ;
            }
            set
            {
                i_Acct = value ;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Responsible for checking whether an account is not null 
        /// and that its properties are valid.
        /// </summary>
        /// <returns></returns>
        private bool AccountAndPropertiesValid()
        {
            return ( Acct != null &&
                            Acct.Activity == null ||
                            Acct.Activity is MaintenanceActivity ) ;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitDateEarlierThanOccurrenceDate()
        {
        }
        #endregion

        #region Data Elements
        private Account i_Acct ;
        #endregion

        #region Constants
        #endregion

    }
}

