using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.DemographicsViews;
using PatientAccess.UI.DemographicsViews.Presenters;
using PatientAccess.UI.DemographicsViews.ViewImpl;
using PatientAccess.UI.DemographicsViews.Views;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.ShortRegistration.ContactViews;
using Peradigm.Framework.Domain.Collections;
using SortOrder = Peradigm.Framework.Domain.Collections.SortOrder;

namespace PatientAccess.UI.ShortRegistration.DemographicsViews
{
    /// <summary>
    /// Summary description for ShortDemographicsView.
    /// </summary>
    public class ShortDemographicsView : ControlView, IShortDemographicsView, IPatientNameView
    {
        #region Events
        public event EventHandler RefreshTopPanel;
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void UpdateAkaName( object sender, EventArgs e )
        {
            var manageAkaDialog = ( ManageAKADialog )sender;
            ModelAccount.Patient = manageAkaDialog.Model_Patient;
            UpdateAkaName();
        }

        private void DemographicsView_Enter( object sender, EventArgs e )
        {
            RegisterEvents();
            leavingView = false;
            if( !loadingModelData && AccountView.GetInstance().IsMedicareAdvisedForPatient() )
            {
                DisplayMessageForMedicareAdvise();
            }
        }

        private void DemographicsView_Leave( object sender, EventArgs e )
        {
            sequesteredPatientPresenter.IsPatientSequestered();
            blnLeaveRun = true;
            leavingView = true;
            
            DateTime admitDate = UpdateAdmitDate();
            SetAdmitDateOnModel( admitDate );

            RuleEngine.EvaluateRule( typeof( OnShortDemographicsForm ), ModelAccount );
            
            blnLeaveRun = false;

            UnregisterEvents();
        }

        private void mtbLastName_Validating( object sender, CancelEventArgs e )
        {
            RegisterEventsForShortMaintenance();
            var lastName = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( lastName );
            if( ModelAccount != null && ModelAccount.Patient != null )
            {
                ModelAccount.Patient.LastName = lastName.Text.Trim();
                Activity currentActivity = ModelAccount.Activity;

                AddPreviousNameToAka( currentActivity );
                RuleEngine.EvaluateRule( typeof( LastNameRequired ), Model );
                 
                if( RefreshTopPanel != null )
                {
                    RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
                }
            }
        }

        private void mtbFirstName_Validating( object sender, CancelEventArgs e )
        {
            var firstName = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( firstName );
            ModelAccount.Patient.FirstName = firstName.Text.Trim();
            Activity currentActivity = ModelAccount.Activity;

            AddPreviousNameToAka( currentActivity );
 
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), Model );
            if( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
        }

        private void AddPreviousNameToAka( Activity activity )
        {
            if( ( activity is AdmitNewbornActivity ) ||
                ( activity is PostMSERegistrationActivity ) ||
                ( activity is PreRegistrationActivity ) ||
                ( activity is RegistrationActivity ) ||
                ( activity is MaintenanceActivity ) ||
                ( activity is ShortPreRegistrationActivity ) ||
                ( activity is ShortRegistrationActivity ) ||
                ( activity is ShortMaintenanceActivity )
                )
            {
                ModelAccount.Patient.AddPreviousNameToAKA();
            }
        }

        private void mtbMiddleInitial_Validating( object sender, CancelEventArgs e )
        {
            var middleInitial = ( MaskedEditTextBox )sender;
            ModelAccount.Patient.Name.MiddleInitial = middleInitial.Text.Trim();
        }

