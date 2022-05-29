using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage : LeafRule
    {
        public MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage()
        {
            //this saves us from checking for null for every operation on the event
            RuleViolatedEvent = new EventHandler( delegate { } );
        }

        public bool IsValidActivity( Activity activity )
        {
            return ( activity is PreRegistrationActivity ||
                    activity is RegistrationActivity ||
                    activity is PostMSERegistrationActivity ||
                    activity is ActivatePreRegistrationActivity ||
                    activity is MaintenanceActivity ||
                    activity is TransferOutToInActivity ||
                    activity is AdmitNewbornActivity ||
                    activity is PreAdmitNewbornActivity ||
                    activity is RegistrationWithOfflineActivity ||
                    activity is AdmitNewbornWithOfflineActivity ||
                    activity is PreAdmitNewbornWithOfflineActivity||
                    activity is TransferOutpatientToERActivity||
                    activity is TransferERToOutpatientActivity
                   );
        }

        /// <exception cref="ArgumentException">If context is of type other than <see cref="Account"/> </exception>
        public override bool CanBeAppliedTo( object context )
        {
            Guard.ThrowIfArgumentIsNull( context, "context" );
            if ( context.GetType() != typeof( Account ) )
            {
                throw new ArgumentException( "Invalid context type, this rule only applies to objects of type Account ", "context" );
            }

            Account account = (Account)context;

            bool isValidActivity = IsValidActivity( account.Activity );

            bool isNotInpatient = IsNotInpatient( account );

            bool isMediCarePrimaryPayor = IsMediCarePrimaryPayor( account );

            bool isMedicarePartACoverageBlankOrYes = IsMedicarePartACoverageBlankOrYes( account );

            bool canBeAppliedTo = !isValidActivity || isNotInpatient || !isMediCarePrimaryPayor || isMedicarePartACoverageBlankOrYes;

            bool ruleIsViolated = !canBeAppliedTo;

            if ( FireEvents && ruleIsViolated )
            {
                InvokeRuleViolatedEvent();
            }

            return canBeAppliedTo;
        }

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            RuleViolatedEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            RuleViolatedEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            RuleViolatedEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        private static bool IsMediCarePrimaryPayor( Account account )
        {
            return account.Insurance.PrimaryCoverage != null &&
                   account.Insurance.PrimaryCoverage is GovernmentMedicareCoverage;
        }

        private static bool IsNotInpatient( IAccount account )
        {
            if ( account.KindOfVisit == null )
            {
                return true;
            }

            return !account.KindOfVisit.IsInpatient;
        }

        private static bool IsMedicarePartACoverageBlankOrYes( Account account )
        {
            GovernmentMedicareCoverage coverage = account.Insurance.PrimaryCoverage as GovernmentMedicareCoverage;
            return coverage == null || 
                   coverage.PartACoverage.Code != YesNoFlag.CODE_NO ;
        }

        private event EventHandler RuleViolatedEvent;

        private void InvokeRuleViolatedEvent()
        {
            EventHandler myEvent = RuleViolatedEvent;
            myEvent( this, EventArgs.Empty );
        }
    }
}
