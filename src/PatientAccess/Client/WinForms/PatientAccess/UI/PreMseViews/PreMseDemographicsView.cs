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
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.DemographicsViews;
using PatientAccess.UI.DemographicsViews.Presenters;
using PatientAccess.UI.DemographicsViews.ViewImpl;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using Peradigm.Framework.Domain.Collections;
using SortOrder = Peradigm.Framework.Domain.Collections.SortOrder;

namespace PatientAccess.UI.PreMSEViews
{
    /// <summary>
    /// Summary description for PreMseRegStep1View.
    /// </summary>
    public class PreMseDemographicsView : ControlView, IPreMseDemographicsView, IBirthTimeView
    {
        #region Events
        public event EventHandler RefreshTopPanel;
        #endregion

        #region Event Handlers

        private void mtbAdmitTime_KeyDown( object sender, KeyEventArgs e )
        {
            DateValidator.AuditTimeEntry( mtbAdmitTime );
        }

        private void mtbAdmitTime_KeyPress( object sender, KeyPressEventArgs e )
        {
            DateValidator.AuditTimeEntry( mtbAdmitTime );
        }

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void PreMseDemographicsView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void PreMseDemographicsView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            sequesteredPatientPresenter.IsPatientSequestered();
            // SR 604 - If the user clicked on 'Next' or 'Finish' or another tab after 
            // modifying the Admit date, update Social Security Number for the new criteria.
            if ( isAdmitDateChange )
            {
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( OnPreMSEDemographicsForm ), ModelAccount );
            blnLeaveRun = false;
        }

        private void UpdateAkaName( object sender, EventArgs e )
        {
            var manageAkaDialog = (ManageAKADialog)sender;
            ModelAccount.Patient = manageAkaDialog.Model_Patient;
            UpdateAkaName();
        }

