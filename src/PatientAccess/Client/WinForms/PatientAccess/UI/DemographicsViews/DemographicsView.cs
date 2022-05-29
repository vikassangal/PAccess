using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.DemographicsViews.Presenters;
using PatientAccess.UI.DemographicsViews.ViewImpl;
using PatientAccess.UI.DemographicsViews.Views;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using Peradigm.Framework.Domain.Collections;
 
using SortOrder = Peradigm.Framework.Domain.Collections.SortOrder;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for DemographicsView.
    /// </summary>
    public partial class DemographicsView : ControlView, IDemographicsView, IBirthTimeView, IPatientNameView
    {
        #region Events
        public event EventHandler RefreshTopPanel;
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void UpdateAkaName( object sender, EventArgs e )
        {
            var manageAkaDialog = (ManageAKADialog)sender;
            ModelAccount.Patient = manageAkaDialog.Model_Patient;
            UpdateAkaName();
        }

        private void DemographicsView_Enter( object sender, EventArgs e )
        {
            RegisterEvents();
            leavingView = false;

            if ( !loadingModelData && AccountView.GetInstance().IsMedicareAdvisedForPatient() )
            {
                DisplayMessageForMedicareAdvise();
            }
        }

        private void DemographicsView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            leavingView = true;
            DateTime admitDate = UpdateAdmitDate();
            DemographicsViewPresenter.ValidatePreOpDate(
                dateTimePicker_Preop.Focused, isAdmitDateChange, isPreOpDateChange, mtbPreopDate.UnMaskedText );
            SetAdmitDateOnModel( admitDate );
            PopulateDobForNewBorn(admitDate);

            sequesteredPatientPresenter.IsPatientSequestered();

            // SR 604 - If the user clicked on 'Next' or 'Finish' or another tab after 
            // modifying the Admit date, update Social Security Number for the new criteria.
            if ( isAdmitDateChange )
            {
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }

            RuleEngine.EvaluateRule( typeof( OnPatientDemographicsForm ), ModelAccount );
            blnLeaveRun = false;

            UnregisterEvents();
        }

        private void mtbLastName_Validating(object sender, CancelEventArgs e)
        {
            var lastName = (MaskedEditTextBox) sender;
            UIColors.SetNormalBgColor(lastName);
            if (ModelAccount != null && ModelAccount.Patient != null)
            {
                ModelAccount.Patient.LastName = lastName.Text.Trim();
                Activity currentActivity = ModelAccount.Activity;

                AddPreviousNameToAka(currentActivity);

                RuleEngine.OneShotRuleEvaluation<LastNameRequired>(Model, LastNameRequiredEventHandler);

                if (RefreshTopPanel != null)
                {
                    RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
                }
            }
        }

        private void mtbFirstName_Validating(object sender, CancelEventArgs e)
        {
            var firstName = (MaskedEditTextBox) sender;
            UIColors.SetNormalBgColor(firstName);
            ModelAccount.Patient.FirstName = firstName.Text.Trim();
            Activity currentActivity = ModelAccount.Activity;

            AddPreviousNameToAka(currentActivity);

            RuleEngine.OneShotRuleEvaluation<FirstNameRequired>(Model, FirstNameRequiredEventHandler);

            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void AddPreviousNameToAka( Activity activity )
        {
            if( ( activity is AdmitNewbornActivity ) ||
                ( activity is PreAdmitNewbornActivity ) ||
                ( activity is PostMSERegistrationActivity ) ||
                ( activity is PreRegistrationActivity ) ||
                ( activity is RegistrationActivity ) ||
                ( activity is MaintenanceActivity )
                )
            {
                ModelAccount.Patient.AddPreviousNameToAKA();
            }
        }

        private void mtbMiddleInitial_Validating( object sender, CancelEventArgs e )
        {
            var middleInitial = (MaskedEditTextBox)sender;
            ModelAccount.Patient.Name.MiddleInitial = middleInitial.Text.Trim();
        }
 
        void mtbAdmitDate_TextChanged( object sender, EventArgs e )
        {
            isAdmitDateChange = true;
            isPreOpDateChange = false;
        }
        
        private void mtbAdmitDate_Validating( object sender, CancelEventArgs e )
        {
            isPreOpDateChange = false;
            if ( ActiveControl == null
                && mtbAdmitDate.Text.Length == 10 )
            {
                return;
            }

            if ( !blnLeaveRun )
            {
                SetAdmitDateNormalBgColor();
            }

            if ( dateTimePicker.Focused )
            {
                return;
            }
            if ( newBornAdmit )
            {
                if ( !VerifyAdmitDateForNewbornRegistration() )
                {
                    return;
                }
            }
            else
            {
                if ( !VerifyAdmitDate() )
                {
                    return;
                }
            }

            DemographicsViewPresenter.HandlePreOpDateDisplayWithDateChange( true, false, GetAdmitDateFromUI(), GetPreopDateFromUI() );

            if ( !blnLeaveRun )
            {
                DateTime admitDate = UpdateAdmitDate();
                SetAdmitDateOnModel( admitDate );
            }

            if ( !dateTimePicker.Focused
                && !blnLeaveRun )
            {
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFiveDaysPast ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFutureDate ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateFiveDaysPast ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateFutureDate ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateTodayOrGreater ), Model );
                
                if(IsPreAdmitNewbornActivity() ||IsEditPreAdmitNewbornActivity())
                {
                    RuleEngine.EvaluateRule( typeof(AdmitDateWithin90DaysFutureDate), Model);
                }
                else
                {
                    RuleEngine.EvaluateRule( typeof( AdmitDateWithinSpecifiedSpan ), Model );
                }
            }

            if ( mtbAdmitDate.UnMaskedText != string.Empty )
            {
                AdmitDateToInsuranceValidation.CheckAdmitDateToInsurance( ModelAccount, Name );
            }

            CheckAdmitDateToAuthorization();
            PopulateDobForNewBorn(ModelAccount.AdmitDate);
            

            if ( ModelAccount.ShouldWeEnablePreopDate() )
            {
                DemographicsViewPresenter.UpdatePreOpDate( mtbPreopDate.UnMaskedText );
            }

            if ( !loadingModelData )
            {
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }
        }

        private void mtbAdmitTime_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbAdmitTime );

            if ( mtbAdmitTime.UnMaskedText.Trim() != string.Empty
                && mtbAdmitTime.UnMaskedText.Trim() != "0000" )
            {
                if ( DateValidator.IsValidTime( mtbAdmitTime ) == false )
                {
                    if ( !dateTimePicker.Focused )
                    {
                        mtbAdmitTime.Focus();
                    }
                    return;
                }
                CheckTimeIsNotInFuture();
                DateTime admitDate = UpdateAdmitDate();
                if (ModelAccount.DischargeDate.ToString("mm/dd/yyyy") != string.Empty && ModelAccount.DischargeDate.ToString("MM/dd/yyyy") == admitDate.ToString("MM/dd/yyyy"))
                {
                    TimeSpan dischargeTimeSpan = TimeSpan.Parse(ModelAccount.DischargeDate.ToString("HH:mm"));
                    TimeSpan admitDateTimeSpan = TimeSpan.Parse(admitDate.ToString("HH:mm"));
                    if (admitDateTimeSpan > dischargeTimeSpan)
                    {
                        UIColors.SetErrorBgColor(mtbAdmitTime);

                        MessageBox.Show(UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                        mtbAdmitTime.Focus();
                    }

                }
            }

            if ( newBornAdmit )
            {
                if ( !VerifyAdmitDateForNewbornRegistration() )
                {
                    return;
                }
            }
            else
            {
                if ( !VerifyAdmitDate() )
                {
                    return;
                }
            }

            ModelAccount.AdmitDate = GetAdmitDateFromUI();
            RuleEngine.OneShotRuleEvaluation<AdmitTimeRequired>(Model, AdmitTimeRequiredEventHandler);
            RuleEngine.OneShotRuleEvaluation<AdmitTimePreferred>(Model, AdmitTimePreferredEventHandler);
        }

        void mtbPreopDate_TextChanged( object sender, EventArgs e )
        {
            isPreOpDateChange = true;
            isAdmitDateChange = false;
        }

        private void mtbPreopDate_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                DemographicsViewPresenter.ValidatePreOpDate( dateTimePicker_Preop.Focused, false, true, mtbPreopDate.UnMaskedText );
            }
        }

        /// <summary>
        /// This private helper method checks if the time entered into the admitTime box is 
        /// in the future relative to the current facility time. 
        /// </summary>
        private void CheckTimeIsNotInFuture()
        {
            if ( DoesCurrentActivityAllowAdmitTimeEdit() )
            {
                DateTime tmp = GetCurrentFacilityDateTime();
                int originalAdmitHour = tmp.Hour;
                int originalAdmitMinute = tmp.Minute;
                int enteredHour;
                int enteredMinute;
                // else get the UI entered time
                try
                {
                    enteredHour = Convert.ToInt32( mtbAdmitTime.Text.Substring( 0, 2 ) );
                }
                catch
                {
                    enteredHour = 0;
                }
                try
                {
                    enteredMinute = Convert.ToInt32( mtbAdmitTime.Text.Substring( 3, 2 ) );
                }
                catch
                {
                    enteredMinute = 0;
                }

                if ( IsTodaysDate() && IsTimeInTheFuture( originalAdmitHour, originalAdmitMinute, enteredHour, enteredMinute ) )
                {
                    mtbAdmitTime.Focus();
                    mtbAdmitTime.Text = String.Format( "{0:D2}{1:D2}", originalAdmitHour, originalAdmitMinute );
                    MessageBox.Show( UIErrorMessages.ADMIT_TIME_CANNOT_BE_IN_FUTURE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
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
            return ModelAccount.Activity.GetType().Equals( typeof( EditAccountActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( EditPreMseActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreMSERegistrationWithOfflineActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreRegistrationWithOfflineActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( RegistrationActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( RegistrationWithOfflineActivity ) );
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
            int todaysTotalSeconds = ( originalAdmitHour * 60 ) + originalAdmitMinute;
            int enteredTotalSeconds = ( enteredHour * 60 ) + enteredMinute;

            if ( enteredTotalSeconds > todaysTotalSeconds )
            {
                return true;
            }
            return false;
        }

        private bool IsTodaysDate()
        {
            DateTime uiEnteredAdmitDate = GetAdmitDateFromUI();
            return uiEnteredAdmitDate.Date.Equals( DateTime.Today );
        }
        /// <summary>
        /// Gets the preop date from UI.
        /// </summary>
        /// <returns></returns>
        public DateTime GetPreopDateFromUI()
        {
            DateTime theDate = DateTime.MinValue;

            if ( mtbPreopDate.UnMaskedText != string.Empty )
            {

                theDate = GetDateAndTimeFrom( mtbPreopDate.UnMaskedText, string.Empty );
            }

            return theDate;
        }

        public DateTime GetAdmitDateFromUI()
        {
            DateTime theDate = DateTime.MinValue;

            if ( mtbAdmitDate.UnMaskedText != string.Empty )
            {

                theDate = GetDateAndTimeFrom( mtbAdmitDate.UnMaskedText, mtbAdmitTime.UnMaskedText );
            }
            else
            {
                if ( mtbAdmitTime.UnMaskedText != string.Empty )
                {
                    ModelAccount.AdmitDate = DateTime.MinValue;
                    string unFormattedDateString = GetUnFormattedDateString( ModelAccount.AdmitDate );
                    theDate = GetDateAndTimeFrom( unFormattedDateString, mtbAdmitTime.UnMaskedText );
                }
            }

            return theDate;
        }

        private DateTime GetDateOfBirth()
        {
            DateTime dateOfBirthEntered = DateTime.MinValue;
            if ( !string.IsNullOrEmpty( mtbDateOfBirth.UnMaskedText ) )
            {
                int hour = 0;
                int minute = 0;
                if (mtbBirthTime.UnMaskedText != string.Empty)
                {
                    hour = Convert.ToInt32(mtbBirthTime.UnMaskedText.Substring(0, 2));
                    minute = Convert.ToInt32(mtbBirthTime.UnMaskedText.Substring(2, 2));
                }

                dateOfBirthEntered = new DateTime(Convert.ToInt32(mtbDateOfBirth.UnMaskedText.Substring(4, 4)),
                                                  Convert.ToInt32(mtbDateOfBirth.UnMaskedText.Substring(0, 2)),
                                                  Convert.ToInt32(mtbDateOfBirth.UnMaskedText.Substring(2, 2)), hour,
                                                  minute, 0);
            }

            return dateOfBirthEntered;
        }

        public string GetPreOpDateUnmaskedText()
        {
            return mtbPreopDate.UnMaskedText;
        }

        public DateTime GetDateAndTimeFrom( string dateText, string timeText )
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

        private void mtbDateOfBirth_Validating( object sender, CancelEventArgs e )
        {
            hour = ModelAccount.Patient.DateOfBirth.Hour;
            minute = ModelAccount.Patient.DateOfBirth.Minute; 

            if ( mtbDateOfBirth.UnMaskedText == string.Empty ||
                mtbDateOfBirth.UnMaskedText == "01010001" )
            {
                UIColors.SetNormalBgColor( mtbDateOfBirth );
                lblPatientAge.Text = String.Empty;
                ModelAccount.Patient.DateOfBirth = DateTime.MinValue;
                RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
                ssnView.ResetSSNControl();
                return;
            }

            lblPatientAge.Text = string.Empty;

            var dateOfBirthTextBox = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor( dateOfBirthTextBox );

            if ( dateOfBirthTextBox.Text.Length != 10 )
            {   // Prevent cursor from advancing to the next control
                UIColors.SetErrorBgColor( dateOfBirthTextBox );
                MessageBox.Show( UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbDateOfBirth.Focus();
                return;
            }

            try
            {   // Check the date entered is not in the future
                var dateOfBirthEntered = new DateTime( Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 6, 4 ) ),
                    Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 0, 2 ) ),
                     Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 3, 2 ) ), hour, minute, 0 );
                

                if ( dateOfBirthEntered.Date > DateTime.Today )
                {
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDateOfBirth.Focus();
                }
                else if ( DateValidator.IsValidDate( dateOfBirthEntered ) == false )
                {
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDateOfBirth.Focus();
                }
                else
                {
                    ModelAccount.Patient.DateOfBirth = dateOfBirthEntered;
                    UpdateConditionCode( dateOfBirthEntered );

                    if ( RefreshTopPanel != null )
                    {
                        RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
                    }
                     
                    SetDateOfBirthAndAge();
                    RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
                    RuleEngine.EvaluateRule( typeof( InValidDateOfBirth ), Model );
                    
                    // Redirect user to Insurance tab if the patient is over 65, and Medicare
                    // has not been specified as either the primary or secondary payor
                    if ( !AccountView.GetInstance().MedicareOver65Checked )
                    {
                        if ( IsMedicareAdvisedForPatient() )
                        {
                            DisplayMessageForMedicareAdvise();
                        }
                    }
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                UIColors.SetErrorBgColor( dateOfBirthTextBox );
                MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbDateOfBirth.Focus();
            }
            BirthTimePresenter.UpdateField();
            if (!loadingModelData)
            {
                ssnView.ResetSSNControl();
            }
        }

        private void SetPatientDOBForNewborn()
        {
            //mtbDateOfBirth_Validating is not executed for Activate Pre-Newborn because mtbDateOfBirth is disabled in this case. 
            //This method is used instead.
            DateTime dateOfBirthEntered = GetDateOfBirth();
            ModelAccount.Patient.DateOfBirth = dateOfBirthEntered;
            if(dateOfBirthEntered != DateTime.MinValue)
                UpdateConditionCode( dateOfBirthEntered );

            if ( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }

            SetDateOfBirthAndAge();
            RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
            RuleEngine.EvaluateRule( typeof( InValidDateOfBirth ), Model );
            
            // Redirect user to Insurance tab if the patient is over 65, and Medicare
            // has not been specified as either the primary or secondary payor
            if ( !AccountView.GetInstance().MedicareOver65Checked )
            {
                if ( IsMedicareAdvisedForPatient() )
                {
                    DisplayMessageForMedicareAdvise();
                }
            }
        }

        private void ssnView_ssnNumberChanged(object sender, EventArgs e)
        {
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void mtbNationalID_Validating( object sender, CancelEventArgs e )
        {
            ModelAccount.Patient.NationalID = mtbNationalID.UnMaskedText.Trim();
        }

        private void patientMailingAddrView_AreaCodeChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = ModelAccount.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.PhoneNumber = patientMailingAddrView.Model_ContactPoint.PhoneNumber;

            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), ModelAccount );
        }

        private void patientMailingAddrView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = ModelAccount.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.PhoneNumber = patientMailingAddrView.Model_ContactPoint.PhoneNumber;

            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), ModelAccount );
        }

        private void patientMailingAddrView_CellPhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mobileContactPoint = ModelAccount.Patient.ContactPointWith(
                TypeOfContactPoint.NewMobileContactPointType() );
            mobileContactPoint.PhoneNumber = patientMailingAddrView.Model_ContactPoint.CellPhoneNumber;
        }
    
        private void patientPhysicalAddrView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint physicalContactPoint = ModelAccount.Patient.ContactPointWith(
                TypeOfContactPoint.NewPhysicalContactPointType() );
            physicalContactPoint.PhoneNumber = patientPhysicalAddrView.Model_ContactPoint.PhoneNumber;
        }

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void DemographicsView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void PatientMailingAddressView_AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint =
                ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.Address = patientMailingAddrView.Model_ContactPoint.Address;
            mailingContactPoint.PhoneNumber = patientMailingAddrView.Model_ContactPoint.PhoneNumber;
            patientPhysicalAddrView.SetCopyFromValues();
            RuleEngine.EvaluateRule( typeof( MailingAddressPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( OnPatientDemographicsForm ), ModelAccount );
        }

        private void PatientPhysicalAddressView_AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint physicalContactPoint =
                ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            physicalContactPoint.Address = patientPhysicalAddrView.Model_ContactPoint.Address;
            patientPhysicalAddrView.SetCopyFromValues();
            RuleEngine.EvaluateRule(typeof(PhysicalAddressRequired), Model);
            RuleEngine.EvaluateRule(typeof(OnPatientDemographicsForm), ModelAccount);
        }

        private void CheckAdmitDateToAuthorization()
        {
            Cursor = Cursors.WaitCursor;
            DialogResult result;

            RuleEngine.RegisterEvent( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateToSecondaryAuthorizationDates ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateEarlierThanSecondaryAuthEffectiveDate ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateLaterThanSecondaryAuthExpDate ), ModelAccount, Name, null );

            bool isAdmiteDateToAuthorzationDates = RuleEngine.EvaluateRule( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, Name );
            bool isAdmiteDateEarlierThanAuthEffectiveDate = RuleEngine.EvaluateRule( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, Name );
            bool isAdmiteDateLaterThanAuthExpirationDate = RuleEngine.EvaluateRule( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, Name );
            bool isAdmiteDateToSecondaryAuthorzationDates = RuleEngine.EvaluateRule( typeof( AdmitDateToSecondaryAuthorizationDates ), ModelAccount, Name );
            bool isAdmiteDateEarlierThanSecondaryAuthEffectiveDate = RuleEngine.EvaluateRule( typeof( AdmitDateEarlierThanSecondaryAuthEffectiveDate ), ModelAccount, Name );
            bool isAdmiteDateLaterThanSecondaryAuthExpirationDate = RuleEngine.EvaluateRule( typeof( AdmitDateLaterThanSecondaryAuthExpDate ), ModelAccount, Name );

            if ( admitDateWarning && !isAdmiteDateToAuthorzationDates || !isAdmiteDateToSecondaryAuthorzationDates )
            {
                result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_ADMIT_DATE_OUT_OF_RANGE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                admitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if ( effectiveGreaterThanAdmitDateWarning && !isAdmiteDateEarlierThanAuthEffectiveDate || !isAdmiteDateEarlierThanSecondaryAuthEffectiveDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_EARLIER_THAN_AUTHORIZATION_EFFECTIVE_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                effectiveGreaterThanAdmitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if ( expirationLesserThanAdmitDateWarning && !isAdmiteDateLaterThanAuthExpirationDate || !isAdmiteDateLaterThanSecondaryAuthExpirationDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_LATER_THAN_AUTHORIZATION_EXPIRATION_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                expirationLesserThanAdmitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }

            RuleEngine.UnregisterEvent( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateToSecondaryAuthorizationDates ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEarlierThanSecondaryAuthEffectiveDate ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateLaterThanSecondaryAuthExpDate ), ModelAccount, null );

            RuleEngine.ClearActionsForRule( typeof( AdmitDateToAuthorizationDateRange ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateEarlierThanAuthEffectiveDate ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateLaterThanAuthExpirationDate ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateToSecondaryAuthorizationDates ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateEarlierThanSecondaryAuthEffectiveDate ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateLaterThanSecondaryAuthExpDate ) );

            Cursor = Cursors.Arrow;
        }

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            SetAdmitDateNormalBgColor();

            DateTime dt = dateTimePicker.Value;
            mtbAdmitDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );

            if ( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT &&
                ModelAccount.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) )
            {
                mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", dt.Hour, dt.Minute );
            }

            mtbAdmitDate.Focus();
        }
        private void dateTimePicker_Preop_CloseUp( object sender, EventArgs e )
        {
            SetPreOpDateNormalBgColor();

            DateTime dt = dateTimePicker_Preop.Value;
            mtbPreopDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbPreopDate.Focus();
        }
 
        /// <summary>
        /// Updates the admit date.
        /// </summary>
        private DateTime UpdateAdmitDate()
        {
            DateTime admitDate = DateTime.MinValue;
            if ( ModelAccount != null )
            {
                if ( mtbAdmitDate.UnMaskedText.Trim() == String.Empty )
                {
                    if ( mtbAdmitTime.UnMaskedText.Trim() == String.Empty )
                    {
                        ModelAccount.AdmitDate = admitDate = DateTime.MinValue;
                    }
                    else
                    {
                        ModelAccount.AdmitDate = admitDate = DateTime.MinValue;
                        string unFormattedDateString = GetUnFormattedDateString( ModelAccount.AdmitDate );

                        GetDateAndTimeFrom( unFormattedDateString, mtbAdmitTime.UnMaskedText );

                    }
                    RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );

                }
                else if ( mtbAdmitDate.Text.Length == 10 )
                {
                    DateTime theDate = DateTime.MinValue;
                    try
                    {
                        theDate = GetDateAndTimeFrom( mtbAdmitDate.UnMaskedText, mtbAdmitTime.UnMaskedText );
                    }
                    catch
                    {
                        SetAdmitDateErrBgColor();
                        MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtbAdmitDate.Focus();
                    }
                    admitDate = theDate;
                }
            }
            return admitDate;
        }

        private void SetAdmitDateOnModel( DateTime admitDate )
        {
            if ( ModelAccount != null )
            {
                ModelAccount.AdmitDate = admitDate;
            }
        }

        private void EnableAdditionalRaceEditButton()
        {
                if (AdditionalRacesViewPresenter.ShouldAdditionRaceEditButtonBeVisible(ModelAccount))
            {
                race2View.Location = new System.Drawing.Point(270, 213);
                lblRace2.Location = new System.Drawing.Point(220, 216);
                MakeEditRacesVisible(true);
                if (!AdditionalRacesViewPresenter.ShouldAdditionRaceEditButtonBeEnabled(ModelAccount))
                {

                    MakeEditRacesEnable(false);
                    ModelAccount.Patient.SetAdditionalRacesToBlank();
                }
                else
                {
                    MakeEditRacesEnable(true);
                }
            }
            else
            {
                MakeEditRacesVisible(false);
                ModelAccount.Patient.SetAdditionalRacesToBlank();
                race2View.Location = new System.Drawing.Point(280, 213);
                lblRace2.Location = new System.Drawing.Point(238, 216);
            } 
        }
        private void MakeEditRacesEnable(bool enable)
        {
            btnEditRace.Enabled = enable;
            lblAdditionalRace.Enabled = enable;
        }

        private void MakeEditRacesVisible(bool visible)
        {
            btnEditRace.Visible = visible;
            lblAdditionalRace.Visible = visible;
        }
        private void raceView_Leave(object sender, EventArgs e)
        {
            EnableAdditionalRaceEditButton();
        }

        private void race2View_Leave(object sender, EventArgs e)
        {
            EnableAdditionalRaceEditButton();
        }
        private void cmbMaritalStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            ModelAccount.Patient.MaritalStatus = cmbMaritalStatus.SelectedItem as MaritalStatus;

            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
        private void btnEditAKA_Click( object sender, EventArgs e )
        {
            using ( var manageAkaDialog = new ManageAKADialog() )
            {
                if ( ModelAccount != null )
                {
                    if ( ModelAccount.Patient != null )
                    {
                        manageAkaDialog.Model = ModelAccount.Patient;
                        manageAkaDialog.UpdateModel();
                        manageAkaDialog.UpdateView();
                        manageAkaDialog.UpdateAKAName += UpdateAkaName;
                        manageAkaDialog.ShowDialog( this );
                    }
                }
            }
        }

        private void btnEditRace_Click(object sender, EventArgs e)
        {
            AdditionalRacesViewPresenter.UpdateView();
            AdditionalRacesViewPresenter.ShowAdditionalRacesView();
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void cmbAppointment_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbAppointment );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidScheduleCode ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidScheduleCodeChange ), Model );
            }
            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), Model );
        }

        private void cmbMaritalStatus_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbMaritalStatus );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidMaritalStatusCode ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidMaritalStatusCodeChange ), Model );
            }

            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
     
        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( Control comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidScheduleCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbAppointment );
        }
         
        private void InvalidMaritalStatusCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbMaritalStatus );
        }
        
        private void InvalidScheduleCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbAppointment );
        }
 
        private void InvalidMaritalStatusCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbMaritalStatus );
        }
      
        //----------------------------------------------------------------------
        #endregion // Event Handlers

        #region Rule Event Handlers
        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>

        private void AdmitDateTodayOrGreaterEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity ) )
            {
                if ( ModelAccount.KindOfVisit == null )
                {
                    return;
                }

                if ( ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_PREREG_ACCOUNT_INVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                    Refresh();
                }
                else
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                    Refresh();
                }
            }
            else if( ModelAccount.Activity.GetType() == typeof( PreRegistrationActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_RANGE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                Refresh();
            }
            else if ( IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity())
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_DATE_IN_FUTURE, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                Refresh();
            }
            else if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                Refresh();
            }
        }

        private void AdmitDateEnteredFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity ) )
            {
                if ( ModelAccount.KindOfVisit == null )
                {
                    return;
                }
                if ( ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_PREREG_ACCOUNT_INVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                }
                else
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                }
            }
            else if( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                || ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity )
                || ModelAccount.Activity.GetType() == typeof( PreAdmitNewbornActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
            }
        }

        private void AdmitDateEnteredFutureDateEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity ) )
            {
                if ( ModelAccount.KindOfVisit == null )
                {
                    return;
                }

                if ( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT )
                {
                    ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG );
                }
            }
            else if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                || ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG );
            }
        }

        delegate void TextBoxDelegate( MaskedEditTextBox aTextbox, string message );

        private static void TextBoxAsync( MaskedEditTextBox aTextbox, string message )
        {
            MessageBox.Show( message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            aTextbox.Focus();
            UIColors.SetErrorBgColor( aTextbox );
        }

        private static void ProcessTextboxErrorEvent( MaskedEditTextBox aTextbox, string message )
        {
            try
            {
                TextBoxDelegate d = TextBoxAsync;
                aTextbox.BeginInvoke( d, new object[] { aTextbox, message } );
            }
            catch
            {
                // intentionally left blank - we get exception when async call for 
                // previous account hasn't returned back with results yet and we already jumped to another activity 
            }
        }

        private void AdmitDateFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ACTIVATE_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( PreAdmitNewbornActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) )
                ||
                (ModelAccount.Activity.GetType() == typeof( UCCPostMseRegistrationActivity ))
                ||
                ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( PreRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
            }
        }

        private void AdmitDateFutureDateEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ACTIVATE_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( UCCPostMseRegistrationActivity ))
                ||
                ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( PreRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( MaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
            }
        }

        private void InValidDateOfBirthEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbDateOfBirth );
            MessageBox.Show( UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            mtbDateOfBirth.Focus();
        }

        private void AdmitDateWithinSpecifiedSpanEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_DATE_TOO_FAR_IN_FUTURE );
        }

        private void AdmitDateWithin90DaysFutureDateEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_DATE_90_DAYS_IN_FUTURE );
        }
 
        private void LastNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbLastName );
        }

        private void FirstNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbFirstName );
        }

        private void DateOfBirthRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbDateOfBirth );
        }
 
        private void MaritalStatusRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbMaritalStatus );
        }
 
        private void AdmitDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitDate );
        }
        private void PreopDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbPreopDate );
        }

        private void PreopDatePreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor(mtbPreopDate);
        }
        
        private void AdmitTimeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitTime );
        }
        private void AdmitTimePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbAdmitTime );
        }
        private void cmbAppointment_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmbAppointment );

            var appointmentComboBox = (ComboBox)sender;
            if ( appointmentComboBox.SelectedIndex > 0 )
            {
                ModelAccount.ScheduleCode = appointmentComboBox.SelectedItem as ScheduleCode;
            }
            else if ( appointmentComboBox.SelectedIndex == 0 )
            {
                ModelAccount.ScheduleCode = null;
            }

            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), ModelAccount );
        }
        private void AppointmentRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAppointment );
        }
        private bool IsCountyRequiredForActivity()
        {
            return (this.ModelAccount.Activity != null &&
                 ( this.ModelAccount.Activity.IsPostMSEActivity() ||
                ( this.ModelAccount.Activity.IsMaintenanceActivity() && 
                            this.ModelAccount.KindOfVisit.IsEmergencyPatient)));
        }

       
        #endregion

        #region Methods
        public override void UpdateView()
        {

            sequesteredPatientPresenter = new SequesteredPatientPresenter(new SequesteredPatientFeatureManager(), ModelAccount);
            sequesteredPatientPresenter.IsPatientSequestered();

            BirthTimePresenter = new BirthTimePresenter(this, new BirthTimeFeatureManager(), new MessageBoxAdapter(), RuleEngine.GetInstance(), ProxyFactory.GetTimeBroker(), User.GetCurrent());
            PatientNamePresenter = new PatientNamePresenter(this, new PatientNameFeatureManager());
            patientPhysicalAddrView.CapturePhysicalAddress = true;
            patientPhysicalAddrView.CountyRequiredForCurrentActivity = IsCountyRequiredForActivity();
            patientMailingAddrView.CaptureMailingAddress = true;
            DemographicsViewPresenter = new DemographicsViewPresenter(this);
            ssnView.SsnFactory = new SsnFactoryCreator(ModelAccount).GetSsnFactory();
            suffixPresenter = new SuffixPresenter(suffixView, ModelAccount , "Patient" );
            suffixPresenter.UpdateView( );

            AdditionalRacesViewPresenter = new AdditionalRacesViewPresenter(new AdditionalRacesView(), ModelAccount,
                new AdditionalRacesFeatureManager());
            EnableAdditionalRaceEditButton();

            if ( loadingModelData )
            {   // Initial entry to the form -- initialize controls and get the data from the model.

                PopulateGenderAndBirthGender();

                PopulateMaritalStatusControl();
                RaceViewPresenter = new RaceViewPresenter(raceView, ModelAccount,Race.RACENATIONALITY_CONTROL);
                RaceViewPresenter.UpdateView();
                Race2ViewPresenter = new RaceViewPresenter(race2View, ModelAccount, Race.RACENATIONALITY2_CONTROL);
                Race2ViewPresenter.UpdateView();
                
                EthnicityViewPresenter = new EthnicityViewPresenter(ethnicityView, ModelAccount, Ethnicity.ETHNICITY_PROPERTY);
                EthnicityViewPresenter.UpdateView();
                Ethnicity2ViewPresenter = new EthnicityViewPresenter(ethnicity2View, ModelAccount, Ethnicity.ETHNICITY2_PROPERTY);
                Ethnicity2ViewPresenter.UpdateView();

                PopulatePatientMailingAddressView();
                PopulatePatientPhysicalAddressView();
                PopulateScheduleCodeComboBox();

                dateOfBirth = DateTime.MinValue;

                if ( ModelAccount==null || ModelAccount.Patient == null )
                {
                    MessageBox.Show( "No patient data is present!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }

                mtbLastName.Text = ModelAccount.Patient.LastName;
                mtbFirstName.Text = ModelAccount.Patient.FirstName;
                mtbMiddleInitial.Text = ModelAccount.Patient.MiddleInitial; 
                UpdateAkaName();

                if ( ( ModelAccount.AdmitDate == DateTime.MinValue
                      && ( ModelAccount.Activity != null && ModelAccount.Activity.GetType() != typeof( PreRegistrationActivity ) 
                            && !IsPreAdmitNewbornActivity()) )
                    ||
                    ( ModelAccount.Activity != null && ModelAccount.Activity.GetType() == typeof( RegistrationActivity )
                      && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) ) 
                    ||
                    IsActivatePreAdmitNewbornActivity() )
                {
                    PreRegAdmitDate = ModelAccount.AdmitDate;
                    SetAdmitDateAndTimeToCurrentFacilityDateAndTime();
                }

                if ( ModelAccount.Activity != null &&
                    ( ModelAccount.Activity.GetType() != typeof( PreRegistrationActivity ) ||
                      ( ModelAccount.Activity.GetType() == typeof( PreRegistrationActivity ) &&
                        ModelAccount.Activity.AssociatedActivityType == typeof( OnlinePreRegistrationActivity ) ) ) )
                {
                    SetAdmitDateOnUIFromTheModel();
                    SetAdmitTimeOnUIFromTheModel();
                }

                if ( ModelAccount.Activity != null &&
                     ModelAccount.Activity.GetType() == typeof( RegistrationActivity ) &&
                     ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) ||
                     IsActivatePreAdmitNewbornActivity()  )
                {
                    DemographicsViewPresenter.HandlePreOpDateDisplayForActivatePreRegistrationInitialViewLoad();
                }

                if( IsNewbornActivity() || IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity() || IsActivatePreAdmitNewbornActivity())
                {
                    //When Register Newborn, Pre-Admit Newborn, Activate Pre-Admit Newborn, Edit Pre-Admit Newborn.
                    mtbDateOfBirth.Enabled = false;
                    if ( !IsEditPreAdmitNewbornActivity() )
                    {
                        //Don't reset AdmitTime and DOB for Edit Pre-Admit Newborn
                        ModelAccount.AdmitDate = ModelAccount.AdmitDate.Date;
                        ModelAccount.PreopDate = ModelAccount.AdmitDate;
                        ModelAccount.Patient.DateOfBirth = ModelAccount.AdmitDate;
                        mtbAdmitTime.UnMaskedText = String.Empty;
                    }
                    
                    newBornAdmit = true;
                    if ( ModelAccount.ScheduleCode !=null)
                        cmbAppointment.SelectedIndex = cmbAppointment.FindString( ModelAccount.ScheduleCode.Code );
                    if (IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity())
                        cmbAppointment.Enabled = true;
                    else
                        cmbAppointment.Enabled = false;
                }
                
                 

                dateOfBirth = ModelAccount.Patient.DateOfBirth;
                mtbDateOfBirth.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                    dateOfBirth.Month,
                    dateOfBirth.Day,
                    dateOfBirth.Year );

                if ( mtbDateOfBirth.Text.Trim().Length != 0
                    && mtbDateOfBirth.Text.Length != 10 )
                {
                    UIColors.SetErrorBgColor( mtbDateOfBirth );
                }

                SetDateOfBirthAndAge();

                SetMaritalStatus();

                if ( ModelAccount.ScheduleCode != null )
                {
                    cmbAppointment.SelectedItem = ModelAccount.ScheduleCode;
                }
                 
                ssnView.Model = ModelAccount.Patient;
                ssnView.ModelAccount = ModelAccount;
                ssnView.UpdateView();
               
                if ( ModelAccount.Patient != null )
                {
                    mtbNationalID.UnMaskedText = ModelAccount.Patient.NationalID.Trim();
                }

                if ( ModelAccount.AdmitDateUnaltered == DateTime.MinValue )
                {
                    ModelAccount.AdmitDateUnaltered = ModelAccount.AdmitDate;
                }

                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFiveDaysPast ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFutureDate ), Model );
            }
            else
            {
                if( ModelAccount.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) ||
                     ModelAccount.Activity.GetType().Equals( typeof( PreAdmitNewbornActivity ) ) )
                {
                    if( ModelAccount.AdmitDate.Hour != 0 ||
                        ModelAccount.AdmitDate.Minute != 0 )
                    {
                        mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}",
                            ModelAccount.AdmitDate.Hour,
                            ModelAccount.AdmitDate.Minute );
                    }
                    else
                    {
                        mtbAdmitTime.UnMaskedText = string.Empty;
                    }
                }

            }

            mtbLastName.Focus();
        
            RegisterEvents();
            RunRules();

            if ( ModelAccount.Activity != null &&
                (ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) ||
                 ModelAccount.Activity.GetType() == typeof( UCCPostMseRegistrationActivity ) ))
            {
                cmbAppointment.SelectedIndex = cmbAppointment.FindString( SCHEDULE_CODE_WALKIN );
                cmbAppointment.Enabled = false;
            }

            if ( mtbAdmitDate.Text != "  /  /"
                && mtbAdmitDate.Text.Length != 10 )
            {
                SetAdmitDateErrBgColor();
            }

            if ( mtbAdmitDate.UnMaskedText == "01010001" )
            {
                mtbAdmitDate.UnMaskedText = string.Empty;
            }

            if ( mtbDateOfBirth.UnMaskedText == "01010001" )
            {
                mtbDateOfBirth.UnMaskedText = string.Empty;
            }

            if ( mtbAdmitTime.UnMaskedText == "0000" )
            {
                mtbAdmitTime.UnMaskedText = string.Empty;
            }

            DemographicsViewPresenter.PopulatePreOpDate( dateTimePicker_Preop.Focused );
            BirthTimePresenter.UpdateField();
            loadingModelData = false;
        }

        private void PopulateGenderAndBirthGender()
        {

            PatientGenderViewPresenter = new GenderViewPresenter(genderView, ModelAccount, Gender.PATIENT_GENDER);
            PatientGenderViewPresenter.RefreshTopPanel += gendersView_RefreshTopPanel;
            var PatientNameFeatureManager = new PatientNameFeatureManager();
            var ShouldAutoPopulateNewbornName =
                PatientNameFeatureManager.IsAutoPopulatePatientName_Enabled(ModelAccount);
            if (ShouldAutoPopulateNewbornName)
            {
                PatientGenderViewPresenter.SetNewBornName += gendersView_SetNewBornName;
            }

            PatientGenderViewPresenter.UpdateView();

            if (ModelAccount.Activity.IsAnyNewBornActivity ||
                ModelAccount.IsNewBornRegistrationMaintenanceAccount)
            {
                DisableBirthGender();
            }
            else
            {
                EnableBirthGender();
                BirthGenderViewPresenter =
                    new GenderViewPresenter(birthGenderView, ModelAccount, Gender.BIRTH_GENDER);
                BirthGenderViewPresenter.UpdateView();
            }
        }

        private void DisableBirthGender()
        {
            lblBirthGender.Enabled = false;
            lblBirthGender.Visible = false;
            birthGenderView.Enabled = false;
            birthGenderView.Visible = false;
        }
        private void EnableBirthGender()
        {
            lblBirthGender.Enabled = true;
            lblBirthGender.Visible = true;
            birthGenderView.Enabled = true;
            birthGenderView.Visible = true;
        }
        public void EnablePreOpDate( bool blnEnable )
        {
            mtbPreopDate.Enabled = dateTimePicker_Preop.Enabled = blnEnable;
        }


        /// <summary>
        /// Sets the admit date and time to current facility date and time.
        /// </summary>
        private void SetAdmitDateAndTimeToCurrentFacilityDateAndTime()
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            DateTime todaysDate = timeBroker.TimeAt( GetUser().Facility.GMTOffset,
                                                     GetUser().Facility.DSTOffset );
            ModelAccount.AdmitDate = todaysDate;
        }

        /// <summary>
        /// Sets the admit time on UI from the model. 
        /// We clear the admit time if the user had entered 00:00. 
        /// This is by design due to the fact that PBAR stores dates as integers 
        /// and we cannot distinguish between midnight and time not entered. 
        /// </summary>
        private void SetAdmitTimeOnUIFromTheModel()
        {
            DateTime admitDate = ModelAccount.AdmitDate;

            if ( IsMidnight( admitDate ) )
            {
                mtbAdmitTime.UnMaskedText = string.Empty;
            }
            else
            {
                mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", admitDate.Hour, admitDate.Minute );
            }
        }

        private void PopulateDobForNewBorn(DateTime admitDate)
        {
            if (newBornAdmit)
            {
                //Admit Newborn
                ModelAccount.Patient.DateOfBirth = admitDate;
                if (admitDate == DateTime.MinValue)
                    mtbDateOfBirth.UnMaskedText = string.Empty;
                else
                {
                    mtbDateOfBirth.Text = String.Format("{0:D2}{1:D2}{2:D4}",
                    admitDate.Month,
                    admitDate.Day,
                    admitDate.Year);
                }

                try
                {
                    SetPatientDOBForNewborn();
                }
                catch
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    UIColors.SetErrorBgColor(mtbDateOfBirth);
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    mtbDateOfBirth.Focus();
                }
            }
        }
        private static bool IsMidnight( DateTime admitDate )
        {
            return admitDate.Hour == 0 && admitDate.Minute == 0;
        }

        private void SetAdmitDateOnUIFromTheModel()
        {
            mtbAdmitDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                       ModelAccount.AdmitDate.Month,
                                                       ModelAccount.AdmitDate.Day,
                                                       ModelAccount.AdmitDate.Year );
        }

        private static string GetUnFormattedDateString( DateTime dateTime )
        {
            return String.Format( "{0:D2}{1:D2}{2:D4}", dateTime.Month, dateTime.Day, dateTime.Year );
        }

        private void DisplayMessageForMedicareAdvise()
        {
            AccountView.GetInstance().MedicareOver65Checked = true;

            DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

            if ( warningResult == DialogResult.Yes )
            {
                if ( leavingView )
                {
                    AccountView.GetInstance().Over65Check = true;
                    leavingView = false;
                }

                if ( EnableInsuranceTab != null )
                {
                    EnableInsuranceTab( this, new LooseArgs( ModelAccount ) );
                }
            }
        }

        private bool IsMedicareAdvisedForPatient()
        {
            bool isPrimaryOrSecondaryCoverageMedicare = IsPrimaryOrSecondaryCoverageMedicare();

            int patientAge = PatientAgeInYears();

            return patientAge >= SIXTY_FIVE && isPrimaryOrSecondaryCoverageMedicare;
        }

        private bool IsPrimaryOrSecondaryCoverageMedicare()
        {
            Coverage primaryCoverage = null;
            Coverage secondaryCoverage = null;

            if ( ModelAccount.Insurance != null )
            {
                primaryCoverage = ModelAccount.Insurance.PrimaryCoverage;
                secondaryCoverage = ModelAccount.Insurance.SecondaryCoverage;
            }

            return ( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) &&
                   ( secondaryCoverage == null || ( secondaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) );
        }

        public void MakeBirthTimeRequired()
        {
            UIColors.SetRequiredBgColor( mtbBirthTime );
        }

        public void MakeBirthTimePreferred()
        {
            UIColors.SetPreferredBgColor( mtbBirthTime );
        }

        public void ShowBirthTimeEnabled()
        {
            lblBirthTime.Visible = true;
            mtbBirthTime.Visible = true;
            mtbBirthTime.Enabled = true;
        }

        public void ShowBirthTimeDisabled()
        {
            lblBirthTime.Visible = true;
            mtbBirthTime.Visible = true;
            mtbBirthTime.Enabled = false;
        }
        public void DisableAndHideBirthTime()
        {
            lblBirthTime.Visible = false;
            mtbBirthTime.Visible = false;
            mtbBirthTime.Enabled = false;
        }

        public void PopulateBirthTime( DateTime birthDate )
        {
            mtbBirthTime.UnMaskedText = string.Empty;

            if ( !( birthDate.Hour == 0 && birthDate.Minute == 0 ) )
            {
                mtbBirthTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", birthDate.Hour, birthDate.Minute );
            }

        }
        /// <summary>
        /// Returns the age of a person in years.
        /// </summary>
        private int PatientAgeInYears()
        {
            string ageString = lblPatientAge.Text;
            if ( ageString != String.Empty )
            {
                switch ( ageString.Substring( ( ageString.Length ) - 1, 1 ) )
                {
                    case "y":
                        {
                            int age = Convert.ToInt32( ageString.Substring( 0, ( ageString.Length ) - 1 ) );
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

        public String PatientFirstName
        {
            get { return mtbFirstName.Text.Trim(); }
            set { mtbFirstName.Text = value; }
        }

        public Gender PatientGender
        {
            get { return (Gender)(((DictionaryEntry)genderView.GenderControl.ComboBox.SelectedItem).Key); }
            set { genderView.GenderControl.ComboBox.SelectedItem = value.AsDictionaryEntry(); }
        }

        #endregion

        #region Properties

        public Account ModelAccount
        {
            get
            {
                return (Account)Model;
            }
            set
            {
                Model = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get
            {
                if ( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }

        private IDemographicsViewPresenter DemographicsViewPresenter { get; set; }
        private IBirthTimePresenter BirthTimePresenter { get; set; }
        private GenderViewPresenter PatientGenderViewPresenter { get; set; }
        private GenderViewPresenter BirthGenderViewPresenter { get; set; }
        public PatientNamePresenter PatientNamePresenter { get; set; }
        public DateTime PreRegAdmitDate { get; set; }
        private IRaceViewPresenter RaceViewPresenter { get; set; }
        private IRaceViewPresenter Race2ViewPresenter { get; set; }
        private IAdditionalRacesViewPresenter AdditionalRacesViewPresenter { get; set; }
        private IEthnicityViewPresenter EthnicityViewPresenter { get; set; }
        private IEthnicityViewPresenter Ethnicity2ViewPresenter { get; set; }
         
        public bool LoadingModelData
        {
            get
            {
                return loadingModelData;
            }
            set
            {
                loadingModelData = value;
            }
        }
     
        #endregion

        #region Private Methods
      
        private static User GetUser()
        {
            User user = User.GetCurrent();
            user.Oid = 5;
            return user;
        }

        private void SetMaritalStatus()
        {
            cmbMaritalStatus.SelectedItem = ModelAccount.Patient.MaritalStatus;
        }

        private void SetDateOfBirthAndAge()
        {
            if ( ModelAccount.Patient.DateOfBirth == DateTime.MinValue )
            {
                lblPatientAge.Text = string.Empty;
            }
            else
            {
                dateOfBirth = ModelAccount.Patient.DateOfBirth;
                mtbDateOfBirth.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                    dateOfBirth.Month,
                    dateOfBirth.Day,
                    dateOfBirth.Year );

                lblPatientAge.Text = GetAgeFor( ModelAccount.Patient );
                //SR 1557: show age as '0d' for future DOB  
                if (IsPreAdmitNewbornActivity() || IsEditPreAdmitNewbornActivity())
                {
                    if ( string.IsNullOrEmpty( lblPatientAge.Text ) )
                        lblPatientAge.Text = "0d";
                }
            }
        }



        /// <summary>
        /// RunRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void RunRules()
        {
            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( mtbAdmitDate );
            UIColors.SetNormalBgColor( mtbAdmitTime );
            UIColors.SetNormalBgColor( mtbDateOfBirth );
            
            UIColors.SetNormalBgColor( cmbMaritalStatus );
            UIColors.SetNormalBgColor( cmbAppointment );
          

            RegisterPatientNameRequiredEvent();
            RuleEngine.EvaluateRule( typeof( MailingAddressPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( EthnicityPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( RacePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitTimePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( OnPatientDemographicsForm ), ModelAccount );
            ssnView.RunRules();
            PatientGenderViewPresenter.RunRules(); 
            RaceViewPresenter.RunInvalidCodeRules();
            RaceViewPresenter.RunRules();
            Race2ViewPresenter.RunInvalidCodeRules();
            EthnicityViewPresenter.RunInvalidCodeRules();
            EthnicityViewPresenter.RunRules();
            Ethnicity2ViewPresenter.RunInvalidCodeRules();

        }
 
        private void PopulateMaritalStatusControl()
        {
            var demographicsBrokerProxy = new DemographicsBrokerProxy();
            ICollection maritalStatusCollection = demographicsBrokerProxy.AllMaritalStatuses( User.GetCurrent().Facility.Oid );

            if ( maritalStatusCollection == null )
            {
                MessageBox.Show( "No marital statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            cmbMaritalStatus.Items.Clear();
            var singleStatus = new MaritalStatus();

            if ( maritalStatusCollection.Count > 0 )
            {
                foreach ( MaritalStatus ms in maritalStatusCollection )
                {
                    cmbMaritalStatus.Items.Add( ms );

                    if ( ms.Description != null
                        && ms.Description.ToUpper() == "SINGLE" )
                    {
                        singleStatus = ms;
                    }
                }
            }

            if( ModelAccount != null
                && ModelAccount.Activity != null
                && ModelAccount.Activity.IsNewBornRelatedActivity()
                && ModelAccount.Patient != null
                && ModelAccount.Patient.MaritalStatus != null
                && ModelAccount.Patient.MaritalStatus.Code != null
                && ModelAccount.Patient.MaritalStatus.Code == "S"
                && ModelAccount.Patient.MaritalStatus.Description != null
                && ModelAccount.Patient.MaritalStatus.Description.ToUpper() == "INVALID" )
            {
                ModelAccount.Patient.MaritalStatus = singleStatus;
            }
        }

        private void PopulatePatientMailingAddressView()
        {
            if ( ModelAccount == null || ModelAccount.Patient == null )
            {
                return;
            }

            patientMailingAddrView.KindOfTargetParty = ModelAccount.Patient.GetType();
            patientMailingAddrView.Context = Address.PatientMailing;
            patientMailingAddrView.PatientAccount = ModelAccount;

            IAddressBroker addressBroker = new AddressBrokerProxy();
            var counties = (ArrayList)addressBroker.AllCountiesFor( User.GetCurrent().Facility.Oid );

            patientMailingAddrView.IsAddressWithCounty = ( counties != null && counties.Count > 1 );

            ContactPoint mailingContactPoint = ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            ContactPoint mobileContactPoint = ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            var generalContactPoint = new ContactPoint( mailingContactPoint.Address, mailingContactPoint.PhoneNumber, mobileContactPoint.PhoneNumber,
                mailingContactPoint.EmailAddress, TypeOfContactPoint.NewMailingContactPointType() );
            patientMailingAddrView.Model = generalContactPoint;
            patientMailingAddrView.UpdateView();
        }

        private void PopulatePatientPhysicalAddressView()
        {
            if ( ModelAccount == null || ModelAccount.Patient == null )
            {
                return;
            }

            patientPhysicalAddrView.KindOfTargetParty = ModelAccount.Patient.GetType();
            patientPhysicalAddrView.Context = Address.PatientPhysical;
            patientPhysicalAddrView.PatientAccount = ModelAccount;

            ContactPoint physicalContactPoint = ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            var generalContactPoint = new ContactPoint( physicalContactPoint.Address, physicalContactPoint.PhoneNumber,
                null, TypeOfContactPoint.NewPhysicalContactPointType() );
            patientPhysicalAddrView.Model = generalContactPoint;
            patientPhysicalAddrView.UpdateView();
        }

        private void PopulateScheduleCodeComboBox()
        {
            var scheduleCodeBrokerProxy = new ScheduleCodeBrokerProxy();
            var scheduleCodesForFacility = (ArrayList)scheduleCodeBrokerProxy.AllScheduleCodes( User.GetCurrent().Facility.Oid );

            cmbAppointment.Items.Clear();

            foreach ( ScheduleCode s in scheduleCodesForFacility )
            {
                cmbAppointment.Items.Add( s );
            }
        }

        private void SetAdmitDateErrBgColor()
        {
            month = day = year = -1;
            UIColors.SetErrorBgColor( mtbAdmitDate );
        }

        private void SetAdmitDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbAdmitDate );
            Refresh();
        }

        public void SetPreOpDateErrBgColor()
        {
            UIColors.SetErrorBgColor( mtbPreopDate );
        }

        public void SetPreOpDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbPreopDate );
            Refresh();
        }

        public void RegisterPreOpDateEvent()
        {
            RuleEngine.RegisterEvent( typeof( PreopDateRequired ), Model, PreopDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( PreopDatePreferred ), Model, PreopDatePreferredEventHandler);
        }

        private void RegisterEvents()
        {
            if ( i_Registered )
            {
                return;
            }

            i_Registered = true;

            RegisterPreOpDateEvent();
            RuleEngine.RegisterEvent( typeof( MailingAddressRequired ), Model, patientMailingAddrView.AddressRequiredEventHandler );
            RuleEngine.RegisterEvent(typeof( PhysicalAddressRequired), Model, patientPhysicalAddrView.AddressRequiredEventHandler);
            RuleEngine.RegisterEvent( typeof( MailingAddressPreferred ), Model, patientMailingAddrView.AddressPreferredEventHandler );
            RegisterPatientNameRequiredEvent();
          
            RuleEngine.RegisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            
            RuleEngine.RegisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
             
            RuleEngine.RegisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
            RuleEngine.RegisterEvent(typeof (AdmitDateWithin90DaysFutureDate), Model, AdmitDateWithin90DaysFutureDateEventHandler);
            RuleEngine.RegisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitTimePreferred ), Model, AdmitTimePreferredEventHandler );
            
            RuleEngine.RegisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, mtbAdmitDate, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, mtbAdmitDate, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( MailingAddressAreaCodePreferred ), Model, patientMailingAddrView.AreaCodePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( MailingAddressPhonePreferred ), Model, patientMailingAddrView.PhonePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
          
            RuleEngine.RegisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
       
            RuleEngine.RegisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
        }

        public void RegisterPatientNameRequiredEvent()
        {
            RuleEngine.RegisterEvent(typeof (LastNameRequired), Model, LastNameRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (FirstNameRequired), Model, FirstNameRequiredEventHandler);
        }

        private void UnregisterEvents()
        {
            i_Registered = false;

            RuleEngine.UnregisterEvent( typeof( MailingAddressRequired ), Model, patientMailingAddrView.AddressRequiredEventHandler );
            RuleEngine.UnregisterEvent(typeof( PhysicalAddressRequired ), Model, patientPhysicalAddrView.AddressRequiredEventHandler);
            RuleEngine.UnregisterEvent( typeof( MailingAddressPreferred ), Model, patientMailingAddrView.AddressPreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
            
            RuleEngine.UnregisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
        
            RuleEngine.UnregisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( PreopDateRequired ), Model, PreopDateRequiredEventHandler);
            RuleEngine.UnregisterEvent( typeof( PreopDatePreferred ), Model, PreopDatePreferredEventHandler);
            RuleEngine.UnregisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateWithin90DaysFutureDate ), Model, AdmitDateWithin90DaysFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimePreferred ), Model, AdmitTimePreferredEventHandler );
           
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.UnregisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MailingAddressAreaCodePreferred ), Model, patientMailingAddrView.AreaCodePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MailingAddressPhonePreferred ), Model, patientMailingAddrView.PhonePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
          
            RuleEngine.UnregisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
           
            RuleEngine.UnregisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
         }

        private bool VerifyAdmitDate()
        {
            if ( mtbAdmitDate.UnMaskedText.Trim() == string.Empty
                || mtbAdmitDate.UnMaskedText.Trim() == "01010001" )
            {
                return true;
            }

            bool result = true;

            if ( mtbAdmitDate.UnMaskedText != string.Empty
                && mtbAdmitDate.UnMaskedText.Trim().Length != 0
                && mtbAdmitDate.Text.Length != 10 )
            {
                SetAdmitDateErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_ERRMSG, "Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                return false;
            }
            DateTime theDate;
            try
            {
                theDate = GetAdmitDateFromUI();
            }
            catch
            {
                SetAdmitDateErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                return false;
            }

            // If GetAdmitDateFromUI throws an exception, the date will be equal to MinValue
            if ( theDate == DateTime.MinValue || DateValidator.IsValidDate( theDate ) == false )
            {
                SetAdmitDateErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                result = false;
            }
            else
            {
                DateTime menstrualDate = GetOccurrenceCode10MenstrualDate();
                if ( menstrualDate != DateTime.MinValue )
                {
                    if ( menstrualDate.AddYears( 1 ) < theDate ||
                        menstrualDate > theDate )
                    {
                        SetAdmitDateErrBgColor();

                        string errMsg = "Either the date of the last menstrual period\n("
                            + CommonFormatting.LongDateFormat( menstrualDate ) + ") or the admit date\n("
                            + mtbAdmitDate.Text + ") must be modified, "
                            + UIErrorMessages.OCCURRENCECODE_BAD_MENSTRUAL_DATE_MSG;

                        MessageBox.Show( errMsg, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );

                        mtbAdmitDate.Focus();
                        result = false;
                    }
                }
            }

            return result;
        }

        public void SetPreOpDateFocus()
        {
            mtbPreopDate.Focus();
        }

        public void ShowPreOpDateIncompleteErrorMessage()
        {
            MessageBox.Show( UIErrorMessages.PREOPDATE_INCOMPLETE_ERRMSG, "Date",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
        }

        public void ShowPreOpDateInvalidErrorMessage()
        {
            MessageBox.Show( UIErrorMessages.PREOPDATE_INVALID_ERRMSG, "Date",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
        }

        public void ShowPreOpDateAfterAdmitDateErrorMessage()
        {
            MessageBox.Show( UIErrorMessages.PREOPDATE_AFTER_ADMITDATE_ERRMSG, "Pre-op Date",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
        }

        public void SetBlankPreOpDate()
        {
            mtbPreopDate.UnMaskedText = String.Empty;
        }

        public void AutoSetPreOpDateWithAdmitDate()
        {
            mtbPreopDate.UnMaskedText = mtbAdmitDate.UnMaskedText;
        }

        public void SetPreopDateFromModel()
        {
            mtbPreopDate.UnMaskedText =
                String.Format( "{0:D2}{1:D2}{2:D4}",
                               ModelAccount.PreopDate.Month,
                               ModelAccount.PreopDate.Day,
                               ModelAccount.PreopDate.Year );
        }

        private DateTime GetOccurrenceCode10MenstrualDate()
        {
            DateTime occ10Date = DateTime.MinValue;
            foreach ( OccurrenceCode occ in ModelAccount.OccurrenceCodes )
            {
                if ( occ.Code == "10" )
                {
                    occ10Date = occ.OccurrenceDate;
                    break;
                }
            }

            return occ10Date;
        }

        private bool VerifyAdmitDateForNewbornRegistration()
        {
            if ( mtbAdmitDate.UnMaskedText.Trim() == string.Empty
                || mtbAdmitDate.UnMaskedText.Trim() == "01010001"
                || mtbAdmitDate.Text == DateTime.MinValue.ToString( "MM/dd/yyyy" ) )
            {
                return true;
            }

            bool result = true;
            SetAdmitDateNormalBgColor();

            if ( mtbAdmitDate.Text.Length != 10 )
            {
                SetAdmitDateErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                return false;
            }

            month = Convert.ToInt32( mtbAdmitDate.Text.Substring( 0, 2 ) );
            day = Convert.ToInt32( mtbAdmitDate.Text.Substring( 3, 2 ) );
            year = Convert.ToInt32( mtbAdmitDate.Text.Substring( 6, 4 ) );

            try
            {   // Check the date entered is not in the future
                var theDate = new DateTime( year, month, day );

                if ( DateValidator.IsValidDate( theDate ) == false )
                {
                    SetAdmitDateErrBgColor();
                    MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                    result = false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                SetAdmitDateErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                result = false;
            }
            return result;
        }

        private static string GetAgeFor( Person person )
        {
            ITimeBroker tb = ProxyFactory.GetTimeBroker();
            DateTime facilityTime = tb.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                               User.GetCurrent().Facility.DSTOffset );
            return ( person.AgeAt( facilityTime ) );
        }

        private void UpdateAkaName()
        {
            if ( ModelAccount.Patient.HasAliases() )
            {
                ArrayList arraylist = ModelAccount.Patient.Aliases;
                var showAkaNames = new ArrayList();
                foreach ( var nameObject in arraylist )
                {
                    var name = (Name)nameObject;
                    //if name is NOT Confidential then show/add AKAs
                    if ( !( name.IsConfidential ) )
                    {
                        showAkaNames.Add( name );
                    }
                }
                var sorter = new Sorter( SortOrder.Ascending, TIMESTAMP, LASTNAME, FIRSTNAME );
                if ( showAkaNames.Count > 0 )
                {
                    showAkaNames.Sort( sorter );
                    var name = (Name)showAkaNames[0];
                    if ( !( name.IsConfidential ) )
                    {
                        lblAKA.Text = name.AsFormattedName();
                        return;
                    }
                }
            }

            lblAKA.Text = String.Empty;
        }

        private void UpdateConditionCode( DateTime dateOfBirthEntered )
        {
            var condBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode code = condBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid,
                ConditionCode.CONDITIONCODE_DOB_OVER_100Y );

            if ( dateOfBirthEntered.AddYears( 100 ) < DateTime.Now )
            {
                ModelAccount.AddConditionCode( code );
            }
            else
            {
                ModelAccount.RemoveConditionCode( code );
            }
        }

        private static DateTime GetCurrentFacilityDateTime()
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                      User.GetCurrent().Facility.DSTOffset );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName ); 
           
        }

        #endregion

        #region Construction and Finalization

        public DemographicsView()
        {
            InitializeComponent();
            ConfigureControls();
         
            patientMailingAddrView.SetGroupBoxText( "Patient mailing address and contact" );
            patientMailingAddrView.EditAddressButtonText = "Edit &Address...";
            patientPhysicalAddrView.EditAddressButtonText = "Ed&it Address...";
            patientPhysicalAddrView.SetGroupBoxText( "Patient physical address (if different)" );

            btnEditAKA.Message = "Click begin manage AKA";

        } 
        
        #endregion

        #region Constants

        private const string
            SCHEDULE_CODE_WALKIN = "W",
            TIMESTAMP = "Timestamp",
            FIRSTNAME = "FirstName",
            LASTNAME = "LastName";
         
        #endregion

        private bool newBornAdmit;
        private bool blnLeaveRun;
        private bool admitDateWarning = true;
        private bool effectiveGreaterThanAdmitDateWarning = true;
        private bool expirationLesserThanAdmitDateWarning = true;
        private bool leavingView;
        private const int SIXTY_FIVE = 65;

        private DateTime dateOfBirth;
        private bool loadingModelData = true;

        private bool i_Registered;
        private int month;
        private int day;
        private int year;
        private int hour;
        private int minute;
        private RuleEngine i_RuleEngine;

        private bool isPreOpDateChange;
        private bool isAdmitDateChange;
        private SuffixPresenter suffixPresenter;
        private SequesteredPatientPresenter sequesteredPatientPresenter;


        private void mtbBirthTime_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbBirthTime );
            BirthTimePresenter.Validate( mtbBirthTime );
        }

        private bool IsPreAdmitNewbornActivity()
        {
            if ( ModelAccount.Activity != null && ModelAccount.Activity.IsPreAdmitNewbornActivity() )
            { 
                return true;
            }
            return false;
        }

        private bool IsEditPreAdmitNewbornActivity()
        {
            if(ModelAccount.Activity != null && ModelAccount.Activity.IsMaintenanceActivity() && 
                ModelAccount.Activity.AssociatedActivityType !=null && 
                ModelAccount.Activity.AssociatedActivityType==typeof(PreAdmitNewbornActivity))
            {
                return true;
            }
            return false;
        }

        private bool IsActivatePreAdmitNewbornActivity()
        {
            if(ModelAccount.Activity != null && ModelAccount.Activity.GetType().Equals( typeof( AdmitNewbornActivity )) &&
                ModelAccount.Activity.AssociatedActivityType !=null && 
                    ModelAccount.Activity.AssociatedActivityType==typeof(ActivatePreRegistrationActivity))
            {
                return true;
            }
            return false;
            
        }

        private bool IsNewbornActivity()
        {
            if ( ModelAccount.Activity != null && ModelAccount.Activity.IsAdmitNewbornActivity() &&
                    (ModelAccount.Activity.AssociatedActivityType == null 
                        || ModelAccount.Activity.AssociatedActivityType != typeof( ActivatePreRegistrationActivity ) ))
            {
                return true;
            }
            return false;
        }

        private void mtbDateOfBirth_TextChanged(object sender, EventArgs e)
        {
            BirthTimePresenter.UpdateField();
        }

        private void gendersView_RefreshTopPanel(object sender, EventArgs e)
        {
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void gendersView_SetNewBornName(object sender, EventArgs e)
        {
            if (!loadingModelData)
            {
                PatientNamePresenter.SetNewbornName();

                UIColors.SetNormalBgColor(mtbFirstName);
                RuleEngine.EvaluateRule(typeof(FirstNameRequired), Model);
            }
        }
    }
 
}

