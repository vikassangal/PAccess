using System;
using Extensions.UI.Builder; 
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BirthTimeRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BirthTimeRequired : LeafRule
    {
        [NonSerialized]
        private BirthTimeFeatureManager _birthTimeFeatureManager;

        private BirthTimeFeatureManager BirthTimeFeatureManager
        {
            get { return _birthTimeFeatureManager; }
            set { _birthTimeFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler BirthTimeRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BirthTimeRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BirthTimeRequiredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            BirthTimeRequiredEvent = null;   
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
            bool result = true;

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  

            var anAccount = context as Account;
            if( anAccount == null || anAccount.Activity == null || anAccount.Patient == null || anAccount.KindOfVisit == null )
            {
                return true;
            }
            BirthTimeFeatureManager = new BirthTimeFeatureManager();
            if (! BirthTimeFeatureManager.IsBirthTimeEnabledForDate(anAccount.Activity, anAccount.AccountCreatedDate))
            {
                return true;
            }
            if (   IsBirthTimeEnabled( anAccount ) && 
                anAccount.Patient.DateOfBirth.TimeOfDay <= TimeSpan.Zero  || anAccount.Patient.DateOfBirth.TimeOfDay == TimeSpan.MinValue  )
            {
                if( FireEvents && BirthTimeRequiredEvent != null )
                {
                    BirthTimeRequiredEvent( this, null );
                }
                result = false;
            }
            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private bool IsBirthTimeEnabled(Account account)
        {
            //SR 1557 BirthTime is always enabled for Pre-Admit Newborn
            if ( account.Activity != null &&
                    ( account.Activity.IsPreAdmitNewbornActivity()
                    || ( account.Activity.GetType() == typeof( AdmitNewbornActivity ) &&
                        account.Activity.AssociatedActivityType != null && account.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) ) ) )
                return true;

            return IsValidActivityForBirthTime(account.Activity) &&
                   BirthTimeFeatureManager.IsBirthTimeEnabledForDate(account.Activity,account.AccountCreatedDate) &&
                   IsBirthTimeVisibleForDOB( account.Patient.DateOfBirth, account.AdmitDate ) &&
                   IsValidVisitType( account.KindOfVisit.Code );
        }
        private static bool IsValidVisitType(string visitTypeCode)
        {
             
            return visitTypeCode == VisitType.INPATIENT ||
                   visitTypeCode == VisitType.OUTPATIENT ||
                   visitTypeCode == VisitType.EMERGENCY_PATIENT ||
                   visitTypeCode == VisitType.RECURRING_PATIENT;
        }

        public bool IsBirthTimeVisibleForDOB(DateTime DOB, DateTime admitDate)
        {

            if (DOB == DateTime.MinValue ||
                admitDate == DateTime.MinValue )
            {
                return false;
            }
            return (DOB.Date >= admitDate.AddDays(-10).Date);
        }
         
        private static bool IsValidActivityForBirthTime(Activity activity)
        {
            return activity.IsAdmitNewbornActivity() ||
                   (activity.IsRegistrationActivity() && !activity.IsActivatePreRegisterActivity()) ||
                   activity.IsMaintenanceActivity() ||
                   activity.IsPreMSEActivities() ||
                   activity.IsPostMSEActivity() ||
                   activity.IsUCCPreMSEActivity() ||
                   activity.IsUccPostMSEActivity() ||
                   activity.IsEditUCCPreMSEActivity();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