        private void patientMailingAddrView_AddressChangedEventHandler( object sender, EventArgs e )
        {
            // model has been updated in addressview
            RunRules();
            patientPhysicalAddrView.SetCopyFromValues();
        }
        private void patientPhysicalAddrView_AddressChangedEventHandler(object sender, EventArgs e)
        {
            patientMailingAddrView.SetCopyFromValues();
        }
        private void PatientMailingAddressView_PhoneNumberChangedEventHandler( object sender, EventArgs e )
        {
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), Model );
        }
        private void PatientMailingAddressView__CellPhoneNumberChanged(object sender, EventArgs e)
        {
            ContactPoint mobileContactPoint = ModelAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            mobileContactPoint.PhoneNumber = patientMailingAddrView.Model_ContactPoint.CellPhoneNumber;
        }
       
        private void mtbLastName_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbLastName );
            var lastName = (MaskedEditTextBox)sender;
            ModelAccount.Patient.LastName = lastName.Text.Trim();

            Activity currentActivity = ModelAccount.Activity;
            if ( ( currentActivity is PreMSERegisterActivity ) ||
                 ( currentActivity is EditPreMseActivity )
               )
            {
                ModelAccount.Patient.AddPreviousNameToAKA();
            }
            else
            {
                if ((currentActivity is UCCPreMSERegistrationActivity) ||
               (currentActivity is EditUCCPreMSEActivity)
             )
                {
                    ModelAccount.Patient.AddPreviousNameToAKA();
                }

            }
            RuleEngine.EvaluateRule( typeof( LastNameRequired ), Model );
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void mtbFirstName_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbFirstName );
            var firstName = (MaskedEditTextBox)sender;
            ModelAccount.Patient.FirstName = firstName.Text.Trim();

            Activity currentActivity = ModelAccount.Activity;
            if ( ( currentActivity is PreMSERegisterActivity ) ||
                 ( currentActivity is EditPreMseActivity )
               )
            {
                ModelAccount.Patient.AddPreviousNameToAKA();
            }
            else if ((currentActivity is UCCPreMSERegistrationActivity) ||
                     (currentActivity is EditUCCPreMSEActivity)
                )
            {
                ModelAccount.Patient.AddPreviousNameToAKA();
            }
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), Model );
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void mtbMiddleInitial_Validating( object sender, CancelEventArgs e )
        {
            var middleInitial = (MaskedEditTextBox)sender;
            ModelAccount.Patient.MiddleInitial = middleInitial.Text.Trim();
        }

        private void mtbNameSuffix_Validating( object sender, CancelEventArgs e )
        {
            var nameSuffix = (MaskedEditTextBox)sender;
            ModelAccount.Patient.Suffix = nameSuffix.Text.Trim();
        }

        private void btnEditAKA_Click( object sender, EventArgs e )
        {
            // Dialog to be developed in another iteration
            var manageAkaDialog = new ManageAKADialog { Model = ModelAccount.Patient };
            manageAkaDialog.UpdateModel();
            manageAkaDialog.UpdateView();
            manageAkaDialog.UpdateAKAName += UpdateAkaName;

            try
            {
                manageAkaDialog.ShowDialog( this );
            }
            finally
            {
                manageAkaDialog.Dispose();
            }
        }

        private void mtbAdmitDate_Validating( object sender, CancelEventArgs e )
        {
            var admitDateTextBox = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor( admitDateTextBox );

            if ( dateTimePicker.Focused )
            {
                return;
            }
            if ( dateTimePicker.Focused )
            {
                Refresh();
                return;
            }

            if ( mtbAdmitDate.UnMaskedText == String.Empty )
            {
                ModelAccount.AdmitDate = DateTime.MinValue;
                RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );
                return;
            }

            if ( mtbAdmitDate.UnMaskedText.Length != 8 && mtbAdmitDate.UnMaskedText.Length != 0 )
            {
                admitDateTextBox.Focus();
                UIColors.SetErrorBgColor( admitDateTextBox );
                MessageBox.Show( UIErrorMessages.ADMIT_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                return;
            }

            admitMonth = Convert.ToInt32( admitDateTextBox.Text.Substring( 0, 2 ) );
            admitDay = Convert.ToInt32( admitDateTextBox.Text.Substring( 3, 2 ) );
            admitYear = Convert.ToInt32( admitDateTextBox.Text.Substring( 6, 4 ) );
            // if the user hits cancel before fixing the date when they bring up an account with an
            // invalid date the time values will not be set. Check them here and if no data is present
            // default them to 0.
            if (mtbAdmitTime.UnMaskedText.Length == 4)
            {
                string admitHourString = mtbAdmitTime.Text.Substring(0, 2);
                string admitMinuteString = mtbAdmitTime.Text.Substring(3, 2);
                admitHour = admitHourString.Trim() != string.Empty ? Convert.ToInt32(mtbAdmitTime.Text.Substring(0, 2)) : 0;
                admitMinute = admitMinuteString.Trim() != string.Empty ? Convert.ToInt32(mtbAdmitTime.Text.Substring(3, 2)) : 0;
            }
            try
            {
                admitDate = new DateTime( admitYear, admitMonth, admitDay, admitHour, admitMinute, admitSecond );

                if ( DateValidator.IsValidDate( admitDate ) == false )
                {
                    admitDateTextBox.Focus();
                    UIColors.SetErrorBgColor( admitDateTextBox );
                    MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    ModelAccount.AdmitDate = admitDate;

                    RuleEngine.EvaluateRule( typeof( AdmitDateFiveDaysPast ), Model );
                    RuleEngine.EvaluateRule( typeof( AdmitDateFutureDate ), Model );
                    RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );

                    if ( !loadingModelData )
                    {
                        ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
                    }
                }
            }
            catch ( ArgumentException )
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                admitDateTextBox.Focus();
                UIColors.SetErrorBgColor( admitDateTextBox );
                MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
            }

            isAdmitDateChange = false;
        }

        private void mtbAdmitTime_Validating( object sender, CancelEventArgs e )
        {
            var admitTime = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor( admitTime );

            if ( admitTime.UnMaskedText.Length > 0 && admitTime.UnMaskedText.Length < 4 )
            {
                admitTime.Focus();
                UIColors.SetErrorBgColor( admitTime );
                MessageBox.Show( UIErrorMessages.TIME_NOT_VALID_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return;
            }

            if ( admitTime.UnMaskedText.Length == 4 )
            {
                admitHour = Convert.ToInt32( admitTime.Text.Substring( 0, 2 ) );
                admitMinute = Convert.ToInt32( admitTime.Text.Substring( 3, 2 ) );

                if ( admitHour > 23 )
                {
                    admitTime.Focus();
                    UIColors.SetErrorBgColor( admitTime );
                    MessageBox.Show( UIErrorMessages.HOUR_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }

                if ( admitMinute > 59 )
                {
                    admitTime.Focus();
                    UIColors.SetErrorBgColor( admitTime );
                    MessageBox.Show( UIErrorMessages.MINUTE_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }
            }
            else
            {
                admitHour = 0;
                admitMinute = 0;
                admitSecond = 0;
            }

            // we can only set the time when we have a valid date

            if ( mtbAdmitDate.UnMaskedText.Length != 8 )
            {
                admitMonth = 1;
                admitDay = 1;
                admitYear = 1;
            }
            else
            {
                admitMonth = Convert.ToInt32( mtbAdmitDate.UnMaskedText.Substring( 0, 2 ) );
                admitDay = Convert.ToInt32( mtbAdmitDate.UnMaskedText.Substring( 2, 2 ) );
                admitYear = Convert.ToInt32( mtbAdmitDate.UnMaskedText.Substring( 4, 4 ) );
            }

            try
            {
                admitDate = new DateTime( admitYear, admitMonth, admitDay, admitHour, admitMinute, admitSecond );

                ModelAccount.AdmitDate = admitDate;
            }
            catch ( ArgumentException )
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                admitTime.Focus();
                UIColors.SetErrorBgColor( admitTime );
                MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
            }
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Model );
        }

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbAdmitDate );

            DateTime dt = dateTimePicker.Value;
            mtbAdmitDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year ); 
            admitSecond = dt.Second;
            mtbAdmitDate.Refresh();
            mtbAdmitDate.Focus();

            ModelAccount.AdmitDate = dt;

            RuleEngine.EvaluateRule( typeof( AdmitDateFiveDaysPast ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitDateFutureDate ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );

        }
        private void mtbBirthTime_Validating( object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( mtbBirthTime );
            BirthTimePresenter.Validate( mtbBirthTime );
        }
        
        private void mtbDateOfBirth_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbDateOfBirth );
            var dateOfBirthTextBox = (MaskedEditTextBox)sender;

            if ( dateOfBirthTextBox.UnMaskedText.Trim() == string.Empty
                || dateOfBirthTextBox.UnMaskedText.Trim() == "01010001" )
            {
                ModelAccount.Patient.DateOfBirth = DateTime.MinValue;
                lblPatientAge.Text = string.Empty;
                UIColors.SetRequiredBgColor( dateOfBirthTextBox );
                ssnView.ResetSSNControl();
                RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
                return;
            }

            if ( dateOfBirthTextBox.UnMaskedText.Length != 8 )
            {   // Prevent cursor from advancing to the next control
                lblPatientAge.Text = String.Empty;
                UIColors.SetErrorBgColor( dateOfBirthTextBox );
                MessageBox.Show( UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                dateOfBirthTextBox.Focus();
                return;
            }

            try
            {   // Check the date entered is not in the future
                dateOfBirth = new DateTime( Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 6, 4 ) ),
                    Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 0, 2 ) ),
                    Convert.ToInt32( dateOfBirthTextBox.Text.Substring( 3, 2 ) ) );

                if ( dateOfBirth > DateTime.Today )
                {
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDateOfBirth.Focus();
                }
                else if ( DateValidator.IsValidDate( dateOfBirth ) == false )
                {
                    lblPatientAge.Text = String.Empty;
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
                else if ( dateOfBirth < earliestDate )
                {
                    UIColors.SetErrorBgColor( dateOfBirthTextBox );
                    MessageBox.Show( UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDateOfBirth.Focus();
                }
                else
                {
                    UIColors.SetNormalBgColor( dateOfBirthTextBox );
                    Refresh();
                    if(mtbBirthTime.UnMaskedText.Trim()=="")
                        ModelAccount.Patient.DateOfBirth = dateOfBirth;
                    else
                    {
                        var hours = Convert.ToInt32( mtbBirthTime.Text.Substring( 0, 2 ) );
                        var minutes = Convert.ToInt32( mtbBirthTime.Text.Substring( 3, 2 ) );
                        ModelAccount.Patient.DateOfBirth = dateOfBirth.AddHours( hours ).AddMinutes(minutes);
                    }
                    
                    
                    lblPatientAge.Text = ModelAccount.Patient.Age();
                }
            }
            catch ( ArgumentException )
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                lblPatientAge.Text = String.Empty;
                UIColors.SetErrorBgColor( dateOfBirthTextBox );
                MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                dateOfBirthTextBox.Focus();
            }
            BirthTimePresenter.UpdateField();
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
            if (!loadingModelData)
            {
                ssnView.ResetSSNControl();
            }
        }

        private void cmbMaritalStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            var maritalStatusComboBox = (ComboBox)sender;
            if ( maritalStatusComboBox.SelectedIndex > -1 )
            {                   
                selectedMaritalStatus = maritalStatusComboBox.SelectedItem as MaritalStatus;
                if ( selectedMaritalStatus != null )
                {
                    ModelAccount.Patient.MaritalStatus = (MaritalStatus)selectedMaritalStatus.Clone();
                }
            }

            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
        private void cmbLanguage_SelectedIndexChanged( object sender, EventArgs e )
        {
            var languageComboBox = (ComboBox)sender;

            if ( languageComboBox != null && languageComboBox.SelectedIndex > -1 )
            {
                UpdateSelectedLanguage( languageComboBox.SelectedItem as Language );
            }
        }


        private void UpdateSelectedLanguage( Language language )
        {
            if ( language != null )
            {
                ModelAccount.Patient.Language = language;
                if ( !blnLeaveRun )
                {
                    UIColors.SetNormalBgColor( cmbLanguage );
                    Refresh();
                    RuleEngine.GetInstance().EvaluateRule( typeof( InvalidLanguageCode ), Model );
                    RuleEngine.GetInstance().EvaluateRule( typeof( InvalidLanguageCodeChange ), Model );
                    RuleEngine.GetInstance().EvaluateRule( typeof( LanguagePreferred ), Model );
                }
                Presenter.SelectedLanguageChanged( ModelAccount.Patient.Language );
            }
        }

        private void cmbBloodless_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cmbBloodless );
            ModelAccount.Bloodless = (YesNoFlag)cmbBloodless.SelectedItem;
            RuleEngine.EvaluateRule( typeof( BloodlessRequired ), Model );
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
     
        private void cmbMaritalStatus_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbMaritalStatus );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidMaritalStatusCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidMaritalStatusCodeChange ), Model );
            }

            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
        }
   
        private void cmbLanguage_Validating( object sender, CancelEventArgs e )
        {
            var languageComboBox = sender as ComboBox;

            if ( languageComboBox != null && languageComboBox.SelectedIndex > -1 )
            {
                UpdateSelectedLanguage( languageComboBox.SelectedItem as Language );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
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
         
        private void InvalidMaritalStatusCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbMaritalStatus );
        }
     
        private void InvalidLanguageCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbLanguage );
        }

        //----------------------------------------------------------------------
         
        private void InvalidMaritalStatusCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbMaritalStatus );
        }
      
        private void InvalidLanguageCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbLanguage );
        }

        private void ssnView_ssnNumberChanged(object sender, EventArgs e)
        {

            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        //----------------------------------------------------------------------

        #endregion

        #region Rule Event Handlers
        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void AdmitDateFutureDateEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( PreMSERegisterActivity ) ||
                ModelAccount.Activity.GetType() == typeof( EditPreMseActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_ED_PATIENT_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
            }
            else if (ModelAccount.Activity.GetType() == typeof(UCCPreMSERegistrationActivity) ||
                ModelAccount.Activity.GetType() == typeof(EditUCCPreMSEActivity))
            {
                UIColors.SetErrorBgColor(mtbAdmitDate);
                MessageBox.Show(UIErrorMessages.ADMIT_UC_PATIENT_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1);
                mtbAdmitDate.Focus();
            }
        }

        private void AdmitDateFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( PreMSERegisterActivity ) ||
                ModelAccount.Activity.GetType() == typeof( EditPreMseActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_ED_PATIENT_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
            }
            else if (ModelAccount.Activity.GetType() == typeof(UCCPreMSERegistrationActivity) ||
                ModelAccount.Activity.GetType() == typeof(EditUCCPreMSEActivity))
            {
                UIColors.SetErrorBgColor(mtbAdmitDate);
                MessageBox.Show(UIErrorMessages.ADMIT_UC_PATIENT_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                mtbAdmitDate.Focus();
            }
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

        private void AdmitTimeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitTime );
        }
        private void BloodlessRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbBloodless );
        }

        private void LanguagePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbLanguage );
        }
        public void MakeOtherLanguageRequired()
        {
            UIColors.SetRequiredBgColor( mtbSpecify );
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

        public void PopulateBirthTime(DateTime birthDate)
        {
            mtbBirthTime.UnMaskedText = string.Empty;

            if ( !( birthDate.Hour == 0 && birthDate.Minute == 0 ) )
            {
                mtbBirthTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", birthDate.Hour, birthDate.Minute );
            }

        }

        public void MakeBirthTimeRequired()
        {
            UIColors.SetRequiredBgColor( mtbBirthTime );
        }

        public void MakeBirthTimePreferred()
        {
            UIColors.SetPreferredBgColor( mtbBirthTime );
        }

        #endregion

        #region Methods

        /// <summary>
        /// runRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.
        /// </summary>
        private void RunRules()
        {
            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            
            UIColors.SetNormalBgColor( mtbDateOfBirth );
            UIColors.SetNormalBgColor( cmbMaritalStatus );
           
            UIColors.SetNormalBgColor( mtbAdmitDate );
            //            UIColors.SetNormalBgColor( mtbAdmitTime );
            UIColors.SetNormalBgColor( cmbBloodless );
            UIColors.SetNormalBgColor( cmbLanguage );
            Refresh();

            RuleEngine.EvaluateRule( typeof( MailingAddressPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MaritalStatusRequired ), Model );
             
            ssnView.RunRules();
            RuleEngine.EvaluateRule( typeof( AdmitDateFiveDaysPast ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitDateFutureDate ), Model );
            RuleEngine.EvaluateRule( typeof( RacePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( LanguagePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MailingAddressPhonePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( MailingAddressAreaCodePreferred ), Model );

            RuleEngine.EvaluateRule( typeof( LastNameRequired ), Model );
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), Model );
           
            RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), Model );
            RuleEngine.EvaluateRule( typeof( BloodlessRequired ), Model );

            RaceViewPresenter.RunInvalidCodeRules();
            RaceViewPresenter.RunRules();
            Race2ViewPresenter.RunInvalidCodeRules();
            RuleEngine.GetInstance().EvaluateRule( typeof( OnPreMSEDemographicsForm ), Model );
            PatientGenderViewPresenter.RunRules(); 
            EthnicityViewPresenter.RunInvalidCodeRules();
            EthnicityViewPresenter.RunRules();
            Ethnicity2ViewPresenter.RunInvalidCodeRules();

        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            sequesteredPatientPresenter = new SequesteredPatientPresenter(new SequesteredPatientFeatureManager(), ModelAccount);
            sequesteredPatientPresenter.IsPatientSequestered();

            Presenter = new PreMseDemographicsPresenter( this, ModelAccount, RuleEngine.GetInstance() );
            ssnView.SsnFactory = new SsnFactoryCreator(ModelAccount).GetSsnFactory();
            RegisterEvents();
            if ( loadingModelData )
            {
                // Initial entry to the form -- initialize controls and get the data from the model.
                lblPatientAge.Text = String.Empty;
                PopulateGenderAndBirthGender();
                PopulateMaritalStatusControl();
                PopulateBloodlessList();
                RaceViewPresenter = new RaceViewPresenter(raceView, ModelAccount, Race.RACENATIONALITY_CONTROL);
                RaceViewPresenter.UpdateView();
                Race2ViewPresenter = new RaceViewPresenter(race2View, ModelAccount, Race.RACENATIONALITY2_CONTROL);
                Race2ViewPresenter.UpdateView();
                EthnicityViewPresenter = new EthnicityViewPresenter(ethnicityView, ModelAccount, Ethnicity.ETHNICITY_PROPERTY);
                EthnicityViewPresenter.UpdateView();
                Ethnicity2ViewPresenter = new EthnicityViewPresenter(ethnicity2View, ModelAccount, Ethnicity.ETHNICITY2_PROPERTY);
                Ethnicity2ViewPresenter.UpdateView();
                patientMailingAddrView.CaptureMailingAddress = true;
                PopulatePatientMailingAddressView();
                PopulatePatientPhysicalAddressView();
                PopulateLanguageControl();
               
                suffixPresenter = new SuffixPresenter(suffixView, ModelAccount, "Patient");
                mtbLastName.Text = ModelAccount.Patient.LastName;
                mtbFirstName.Text = ModelAccount.Patient.FirstName;
                mtbMiddleInitial.Text = ModelAccount.Patient.MiddleInitial;
                suffixPresenter.UpdateView();

                if ( ModelAccount.Patient.HasAliases() )
                {   // Display alias name in AKA field
                    ArrayList arraylist = ModelAccount.Patient.Aliases;
                    var name = (Name)arraylist[0];
                    lblAKA.Text = name.AsFormattedName();
                }

                UpdateAkaName();

                if ( ModelAccount.AdmitDate != DateTime.MinValue )
                {
                    mtbAdmitDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                        ModelAccount.AdmitDate.Month,
                        ModelAccount.AdmitDate.Day,
                        ModelAccount.AdmitDate.Year );

                    if ( ModelAccount.AdmitDate.Hour != 0 ||
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
                else
                {
                    ITimeBroker broker = ProxyFactory.GetTimeBroker();
                    DateTime facilityDate = broker.TimeAt( ModelAccount.Facility.GMTOffset, ModelAccount.Facility.DSTOffset );

                    admitDate = facilityDate;
                    admitMonth = facilityDate.Month;
                    admitDay = facilityDate.Day;
                    admitYear = facilityDate.Year;
                    admitHour = facilityDate.Hour;
                    admitMinute = facilityDate.Minute;

                    if ( ModelAccount.Activity.GetType() == typeof( PreMSERegisterActivity ) ||
                         ModelAccount.Activity.GetType() == typeof(UCCPreMSERegistrationActivity))
                    {
                        mtbAdmitDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", admitMonth, admitDay, admitYear );
                        mtbAdmitTime.Text = String.Format( "{0:D2}{1:D2}", admitHour, admitMinute );

                        ModelAccount.AdmitDate = new DateTime( admitYear, admitMonth, admitDay, admitHour, admitMinute, 0 );
                    }
                }

                dateOfBirth = ModelAccount.Patient.DateOfBirth;
                mtbDateOfBirth.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                     dateOfBirth.Month,
                                                     dateOfBirth.Day,
                                                     dateOfBirth.Year );

                if ( mtbDateOfBirth.Text.Length != 10 )
                {
                    UIColors.SetErrorBgColor( mtbDateOfBirth );
                }

                lblPatientAge.Text = ModelAccount.Patient.Age();

                if ( !string.IsNullOrEmpty( ModelAccount.Patient.BloodlessPatient ) )
                {
                    cmbBloodless.SelectedIndex = cmbBloodless.FindString( ModelAccount.Patient.BloodlessPatient.ToUpper() );
                }

                if ( ModelAccount.Patient.MaritalStatus != null )
                    cmbMaritalStatus.SelectedItem = ModelAccount.Patient.MaritalStatus;
               
                if ( ModelAccount.Patient.Language != null )
                    cmbLanguage.SelectedItem = ModelAccount.Patient.Language;

                ssnView.Model = ModelAccount.Patient;
                ssnView.ModelAccount = ModelAccount;
                ssnView.UpdateView();
                isAdmitDateChange = false;
                BirthTimePresenter = new BirthTimePresenter( this, new BirthTimeFeatureManager(),
                                                new MessageBoxAdapter(), RuleEngine.GetInstance(), ProxyFactory.GetTimeBroker(),User.GetCurrent());
                BirthTimePresenter.UpdateField();
            }

            if ( mtbAdmitDate.UnMaskedText == "01010001" )
            {
                mtbAdmitDate.Text = String.Empty;
            }

            if ( mtbDateOfBirth.UnMaskedText == "01010001" )
            {
                mtbDateOfBirth.Text = String.Empty;
            }

            RunRules();
            loadingModelData = false;
        }

        private void PopulateGenderAndBirthGender()
        {
            PatientGenderViewPresenter = new GenderViewPresenter(genderView, ModelAccount, Gender.PATIENT_GENDER);
            PatientGenderViewPresenter.RefreshTopPanel += new System.EventHandler(gendersView_RefreshTopPanel);
            PatientGenderViewPresenter.UpdateView();


            EnableBirthGender();
            BirthGenderViewPresenter =
                new GenderViewPresenter(birthGenderView, ModelAccount, Gender.BIRTH_GENDER);
            BirthGenderViewPresenter.UpdateView();
        }

        private void gendersView_RefreshTopPanel(object sender, EventArgs e)
        {
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void EnableBirthGender()
        {
            lblBirthGender.Enabled = true;
            lblBirthGender.Visible = true;
            birthGenderView.Enabled = true;
            birthGenderView.Visible = true;
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

        private IPreMseDemographicsPresenter Presenter { get; set; }
        private IRaceViewPresenter RaceViewPresenter { get; set; }
        private IRaceViewPresenter Race2ViewPresenter { get; set; }
        private IEthnicityViewPresenter EthnicityViewPresenter { get; set; }
        private IEthnicityViewPresenter Ethnicity2ViewPresenter { get; set; }

        public void PopulateOtherLanguage()
        {
            if ( ModelAccount != null && ModelAccount.Patient != null && ModelAccount.Patient != null )
            {
                if (ModelAccount.Patient.OtherLanguage != null)
                {
                    mtbSpecify.Text = ModelAccount.Patient.OtherLanguage.Trim();
                }
                else
                {
                    mtbSpecify.Text = string.Empty;
                }
            }
        }
        public void ClearOtherLanguage()
        {
            mtbSpecify.Text = string.Empty;
        }
        public bool OtherLanguageVisibleAndEnabled
        {
            set
            {
                mtbSpecify.Visible = value;
                lblSpecify.Visible = value;
                mtbSpecify.Enabled = value;
                lblSpecify.Enabled = value;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateBloodlessList()
        {
            var blank = new YesNoFlag();
            blank.SetBlank( String.Empty );
            cmbBloodless.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes( "Yes, desires treatment without blood" );
            cmbBloodless.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo( "No, desires treatment with blood" );
            cmbBloodless.Items.Add( no );

            if ( ModelAccount.Bloodless != null )
            {
                cmbBloodless.SelectedIndex = cmbBloodless.FindString( ModelAccount.Bloodless.Description.ToUpper() );
            }
            else
            {
                cmbBloodless.SelectedIndex = 0;
            }
        }

        private void PopulateMaritalStatusControl()
        {
            var demographicsBrokerProxy = new DemographicsBrokerProxy();
            ICollection maritalStatusCollection = demographicsBrokerProxy.AllMaritalStatuses( User.GetCurrent().Facility.Oid );

            if ( maritalStatusCollection == null )
            {
                MessageBox.Show( "No marial statusus were found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            cmbMaritalStatus.Items.Clear();

            foreach ( MaritalStatus ms in maritalStatusCollection )
            {
                cmbMaritalStatus.Items.Add( ms );
            }
        }

        private void PopulateLanguageControl()
        {
            var demographicsBrokerProxy = new DemographicsBrokerProxy();
            ICollection languageCollection = demographicsBrokerProxy.AllLanguages( User.GetCurrent().Facility.Oid );

            if ( languageCollection == null )
            {
                MessageBox.Show( "No languages were found", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            cmbLanguage.Items.Clear();

            foreach ( Language ms in languageCollection )
            {
                cmbLanguage.Items.Add( ms );
            }
        }

        private void PopulatePatientMailingAddressView()
        {
            if ( ModelAccount == null || ModelAccount.Patient == null )
            {
                return;
            }

            patientMailingAddrView.Context = Address.PatientMailing;
            patientMailingAddrView.KindOfTargetParty = ModelAccount.Patient.GetType();
            patientMailingAddrView.PatientAccount = ModelAccount;

            IAddressBroker addressBroker = new AddressBrokerProxy();
            var counties = (ArrayList)addressBroker.AllCountiesFor( User.GetCurrent().Facility.Oid );

            patientMailingAddrView.IsAddressWithCounty = ( counties != null && counties.Count > 1 );
            patientMailingAddrView.Model = ModelAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            patientMailingAddrView.UpdateView();
        }

        private void PopulatePatientPhysicalAddressView()
        {
            if ( ModelAccount == null || ModelAccount.Patient == null )
            {
                return;
            }

            patientPhysicalAddrView.Context = Address.PatientPhysical;
            patientPhysicalAddrView.KindOfTargetParty = ModelAccount.Patient.GetType();
            patientPhysicalAddrView.PatientAccount = ModelAccount;
            patientPhysicalAddrView.Model_ContactPoint =
                ModelAccount.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );

            patientPhysicalAddrView.UpdateView();
        }
        private void RegisterEvents()
        {
            if ( eventsRegistered )
            {
                return;
            }

            eventsRegistered = true;

            RuleEngine.LoadRules( ModelAccount );

            RuleEngine.GetInstance().RegisterEvent( typeof( MailingAddressPreferred ), Model, patientMailingAddrView.AddressPreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( MailingAddressPhonePreferred ), Model, patientMailingAddrView.PhonePreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( MailingAddressAreaCodePreferred ), Model, patientMailingAddrView.AreaCodePreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
           
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( BloodlessRequired ), Model, BloodlessRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( LanguagePreferred ), Model, LanguagePreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
 
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
   
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidLanguageCode ), Model, InvalidLanguageCodeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidLanguageCodeChange ), Model, InvalidLanguageCodeChangeEventHandler );
            Presenter.RegisterOtherLanguageRequiredRule();
            }

        private void UnregisterEvents()
        {
            eventsRegistered = false;

            try
            {
                RuleEngine.GetInstance().UnregisterEvent( typeof( MailingAddressPreferred ), Model, patientMailingAddrView.AddressPreferredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( MailingAddressPhonePreferred ), Model, patientMailingAddrView.PhonePreferredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( MailingAddressAreaCodePreferred ), Model, patientMailingAddrView.AreaCodePreferredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( MaritalStatusRequired ), Model, MaritalStatusRequiredEventHandler );
          
                RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( BloodlessRequired ), BloodlessRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( LanguagePreferred ), LanguagePreferredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
 
                RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidMaritalStatusCode ), Model, InvalidMaritalStatusCodeEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidMaritalStatusCodeChange ), Model, InvalidMaritalStatusCodeChangeEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidLanguageCode ), Model, InvalidLanguageCodeEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidLanguageCodeChange ), Model, InvalidLanguageCodeChangeEventHandler );
                Presenter.UnRegisterOtherLanguageRequiredRule();
            }
            catch ( Exception )
            {
                //Do nothing
            }
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

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );
            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix( mtbNameSuffix );
            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix( mtbMiddleInitial );
            MaskedEditTextBoxBuilder.ConfigureOtherLanguage( mtbSpecify );
            
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager( typeof( PreMseDemographicsView ) );
            this.grpPatientName = new System.Windows.Forms.GroupBox();
            this.btnEditAKA = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAKA = new System.Windows.Forms.Label();
            this.lblStaticAKA = new System.Windows.Forms.Label();
            this.mtbNameSuffix = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.suffixView = new SuffixView();
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMI = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticLastName = new System.Windows.Forms.Label();
            this.mtbAdmitTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblBirthTime = new Label();
            this.mtbBirthTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.mtbAdmitDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.genderView = new GenderView();
            this.birthGenderView = new GenderView();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblBirthGender = new System.Windows.Forms.Label();

             
            this.lblPatientAge = new System.Windows.Forms.Label();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.cmbMaritalStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.mtbDateOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblRace = new System.Windows.Forms.Label();
            this.lblRace2 = new System.Windows.Forms.Label();
           
            this.lblEthnicity = new System.Windows.Forms.Label();
            this.lblEthnicity2 = new System.Windows.Forms.Label();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmbLanguage = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.patientMailingAddrView = new PatientAccess.UI.AddressViews.AddressView();
            this.patientPhysicalAddrView = new PatientAccess.UI.AddressViews.AddressView();
            this.lblBloodless = new System.Windows.Forms.Label();
            this.cmbBloodless = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblSpecify = new System.Windows.Forms.Label();
            this.mtbSpecify = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.ethnicityView = new EthnicityView();
            this.ethnicity2View = new EthnicityView();

            this.raceView = new RaceView();
            this.race2View = new RaceView();
            this.grpPatientName.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientName
            // 
            this.grpPatientName.Controls.Add(this.btnEditAKA);
            this.grpPatientName.Controls.Add(this.lblAKA);
            this.grpPatientName.Controls.Add(this.lblStaticAKA);
            this.grpPatientName.Controls.Add(this.suffixView);
            this.grpPatientName.Controls.Add(this.mtbMiddleInitial);
            this.grpPatientName.Controls.Add(this.lblStaticMI);
            this.grpPatientName.Controls.Add(this.mtbFirstName);
            this.grpPatientName.Controls.Add(this.lblFirstName);
            this.grpPatientName.Controls.Add(this.mtbLastName);
            this.grpPatientName.Controls.Add(this.lblStaticLastName);
            this.grpPatientName.Location = new System.Drawing.Point(8, 8);
            this.grpPatientName.Name = "grpPatientName";
            this.grpPatientName.Size = new System.Drawing.Size(661, 90);
            this.grpPatientName.TabIndex = 0;
            this.grpPatientName.TabStop = false;
            this.grpPatientName.Text = "Patient name";
            // 
            // btnEditAKA
            // 
            this.btnEditAKA.Location = new System.Drawing.Point(462, 56);
            this.btnEditAKA.Message = null;
            this.btnEditAKA.Name = "btnEditAKA";
            this.btnEditAKA.Size = new System.Drawing.Size(98, 23);
            this.btnEditAKA.TabIndex = 5;
            this.btnEditAKA.Text = "Manage A&KAs...";
            this.btnEditAKA.Click += new System.EventHandler(this.btnEditAKA_Click);
            // 
            // lblAKA
            // 
            this.lblAKA.Location = new System.Drawing.Point(38, 57);
            this.lblAKA.Name = "lblAKA";
            this.lblAKA.Size = new System.Drawing.Size(410, 23);
            this.lblAKA.TabIndex = 0;
            this.lblAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticAKA
            // 
            this.lblStaticAKA.Location = new System.Drawing.Point(9, 57);
            this.lblStaticAKA.Name = "lblStaticAKA";
            this.lblStaticAKA.Size = new System.Drawing.Size(32, 23);
            this.lblStaticAKA.TabIndex = 0;
            this.lblStaticAKA.Text = "AKA:";
            this.lblStaticAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // suffixView
            // 
            this.suffixView.Location = new System.Drawing.Point(561, 20);
            this.suffixView.Name = "suffixView";
            this.suffixView.Size = new System.Drawing.Size(95, 27);
            this.suffixView.TabIndex = 4;
            this.suffixView.Visible = true;
         
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.Location = new System.Drawing.Point(534, 21);
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size(18, 20);
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.Validating += new System.ComponentModel.CancelEventHandler(this.mtbMiddleInitial_Validating);
            // 
            // lblStaticMI
            // 
            this.lblStaticMI.Location = new System.Drawing.Point(513, 24);
            this.lblStaticMI.Name = "lblStaticMI";
            this.lblStaticMI.Size = new System.Drawing.Size(21, 23);
            this.lblStaticMI.TabIndex = 0;
            this.lblStaticMI.Text = "MI:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point(338, 21);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(162, 20);
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbFirstName_Validating);
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(308, 24);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(30, 23);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point(38, 21);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(257, 20);
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbLastName_Validating);
            // 
            // lblStaticLastName
            // 
            this.lblStaticLastName.Location = new System.Drawing.Point(9, 24);
            this.lblStaticLastName.Name = "lblStaticLastName";
            this.lblStaticLastName.Size = new System.Drawing.Size(29, 23);
            this.lblStaticLastName.TabIndex = 0;
            this.lblStaticLastName.Text = "Last:";
            // 
            // mtbAdmitTime
            // 
            this.mtbAdmitTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbAdmitTime.KeyPressExpression = "^([0-2]?|[0-1][0-9]?|[0-1][0-9][0-5]?|[0-1][0-9][0-5][0-9]?|2[0-3]?|2[0-3][0-5]?|" +
    "2[0-3][0-5][0-9]?)$";
            this.mtbAdmitTime.Location = new System.Drawing.Point(886, 31);
            this.mtbAdmitTime.Mask = "  :  ";
            this.mtbAdmitTime.MaxLength = 5;
            this.mtbAdmitTime.Name = "mtbAdmitTime";
            this.mtbAdmitTime.Size = new System.Drawing.Size(48, 20);
            this.mtbAdmitTime.TabIndex = 7;
            this.mtbAdmitTime.ValidationExpression = "^[0-2][0-9][0-5][0-9]$";
            this.mtbAdmitTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbAdmitTime_KeyDown);
            this.mtbAdmitTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtbAdmitTime_KeyPress);
            this.mtbAdmitTime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbAdmitTime_Validating);
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(853, 33);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(40, 23);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time:";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point(812, 31);
            this.dateTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler(this.dateTimePicker_CloseUp);
            // 
            // mtbAdmitDate
            // 
            this.mtbAdmitDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitDate.KeyPressExpression = "^\\d*$";
            this.mtbAdmitDate.Location = new System.Drawing.Point(744, 31);
            this.mtbAdmitDate.Mask = "  /  /";
            this.mtbAdmitDate.MaxLength = 10;
            this.mtbAdmitDate.Name = "mtbAdmitDate";
            this.mtbAdmitDate.Size = new System.Drawing.Size(70, 20);
            this.mtbAdmitDate.TabIndex = 6;
            this.mtbAdmitDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbAdmitDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbAdmitDate_Validating);
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.Location = new System.Drawing.Point(681, 33);
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size(63, 23);
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Admit date:";
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(8, 108);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(60, 23);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // lblBirthGender
            // 
            this.lblBirthGender.Location = new System.Drawing.Point(230, 108);
            this.lblBirthGender.Name = "lblBirthGender";
            this.lblBirthGender.Size = new System.Drawing.Size(60, 23);
            this.lblBirthGender.TabIndex = 0;
            this.lblBirthGender.Text = "Birth Sex:";

            // 
            // Gender View
            // 
            this.genderView.Location = new System.Drawing.Point(83, 102);
            this.genderView.Model = null;
            this.genderView.Name = "genderView";
            this.genderView.Size = new System.Drawing.Size(100, 30);
            this.genderView.TabIndex = 9;
            this.genderView.GenderViewPresenter = null;
            // 
            // Birth Gender View
            // 
            this.birthGenderView.Location = new System.Drawing.Point(278, 102);
            this.birthGenderView.Model = null;
            this.birthGenderView.Name = "genderView";
            this.birthGenderView.Size = new System.Drawing.Size(100, 30);
            this.birthGenderView.TabIndex = 9;
            this.birthGenderView.GenderViewPresenter = null;
            
            // 
            // lblPatientAge
            // 
            this.lblPatientAge.Location = new System.Drawing.Point(194, 138);
            this.lblPatientAge.Name = "lblPatientAge";
            this.lblPatientAge.Size = new System.Drawing.Size(95, 23);
            this.lblPatientAge.TabIndex = 0;
            // 
            // lblStaticAge
            // 
            this.lblStaticAge.Location = new System.Drawing.Point(166, 138);
            this.lblStaticAge.Name = "lblStaticAge";
            this.lblStaticAge.Size = new System.Drawing.Size(40, 23);
            this.lblStaticAge.TabIndex = 0;
            this.lblStaticAge.Text = "Age:";
            // 
            // cmbMaritalStatus
            // 
            this.cmbMaritalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaritalStatus.Location = new System.Drawing.Point(83, 184);
            this.cmbMaritalStatus.Name = "cmbMaritalStatus";
            this.cmbMaritalStatus.Size = new System.Drawing.Size(100, 21);
            this.cmbMaritalStatus.TabIndex = 10;
            this.cmbMaritalStatus.Validating += new System.ComponentModel.CancelEventHandler(this.cmbMaritalStatus_Validating);
            this.cmbMaritalStatus.SelectedIndexChanged += new System.EventHandler( this.cmbMaritalStatus_SelectedIndexChanged );
            // 
            // mtbDateOfBirth
            // 
            this.mtbDateOfBirth.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDateOfBirth.KeyPressExpression = "^\\d*$";
            this.mtbDateOfBirth.Location = new System.Drawing.Point(83, 135);
            this.mtbDateOfBirth.Mask = "  /  /";
            this.mtbDateOfBirth.MaxLength = 10;
            this.mtbDateOfBirth.Name = "mtbDateOfBirth";
            this.mtbDateOfBirth.Size = new System.Drawing.Size(70, 20);
            this.mtbDateOfBirth.TabIndex = 9;
            this.mtbDateOfBirth.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbDateOfBirth.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDateOfBirth_Validating);
            // 
            // lblMaritalStatus
            // 
            this.lblMaritalStatus.Location = new System.Drawing.Point(8, 186);
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size(75, 23);
            this.lblMaritalStatus.TabIndex = 0;
            this.lblMaritalStatus.Text = "Marital status:";
            // 
            // lblDOB
            // 
            this.lblDOB.Location = new System.Drawing.Point(8, 138);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(60, 23);
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Text = "DOB:";

            // 
            // lblBirthTime
            // 
            this.lblBirthTime.Location = new System.Drawing.Point( 8, 162 );
            this.lblBirthTime.Name = "lblBirthTime";
            this.lblBirthTime.Size = new System.Drawing.Size( 60, 23 );
            this.lblBirthTime.TabIndex = 0;
            this.lblBirthTime.Text = "Birth Time:";
            // 
            // mtbBirthTime
            // 
            this.mtbBirthTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbBirthTime.KeyPressExpression = "^\\d*$";
            this.mtbBirthTime.Location = new System.Drawing.Point( 83, 160 );
            this.mtbBirthTime.Mask = "  :";
            this.mtbBirthTime.MaxLength = 5;
            this.mtbBirthTime.Name = "mtbBirthTime";
            this.mtbBirthTime.Size = new System.Drawing.Size( 48, 20 );
            this.mtbBirthTime.TabIndex = 9;
            this.mtbBirthTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbBirthTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbBirthTime_Validating );
            //
            // raceView
            //
            this.raceView.Location = new System.Drawing.Point(83, 213);
            this.raceView.Name = "raceView";
            this.raceView.TabIndex = 11;
            this.raceView.Size = new System.Drawing.Size(135, 25);
            //
            // race2View
            //
            this.race2View.Location = new System.Drawing.Point(278, 213);
            this.race2View.Name = "race2View";
            this.race2View.TabIndex = 12;
            this.race2View.Size = new System.Drawing.Size(135, 25);
            // 
            // lblRace
            // 
            this.lblRace.Location = new System.Drawing.Point(8, 216);
            this.lblRace.Name = "lblRace";
            this.lblRace.Size = new System.Drawing.Size(60, 23);
            this.lblRace.TabIndex = 0;
            this.lblRace.Text = "Race:";
            // 
            // lblRace2
            // 
            this.lblRace2.Location = new System.Drawing.Point(230, 216);
            this.lblRace2.Name = "lblRace2";
            this.lblRace2.Size = new System.Drawing.Size(40, 23);
            this.lblRace2.TabIndex = 0;
            this.lblRace2.Text = "Race 2:";
             
            // 
            // lblEthnicity
            // 
            this.lblEthnicity.Location = new System.Drawing.Point(8, 245);
            this.lblEthnicity.Name = "lblEthnicity";
            this.lblEthnicity.Size = new System.Drawing.Size(60, 23);
            this.lblEthnicity.TabIndex = 0;
            this.lblEthnicity.Text = "Ethnicity:";
            // 
            // lblEthnicity2
            // 
            this.lblEthnicity2.Location = new System.Drawing.Point(218, 245);
            this.lblEthnicity2.Name = "lblEthnicity2";
            this.lblEthnicity2.Size = new System.Drawing.Size(60, 23);
            this.lblEthnicity2.TabIndex = 0;
            this.lblEthnicity2.Text = "Ethnicity 2:";

            //
            // ethnicityView
            //
            this.ethnicityView.Location = new System.Drawing.Point(83, 240);
            this.ethnicityView.Name = "ethnicityView";
            this.ethnicityView.TabIndex = 13;
            this.ethnicityView.Size = new System.Drawing.Size(135, 25);
            //
            // ethnicity2View
            //
            this.ethnicity2View.Location = new System.Drawing.Point(278, 240);
            this.ethnicity2View.Name = "ethnicity2View";
            this.ethnicity2View.TabIndex = 14;
            this.ethnicity2View.Size = new System.Drawing.Size(135, 25);
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point(8, 265);
            this.ssnView.Model = null;
            this.ssnView.ModelAccount = ( (PatientAccess.Domain.Account)( resources.GetObject( "ssnView.ModelAccount" ) ) );
            this.ssnView.Name = "ssnView";
            this.ssnView.Size = new System.Drawing.Size(265, 72);
            this.ssnView.SsnContext = SsnViewContext.PreMseDemographicsView;
            this.ssnView.TabIndex = 15;
            this.ssnView.ssnNumberChanged += new System.EventHandler(this.ssnView_ssnNumberChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.Location = new System.Drawing.Point(8, 379);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(70, 26);
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Language:";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.Location = new System.Drawing.Point(83, 379);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(163, 21);
            this.cmbLanguage.TabIndex = 17;
            this.cmbLanguage.Validating += new System.ComponentModel.CancelEventHandler(this.cmbLanguage_Validating);
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler( this.cmbLanguage_SelectedIndexChanged );
            // 
            // patientMailingAddrView
            // 
            this.patientMailingAddrView.Context = null;
            this.patientMailingAddrView.EditAddressButtonText = "Edit Address...";
            this.patientMailingAddrView.IsAddressWithCounty = false;
            this.patientMailingAddrView.IsAddressWithStreet2 = true;
            this.patientMailingAddrView.KindOfTargetParty = null;
            this.patientMailingAddrView.Location = new System.Drawing.Point(415, 99);
            this.patientMailingAddrView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONECELL;
            this.patientMailingAddrView.Model = null;
            this.patientMailingAddrView.Model_ContactPoint = null;
            this.patientMailingAddrView.Name = "patientMailingAddrView";
            this.patientMailingAddrView.PatientAccount = null;
            this.patientMailingAddrView.ShowStatus = false;
            this.patientMailingAddrView.Size = new System.Drawing.Size( 265, 186 );
            this.patientMailingAddrView.TabIndex = 18;
            this.patientMailingAddrView.AddressChanged += new System.EventHandler(this.patientMailingAddrView_AddressChangedEventHandler);
            this.patientMailingAddrView.AreaCodeChanged += new System.EventHandler(this.PatientMailingAddressView_PhoneNumberChangedEventHandler);
            this.patientMailingAddrView.PhoneNumberChanged += new System.EventHandler(this.PatientMailingAddressView_PhoneNumberChangedEventHandler);
            this.patientMailingAddrView.CellPhoneNumberChanged += new EventHandler(this.PatientMailingAddressView__CellPhoneNumberChanged);
            // 
            // patientPhysicalAddrView
            // 
            this.patientPhysicalAddrView.Context = null;
            this.patientPhysicalAddrView.EditAddressButtonText = "Edit Address...";
            this.patientPhysicalAddrView.IsAddressWithCounty = true;
            this.patientPhysicalAddrView.IsAddressWithStreet2 = true;
            this.patientPhysicalAddrView.KindOfTargetParty = null;
            this.patientPhysicalAddrView.Location = new System.Drawing.Point(722, 99);
            this.patientPhysicalAddrView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONE;
            this.patientPhysicalAddrView.Model = null;
            this.patientPhysicalAddrView.Model_ContactPoint = null;
            this.patientPhysicalAddrView.Name = "patientPhysicalAddrView";
            this.patientPhysicalAddrView.PatientAccount = null;
            this.patientPhysicalAddrView.ShowStatus = false;
            this.patientPhysicalAddrView.Size = new System.Drawing.Size( 265, 186 );
            this.patientPhysicalAddrView.TabIndex = 19;
            this.patientPhysicalAddrView.AddressChanged += new System.EventHandler(this.patientPhysicalAddrView_AddressChangedEventHandler);
            // 
            // lblBloodless
            // 
            this.lblBloodless.Location = new System.Drawing.Point(8, 343);
            this.lblBloodless.Name = "lblBloodless";
            this.lblBloodless.Size = new System.Drawing.Size(70, 23);
            this.lblBloodless.TabIndex = 0;
            this.lblBloodless.Text = "Bloodless:";
            // 
            // cmbBloodless
            // 
            this.cmbBloodless.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBloodless.Location = new System.Drawing.Point(83, 343);
            this.cmbBloodless.Name = "cmbBloodless";
            this.cmbBloodless.Size = new System.Drawing.Size(265, 21);
            this.cmbBloodless.TabIndex = 16;
            this.cmbBloodless.Validating += new System.ComponentModel.CancelEventHandler(this.cmbBloodless_Validating);
            // 
            // lblSpecify
            // 
            this.lblSpecify.Location = new System.Drawing.Point(8, 410);
            this.lblSpecify.Name = "lblSpecify";
            this.lblSpecify.Size = new System.Drawing.Size(70, 26);
            this.lblSpecify.TabIndex = 0;
            this.lblSpecify.Text = "Specify:";
            // 
            // mtbSpecify
            // 
            this.mtbSpecify.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbSpecify.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSpecify.Location = new System.Drawing.Point(83, 411);
            this.mtbSpecify.Mask = "";
            this.mtbSpecify.MaxLength = 20;
            this.mtbSpecify.Name = "mtbSpecify";
            this.mtbSpecify.Size = new System.Drawing.Size(163, 20);
            this.mtbSpecify.TabIndex = 17;
            this.mtbSpecify.Validating += new System.ComponentModel.CancelEventHandler(this.mtbSpecify_Validating);
            // 
            // PreMseDemographicsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mtbSpecify);
            this.Controls.Add(this.lblSpecify);
            this.Controls.Add(this.cmbBloodless);
            this.Controls.Add(this.lblBloodless);
            this.Controls.Add(this.patientPhysicalAddrView);
            this.Controls.Add(this.patientMailingAddrView);
            this.Controls.Add(this.genderView);
            this.Controls.Add(this.birthGenderView);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblBirthGender);

            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.ssnView);
            
            this.Controls.Add(this.lblEthnicity);
            this.Controls.Add(this.lblEthnicity2);
            this.Controls.Add(this.ethnicityView);
            this.Controls.Add(this.ethnicity2View);

            this.Controls.Add(this.lblRace);
            this.Controls.Add(this.lblRace2);
            this.Controls.Add(this.raceView);
            this.Controls.Add(this.race2View);
            this.Controls.Add(this.lblPatientAge);
            this.Controls.Add(this.lblStaticAge);
            this.Controls.Add(this.cmbMaritalStatus);
            this.Controls.Add(this.mtbDateOfBirth);
            this.Controls.Add(this.lblMaritalStatus);
            this.Controls.Add(this.lblDOB);
            Controls.Add(lblBirthTime);
            Controls.Add(mtbBirthTime);
            
            this.Controls.Add(this.mtbAdmitTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.mtbAdmitDate);
            this.Controls.Add(this.lblStaticAdmitDate);
            this.Controls.Add(this.grpPatientName);
            this.Name = "PreMseDemographicsView";
            this.Size = new System.Drawing.Size(1000, 498);
            this.Disposed += new System.EventHandler(this.PreMseDemographicsView_Disposed);
            this.Leave += new System.EventHandler( this.PreMseDemographicsView_Leave );
            this.grpPatientName.ResumeLayout(false);
            this.grpPatientName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PreMseDemographicsView()
        {
            loadingModelData = true;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();

            patientPhysicalAddrView.GroupBoxText = "Patient physical address (if different)";
            patientPhysicalAddrView.EditAddressButtonText = "Ed&it Address...";
            patientMailingAddrView.GroupBoxText = "Patient mailing address and contact";
            patientMailingAddrView.EditAddressButtonText = "Edit &Address...";

            EnableThemesOn( this );
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton btnEditAKA;

        private PatientAccessComboBox cmbBloodless;
        private PatientAccessComboBox cmbMaritalStatus;
         
        private PatientAccessComboBox cmbLanguage;

        private DateTimePicker dateTimePicker;

        private GroupBox grpPatientName;

        private Label lblAKA;
        private Label lblBloodless;
        private Label lblStaticAKA;
        private Label lblStaticMI;
        private Label lblFirstName;
        private Label lblStaticLastName;
        private Label lblTime;
        private Label lblStaticAdmitDate;
    
        private Label lblPatientAge;
        private Label lblStaticAge;
        private Label lblMaritalStatus;
        private Label lblDOB;
        private Label lblRace;
        private Label lblRace2;
        private Label lblEthnicity;
        private Label lblEthnicity2;

        private Label lblLanguage;
        private Label lblBirthTime;

        private MaskedEditTextBox mtbNameSuffix;
        private MaskedEditTextBox mtbMiddleInitial;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbAdmitTime;
        private MaskedEditTextBox mtbAdmitDate;
        private MaskedEditTextBox mtbDateOfBirth;
        private MaskedEditTextBox mtbBirthTime;
        private SuffixView suffixView;

        private AddressView patientPhysicalAddrView;
        private AddressView patientMailingAddrView;
        private SSNControl ssnView;

        private GenderView genderView;
        private GenderView birthGenderView;
        private Label lblGender;
        private Label lblBirthGender;


        private DateTime admitDate;
        private DateTime dateOfBirth;
        private readonly DateTime earliestDate = new DateTime( 1800, 01, 01 );

        private int admitMonth;
        private int admitDay;
        private int admitYear;
        private int admitHour;
        private int admitMinute;
        private int admitSecond;
        private bool loadingModelData;
        private bool eventsRegistered;
      
        private MaritalStatus selectedMaritalStatus;
        private RuleEngine i_RuleEngine;
        private RaceView raceView;
        private RaceView race2View;
        private EthnicityView ethnicityView;
        private EthnicityView ethnicity2View;


        private bool blnLeaveRun;
        private bool isAdmitDateChange;
        private IBirthTimePresenter BirthTimePresenter { get; set; }
        private SuffixPresenter suffixPresenter;
        private SequesteredPatientPresenter sequesteredPatientPresenter;
        private GenderViewPresenter PatientGenderViewPresenter { get; set; }
        private GenderViewPresenter BirthGenderViewPresenter { get; set; }
        #endregion

        #region Constants
        private const string
            TIMESTAMP = "Timestamp",
            FIRSTNAME = "FirstName",
            LASTNAME = "LastName";
        #endregion
        private Label lblSpecify;
        private MaskedEditTextBox mtbSpecify;

        private void mtbSpecify_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpecify );
            Presenter.UpdateOtherLanguage( mtbSpecify.Text );
        }

      
    }
}
