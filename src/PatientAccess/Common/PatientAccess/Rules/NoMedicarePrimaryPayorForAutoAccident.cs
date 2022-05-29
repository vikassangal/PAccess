using System;
using System.Configuration;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for NoMedicarePrimaryPayorForAutoAccident.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class NoMedicarePrimaryPayorForAutoAccident : LeafRule
    {
        #region Events
        
        public event EventHandler NoMedicarePrimaryPayorForAutoAccidentEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            NoMedicarePrimaryPayorForAutoAccidentEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            NoMedicarePrimaryPayorForAutoAccidentEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            NoMedicarePrimaryPayorForAutoAccidentEvent = null;   
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( !UseMedicareCannotBePrimaryPayorForAutoAccidentRule )
            {
                return true;
            }

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = ( Account )context ;
            
            if( CannotBeAppliedTo( account.Activity ) )
            {
                return true;
            }

            InsurancePlan modifiedInsurancePlan = AssociatedControl as InsurancePlan;
            if (account.Insurance == null)
            {
                return true;
            }
            if (account.Insurance.PrimaryCoverage == null)
            {
                return true;
            }
            Insurance insurance = account.Insurance;
            Coverage primaryCoverage = insurance.PrimaryCoverage;

            if( primaryCoverage == null && modifiedInsurancePlan == null )
            {
                return true;
            }

            if( modifiedInsurancePlan != null && modifiedInsurancePlan.IsNotMedicareOrMedicaid() )
            {
                return true;
            }

            if( modifiedInsurancePlan == null && primaryCoverage.IsNotMedicareOrMedicaid() )
            {
                return true;
            }

            if( OccurrenceCodeIsNotAutoAccidentEmploymentRelatedOrTortLiability( account ) )
            {
                return true;
            }

            if( FireEvents && NoMedicarePrimaryPayorForAutoAccidentEvent != null )
            {
                NoMedicarePrimaryPayorForAutoAccidentEvent( this, null );
            }

            return false;
        }

        private static bool OccurrenceCodeIsNotAutoAccidentEmploymentRelatedOrTortLiability( Account account )
        {
            bool occurrenceCodeIsNotAutoAccidentEmploymentRelatedOrTortLiability = true;
            Diagnosis diagnosis = account.Diagnosis;
            if( diagnosis != null && diagnosis.Condition != null )
            {
                Accident accidentCondition = diagnosis.Condition as Accident;
                if( accidentCondition != null && accidentCondition.Kind != null && accidentCondition.Kind.OccurrenceCode !=null &&
                    accidentCondition.Kind.OccurrenceCode.IsAutoAccidentOrEmploymentRelatedOrTortLiabilityOccurrenceCode() )
                {
                    occurrenceCodeIsNotAutoAccidentEmploymentRelatedOrTortLiability = false;
                }
            }

            return occurrenceCodeIsNotAutoAccidentEmploymentRelatedOrTortLiability;
        }

        private static bool CannotBeAppliedTo( Activity activity )
        {
            bool cannotBeAppliedTo = true;
            if( activity is PreRegistrationActivity ||
                activity is RegistrationActivity ||
                activity is PostMSERegistrationActivity ||
                activity is ActivatePreRegistrationActivity ||
                activity is MaintenanceActivity ||
                activity is ShortRegistrationActivity ||
                activity is ShortPreRegistrationActivity ||
                activity is ShortMaintenanceActivity ||
                activity is TransferOutToInActivity ||
                activity is PreAdmitNewbornActivity ||
                activity is TransferERToOutpatientActivity ||
                activity is TransferOutpatientToERActivity
              )
            {
                cannotBeAppliedTo = false;
            }

            return cannotBeAppliedTo;
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
        private static bool UseMedicareCannotBePrimaryPayorForAutoAccidentRule
        {
            get
            {
                return useMedicareCannotBePrimaryPayorForAutoAccidentRule;
            }
        }
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        private static bool useMedicareCannotBePrimaryPayorForAutoAccidentRule =
            Boolean.Parse( ConfigurationManager.AppSettings[USE_NO_MEDICARE_FOR_AUTO_ACCIDENT_RULE] );
        #endregion

        #region Constants
        private const string USE_NO_MEDICARE_FOR_AUTO_ACCIDENT_RULE = "UseMedicareCannotBePrimaryPayorForAutoAccidentRule";
        #endregion
    }


}