        private void maskNameSuffix_Validating( object sender, CancelEventArgs e )
        {
            var nameSuffix = ( MaskedEditTextBox )sender;
            ModelAccount.Patient.Name.Suffix = nameSuffix.Text.Trim();
        }

        
        private void mtbAdmitDate_Validating( object sender, CancelEventArgs e )
        {
            if (ActiveControl == null
                && mtbAdmitDate.Text.Length == 10)
            {
                return;
            }

            if (!blnLeaveRun)
            {
                SetAdmitDateNormalBgColor();
            }

            if (dateTimePicker.Focused)
            {
                return;
            }
            if (newBornAdmit)
            {
                if (!VerifyAdmitDateForNewbornRegistration())
                {
                    return;
                }
            }
            else
            {
                if (!VerifyAdmitDate())
                {
                    return;
                }
            }

            if (!blnLeaveRun)
            {
                DateTime admitDate = UpdateAdmitDate();
                SetAdmitDateOnModel(admitDate);
            }

            if (!dateTimePicker.Focused
                && !blnLeaveRun)
            {
                RuleEngine.EvaluateRule(typeof (AdmitDateEnteredFiveDaysPast), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateEnteredFutureDate), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateRequired), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateFiveDaysPast), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateFutureDate), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateTodayOrGreater), Model);
                RuleEngine.EvaluateRule(typeof (AdmitDateWithinSpecifiedSpan), Model);
                RuleEngine.EvaluateRule(typeof (AdmitTimePreferred ), Model);
                RuleEngine.EvaluateRule(typeof (AdmitTimeRequired), Model);
            }

            if (mtbAdmitDate.UnMaskedText != string.Empty)
            {
                AdmitDateToInsuranceValidation.CheckAdmitDateToInsurance(ModelAccount, Name);
            }

            CheckAdmitDateToAuthorization();

            if (newBornAdmit)
            {
                ModelAccount.Patient.DateOfBirth = ModelAccount.AdmitDate;
                mtbDateOfBirth.Text = String.Format("{0:D2}{1:D2}{2:D4}",
                                                    ModelAccount.AdmitDate.Month,
                                                    ModelAccount.AdmitDate.Day,
                                                    ModelAccount.AdmitDate.Year);
            }

            if (!loadingModelData)
            {
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }
        }

        private void mtbAdmitTime_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbAdmitTime );

            if (mtbAdmitTime.UnMaskedText.Trim() != string.Empty
                && mtbAdmitTime.UnMaskedText.Trim() != "0000" )
            {
                if(  DateValidator.IsValidTime( mtbAdmitTime ) == false )
                {
                    if( !dateTimePicker.Focused )
                    {
                        mtbAdmitTime.Focus();
                    }
                    return;
                }
                CheckTimeIsNotInFuture() ;
            }

            if( newBornAdmit )
            {
                if( !VerifyAdmitDateForNewbornRegistration() )
                {
                    return;
                }
            }
            else
            {
                if( !VerifyAdmitDate() )
                {
                    return;
                }
            }

            ModelAccount.AdmitDate = GetAdmitDateFromUI();
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitTimePreferred ), Model );
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
               
                if ( IsTodaysDate() && IsTimeInTheFuture(originalAdmitHour, originalAdmitMinute, enteredHour, enteredMinute))
                {
                    mtbAdmitTime.Focus();
                    mtbAdmitTime.Text = String.Format( "{0:D2}{1:D2}", originalAdmitHour, originalAdmitMinute );
                    MessageBox.Show(UIErrorMessages.ADMIT_TIME_CANNOT_BE_IN_FUTURE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
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
                             ModelAccount.Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreMSERegisterActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreMSERegistrationWithOfflineActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( PreRegistrationWithOfflineActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( RegistrationActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) ||
                             ModelAccount.Activity.GetType().Equals( typeof( ShortRegistrationWithOfflineActivity)) ||
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
        private static bool IsTimeInTheFuture(int originalAdmitHour, int originalAdmitMinute, int enteredHour, int enteredMinute)
        {
            int todaysTotalSeconds = (originalAdmitHour * 60) + originalAdmitMinute;
            int enteredTotalSeconds = (enteredHour * 60) + enteredMinute;

            if (enteredTotalSeconds > todaysTotalSeconds)
            {
                return true;
            }
            return false;
        }

        private bool IsTodaysDate()
        {
            DateTime uiEnteredAdmitDate = GetAdmitDateFromUI() ;
            return uiEnteredAdmitDate.Date.Equals(DateTime.Today);
        }

        public DateTime GetAdmitDateFromUI()
        {
            DateTime theDate = DateTime.MinValue;

            if( mtbAdmitDate.UnMaskedText != string.Empty )
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
            if( mtbDateOfBirth.UnMaskedText == string.Empty ||
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

            var dateOfBirthTextBox = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( dateOfBirthTextBox );

            if( dateOfBirthTextBox.Text.Length != 10 )
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
                    Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 3, 2 ) ) );

                if( dateOfBirthEntered > DateTime.Today )
                {
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDateOfBirth.Focus();
                }
                else if( DateValidator.IsValidDate( dateOfBirthEntered ) == false )
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

                    if( RefreshTopPanel != null )
                    {
                        RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
                    }

                    SetDateOfBirthAndAge();
                    RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
                    RuleEngine.EvaluateRule( typeof( InValidDateOfBirth ), Model );

                    // Redirect user to Insurance tab if the patient is over 65, and Medicare
                    // has not been specified as either the primary or secondary payor
                    if( !AccountView.GetInstance().MedicareOver65Checked )
                    {
                        if( IsMedicareAdvisedForPatient() )
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
            if (!loadingModelData)
            {
                ssnView.ResetSSNControl();
            }
        }

        private void ssnView_ssnNumberChanged( object sender, EventArgs e )
        {
            if( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
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

            
            RuleEngine.EvaluateRule( typeof( OnShortDemographicsForm ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( MailingAddressRequired ), ModelAccount );
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

            if( admitDateWarning && !isAdmiteDateToAuthorzationDates || !isAdmiteDateToSecondaryAuthorzationDates )
            {
                result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_ADMIT_DATE_OUT_OF_RANGE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                admitDateWarning = false;
                if( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if( effectiveGreaterThanAdmitDateWarning && !isAdmiteDateEarlierThanAuthEffectiveDate || !isAdmiteDateEarlierThanSecondaryAuthEffectiveDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_EARLIER_THAN_AUTHORIZATION_EFFECTIVE_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                effectiveGreaterThanAdmitDateWarning = false;
                if( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if( expirationLesserThanAdmitDateWarning && !isAdmiteDateLaterThanAuthExpirationDate || !isAdmiteDateLaterThanSecondaryAuthExpirationDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_LATER_THAN_AUTHORIZATION_EXPIRATION_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                expirationLesserThanAdmitDateWarning = false;
                if( result == DialogResult.No )
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

            if( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT &&
                ModelAccount.Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) )
            {
                mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", dt.Hour, dt.Minute );
            }
            
            mtbAdmitDate.Focus();
        }
         
        /// <summary>
        /// Updates the admit date.
        /// </summary>
        private DateTime UpdateAdmitDate()
        {
            DateTime admitDate = DateTime.MinValue;
            if( ModelAccount != null )
            {
                if( mtbAdmitDate.UnMaskedText.Trim() == String.Empty )
                {
                    if(mtbAdmitTime.UnMaskedText.Trim() == String.Empty)
                    {
                        ModelAccount.AdmitDate = admitDate = DateTime.MinValue;
                    }
                    else
                    {
                        ModelAccount.AdmitDate = admitDate = DateTime.MinValue;
                        string unFormattedDateString = GetUnFormattedDateString( ModelAccount.AdmitDate );

                        GetDateAndTimeFrom( unFormattedDateString, mtbAdmitTime.UnMaskedText );
                         
                    }
                    RuleEngine.EvaluateRule(typeof(AdmitDateRequired), Model);
                    
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

     
        private void cmbMaritalStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            ModelAccount.Patient.MaritalStatus = cmbMaritalStatus.SelectedItem as MaritalStatus;
            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
        private void btnEditRace_Click(object sender, EventArgs e)
        {
            AdditionalRacesViewPresenter.UpdateView();
            AdditionalRacesViewPresenter.ShowAdditionalRacesView();
        }
        private void raceView_Leave(object sender, EventArgs e)
        {
            EnableAdditionalRaceEditButton();
        }
        private void race2View_Leave(object sender, EventArgs e)
        {
            EnableAdditionalRaceEditButton();
        }
        private void EnableAdditionalRaceEditButton()
        {
            if (AdditionalRacesViewPresenter.ShouldAdditionRaceEditButtonBeVisible(ModelAccount))
            {
                race2View.Location = new System.Drawing.Point(245, 187);
                lblRace2.Location = new System.Drawing.Point(202, 190);
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
                race2View.Location = new System.Drawing.Point(258, 187);
                lblRace2.Location = new System.Drawing.Point(214, 190);
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

        private void btnEditAKA_Click( object sender, EventArgs e )
        {
            using ( var manageAkaDialog = new ManageAKADialog() )
            {
                if( ModelAccount != null )
                {
                    if( ModelAccount.Patient != null )
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

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void cmbAppointment_Validating( object sender, CancelEventArgs e )
        {
            cmbAppointment_SelectedIndexChanged(sender, e);
            if( !blnLeaveRun )
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
            if( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbMaritalStatus );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidMaritalStatusCode ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidMaritalStatusCodeChange ), Model );
            }
            
            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
        
        private void driversLicenseView_DriversLicenseNumberChanged( object sender, EventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( DriversLicenseStateRequired ), Model );
            passportView.CheckForValidDriverLicense();
        }

        private void passportView_PassportNumberChanged( object sender, EventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( PassportCountryRequired ), Model );
        }

        private void emergencyContactView1_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( shortEmergencyContactView.RelationshipView.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1Rel ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1RelChange ), Model );
                shortEmergencyContactView.RunRules();
            }
        }

        private void cmbLanguage_SelectedIndexChanged( object sender, EventArgs e )
        {
            var language = cmbLanguage.SelectedItem as Language;
            if ( language != null )
            {
                UpdateSelectedLanguage( language );
            }
        }

        private void cmbLanguage_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor(cmbLanguage);
            Refresh();
            RuleEngine.GetInstance().EvaluateRule(typeof(InvalidLanguageCode), Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(InvalidLanguageCodeChange), Model);
            RuleEngine.OneShotRuleEvaluation<LanguageRequired>(Model, LanguageRequiredEventHandler);
            RuleEngine.OneShotRuleEvaluation<LanguagePreferred>(Model, LanguagePreferredEventHandler);
        }

        private void mtbSpecify_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbOtherLanguage );
            Presenter.UpdateOtherLanguage( mtbOtherLanguage.Text );
        }

        private void UpdateSelectedLanguage( Language language )
        {
            if ( language != null )
            {
                ModelAccount.Patient.Language = language;
                if ( !blnLeaveRun )
                {
                    UIColors.SetNormalBgColor(cmbLanguage);
                    Refresh();
                    RuleEngine.GetInstance().EvaluateRule(typeof (InvalidLanguageCode), Model);
                    RuleEngine.GetInstance().EvaluateRule(typeof (InvalidLanguageCodeChange), Model);
                    RuleEngine.OneShotRuleEvaluation<LanguageRequired>(Model, LanguageRequiredEventHandler);
                    RuleEngine.OneShotRuleEvaluation<LanguagePreferred>(Model, LanguagePreferredEventHandler);
                }
                Presenter.SelectedLanguageChanged( ModelAccount.Patient.Language );
            }
        }
        private IEthnicityViewPresenter EthnicityViewPresenter { get; set; }
        private IEthnicityViewPresenter Ethnicity2ViewPresenter { get; set; }

        public void PopulateOtherLanguage()
        {
            if ( ModelAccount != null && ModelAccount.Patient != null && ModelAccount.Patient.Language != null )
            {
                mtbOtherLanguage.Text = ModelAccount.Patient.OtherLanguage != null 
                                            ? ModelAccount.Patient.OtherLanguage.Trim() : string.Empty;
            }
        }

        public void ClearOtherLanguage()
        {
            mtbOtherLanguage.Text = string.Empty;
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( Control comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidLanguageCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbLanguage );
        }
        private void InvalidLanguageCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbLanguage );
        }

        private void InvalidScheduleCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbAppointment );
        }
     
        private void InvalidMaritalStatusCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbMaritalStatus );
        }
        
        private void InvalidEmergContact_1RelChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( shortEmergencyContactView.RelationshipView.ComboBox );
        }

        //-----------------------------------------------------------------

        private void InvalidEmergContact_1RelEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( shortEmergencyContactView.RelationshipView.ComboBox );
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
            if( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
            {
                if( ModelAccount.KindOfVisit == null )
                {
                    return;
                }
                
                if( ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
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
            else if( ModelAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_RANGE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                Refresh();
            }
            else if( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity ) )
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
            if( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
            {
                if( ModelAccount.KindOfVisit == null )
                {
                    return;
                }
                if( ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
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
            else if( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                || ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
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
            if( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
            {
                if( ModelAccount.KindOfVisit == null )
                {
                    return;
                }
                
                if( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT )
                {
                    ProcessTextboxErrorEvent(mtbAdmitDate, UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG);
                }
            }
            else if( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                || ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
            {
                ProcessTextboxErrorEvent(mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG); 
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

            if( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ACTIVATE_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG );
            }
            else if( ModelAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
            }
        }

        private void AdmitDateFutureDateEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            if ( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ACTIVATE_ERRMSG );
            }
            else if ( ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) )
                ||
                ( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity )
                && ModelAccount.KindOfVisit != null && ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_REGISTER_ERRMSG );
            }
            else if( ModelAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity )
                ||
                ( ModelAccount.Activity.GetType() == typeof( ShortMaintenanceActivity )
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

        private void LanguagePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbLanguage );
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void LanguageRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbLanguage );
        }
        
        public void MakeOtherLanguageRequired()
        {
            UIColors.SetRequiredBgColor( mtbOtherLanguage );
        }

        private void AdmitDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitDate );
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

            var appointmentComboBox = ( ComboBox )sender;
            if( appointmentComboBox.SelectedIndex > 0 )
            {
                ModelAccount.ScheduleCode = (ScheduleCode) appointmentComboBox.SelectedItem ;
            }
            else if( appointmentComboBox.SelectedIndex == 0 )
            {
                ModelAccount.ScheduleCode = null;
            }

            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), Model );
        }
        private void MaritalStatusRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbMaritalStatus );
        }

        private void AppointmentRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAppointment );
        }
        
        #endregion

        #region Methods
        public override void UpdateView()
        {

            sequesteredPatientPresenter = new SequesteredPatientPresenter(new SequesteredPatientFeatureManager(), ModelAccount);
            sequesteredPatientPresenter.IsPatientSequestered();

            Presenter = new ShortDemographicsViewPresenter( this, ModelAccount, RuleEngine.GetInstance() );
            PatientNamePresenter = new PatientNamePresenter(this, new PatientNameFeatureManager());
            ssnView.SsnFactory = new SsnFactoryCreator(ModelAccount).GetSsnFactory();
            suffixPresenter = new SuffixPresenter(suffixView, ModelAccount, "Patient");
            patientMailingAddrView.CaptureMailingAddress = true;
            AdditionalRacesViewPresenter = new AdditionalRacesViewPresenter(new AdditionalRacesView(), ModelAccount,
                new AdditionalRacesFeatureManager());
            EnableAdditionalRaceEditButton();

            if ( loadingModelData )
            {   // Initial entry to the form -- initialize controls and get the data from the model.
                PopulateGenderAndBirthGender();
                PopulateMaritalStatusControl();
                raceView.SetSizeForRaceDropdownButton();
                race2View.SetSizeForRaceDropdownButton();
                RaceViewPresenter = new RaceViewPresenter(raceView, ModelAccount, Race.RACENATIONALITY_CONTROL);
                RaceViewPresenter.UpdateView();
                Race2ViewPresenter = new RaceViewPresenter(race2View, ModelAccount, Race.RACENATIONALITY2_CONTROL);
                Race2ViewPresenter.UpdateView();
       
                ethnicityView.SetSizeForEthnicityDropdownButton();
                ethnicity2View.SetSizeForEthnicityDropdownButton();
                EthnicityViewPresenter = new EthnicityViewPresenter(ethnicityView, ModelAccount, Ethnicity.ETHNICITY_PROPERTY);
                EthnicityViewPresenter.UpdateView();
                Ethnicity2ViewPresenter = new EthnicityViewPresenter(ethnicity2View, ModelAccount, Ethnicity.ETHNICITY2_PROPERTY);
                Ethnicity2ViewPresenter.UpdateView();
               
                PopulatePatientMailingAddressView();
                PopulateScheduleCodeComboBox();
                PopulateLanguageControl();

                dateOfBirth = DateTime.MinValue;

                if( ModelAccount.Patient == null )
                {
                    MessageBox.Show( "No patient data is present!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }

                mtbLastName.Text = ModelAccount.Patient.LastName;
                mtbFirstName.Text = ModelAccount.Patient.FirstName;
                mtbMiddleInitial.Text = ModelAccount.Patient.MiddleInitial;
                suffixPresenter.UpdateView();

                UpdateAkaName();

                if( ( ModelAccount.AdmitDate == DateTime.MinValue
                      && ( ModelAccount.Activity != null && ModelAccount.Activity.GetType() != typeof( ShortPreRegistrationActivity ) ) )
                    ||
                    ( ModelAccount.Activity != null && ModelAccount.Activity.GetType() == typeof( ShortRegistrationActivity )
                      && ModelAccount.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) ) )
                {
                    PreRegAdmitDate = ModelAccount.AdmitDate;
                    SetAdmitDateAndTimeToCurrentFacilityDateAndTime();
                }

                if( ModelAccount.Activity != null && 
                    ( ( ModelAccount.Activity.GetType() != typeof( PreRegistrationActivity ) &&
                        ModelAccount.Activity.GetType() != typeof( ShortPreRegistrationActivity ) ) ||
                        //Convert To Short Prereg activity
                        ( ModelAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity ) && 
                            ModelAccount.Activity.AssociatedActivityType == typeof( MaintenanceActivity ) ) ||

                      ( (  ModelAccount.Activity.GetType() == typeof( PreRegistrationActivity ) || 
                           ModelAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity ) ) &&
                        ModelAccount.Activity.AssociatedActivityType == typeof( OnlinePreRegistrationActivity ) ) ) )
                {
                    SetAdmitDateOnUIFromTheModel();
                    SetAdmitTimeOnUIFromTheModel();
                }

                if( ModelAccount.Activity != null && ModelAccount.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
                {
                    mtbDateOfBirth.Enabled = false;
                    mtbAdmitTime.Text = String.Empty;
                    mtbAdmitTime.UnMaskedText = String.Empty;
                    newBornAdmit = true;
                    cmbAppointment.SelectedIndex = cmbAppointment.FindString( ModelAccount.ScheduleCode.Code );
                    cmbAppointment.Enabled = false;
                }
                dateOfBirth = ModelAccount.Patient.DateOfBirth;
                mtbDateOfBirth.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                    dateOfBirth.Month,
                    dateOfBirth.Day,
                    dateOfBirth.Year );

                if( mtbDateOfBirth.Text.Trim().Length != 0
                    && mtbDateOfBirth.Text.Length != 10 )
                {
                    UIColors.SetErrorBgColor( mtbDateOfBirth );
                }

                SetDateOfBirthAndAge();
                SetMaritalStatus();

                if( ModelAccount.ScheduleCode != null )
                {
                    cmbAppointment.SelectedItem = ModelAccount.ScheduleCode;
                }
               
                ssnView.Model = ModelAccount.Patient;
                ssnView.ModelAccount = ModelAccount;
                ssnView.UpdateView();

                if( ModelAccount.AdmitDateUnaltered == DateTime.MinValue )
                {
                    ModelAccount.AdmitDateUnaltered = ModelAccount.AdmitDate;
                }

                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFiveDaysPast ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFutureDate ), Model );
            }
            else
            {
                if( ModelAccount.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
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

            if ( ModelAccount != null && ModelAccount.Patient != null )
            {
                if ( ModelAccount.Patient.DriversLicense != null )
                {
                    driversLicenseView.Model = ModelAccount;
                    driversLicenseView.UpdateView();
                    driversLicenseView.CheckForNewBornActivity();
                }
                if ( ModelAccount.Patient.Passport != null )
                {
                    passportView.Model = ModelAccount;
                    passportView.UpdateView();
                    passportView.CheckForNewBornActivity();
                    passportView.CheckForValidDriverLicense();
                }

                if ( ModelAccount.Patient.Language != null )
                {
                    cmbLanguage.SelectedItem = ModelAccount.Patient.Language;
                }
            }

            if ( ModelAccount != null )
            {
                shortEmergencyContactView.Model = ModelAccount;
                shortEmergencyContactView.Model_EmergencyContact = ModelAccount.EmergencyContact1;
                shortEmergencyContactView.RegisterEvents();
                shortEmergencyContactView.UpdateView();
                shortEmergencyContactView.RunRules();
            }

            mtbLastName.Focus();

            RegisterEvents();
            Presenter.RegisterOtherLanguageRequiredRule();
            RunRules();

            if( ModelAccount != null && ModelAccount.Activity != null && 
                ModelAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) )
            {
                cmbAppointment.SelectedIndex = cmbAppointment.FindString( SCHEDULE_CODE_WALKIN );
                cmbAppointment.Enabled = false;
            }

            if( mtbAdmitDate.Text != "  /  /"
                && mtbAdmitDate.Text.Length != 10 )
            {
                SetAdmitDateErrBgColor();
            }

            if( mtbAdmitDate.UnMaskedText == "01010001" )
            {
                mtbAdmitDate.UnMaskedText = string.Empty;
            }

            if( mtbDateOfBirth.UnMaskedText == "01010001" )
            {
                mtbDateOfBirth.UnMaskedText = string.Empty;
            }

            if( mtbAdmitTime.UnMaskedText == "0000" )
            {
                mtbAdmitTime.UnMaskedText = string.Empty;
            }
            
            loadingModelData = false;
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

            mtbAdmitTime.UnMaskedText = IsMidnight( admitDate ) ? string.Empty : String.Format( "{0:D2}{1:D2}", admitDate.Hour,admitDate.Minute );
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

            if( warningResult == DialogResult.Yes )
            {
                if( leavingView )
                {
                    AccountView.GetInstance().Over65Check = true;
                    leavingView = false;
                }

                if( EnableInsuranceTab != null )
                {
                    EnableInsuranceTab( this, new LooseArgs( ModelAccount ) );
                }
            }
        }

        private bool IsMedicareAdvisedForPatient()
        {
            bool isPrimaryOrSecondaryCoverageMedicare = IsPrimaryOrSecondaryCoverageMedicare();

            int patientAge = PatientAgeInYears();

            return  patientAge >= SIXTY_FIVE && isPrimaryOrSecondaryCoverageMedicare;
        }

        private bool IsPrimaryOrSecondaryCoverageMedicare() 
        {
            Coverage primaryCoverage = null;
            Coverage secondaryCoverage = null;

            if( ModelAccount.Insurance != null )
            {
                primaryCoverage = ModelAccount.Insurance.PrimaryCoverage;
                secondaryCoverage = ModelAccount.Insurance.SecondaryCoverage;
            }

            return ( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) && 
                   ( secondaryCoverage == null || ( secondaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) );
        }

        /// <summary>
        /// Returns the age of a person in years.
        /// </summary>
        private int PatientAgeInYears()
        {
            string ageString = lblPatientAge.Text;
            if( ageString != String.Empty )
            {
                switch( ageString.Substring( ( ageString.Length ) - 1, 1 ) )
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
        #endregion

        #region Properties

        public Account ModelAccount
        {
            get
            {
                return ( Account )Model;
            }
            set
            {
                Model = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? ( i_RuleEngine = RuleEngine.GetInstance() ); }
        }

        public DateTime PreRegAdmitDate { get; set; }

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

        private IShortDemographicsViewPresenter Presenter { get; set; }
        private IRaceViewPresenter RaceViewPresenter { get; set; }
        private IRaceViewPresenter Race2ViewPresenter { get; set; }
        private IAdditionalRacesViewPresenter AdditionalRacesViewPresenter { get; set; }
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
        public PatientNamePresenter PatientNamePresenter { get; set; }
        public bool OtherLanguageVisibleAndEnabled
        {
            set
            {
                mtbOtherLanguage.Visible = value;
                lblSpecify.Visible = value;
                mtbOtherLanguage.Enabled = value;
                lblSpecify.Enabled = value;
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
            if( ModelAccount.Patient.DateOfBirth == DateTime.MinValue )
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
            }
        }
        

        /// <summary>
        /// RunRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void RunRules()
        {
            RegisterEventsForShortMaintenance();
            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( mtbAdmitDate );
            UIColors.SetNormalBgColor( mtbAdmitTime );
            UIColors.SetNormalBgColor( mtbDateOfBirth );
         
            UIColors.SetNormalBgColor( cmbMaritalStatus );
            
            UIColors.SetNormalBgColor( cmbAppointment );
            UIColors.SetNormalBgColor( cmbLanguage );
            UIColors.SetNormalBgColor( mtbOtherLanguage );
           
            RuleEngine.EvaluateRule( typeof( OnShortDemographicsForm ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( LastNameRequired ), ModelAccount );
           
            RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactNamePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactAreaCodePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactPhonePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactNameRequired), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactAreaCodeRequired), ModelAccount );
            RuleEngine.EvaluateRule( typeof( ContactPhoneRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( LanguageRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( LanguagePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( OtherLanguageRequired ), ModelAccount );
            
            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), ModelAccount );
           
            
            RuleEngine.EvaluateRule( typeof( RaceRequired ), Model );
            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
            RuleEngine.EvaluateRule( typeof( EthnicityRequired ), Model );
            Presenter.EvaluateOtherLanguageRequired();
            shortEmergencyContactView.RunRules();
            ssnView.RunRules();
            RaceViewPresenter.RunInvalidCodeRules();
            RaceViewPresenter.RunRules();
            Race2ViewPresenter.RunInvalidCodeRules();
            PatientGenderViewPresenter.RunRules();
            EthnicityViewPresenter.RunInvalidCodeRules();
            EthnicityViewPresenter.RunRules();
            Ethnicity2ViewPresenter.RunInvalidCodeRules();

           
        }

        private void PopulateLanguageControl()
        {
            var demographicsBrokerProxy = new DemographicsBrokerProxy();
            ICollection languageCollection = demographicsBrokerProxy.AllLanguages( User.GetCurrent().Facility.Oid );

            cmbLanguage.Items.Clear();

            if ( languageCollection == null )
            {
                MessageBox.Show( "No languages were found", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            foreach ( Language ms in languageCollection )
            {
                cmbLanguage.Items.Add( ms );
            }
        }

        private void PopulateMaritalStatusControl()
        {
            var demographicsBrokerProxy = new DemographicsBrokerProxy();
            ICollection maritalStatusCollection =
                demographicsBrokerProxy.AllMaritalStatuses(User.GetCurrent().Facility.Oid);

            if (maritalStatusCollection == null)
            {
                MessageBox.Show("No marital statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmbMaritalStatus.Items.Clear();
            var singleStatus = new MaritalStatus();

            if (maritalStatusCollection.Count > 0)
            {
                foreach (MaritalStatus ms in maritalStatusCollection)
                {
                    cmbMaritalStatus.Items.Add(ms);

                    if (ms.Description != null
                        && ms.Description.ToUpper() == "SINGLE")
                    {
                        singleStatus = ms;
                    }
                }
            }

            if (ModelAccount != null
                && ModelAccount.Activity != null
                && ModelAccount.Activity.GetType().Equals(typeof(AdmitNewbornActivity))
                && ModelAccount.Patient != null
                && ModelAccount.Patient.MaritalStatus != null
                && ModelAccount.Patient.MaritalStatus.Code != null
                && ModelAccount.Patient.MaritalStatus.Code == "S"
                && ModelAccount.Patient.MaritalStatus.Description != null
                && ModelAccount.Patient.MaritalStatus.Description.ToUpper() == "INVALID")
            {
                ModelAccount.Patient.MaritalStatus = singleStatus;
            }
        }

        private void PopulatePatientMailingAddressView()
        {
            if( ModelAccount == null || ModelAccount.Patient == null )
            {
                return;
            }

            patientMailingAddrView.KindOfTargetParty = ModelAccount.Patient.GetType();
            patientMailingAddrView.Context = Address.PatientMailing;
            patientMailingAddrView.PatientAccount = ModelAccount;

            IAddressBroker addressBroker = new AddressBrokerProxy();
            var counties = ( ArrayList )addressBroker.AllCountiesFor( User.GetCurrent().Facility.Oid );

            patientMailingAddrView.IsAddressWithCounty = ( counties != null && counties.Count > 1 );

            ContactPoint mailingContactPoint = ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            ContactPoint mobileContactPoint = ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            var generalContactPoint = new ContactPoint( mailingContactPoint.Address, mailingContactPoint.PhoneNumber, mobileContactPoint.PhoneNumber,
                mailingContactPoint.EmailAddress, TypeOfContactPoint.NewMailingContactPointType() );
            patientMailingAddrView.Model = generalContactPoint;
            patientMailingAddrView.UpdateView();
        }
        private void PopulateScheduleCodeComboBox()
        {
            var scheduleCodeBrokerProxy = new ScheduleCodeBrokerProxy();
            var scheduleCodesForFacility = (ArrayList)scheduleCodeBrokerProxy.AllScheduleCodes( User.GetCurrent().Facility.Oid );

            cmbAppointment.Items.Clear();

            foreach( ScheduleCode s in scheduleCodesForFacility )
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

        private void SetAdmitTimeNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbAdmitTime );
            Refresh();
        }

        private void RegisterEventsForShortMaintenance()
        {
            UnregisterEvents();
            RegisterEvents();
            shortEmergencyContactView.UnregisterEvents();
            shortEmergencyContactView.RegisterEvents();
            ssnView.RegisterRules();
        }

        private void RegisterEvents()
        {
            if( i_Registered )
            {
                return;
            }

            i_Registered = true;

            RuleEngine.RegisterEvent( typeof( MailingAddressRequired ), Model, patientMailingAddrView.AddressRequiredEventHandler );
            RegisterPatientNameRequiredEvent();
            RuleEngine.RegisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler ); 
             
            RuleEngine.RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitTimePreferred ), Model, AdmitTimePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
            RegisterAdmitDateValidationRules();
            RuleEngine.RegisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );
			RuleEngine.RegisterEvent( typeof( LanguageRequired ), Model, LanguageRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( LanguagePreferred ), Model, LanguagePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( MailingAddressAreaCodePreferred ), Model, patientMailingAddrView.AreaCodePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( MailingAddressPhonePreferred ), Model, patientMailingAddrView.PhonePreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
             
            RuleEngine.RegisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidEmergContact_1Rel ), Model, InvalidEmergContact_1RelEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidEmergContact_1RelChange ), Model, InvalidEmergContact_1RelChangeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidLanguageCode ), Model, InvalidLanguageCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidLanguageCodeChange ), Model, InvalidLanguageCodeChangeEventHandler );
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
             RuleEngine.UnregisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimePreferred ), Model, AdmitTimePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
            UnRegisterAdmitDateValidationRules();
			RuleEngine.UnregisterEvent( typeof( LanguageRequired ), Model, LanguageRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( LanguagePreferred ), Model, LanguagePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidEmergContact_1Rel ), Model, InvalidEmergContact_1RelEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidEmergContact_1RelChange ), Model, InvalidEmergContact_1RelChangeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidLanguageCode ), Model, InvalidLanguageCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidLanguageCodeChange ), Model, InvalidLanguageCodeChangeEventHandler );
            Presenter.UnRegisterOtherLanguageRequiredRule();
        }

        private void RegisterAdmitDateValidationRules()
        {
            RuleEngine.RegisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, mtbAdmitDate, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, mtbAdmitDate, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
        }

        private void UnRegisterAdmitDateValidationRules()
        {
            RuleEngine.UnregisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
        }

        private bool VerifyAdmitDate()
        {
            if( mtbAdmitDate.UnMaskedText.Trim() == string.Empty
                || mtbAdmitDate.UnMaskedText.Trim() == "01010001" )
            {
                return true;
            }

            bool result = true;

            if( mtbAdmitDate.UnMaskedText != string.Empty
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
            DateTime theDate = DateTime.MinValue;
            try
            {
                 theDate = GetAdmitDateFromUI();
            }
            catch
            {
                SetAdmitDateErrBgColor();
                MessageBox.Show(UIErrorMessages.ADMIT_INVALID_ERRMSG, "Date",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                mtbAdmitDate.Focus();
                return false;
            }

            // If GetAdmitDateFromUI throws an exception, the date will be equal to MinValue
            if( theDate == DateTime.MinValue || DateValidator.IsValidDate( theDate ) == false )
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
                if( menstrualDate != DateTime.MinValue )
                {
                    if( menstrualDate.AddYears( 1 ) < theDate ||
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
        
        private DateTime GetOccurrenceCode10MenstrualDate()
        {
            DateTime occ10Date = DateTime.MinValue;
            foreach( OccurrenceCode occ in ModelAccount.OccurrenceCodes )
            {
                if( occ.Code == "10" )
                {
                    occ10Date = occ.OccurrenceDate;
                    break;
                }
            }

            return occ10Date;
        }

        private bool VerifyAdmitDateForNewbornRegistration()
        {
            if( mtbAdmitDate.UnMaskedText.Trim() == string.Empty
                || mtbAdmitDate.UnMaskedText.Trim() == "01010001"
                || mtbAdmitDate.Text == DateTime.MinValue.ToString( "MM/dd/yyyy" ) )
            {
                return true;
            }

            bool result = true;
            SetAdmitDateNormalBgColor();
            SetAdmitTimeNormalBgColor();

            if( mtbAdmitDate.Text.Length != 10 )
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
                
                if( DateValidator.IsValidDate( theDate ) == false )
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
            if( ModelAccount.Patient.HasAliases() )
            {
                var patientAliases = ModelAccount.Patient.Aliases;
                var showAkaNames = new ArrayList();
                foreach ( var nameObject in patientAliases )
                {
                    var name = (Name)nameObject;
                    //if name is NOT Confidential then show/add AKAs
                    if( !( name.IsConfidential ) )
                    {
                        showAkaNames.Add( name );
                    }
                }
                var sorter = new Sorter( SortOrder.Ascending, TIMESTAMP, LASTNAME, FIRSTNAME );
                if( showAkaNames.Count > 0 )
                {
                    showAkaNames.Sort( sorter );
                    var name = (Name)showAkaNames[0];
                    if( !( name.IsConfidential ) )
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

            if( dateOfBirthEntered.AddYears( 100 ) < DateTime.Now )
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
            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix(mtbMiddleInitial);
            MaskedEditTextBoxBuilder.ConfigureOtherLanguage( mtbOtherLanguage );
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

            if (ModelAccount.Activity.IsAnyNewBornActivity)
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager( typeof( ShortDemographicsView ) );
            this.grpPatientName = new System.Windows.Forms.GroupBox();
            this.btnEditAKA = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAKA = new System.Windows.Forms.Label();
            this.lblStaticAKA = new System.Windows.Forms.Label();
            this.suffixView = new SuffixView(); 
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMI = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticLastName = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.mtbAdmitDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.mtbAdmitTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.lblRace = new System.Windows.Forms.Label();
            this.lblRace2 = new System.Windows.Forms.Label();
            this.lblEthnicity = new System.Windows.Forms.Label();
            this.lblEthnicity2 = new System.Windows.Forms.Label();

            this.mtbDateOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.lblPatientAge = new System.Windows.Forms.Label();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.grpPatientStay = new System.Windows.Forms.GroupBox();
            this.cmbAppointment = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAppointment = new System.Windows.Forms.Label();
            this.patientMailingAddrView = new PatientAccess.UI.AddressViews.AddressView();
        
            this.cmbMaritalStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            
            this.genderView = new GenderView();
            this.birthGenderView = new GenderView();
            this.lblPatientGender = new System.Windows.Forms.Label();
            this.lblBirthGender = new System.Windows.Forms.Label(); 

            this.driversLicenseView = new PatientAccess.UI.CommonControls.DriversLicenseView();
            this.passportView = new PatientAccess.UI.CommonControls.PassportView();
            this.shortEmergencyContactView = new PatientAccess.UI.ShortRegistration.ContactViews.ShortEmergencyContactView();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblSpecify = new System.Windows.Forms.Label();
            this.cmbLanguage = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.mtbOtherLanguage = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.raceView = new RaceView();
            this.race2View = new RaceView();
            this.ethnicityView = new EthnicityView();
            this.ethnicity2View = new EthnicityView();
            this.btnEditRace = new LoggingButton();
            this.lblAdditionalRace = new Label();

            this.grpPatientName.SuspendLayout();
            this.grpPatientStay.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientName
            // 
            this.grpPatientName.Controls.Add( this.btnEditAKA );
            this.grpPatientName.Controls.Add( this.lblAKA );
            this.grpPatientName.Controls.Add( this.lblStaticAKA );
            this.grpPatientName.Controls.Add(this.suffixView); 
            this.grpPatientName.Controls.Add( this.mtbMiddleInitial );
            this.grpPatientName.Controls.Add( this.lblStaticMI );
            this.grpPatientName.Controls.Add( this.mtbFirstName );
            this.grpPatientName.Controls.Add( this.lblFirstName );
            this.grpPatientName.Controls.Add( this.mtbLastName );
            this.grpPatientName.Controls.Add( this.lblStaticLastName );
            this.grpPatientName.Location = new System.Drawing.Point( 8, 8 );
            this.grpPatientName.Name = "grpPatientName";
            this.grpPatientName.Size = new System.Drawing.Size( 666, 87 );
            this.grpPatientName.TabIndex = 0;
            this.grpPatientName.TabStop = false;
            this.grpPatientName.Text = "Patient name";
            // 
            // btnEditAKA
            // 
            this.btnEditAKA.Location = new System.Drawing.Point( 462, 56 );
            this.btnEditAKA.Message = null;
            this.btnEditAKA.Name = "btnEditAKA";
            this.btnEditAKA.Size = new System.Drawing.Size( 90, 23 );
            this.btnEditAKA.TabIndex = 5;
            this.btnEditAKA.Text = "Manage A&KA...";
            this.btnEditAKA.Click += new System.EventHandler( this.btnEditAKA_Click );
            // 
            // lblAKA
            // 
            this.lblAKA.Location = new System.Drawing.Point( 38, 57 );
            this.lblAKA.Name = "lblAKA";
            this.lblAKA.Size = new System.Drawing.Size( 410, 23 );
            this.lblAKA.TabIndex = 0;
            this.lblAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticAKA
            // 
            this.lblStaticAKA.Location = new System.Drawing.Point( 9, 57 );
            this.lblStaticAKA.Name = "lblStaticAKA";
            this.lblStaticAKA.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticAKA.TabIndex = 0;
            this.lblStaticAKA.Text = "AKA:";
            this.lblStaticAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // suffixView
            // 
            this.suffixView.Location = new System.Drawing.Point(566, 19);
            this.suffixView.Name = "suffixView";
            this.suffixView.Size = new System.Drawing.Size(97, 27);
            this.suffixView.TabIndex = 4;
            this.suffixView.Visible = true;
            
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.Location = new System.Drawing.Point( 533, 21 );
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size( 18, 20 );
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMiddleInitial_Validating );
            // 
            // lblStaticMI
            // 
            this.lblStaticMI.Location = new System.Drawing.Point( 510, 24 );
            this.lblStaticMI.Name = "lblStaticMI";
            this.lblStaticMI.Size = new System.Drawing.Size( 21, 23 );
            this.lblStaticMI.TabIndex = 0;
            this.lblStaticMI.Text = "MI:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point( 335, 21 );
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size( 162, 20 );
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFirstName_Validating );
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 304, 24 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 30, 23 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point( 38, 21 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 255, 20 );
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // lblStaticLastName
            // 
            this.lblStaticLastName.Location = new System.Drawing.Point( 9, 24 );
            this.lblStaticLastName.Name = "lblStaticLastName";
            this.lblStaticLastName.Size = new System.Drawing.Size( 29, 23 );
            this.lblStaticLastName.TabIndex = 0;
            this.lblStaticLastName.Text = "Last:";
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.Location = new System.Drawing.Point( 9, 24 );
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size( 63, 23 );
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Admit date:";
            // 
            // mtbAdmitDate
            // 
            this.mtbAdmitDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitDate.KeyPressExpression = "^\\d*$";
            this.mtbAdmitDate.Location = new System.Drawing.Point( 80, 21 );
            this.mtbAdmitDate.Mask = "  /  /";
            this.mtbAdmitDate.MaxLength = 10;
            this.mtbAdmitDate.Name = "mtbAdmitDate";
            this.mtbAdmitDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbAdmitDate.TabIndex = 0;
            this.mtbAdmitDate.ValidationExpression = resources.GetString( "mtbAdmitDate.ValidationExpression" );
            
            this.mtbAdmitDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitDate_Validating );
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point( 150, 21 );
            this.dateTimePicker.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 175, 24 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 40, 23 );
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time:";
            // 
            // mtbAdmitTime
            // 
            this.mtbAdmitTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitTime.KeyPressExpression = "^\\d*$";
            this.mtbAdmitTime.Location = new System.Drawing.Point( 209, 21 );
            this.mtbAdmitTime.Mask = "  :";
            this.mtbAdmitTime.MaxLength = 5;
            this.mtbAdmitTime.Name = "mtbAdmitTime";
            this.mtbAdmitTime.Size = new System.Drawing.Size( 48, 20 );
            this.mtbAdmitTime.TabIndex = 1;
            this.mtbAdmitTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbAdmitTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitTime_Validating );
           
            // 
            // lblDOB
            // 
            this.lblDOB.Location = new System.Drawing.Point( 8, 131 );
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size( 60, 23 );
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Text = "DOB:";
            // 
            // lblMaritalStatus
            // 
            this.lblMaritalStatus.Location = new System.Drawing.Point( 8, 160 );
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size( 75, 23 );
            this.lblMaritalStatus.TabIndex = 0;
            this.lblMaritalStatus.Text = "Marital status:";
            // 
            // lblRace
            // 
            this.lblRace.Location = new System.Drawing.Point( 8, 190 );
            this.lblRace.Name = "lblRace";
            this.lblRace.Size = new System.Drawing.Size( 60, 23 );
            this.lblRace.TabIndex = 0;
            this.lblRace.Text = "Race:";
            // 
            // lblRace2
            // 
            this.lblRace2.Location = new System.Drawing.Point(202, 190);
            this.lblRace2.Name = "lblRace2";
            this.lblRace2.Size = new System.Drawing.Size(40, 23);
            this.lblRace2.TabIndex = 0;
            this.lblRace2.Text = "Race 2:";
            // 
            // lblEthnicity
            // 
            this.lblEthnicity.Location = new System.Drawing.Point( 8, 220 );
            this.lblEthnicity.Name = "lblEthnicity";
            this.lblEthnicity.Size = new System.Drawing.Size( 60, 23 );
            this.lblEthnicity.TabIndex = 0;
            this.lblEthnicity.Text = "Ethnicity:";
            // 
            // mtbDateOfBirth
            // 
            this.mtbDateOfBirth.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDateOfBirth.KeyPressExpression = "^\\d*$";
            this.mtbDateOfBirth.Location = new System.Drawing.Point( 83, 128 );
            this.mtbDateOfBirth.Mask = "  /  /";
            this.mtbDateOfBirth.MaxLength = 10;
            this.mtbDateOfBirth.Name = "mtbDateOfBirth";
            this.mtbDateOfBirth.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDateOfBirth.TabIndex = 10;
            this.mtbDateOfBirth.ValidationExpression = resources.GetString( "mtbDateOfBirth.ValidationExpression" );
            this.mtbDateOfBirth.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDateOfBirth_Validating );
            // 
            // lblStaticAge
            // 
            this.lblStaticAge.Location = new System.Drawing.Point( 166, 131 );
            this.lblStaticAge.Name = "lblStaticAge";
            this.lblStaticAge.Size = new System.Drawing.Size( 40, 23 );
            this.lblStaticAge.TabIndex = 0;
            this.lblStaticAge.Text = "Age:";
            // 
            // lblPatientAge
            // 
            this.lblPatientAge.Location = new System.Drawing.Point( 194, 131 );
            this.lblPatientAge.Name = "lblPatientAge";
            this.lblPatientAge.Size = new System.Drawing.Size( 95, 23 );
            this.lblPatientAge.TabIndex = 0;
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point( 8, 241 );
            this.ssnView.Model = null;
            this.ssnView.ModelAccount = ( ( PatientAccess.Domain.Account )( resources.GetObject( "ssnView.ModelAccount" ) ) );
            this.ssnView.Name = "ssnView";
            this.ssnView.Size = new System.Drawing.Size( 198, 72 );
            this.ssnView.SsnContext = SsnViewContext.ShortDemographicsView;
            this.ssnView.TabIndex = 16;
            this.ssnView.ssnNumberChanged += new System.EventHandler( this.ssnView_ssnNumberChanged );
            // 
            // grpPatientStay
            // 
            this.grpPatientStay.Controls.Add( this.cmbAppointment );
            this.grpPatientStay.Controls.Add( this.lblAppointment );
            this.grpPatientStay.Controls.Add( this.mtbAdmitDate );
            this.grpPatientStay.Controls.Add( this.mtbAdmitTime );
            this.grpPatientStay.Controls.Add( this.lblTime );
            this.grpPatientStay.Controls.Add( this.lblStaticAdmitDate );
            this.grpPatientStay.Controls.Add( this.dateTimePicker );
            this.grpPatientStay.Location = new System.Drawing.Point( 688, 8 );
            this.grpPatientStay.Name = "grpPatientStay";
            this.grpPatientStay.Size = new System.Drawing.Size( 300, 80 );
            this.grpPatientStay.TabIndex = 19;
            this.grpPatientStay.TabStop = false;
            this.grpPatientStay.Text = "Patient stay information";
            // 
            // cmbAppointment
            // 
            this.cmbAppointment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppointment.Location = new System.Drawing.Point( 80, 49 );
            this.cmbAppointment.Name = "cmbAppointment";
            this.cmbAppointment.Size = new System.Drawing.Size( 204, 21 );
            this.cmbAppointment.TabIndex = 4;
            this.cmbAppointment.SelectedIndexChanged += new System.EventHandler( this.cmbAppointment_SelectedIndexChanged );
            this.cmbAppointment.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAppointment_Validating );
            // 
            // lblAppointment
            // 
            this.lblAppointment.Location = new System.Drawing.Point( 9, 50 );
            this.lblAppointment.Name = "lblAppointment";
            this.lblAppointment.Size = new System.Drawing.Size( 68, 23 );
            this.lblAppointment.TabIndex = 0;
            this.lblAppointment.Text = "Appointment:";
            // 
            // patientMailingAddrView
            // 
            this.patientMailingAddrView.Context = null;
            this.patientMailingAddrView.EditAddressButtonText = "Edit Address...";
            this.patientMailingAddrView.IsAddressWithCounty = false;
            this.patientMailingAddrView.IsAddressWithStreet2 = true;
            this.patientMailingAddrView.KindOfTargetParty = null;
            this.patientMailingAddrView.Location = new System.Drawing.Point( 417, 95 );
            this.patientMailingAddrView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONECELL;
            this.patientMailingAddrView.Model = null;
            this.patientMailingAddrView.Model_ContactPoint = null;
            this.patientMailingAddrView.Name = "patientMailingAddrView";
            this.patientMailingAddrView.PatientAccount = null;
            this.patientMailingAddrView.ShowStatus = false;
            this.patientMailingAddrView.Size = new System.Drawing.Size( 265, 215 );
            this.patientMailingAddrView.TabIndex = 18;
            this.patientMailingAddrView.AddressChanged += new System.EventHandler( this.PatientMailingAddressView_AddressChangedEventHandler );
            this.patientMailingAddrView.AreaCodeChanged += new System.EventHandler( this.patientMailingAddrView_AreaCodeChanged );
            this.patientMailingAddrView.PhoneNumberChanged += new System.EventHandler( this.patientMailingAddrView_PhoneNumberChanged );
            this.patientMailingAddrView.CellPhoneNumberChanged += new System.EventHandler( this.patientMailingAddrView_CellPhoneNumberChanged );
            //
            // raceView
            //
            this.raceView.Location = new System.Drawing.Point(83, 187);
            this.raceView.Name = "raceView";
            this.raceView.TabIndex = 12;
            this.raceView.Size = new System.Drawing.Size(135, 25);
            this.raceView.Leave += new EventHandler(raceView_Leave);
            //
            // race2View
            //
            this.race2View.Location = new System.Drawing.Point(245, 187);
            this.race2View.Name = "race2View";
            this.race2View.TabIndex = 13;
            this.race2View.Size = new System.Drawing.Size(127, 25);
            this.race2View.Leave += new EventHandler(race2View_Leave);
            //
            //lblAdditionalRace
            //
            this.lblAdditionalRace.Location = new System.Drawing.Point(370, 160);
            this.lblAdditionalRace.Name = "lblAdditionalRace";
            this.lblAdditionalRace.Text = "Additional\nRace";
            this.lblAdditionalRace.TabIndex = 14;
            this.lblAdditionalRace.Size = new System.Drawing.Size(100, 100);
            //
            // btnEditRace
            //
            this.btnEditRace.Location = new System.Drawing.Point(372, 187);
            this.btnEditRace.Name = "btnEditRace";
            this.btnEditRace.TabIndex = 13;
            this.btnEditRace.Text = "Edit";
            this.btnEditRace.Size = new System.Drawing.Size(36, 23);
            this.btnEditRace.Click += new System.EventHandler(btnEditRace_Click);
            //
            // ethnicityView
            //
            this.ethnicityView.Location = new System.Drawing.Point(83, 217);
            this.ethnicityView.Name = "ethnicityView";
            this.ethnicityView.TabIndex = 14;
            this.ethnicityView.Size = new System.Drawing.Size(135, 25);
            //
            // ethnicity2View
            //
            this.ethnicity2View.Location = new System.Drawing.Point(258, 217);
            this.ethnicity2View.Name = "ethnicity2View";
            this.ethnicity2View.TabIndex = 15;
            this.ethnicity2View.Size = new System.Drawing.Size(135, 25);
            // 
            // lblEthnicity
            // 
            this.lblEthnicity.Location = new System.Drawing.Point(8, 220);
            this.lblEthnicity.Name = "lblEthnicity";
            this.lblEthnicity.Size = new System.Drawing.Size(60, 23);
            this.lblEthnicity.TabIndex = 0;
            this.lblEthnicity.Text = "Ethnicity:";
            // 
            // lblEthnicity2
            // 
            this.lblEthnicity2.Location = new System.Drawing.Point(203, 220);
            this.lblEthnicity2.Name = "lblEthnicity2";
            this.lblEthnicity2.Size = new System.Drawing.Size(52, 23);
            this.lblEthnicity2.TabIndex = 0;
            this.lblEthnicity2.Text = "Ethnicity 2:";
            // 
            // cmbMaritalStatus
            // 
            this.cmbMaritalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaritalStatus.Location = new System.Drawing.Point( 83, 157 );
            this.cmbMaritalStatus.Name = "cmbMaritalStatus";
            this.cmbMaritalStatus.Size = new System.Drawing.Size( 100, 21 );
            this.cmbMaritalStatus.TabIndex = 11;
            this.cmbMaritalStatus.SelectedIndexChanged += new System.EventHandler( this.cmbMaritalStatus_SelectedIndexChanged );
            this.cmbMaritalStatus.Validating += new System.ComponentModel.CancelEventHandler( this.cmbMaritalStatus_Validating );

            //
            // lblPatientGender
            // 
            this.lblPatientGender.Location = new System.Drawing.Point(8, 108);
            this.lblPatientGender.Name = "lblPatientGender";
            this.lblPatientGender.Size = new System.Drawing.Size(60, 23);
            this.lblPatientGender.TabIndex = 0;
            this.lblPatientGender.Text = "Gender:";
            // 
            // Gender View
            // 
            this.genderView.Location = new System.Drawing.Point(80, 102);
            this.genderView.Model = null;
            this.genderView.Name = "genderView";
            this.genderView.Size = new System.Drawing.Size(100, 30);
            this.genderView.TabIndex = 9;
            this.genderView.GenderViewPresenter = null;
            //
            // lblBirthGender
            // 
            this.lblBirthGender.Location = new System.Drawing.Point(192, 108);
            this.lblBirthGender.Name = "lblBirthGender";
            this.lblBirthGender.Size = new System.Drawing.Size(60, 23);
            this.lblBirthGender.TabIndex = 0;
            this.lblBirthGender.Text = "Birth Sex:";

            // 
            // Birth Gender View
            // 
            this.birthGenderView.Location = new System.Drawing.Point(260, 102);
            this.birthGenderView.Model = null;
            this.birthGenderView.Name = "birthGenderView";
            this.birthGenderView.Size = new System.Drawing.Size(100, 30);
            this.birthGenderView.TabIndex = 9;
            this.birthGenderView.GenderViewPresenter = null;
            // 
            // driversLicenseView
            // 
            this.driversLicenseView.Location = new System.Drawing.Point( 688, 90 );
            this.driversLicenseView.Model = null;
            this.driversLicenseView.Model_Account = null;
            this.driversLicenseView.Name = "driversLicenseView";
            this.driversLicenseView.Size = new System.Drawing.Size( 300, 84 );
            this.driversLicenseView.TabIndex = 20;
            this.driversLicenseView.DriversLicenseNumberChanged += new System.EventHandler( this.driversLicenseView_DriversLicenseNumberChanged );
            // 
            // passportView
            // 
            this.passportView.Location = new System.Drawing.Point( 688, 177 );
            this.passportView.Model = null;
            this.passportView.Model_Account = null;
            this.passportView.Name = "passportView";
            this.passportView.Size = new System.Drawing.Size( 300, 84 );
            this.passportView.TabIndex = 21;
            this.passportView.PassportNumberChanged += new System.EventHandler( this.passportView_PassportNumberChanged );
            // 
            // shortEmergencyContactView
            // 
            this.shortEmergencyContactView.Location = new System.Drawing.Point( 688, 263);
            this.shortEmergencyContactView.Model = null;
            this.shortEmergencyContactView.Model_EmergencyContact = null;
            this.shortEmergencyContactView.Name = "shortEmergencyContactView";
            this.shortEmergencyContactView.Size = new System.Drawing.Size( 300, 105 );
            this.shortEmergencyContactView.TabIndex = 22;
            this.shortEmergencyContactView.EmergencyRelationshipValidating += new System.ComponentModel.CancelEventHandler( this.emergencyContactView1_EmergencyRelationshipValidating );
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point( 8, 323 );
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size( 58, 13 );
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Language:";
            // 
            // lblSpecify
            // 
            this.lblSpecify.AutoSize = true;
            this.lblSpecify.Location = new System.Drawing.Point( 8, 350 );
            this.lblSpecify.Name = "lblSpecify";
            this.lblSpecify.Size = new System.Drawing.Size( 45, 13 );
            this.lblSpecify.TabIndex = 0;
            this.lblSpecify.Text = "Specify:";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.Location = new System.Drawing.Point( 83, 323 );
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size( 170, 21 );
            this.cmbLanguage.TabIndex = 17;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler( this.cmbLanguage_SelectedIndexChanged );
            this.cmbLanguage.Validating += new System.ComponentModel.CancelEventHandler( this.cmbLanguage_Validating );
            // 
            // mtbOtherLanguage
            // 
            this.mtbOtherLanguage.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbOtherLanguage.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOtherLanguage.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbOtherLanguage.Location = new System.Drawing.Point( 83, 350 );
            this.mtbOtherLanguage.Mask = "";
            this.mtbOtherLanguage.MaxLength = 20;
            this.mtbOtherLanguage.Name = "mtbOtherLanguage";
            this.mtbOtherLanguage.Size = new System.Drawing.Size( 121, 20 );
            this.mtbOtherLanguage.TabIndex = 17;
            this.mtbOtherLanguage.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbOtherLanguage.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpecify_Validating );
            // 
            // ShortDemographicsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.mtbOtherLanguage );
            this.Controls.Add( this.cmbLanguage );
            this.Controls.Add( this.lblSpecify );
            this.Controls.Add( this.lblLanguage );
            this.Controls.Add( this.shortEmergencyContactView );
            this.Controls.Add( this.passportView );
            this.Controls.Add( this.driversLicenseView );
            this.Controls.Add( this.grpPatientStay );
            this.Controls.Add( this.patientMailingAddrView );
            this.Controls.Add( this.ssnView );
            this.Controls.Add( this.lblPatientAge );
            this.Controls.Add( this.lblStaticAge );
            
            this.Controls.Add( this.cmbMaritalStatus );
            this.Controls.Add( this.mtbDateOfBirth );

            this.Controls.Add(this.genderView);
            this.Controls.Add(this.birthGenderView);
            this.Controls.Add(this.lblPatientGender);
            this.Controls.Add(this.lblBirthGender); 

            this.Controls.Add( this.lblEthnicity );
            this.Controls.Add(this.lblEthnicity2);
            this.Controls.Add(this.ethnicityView);
            this.Controls.Add(this.ethnicity2View);

            this.Controls.Add( this.lblRace );
            this.Controls.Add( this.lblRace2 );
            this.Controls.Add( this.raceView );
            this.Controls.Add( this.race2View );
            this.Controls.Add( this.btnEditRace );
            this.Controls.Add(this.lblAdditionalRace);
            this.Controls.Add( this.lblMaritalStatus );
            this.Controls.Add( this.lblDOB );
             
            this.Controls.Add( this.grpPatientName );
            this.Name = "ShortDemographicsView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Enter += new System.EventHandler( this.DemographicsView_Enter );
            this.Leave += new System.EventHandler( this.DemographicsView_Leave );
            this.Disposed += new System.EventHandler( this.DemographicsView_Disposed );
            this.grpPatientName.ResumeLayout( false );
            this.grpPatientName.PerformLayout();
            this.grpPatientStay.ResumeLayout( false );
            this.grpPatientStay.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public ShortDemographicsView()
        {
            InitializeComponent();
            ConfigureControls();

            patientMailingAddrView.SetGroupBoxText( "Patient mailing address and contact" );
            patientMailingAddrView.EditAddressButtonText = "Edit &Address...";

            btnEditAKA.Message = "Click begin manage AKA";

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;

        private LoggingButton btnEditAKA;
         
        private PatientAccessComboBox cmbMaritalStatus;
        private PatientAccessComboBox cmbLanguage;
        private PatientAccessComboBox cmbAppointment;

        private GroupBox grpPatientStay;
        private GroupBox grpPatientName;
       
        private DateTimePicker dateTimePicker;

        private GenderView genderView;
        private GenderView birthGenderView;
        private Label lblPatientGender;
        private Label lblBirthGender; 

        private Label lblAKA;
        private Label lblDOB;
        private Label lblEthnicity;
        private Label lblEthnicity2;

        private Label lblFirstName;
        private Label lblMaritalStatus;
        private Label lblPatientAge;
        private Label lblRace;
        private Label lblRace2;

        private Label lblStaticAge;
        private Label lblStaticAKA;
        private Label lblStaticLastName; 
        private Label lblStaticMI;
        private Label lblStaticAdmitDate;
        private Label lblTime;
        private Label lblAppointment;
        private Label lblLanguage;
        private Label lblSpecify;

        private MaskedEditTextBox mtbAdmitDate;
        private MaskedEditTextBox mtbAdmitTime;
        private MaskedEditTextBox mtbDateOfBirth;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbMiddleInitial;
        private SuffixView suffixView;
        private MaskedEditTextBox mtbOtherLanguage;

        private AddressView patientMailingAddrView;
        private SSNControl ssnView;

        private DriversLicenseView driversLicenseView;
        private PassportView passportView;

        private ShortEmergencyContactView shortEmergencyContactView;

        private DateTime dateOfBirth;
        private bool loadingModelData = true;

        private bool i_Registered;
        private int month;
        private int day;
        private int year;
        private int hour;
        private int minute;
        private RuleEngine i_RuleEngine;
        private bool newBornAdmit;
        private bool blnLeaveRun;
        private bool admitDateWarning = true;
        private bool effectiveGreaterThanAdmitDateWarning = true;
        private bool expirationLesserThanAdmitDateWarning = true;
        private bool leavingView;
        private SuffixPresenter suffixPresenter;
        private SequesteredPatientPresenter sequesteredPatientPresenter;
        private GenderViewPresenter PatientGenderViewPresenter { get; set; }
        private GenderViewPresenter BirthGenderViewPresenter { get; set; }
		private RaceView raceView;
        private RaceView race2View;
        private EthnicityView ethnicityView;
        private EthnicityView ethnicity2View;
        private LoggingButton btnEditRace;
        private Label lblAdditionalRace;

        #endregion

        #region Constants
        private const string 
            SCHEDULE_CODE_WALKIN = "W",
            TIMESTAMP = "Timestamp",
            FIRSTNAME = "FirstName",
            LASTNAME = "LastName";
        private const int SIXTY_FIVE = 65;

        #endregion
    }
}
