using System;
using System.Configuration;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ModeOfArrivalRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ModeOfArrivalRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler ModeOfArrivalRequiredEvent;
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ModeOfArrivalRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ModeOfArrivalRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            ModeOfArrivalRequiredEvent = null;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;

            if ( anAccount == null )
            {
                return false;
            }

            if ( anAccount.Facility != null && IsFacilityInCalifornia( anAccount.Facility ) &&
                 IsModeOfArrivalRequiredForDate( anAccount.AccountCreatedDate ) &&
                 IspatientTypeValidForModeOfArrivalRequired( anAccount.KindOfVisit ) &&
                 anAccount.ModeOfArrival != null && anAccount.ModeOfArrival.Code.Equals( string.Empty )
               )
            {
                if ( FireEvents && ModeOfArrivalRequiredEvent != null )
                {
                    ModeOfArrivalRequiredEvent( this, null );
                }

                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods
        private static bool IsFacilityInCalifornia( Facility facility )
        {
            return facility.IsFacilityInState( State.CALIFORNIA_CODE );
        }

        private static bool IspatientTypeValidForModeOfArrivalRequired( VisitType kindOfVisit )
        {
            return kindOfVisit.Code.Equals( VisitType.EMERGENCY_PATIENT );
        }

        private bool IsModeOfArrivalRequiredForDate( DateTime date )
        {
            if ( ( date == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                 ( date >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }
        private DateTime FeatureStartDate
        {
            get
            {
                if ( ModeOfArrivalRequiredFeatureStartDate.Equals( DateTime.MinValue ) )
                {

                    ModeOfArrivalRequiredFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[MODEOFARRIVAL_REQUIRED_START_DATE] );
                }
                return ModeOfArrivalRequiredFeatureStartDate;
            }
        }


        #endregion

        #region Private Properties

        private DateTime ModeOfArrivalRequiredFeatureStartDate = DateTime.MinValue;

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string MODEOFARRIVAL_REQUIRED_START_DATE = "MODEOFARRIVALREQUIRED_START_DATE";

        #endregion
    }
}
