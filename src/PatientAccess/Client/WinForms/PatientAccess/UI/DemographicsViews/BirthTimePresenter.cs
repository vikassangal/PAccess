using System;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews
{

    public interface IBirthTimePresenter
    {
        void UpdateField();
        void Validate( MaskedEditTextBox mtbBirthText );
    }

    public class BirthTimePresenter : IBirthTimePresenter
    {

        #region Fields
        #endregion Fields

        #region Constructors

        public BirthTimePresenter( IBirthTimeView view, IBirthTimeFeatureManager birthTimeFeatureManager, IMessageBoxAdapter messageBoxAdapter, IRuleEngine ruleEngine, ITimeBroker timeBroker, IUser user  )
        {
            View = view;
            Account = View.ModelAccount;
            Activity = Account.Activity;
            BirthTimeFeatureManager = birthTimeFeatureManager;
            Patient = Account.Patient;
            RuleEngine = ruleEngine;
            MessageBox = messageBoxAdapter;
            TimeBroker = timeBroker;
            User = user;
        }
        

        #endregion Constructors

        #region Properties

        private IUser User { get; set; }

        private IMessageBoxAdapter MessageBox { get; set; }

        private Account Account { get; set; }

        private Patient Patient { get; set; }

        private Activity Activity { get; set; }

        private IBirthTimeView View { get; set; }

        private IRuleEngine RuleEngine { get; set; }

        private IBirthTimeFeatureManager BirthTimeFeatureManager { get; set; }

        public ITimeBroker TimeBroker { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Updates the visibility and contents of the field based on the feature enable date and visit type
        /// </summary>
        public void UpdateField()
        {
            var isValidActivityForBirthTime = IsValidActivityForBirthTime();
            var isBirthTimeEnabledForDate = BirthTimeFeatureManager.IsBirthTimeEnabledForDate(Account.Activity, Account.AccountCreatedDate);
            var isValidVisitType = IsValidVisitType();

            if ( isValidActivityForBirthTime &&
                isBirthTimeEnabledForDate &&
                IsFieldVisibleForDateOfBirth() &&
                isValidVisitType )
            {
                var isFieldEnabledForDateOfBirth = IsFieldEnabledForDateOfBirth();
                if ( isFieldEnabledForDateOfBirth )
                {
                    View.ShowBirthTimeEnabled();
                }
                
                else
                {
                    View.ShowBirthTimeDisabled();
                }
                
                View.PopulateBirthTime( Patient.DateOfBirth );
            }

            else
            {
                View.DisableAndHideBirthTime();
            }

            EvaluateBirthTimeRequired();
            EvaluateBirthTimePreferred();
        }

        private bool IsFieldEnabledForDateOfBirth()
        {
            if ( View.ModelAccount != null && View.ModelAccount.BirthTimeIsEntered )
                return true;

            return ( Patient.DateOfBirth.TimeOfDay <= TimeSpan.Zero ||
                       Patient.DateOfBirth.TimeOfDay == TimeSpan.MinValue );
        }

        public void Validate( MaskedEditTextBox mtbBirthTime )
        {
            int hour = 0;
            int minute = 0;
            string birthTimeText = mtbBirthTime.UnMaskedText.Trim();
            if ( birthTimeText.Trim() == "0000" )
            {
                UIColors.SetErrorBgColor( mtbBirthTime );
                MessageBox.ShowMessageBox( UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG,
                                                 UIErrorMessages.TIME,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );
                mtbBirthTime.Focus();
                return;
            }
            if ( birthTimeText != string.Empty )
            {
                if ( DateValidator.IsValidTime( mtbBirthTime , UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG , UIErrorMessages.TIME, MessageBox) == false )
                {
                    mtbBirthTime.Focus();
                    return;
                }

                if ( birthTimeText.Length == 4 )
                {
                    hour = Convert.ToInt32( birthTimeText.Substring( 0, 2 ) );
                    minute = Convert.ToInt32( birthTimeText.Substring( 2, 2 ) );
                }
                //SR 1557 do not check birthtime is future for Pre-Admit Newborn / Edit Pre-Admit Newborn
                if ( !Activity.IsPreAdmitNewbornActivity() &&
                        !( Activity.IsMaintenanceActivity() && Activity.AssociatedActivityType != null
                            && Activity.AssociatedActivityType == typeof( PreAdmitNewbornActivity ) ) )
                {
                    CheckBirthTimeIsNotInFuture(hour, minute);
                }

                switch ( BirthTimeComparedToAdmitTime( hour, minute ))
                {
                    case 1:
                        UIColors.SetErrorBgColor( mtbBirthTime );
                        MessageBox.ShowMessageBox( UIErrorMessages.BIRTH_TIME_CANNOT_BE_AFTER_ADMIT_TIME,
                                                UIErrorMessages.BIRTH_TIME,
                                                MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                MessageBoxAdapterDefaultButton.Button1 );
                        mtbBirthTime.Focus();
                        return;
                            
                    case -1:
                        if ( View.ModelAccount.Activity != null && View.ModelAccount.Activity.IsAdmitNewbornActivity() )
                        {

                            var result = MessageBox.ShowMessageBox(UIErrorMessages.BIRTH_TIME_BEFORE_ADMIT_TIME,
                                                                   UIErrorMessages.BIRTH_TIME,
                                                                   MessageBoxAdapterButtons.YesNo,
                                                                   MessageBoxAdapterIcon.Warning,
                                                                   MessageBoxAdapterDefaultButton.Button2);
                            if (result == MessageBoxAdapterResult.No)
                            {
                                UIColors.SetErrorBgColor(mtbBirthTime);
                                mtbBirthTime.Focus();
                                return;
                            }
                        }
                        break;
                }
                    
            }

            if ( ( hour >= 0 && hour <= 23 ) && ( minute >= 0 && minute <= 59 ) )
            {
                var patientDateOfBirth = Patient.DateOfBirth;
                int existingPatientDateOfBirthHour = patientDateOfBirth.Hour;
                int existingPatientDateOfBirthMinute = patientDateOfBirth.Minute;

                var birthTime = new DateTime( patientDateOfBirth.Year, patientDateOfBirth.Month, patientDateOfBirth.Day, hour, minute, 0 );

                if ( HasBirthTimeChanged( existingPatientDateOfBirthHour, existingPatientDateOfBirthMinute, hour,minute) 
                    && (Activity.IsMaintenanceActivity() || Activity.IsEditPreMseActivity()) )
                {
                    MessageBoxAdapterResult action = ShowEditBirthTimeWarning();

                    if ( action == MessageBoxAdapterResult.No )
                    {
                        View.PopulateBirthTime( View.ModelAccount.Patient.DateOfBirth );
                    }
                    else
                    {
                        View.ModelAccount.BirthTimeIsEntered = true;
                        Patient.DateOfBirth = birthTime;
                    }
                }

                else
                {
                    View.ModelAccount.BirthTimeIsEntered = true;
                    Patient.DateOfBirth = birthTime;
                }

            }

            EvaluateBirthTimeRequired();
            EvaluateBirthTimePreferred();
        }

        private int BirthTimeComparedToAdmitTime( int hour, int minute )
        {
            var birthTime = new DateTime( View.ModelAccount.Patient.DateOfBirth.Year, View.ModelAccount.Patient.DateOfBirth.Month, 
                View.ModelAccount.Patient.DateOfBirth.Day, hour, minute, 0 );
            if(birthTime>View.ModelAccount.AdmitDate)
                return 1;
            else if(birthTime<View.ModelAccount.AdmitDate)
            {
                return -1;
            }
            return 0;
        }

        
        private bool IsEditPostMseActivity()
        {
            return Activity.IsMaintenanceActivity() && Activity.AssociatedActivityType!=null && Activity.AssociatedActivityType==typeof(PostMSERegistrationActivity);
        }

        private MessageBoxAdapterResult ShowEditBirthTimeWarning()
        {
            return MessageBox.ShowMessageBox( UIErrorMessages.EDIT_BIRTHTIME_MESSAGE,
                                                "Patient's Birth Time",
                                                MessageBoxAdapterButtons.YesNo, MessageBoxAdapterIcon.Warning,
                                                MessageBoxAdapterDefaultButton.Button2 );
        }

        #endregion Public Methods

        #region Private Methods
        private bool HasBirthTimeChanged( int existingPatientDateOfBirthHour, int existingPatientDateOfBirthMinute, int hour, int minute )
        {
            if  ( existingPatientDateOfBirthHour != hour )
            {
                return true;
            }
            else
            {
                if(existingPatientDateOfBirthMinute != minute)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsValidVisitType()
        {
            var visitTypeCode = Account.KindOfVisit.Code;
            return (visitTypeCode == VisitType.PREREG_PATIENT && Account.IsNewBorn )||
                   visitTypeCode == VisitType.INPATIENT ||
                   visitTypeCode == VisitType.OUTPATIENT ||
                   visitTypeCode == VisitType.EMERGENCY_PATIENT ||
                   visitTypeCode == VisitType.RECURRING_PATIENT;
        }

        private bool IsFieldVisibleForDateOfBirth()
        {
            //SR 1557, always display Birth Time for Create/Edit/Activate Pre-Admit Newborn
            if ( IsPreAdmitNewbornActivities( View.ModelAccount.Activity ) )
                return true;
            var dateOfBirth = Patient.DateOfBirth;
            var admitDate = View.ModelAccount.AdmitDate;
            if (dateOfBirth.Date == DateTime.MinValue ||
                admitDate.Date == DateTime.MinValue)
            {
                return false;
            }
             
            return ( dateOfBirth.Date >= View.ModelAccount.AdmitDate.AddDays( -10 ).Date );
        }

        private DateTime GetCurrentFacilityDateTime()
        {
            return TimeBroker.TimeAt( User.Facility.GMTOffset, User.Facility.DSTOffset );
        }

        private bool IsValidActivityForBirthTime()
        {
            return Activity.IsNewBornRelatedActivity() ||
                   ( Activity.IsRegistrationActivity() && !Activity.IsActivatePreRegisterActivity() ) ||
                   Activity.IsMaintenanceActivity() ||
                   Activity.IsPostMSEActivity() ||
                   Activity.IsPreMSEActivities() ||
                   Activity.IsUCCPreMSEActivity() ||
                   Activity.IsUccPostMSEActivity() ||
                   Activity.IsEditUCCPreMSEActivity() ;
        }

        private void CheckBirthTimeIsNotInFuture( int hour, int minute )
        {

            DateTime tmp = GetCurrentFacilityDateTime();
            int facilityHour = tmp.Hour;
            int facilityMinute = tmp.Minute;

            if ( IsDateOfBirthTodaysDate() && IsTimeInTheFuture( facilityHour, facilityMinute, hour, minute ) )
            {
                MessageBox.ShowMessageBox( UIErrorMessages.BIRTH_TIME_CANNOT_BE_IN_FUTURE,
                                                 "Error",
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );

            }
        }

        private static bool IsTimeInTheFuture( int facilityHour, int facilityMinute, int enteredHour, int enteredMinute )
        {
            int todaysTotalMinutes = ( facilityHour * 60 ) + facilityMinute;
            int enteredTotalMinutes = ( enteredHour * 60 ) + enteredMinute;

            return enteredTotalMinutes > todaysTotalMinutes;
        }

        private bool IsDateOfBirthTodaysDate()
        {
            return Patient.DateOfBirth.Date.Equals( DateTime.Today.Date );
        }

        private void EvaluateBirthTimeRequired()
        {
            RuleEngine.OneShotRuleEvaluation<BirthTimeRequired>( View.ModelAccount, BirthTimeRequiredEvent );
        }

        private void EvaluateBirthTimePreferred()
        {
            RuleEngine.OneShotRuleEvaluation<BirthTimePreferred>( View.ModelAccount, BirthTimePreferredEvent );
        }
        private void BirthTimeRequiredEvent( object sender, EventArgs e )
        {
            View.MakeBirthTimeRequired();
        }

        private void BirthTimePreferredEvent( object sender, EventArgs e )
        {
            View.MakeBirthTimePreferred();
        }

        private bool IsPreAdmitNewbornActivities(Activity activity)
        {
            // for Create/Edit/Activate Pre-Admit Newborn
            if ( activity != null &&
                ( activity.IsPreAdmitNewbornActivity()
                || ( activity.IsAdmitNewbornActivity() &&
                    activity.AssociatedActivityType != null &&
                        activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
                || ( activity.IsMaintenanceActivity() &&
                    activity.AssociatedActivityType != null &&
                        activity.AssociatedActivityType == typeof( PreAdmitNewbornActivity ) ) ) )
                return true;
            return false;
        }

        #endregion Private Methods
    }
}