using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    internal class QuickAccountCreationPresenter : IQuickAccountCreationPresenter
    {
        #region Constructors

        internal QuickAccountCreationPresenter( IQuickAccountCreationView view, IMessageBoxAdapter messageBoxAdapter, Account modelAccount, IRuleEngine ruleEngine )
        {
            View = view;
            MessageBoxAdapter = messageBoxAdapter;
            Account = modelAccount;
            RuleEngine = ruleEngine;
        }

        #endregion Constructors

        #region Properties

        private IQuickAccountCreationView View { get; set; }
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }
        private Account Account { get; set; }
        private IRuleEngine RuleEngine { get; set; }

        #endregion Properties

        #region Public Methods

        public bool UpdateDateOfBirth()
        {
            var mtbDateOfBirth = View.DateOfBirth;
            var mtbDateOfBirthText = View.DateOfBirthText;
            if ( mtbDateOfBirth == string.Empty || mtbDateOfBirth == "01010001" )
            {
                View.SetNormalColorForDateOfBirth();
                View.Age = String.Empty;
                Account.Patient.DateOfBirth = DateTime.MinValue;
                View.SSNView.ResetSSNControl();
                RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Account );
                return true;
            }

            View.Age = String.Empty;

            if ( mtbDateOfBirthText.Length != 10 )
            {   // Prevent cursor from advancing to the next control

                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.DOB_INCOMPLETE_ERRMSG,
                                                 UIErrorMessages.ERROR,
                                                 MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                 MessageBoxAdapterDefaultButton.Button1 );

                return false;
            }

            try
            {   // Check the date entered is not in the future
                var dateOfBirthEntered = new DateTime( Convert.ToInt32( mtbDateOfBirthText.Substring( 6, 4 ) ),
                    Convert.ToInt32( mtbDateOfBirthText.Substring( 0, 2 ) ),
                    Convert.ToInt32( mtbDateOfBirthText.Substring( 3, 2 ) ) );

                if ( dateOfBirthEntered > DateTime.Today )
                {
                    MessageBoxAdapter.ShowMessageBox( UIErrorMessages.DOB_FUTURE_ERRMSG,
                                               UIErrorMessages.ERROR,
                                               MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                               MessageBoxAdapterDefaultButton.Button1 );
                    return false;
                }

                if ( DateValidator.IsValidDate( dateOfBirthEntered ) == false )
                {
                    MessageBoxAdapter.ShowMessageBox( UIErrorMessages.DOB_NOTVALID_ERRMSG,
                                                      UIErrorMessages.ERROR,
                                                      MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                      MessageBoxAdapterDefaultButton.Button1 );
                    return false;
                }

                Account.Patient.DateOfBirth = dateOfBirthEntered;
                UpdateConditionCode( dateOfBirthEntered );

                SetDateOfBirthAndAge();
                RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Account );
                RuleEngine.EvaluateRule( typeof( InValidDateOfBirth ), Account );
                // Redirect user to Insurance tab if the patient is over 65, and Medicare
                // has not been specified as either the primary or secondary payor
                if ( !AccountView.GetInstance().MedicareOver65Checked )
                {
                    if ( IsMedicareAdvisedForPatient() )
                    {
                        View.DisplayMessageForMedicareAdvise();
                    }
                }
            }

            catch
            {
                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.DOB_NOTVALID_ERRMSG,
                                             UIErrorMessages.ERROR,
                                             MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                             MessageBoxAdapterDefaultButton.Button1 );

                return false;
            }

            return true;
        }

        public bool PopulateDateOfBirth()
        {
            var dateOfBirth = Account.Patient.DateOfBirth;
            var mtbDateOfBirthText = String.Format( "{0:D2}{1:D2}{2:D4}",
                dateOfBirth.Month,
                dateOfBirth.Day,
                dateOfBirth.Year );

            if ( mtbDateOfBirthText.Length != 0
                && mtbDateOfBirthText.Length != 10 )
            {
                return false;
            }

            View.DateOfBirth = mtbDateOfBirthText;

            return true;
        }

        public void SetDateOfBirthAndAge()
        {
            if ( Account.Patient.DateOfBirth == DateTime.MinValue )
            {
                View.Age = string.Empty;
            }

            else
            {
                var dateOfBirth = Account.Patient.DateOfBirth;
                View.DateOfBirth = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                 dateOfBirth.Month,
                                                 dateOfBirth.Day,
                                                 dateOfBirth.Year );

                View.Age = GetAgeFor( Account.Patient );
            }
        }

        public bool UpdateAdmitTime()
        {
            var mtbAdmitTime = View.AdmitTime;

            if ( mtbAdmitTime != string.Empty && mtbAdmitTime != ZERO_TIME )
            {
                var timeValidationResult = DateValidator.IsValidTime( mtbAdmitTime );

                switch ( timeValidationResult )
                {
                    case TimeValidationResult.TimeIsInvalid:
                        MessageBoxAdapter.ShowMessageBox( UIErrorMessages.TIME_NOT_VALID_MSG,
                                     UIErrorMessages.ERROR,
                                     MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                     MessageBoxAdapterDefaultButton.Button1 );
                        return false;

                    case TimeValidationResult.HourIsInvalid:
                        MessageBoxAdapter.ShowMessageBox( UIErrorMessages.HOUR_INVALID_ERRMSG,
                                   UIErrorMessages.ERROR,
                                   MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                   MessageBoxAdapterDefaultButton.Button1 );
                        return false;

                    case TimeValidationResult.MinuteIsInvalid:
                        MessageBoxAdapter.ShowMessageBox( UIErrorMessages.MINUTE_INVALID_ERRMSG,
                              UIErrorMessages.ERROR,
                              MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                              MessageBoxAdapterDefaultButton.Button1 );
                        return false;
                    case TimeValidationResult.Valid:
                        break;
                }

                CheckTimeIsNotInFuture( mtbAdmitTime );
            }

            if ( !VerifyAdmitDate() )
            {
                return false;
            }

            Account.AdmitDate = GetAdmitDateFromUI();
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Account );

            return true;
        }

        public bool VerifyAdmitDate()
        {
            var mtbAdmitDate = View.AdmitDate;
            var mtbAdmitDateText = View.AdmitDateText;

            if ( mtbAdmitDate == string.Empty || mtbAdmitDate == "01010001" )
            {
                return true;
            }

            var result = true;

            if ( mtbAdmitDate != string.Empty
                && mtbAdmitDate.Length != 0
                && mtbAdmitDateText.Length != 10 )
            {
                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.ADMIT_ERRMSG,
                             UIErrorMessages.DATE,
                            MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                            MessageBoxAdapterDefaultButton.Button1 );
                View.SetAdmitDateError();
                return false;
            }

            var theDate = DateTime.MinValue;
            
            try
            {
                theDate = GetAdmitDateFromUI();
            }

            catch
            {

                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.ADMIT_INVALID_ERRMSG,
                             UIErrorMessages.DATE,
                            MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                            MessageBoxAdapterDefaultButton.Button1 );
                View.SetAdmitDateError();
                return false;
            }

            // If GetAdmitDateFromUI throws an exception, the date will be equal to MinValue
            if ( theDate == DateTime.MinValue || DateValidator.IsValidDate( theDate ) == false )
            {

                MessageBoxAdapter.ShowMessageBox( UIErrorMessages.ADMIT_INVALID_ERRMSG,
                           UIErrorMessages.DATE,
                          MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                          MessageBoxAdapterDefaultButton.Button1 );
                View.SetAdmitDateError();
                result = false;
            }

            else
            {
                var menstrualDate = GetOccurrenceCode10MenstrualDate();
                if ( menstrualDate != DateTime.MinValue )
                {
                    if ( menstrualDate.AddYears( 1 ) < theDate ||
                        menstrualDate > theDate )
                    {

                        var errMsg = "Either the date of the last menstrual period\n("
                            + CommonFormatting.LongDateFormat( menstrualDate ) + ") or the admit date\n("
                            + mtbAdmitDateText + ") must be modified, "
                            + UIErrorMessages.OCCURRENCECODE_BAD_MENSTRUAL_DATE_MSG;

                        MessageBoxAdapter.ShowMessageBox( errMsg,
                          UIErrorMessages.ERROR,
                         MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                         MessageBoxAdapterDefaultButton.Button1 );

                        View.SetAdmitDateError();
                        result = false;
                    }
                }
            }

            return result;
        }

        private void SetAdmitDateOnModel( DateTime admitDate )
        {
            if ( Account != null )
            {
                Account.AdmitDate = admitDate;
            }
        }

        /// <summary>
        /// Updates the admit date.
        /// </summary>
        public void UpdateAdmitDate()
        {
            if ( Account != null )
            {
                var mtbAdmitDate = View.AdmitDate;
                var mtbAdmitDateText = View.AdmitDateText;
                var mtbAdmitTime = View.AdmitTime;
                if ( mtbAdmitDate == String.Empty )
                {
                    if ( mtbAdmitTime == String.Empty )
                    {
                        Account.AdmitDate = DateTime.MinValue;
                    }
                    else
                    {
                        Account.AdmitDate = DateTime.MinValue;
                        var unFormattedDateString = GetUnFormattedDateString( Account.AdmitDate );

                        GetDateAndTimeFrom( unFormattedDateString, mtbAdmitTime );

                    }
                    RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Account );

                }
                else if ( mtbAdmitDateText.Length == 10 )
                {
                    var theDate = DateTime.MinValue;
                    try
                    {
                        theDate = GetDateAndTimeFrom( mtbAdmitDate, mtbAdmitTime );
                    }
                    catch
                    {
                        MessageBoxAdapter.ShowMessageBox( UIErrorMessages.ADMIT_INVALID_ERRMSG,
                       UIErrorMessages.ERROR,
                      MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                      MessageBoxAdapterDefaultButton.Button1 );
                        View.SetAdmitDateError();
                    }

                    SetAdmitDateOnModel( theDate );

                }
            }
        }

        public void RunAdmitDateValidationRules()
        {
            RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFiveDaysPast ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFutureDate ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateFiveDaysPast ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateFutureDate ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateTodayOrGreater ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitDateWithinSpecifiedSpan ), Account );
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Account );

        }

        public void SetScheduleCode()
        {
            View.ScheduleCode = Account.ScheduleCode;
        }

        public void SetAdmitDateOnUIFromTheModel()
        {
            View.AdmitDate = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                   Account.AdmitDate.Month,
                                                   Account.AdmitDate.Day,
                                                   Account.AdmitDate.Year );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Account );
        }

        public void AdmitDateUnaltered()
        {
            if ( Account.AdmitDateUnaltered == DateTime.MinValue )
            {
                Account.AdmitDateUnaltered = Account.AdmitDate;
            }
        }

        public void SetAdmitTimeOnUIFromTheModel()
        {
            var admitDate = Account.AdmitDate;

            View.AdmitTime = IsMidnight( admitDate ) ? string.Empty : String.Format( "{0:D2}{1:D2}", admitDate.Hour, admitDate.Minute );
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Account );
        }

        public void PopulateFirstName()
        {
            View.PopulateFirstName();
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), Account );
        }

        public void PopulateLastName()
        {
            View.PopulateLastName();
            RuleEngine.EvaluateRule( typeof( LastNameRequired ), Account );
        }

        public void PopulateMiddleInitial()
        {
            View.PopulateMiddleInitial();
        }

        public void UpdateFirstName( string firstName )
        {
            Account.Patient.FirstName = firstName;
            Account.Patient.AddPreviousNameToAKA();
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), Account );
        }

        public void UpdateMiddleInitial( string middleInitial )
        {
            Account.Patient.Name.MiddleInitial = middleInitial;
        }

        public void UpdateLastName( string lastName )
        {
            Account.Patient.LastName = lastName;
            Account.Patient.AddPreviousNameToAKA();
            RuleEngine.EvaluateRule( typeof( LastNameRequired ), Account );
        }

        public void PopulateGenderControl( GenderControl genderControl )
        {
            genderControl.InitializeGendersComboBox();
            View.PopulateGender();
            RuleEngine.EvaluateRule( typeof( GenderRequired ), Account );
        }

        public void UpdateGender( Gender gender )
        {
            if ( Account == null )
            {
                return;
            }
            if ( Account.Patient == null )
            {
                return;
            }

            if ( gender != null )
            {
                Account.Patient.Sex = gender;
            }

            else
            {
                Account.Patient.Sex = new Gender();
            }

            if ( Account.Patient.Sex != null &&
                Account.Patient.Sex.Code == Gender.MALE_CODE )
            {
                RemoveLastMenstruationOccurrenceCode();
            }

            RuleEngine.EvaluateRule( typeof( GenderRequired ), Account );
        }

        private void RemoveLastMenstruationOccurrenceCode()
        {
            var occCodes = Account.OccurrenceCodes;

            if ( occCodes == null || occCodes.Count == 0 )
            {
                return;
            }

            for ( var i = 0; i < occCodes.Count; i++ )
            {
                var occurrenceCode = occCodes[i] as OccurrenceCode;
                if ( occurrenceCode != null && occurrenceCode.Code == OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION )
                {
                    Account.OccurrenceCodes.RemoveAt( i );
                    break;
                }
            }
        }

        public void PopulateScheduleCodeComboBox()
        {
            var scheduleCodeBrokerProxy = new ScheduleCodeBrokerProxy();
            var scheduleCodesForFacility = (ArrayList)scheduleCodeBrokerProxy.AllScheduleCodes( User.GetCurrent().Facility.Oid );
            View.PopulateScheduleCodeComboBox( scheduleCodesForFacility );
        }

        public void UpdateAppointment( ScheduleCode scheduleCode )
        {
            Account.ScheduleCode = scheduleCode;
            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), Account );
        }

        #endregion Public Methods

        #region Private Methods

        private bool IsMedicareAdvisedForPatient()
        {
            var isPrimaryOrSecondaryCoverageMedicare = IsPrimaryOrSecondaryCoverageMedicare();

            var patientAge = PatientAgeInYears();

            return patientAge >= SIXTY_FIVE && isPrimaryOrSecondaryCoverageMedicare;
        }

        private bool IsPrimaryOrSecondaryCoverageMedicare()
        {
            Coverage primaryCoverage = null;
            Coverage secondaryCoverage = null;

            if ( Account.Insurance != null )
            {
                primaryCoverage = Account.Insurance.PrimaryCoverage;
                secondaryCoverage = Account.Insurance.SecondaryCoverage;
            }

            return ( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) &&
                   ( secondaryCoverage == null || ( secondaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) );
        }

        private static string GetAgeFor( Person person )
        {
            var tb = ProxyFactory.GetTimeBroker();
            var facilityTime = tb.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                               User.GetCurrent().Facility.DSTOffset );
            return ( person.AgeAt( facilityTime ) );
        }


        private DateTime GetOccurrenceCode10MenstrualDate()
        {
            var occ10Date = DateTime.MinValue;

            foreach ( OccurrenceCode occ in Account.OccurrenceCodes )
            {
                if ( occ.Code == "10" )
                {
                    occ10Date = occ.OccurrenceDate;
                    break;
                }
            }

            return occ10Date;
        }

        private DateTime GetDateAndTimeFrom( string dateText, string timeText )
        {
            DateTime theDate;
            month = Convert.ToInt32( dateText.Substring( 0, 2 ) );
            day = Convert.ToInt32( dateText.Substring( 2, 2 ) );
            year = Convert.ToInt32( dateText.Substring( 4, 4 ) );

            hour = 0;
            minute = 0;

            if ( timeText.Length == 4 )
            {
                hour = Convert.ToInt32( timeText.Substring( 0, 2 ) );
                minute = Convert.ToInt32( timeText.Substring( 2, 2 ) );
            }

            if ( ( hour >= 0 && hour <= 23 ) && ( minute >= 0 && minute <= 59 ) )
            {
                theDate = new DateTime( year, month, day, hour, minute, 0 );
            }

            else
            {
                theDate = new DateTime( year, month, day );
            }

            return theDate;
        }

        private static string GetUnFormattedDateString( DateTime dateTime )
        {
            return String.Format( "{0:D2}{1:D2}{2:D4}", dateTime.Month, dateTime.Day, dateTime.Year );
        }

        private DateTime GetAdmitDateFromUI()
        {
            var theDate = DateTime.MinValue;
            var mtbAdmitDate = View.AdmitDate;
            var mtbAdmitTime = View.AdmitTime;

            if ( mtbAdmitDate != string.Empty )
            {
                theDate = GetDateAndTimeFrom( mtbAdmitDate, mtbAdmitTime );
            }

            else
            {
                if ( mtbAdmitTime != string.Empty )
                {
                    Account.AdmitDate = DateTime.MinValue;
                    var unFormattedDateString = GetUnFormattedDateString( Account.AdmitDate );
                    theDate = GetDateAndTimeFrom( unFormattedDateString, mtbAdmitTime );
                }
            }

            return theDate;
        }

        /// <summary>
        /// This private helper method checks if the time entered into the admitTime box is 
        /// in the future relative to the current facility time. 
        /// </summary>
        private void CheckTimeIsNotInFuture( string mtbAdmitTime )
        {
            if ( DoesCurrentActivityAllowAdmitTimeEdit() )
            {
                var currentFacilityDateTime = GetCurrentFacilityDateTime();
                var originalAdmitHour = currentFacilityDateTime.Hour;
                var originalAdmitMinute = currentFacilityDateTime.Minute;
                int enteredHour;
                int enteredMinute;
                // else get the UI entered time
                try
                {
                    enteredHour = Convert.ToInt32( mtbAdmitTime.Substring( 0, 2 ) );
                }
                catch
                {
                    enteredHour = 0;
                }
                try
                {
                    enteredMinute = Convert.ToInt32( mtbAdmitTime.Substring( 2, 2 ) );
                }
                catch
                {
                    enteredMinute = 0;
                }

                if ( IsTodaysDate() &&
                    IsTimeInTheFuture( originalAdmitHour, originalAdmitMinute, enteredHour, enteredMinute ) )
                {
                    View.SetFocusToAdmitTime(); //mtbAdmitTime.Focus();
                    View.AdmitTime = String.Format( "{0:D2}{1:D2}", originalAdmitHour, originalAdmitMinute );

                    MessageBoxAdapter.ShowMessageBox( UIErrorMessages.ADMIT_TIME_CANNOT_BE_IN_FUTURE,
                    UIErrorMessages.ERROR,
                    MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                    MessageBoxAdapterDefaultButton.Button1 );

                }
            }
        }
        
        /// <summary>
        /// Returns true if the current activity matches any of the 
        /// activities in the list and that allow the admit time to be 
        /// edited, else returns false.
        /// </summary>
        /// <returns></returns>
        private bool DoesCurrentActivityAllowAdmitTimeEdit()
        {
            return Account.Activity.GetType().Equals( typeof( EditAccountActivity ) ) ||
                             Account.Activity.GetType().Equals( typeof( MaintenanceActivity ) );
        }

        /// <summary>
        /// This method returns true if the entered admit time is after the current facility time, else false.
        /// Assumes the date is today's date.
        /// </summary>
        /// <param name="originalAdmitHour"></param>
        /// <param name="originalAdmitMinute"></param>
        /// <param name="enteredHour"></param>
        /// <param name="enteredMinute"></param>
        /// <returns></returns>
        private static bool IsTimeInTheFuture( int originalAdmitHour, int originalAdmitMinute, int enteredHour, int enteredMinute )
        {
            var todaysTotalSeconds = ( originalAdmitHour * 60 ) + originalAdmitMinute;
            var enteredTotalSeconds = ( enteredHour * 60 ) + enteredMinute;

            if ( enteredTotalSeconds > todaysTotalSeconds )
            {
                return true;
            }
            return false;
        }

        private bool IsTodaysDate()
        {
            var uiEnteredAdmitDate = GetAdmitDateFromUI();
            return uiEnteredAdmitDate.Date.Equals( DateTime.Today );
        }

        private static DateTime GetCurrentFacilityDateTime()
        {
            var timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                      User.GetCurrent().Facility.DSTOffset );
        }

        private static bool IsMidnight( DateTime admitDate )
        {
            return admitDate.Hour == 0 && admitDate.Minute == 0;
        }

        private void UpdateConditionCode( DateTime dateOfBirthEntered )
        {
            var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            var code = conditionCodeBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid,
                ConditionCode.CONDITIONCODE_DOB_OVER_100Y );

            if ( dateOfBirthEntered.AddYears( 100 ) < DateTime.Now )
            {
                Account.AddConditionCode( code );
            }

            else
            {
                Account.RemoveConditionCode( code );
            }
        }
        
        /// <summary>
        /// Returns the age of a person in years.
        /// </summary>
        private int PatientAgeInYears()
        {
            var ageString = View.Age;
            if ( ageString != String.Empty )
            {
                switch ( ageString.Substring( ( ageString.Length ) - 1, 1 ) )
                {
                    case "y":
                        {
                            var age = Convert.ToInt32( ageString.Substring( 0, ( ageString.Length ) - 1 ) );
                            return age;
                        }

                    default:
                        {
                            // less than 1yr old
                            return 0;
                        }
                }
            }

            // less than 1yr old
            return 0;
        }

        #endregion Private Methods

        #region Data Elements

        private int month;
        private int day;
        private int year;
        private int hour;
        private int minute;

        #endregion Data Elements

        #region Constants

        private const int SIXTY_FIVE = 65;
        private const string ZERO_TIME = "0000";

        #endregion Constants
    }
}
