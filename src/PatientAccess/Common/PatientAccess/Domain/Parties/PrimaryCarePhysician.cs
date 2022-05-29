using System;
using System.Configuration;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class PrimaryCarePhysician : PhysicianRole
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool IsValidFor( Account selectedAccount, Physician physician )
        {
            return true;
        }

        public override PhysicianRole Role()
        {
            return new PhysicianRole( PRIMARYCARE, PRIMARYCARENAME );
        }

        /// <exception cref="InvalidPhysicianAssignmentException"><c>InvalidPhysicianAssignmentException</c>.</exception>
        public override void ValidateFor( Account selectedAccount, Physician physician )
        {
            aPhysician = physician;
            ValidateIfPhysicianIsActive();
            ValidatePatientTypesForOptional( selectedAccount );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void ValidateIfPhysicianIsActive()
        {
            if ( aPhysician.ActiveInactiveFlag != ACTIVE )
            {
                ThrowAnException();
            }
        }

        private static string[] GetPatientTypesForOptional()
        {
            string[] types = {
                string.Empty,
                VisitType.PREREG_PATIENT,
                VisitType.INPATIENT,
                VisitType.OUTPATIENT,
                VisitType.EMERGENCY_PATIENT,
                VisitType.RECURRING_PATIENT ,
                VisitType.NON_PATIENT };
            return types;
        }

        private void ValidatePatientTypesForOptional( Account selectedAccount )
        {
            bool enablePrimaryCarePhysician;
            var primaryCarePhysicianForPreMseFeatureManager = new PrimaryCarePhysicianForPreMseFeatureManager( ConfigurationManager.AppSettings );
            if ( primaryCarePhysicianForPreMseFeatureManager.IsEnabledFor( selectedAccount.AccountCreatedDate ) )
            {
                enablePrimaryCarePhysician = true;
            }
            else
            {
                enablePrimaryCarePhysician = false;
            }

            bool aIsValid = false;
            string[] aPatientTypes = GetPatientTypesForOptional();
            string aPatientTypeCode = selectedAccount.KindOfVisit.Code;

            for ( int i = 0; i < aPatientTypes.Length; i++ )
            {
                if ( aPatientTypeCode == aPatientTypes[i] )
                {
                    if ( IsPreMseActivity( selectedAccount.Activity ) )
                    {
                        aIsValid = enablePrimaryCarePhysician;
                    }
                    else
                    {
                        aIsValid = true;
                    }

                    break;
                }
            }


            if ( !aIsValid )
            {
                ThrowAnException();
            }

        }

        private void ThrowAnException()
        {
            var aException = new InvalidPhysicianAssignmentException
            {
                PhysicianName = aPhysician.Name.DisplayString,
                PhysicianNumber = aPhysician.PhysicianNumber,
                RelationshipType = PRIMARYCARENAME
            };

            throw aException;
        }

        private static bool IsPreMseActivity( Activity activity )
        {
            bool preMSE = false;
            if ( activity != null )
            {
                // if PreMSE
                if (activity.GetType() == typeof (PreMSERegisterActivity) ||
                    activity.GetType() == typeof (EditPreMseActivity) ||
                    activity.GetType() == typeof (UCCPreMSERegistrationActivity) ||
                    activity.GetType() == typeof (EditUCCPreMSEActivity)
                    )
                {
                    preMSE = true;
                }

            }
            return preMSE;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PrimaryCarePhysician()
            : base( PRIMARYCARE, PRIMARYCARENAME )
        {
        }
        #endregion

        #region Data Elements
        private Physician aPhysician;
        #endregion

        #region Constants
        #endregion
    }
}
