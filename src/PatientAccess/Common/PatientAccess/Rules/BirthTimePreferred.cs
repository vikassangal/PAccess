using System;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BirthTimePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BirthTimePreferred : LeafRule
    {
        [NonSerialized]
        private BirthTimeFeatureManager _birthTimeFeatureManager;

        internal BirthTimeFeatureManager BirthTimeFeatureManager
        {
            get { return _birthTimeFeatureManager; }
            set { _birthTimeFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler BirthTimePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BirthTimePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BirthTimePreferredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            BirthTimePreferredEvent = null;   
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
            if (! BirthTimeFeatureManager.IsBirthTimeEnabledForDate(anAccount.AccountCreatedDate))
            {
                return true;
            }
            if (   IsBirthTimeEnabled( anAccount ) && 
                anAccount.Patient.DateOfBirth.TimeOfDay <= TimeSpan.Zero  || anAccount.Patient.DateOfBirth.TimeOfDay == TimeSpan.MinValue  )
            {
                if ( FireEvents && BirthTimePreferredEvent != null )
                {
                    BirthTimePreferredEvent( this, null );
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
            //SR 1557 always display Birth Time for Create/Edit/Activate Pre-Admit Newborn
            if ( account.Activity !=null && 
                    (account.Activity.IsPreAdmitNewbornActivity() 
                    ||( account.Activity.GetType()==typeof(AdmitNewbornActivity) &&
                        account.Activity.AssociatedActivityType!=null && 
                        account.Activity.AssociatedActivityType==typeof(ActivatePreRegistrationActivity))
                    || ( account.Activity.IsMaintenanceActivity() &&
                        account.Activity.AssociatedActivityType != null &&
                        account.Activity.AssociatedActivityType == typeof( PreAdmitNewbornActivity ) ) ) )
                return true;

            return IsValidActivityForBirthTime(account.Activity) &&
                   BirthTimeFeatureManager.IsBirthTimeEnabledForDate(account.AccountCreatedDate) &&
                   IsBirthTimeVisibleForDOB( account.Patient.DateOfBirth, account.AdmitDate ) &&
                   IsValidVisitType( account.KindOfVisit.Code );
        }
        private static bool IsValidVisitType(string visitTypeCode)
        {

            return visitTypeCode == VisitType.PREREG_PATIENT ||
                   visitTypeCode == VisitType.INPATIENT ||
                   visitTypeCode == VisitType.OUTPATIENT ||
                   visitTypeCode == VisitType.EMERGENCY_PATIENT ||
                   visitTypeCode == VisitType.RECURRING_PATIENT;
        }
        private static  bool IsBirthTimeVisibleForDOB(DateTime DOB,DateTime admitTime)
        {
            
            if ( DOB == DateTime.MinValue )
            {
                return false;
            }
           
            return ( DOB.Date >= admitTime.AddDays( -10 ).Date );
        }
        private static DateTime GetCurrentFacilityDateTime( int gmtOffset, int dstOffset )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }
        private static bool IsValidActivityForBirthTime(Activity activity)
        {
            return activity.IsAdmitNewbornActivity() || activity.IsPreAdmitNewbornActivity() ||
                  ( activity.IsRegistrationActivity() && !activity.IsActivatePreRegisterActivity() ) ||
                   activity.IsMaintenanceActivity();
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
