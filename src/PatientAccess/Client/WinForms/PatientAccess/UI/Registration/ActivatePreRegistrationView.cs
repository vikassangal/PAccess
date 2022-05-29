using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Builder;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.RegulatoryViews.ViewImpl;

namespace PatientAccess.UI.Registration
{
    /// <summary>
    /// ActivatePreRegistrationView - The user has chosen to activate a Pre-Registration.  This form
    /// presents a summary view of the remaining items required to register the patient (including new
    /// Patient type (PT), Hospital service code, location, accomodate, admit source and regulatory
    /// information.  This form is navigated to from the List of Accounts view if all required fields
    /// from the PreRegististration process are complete.  (If the account has successfully processed thru
    /// PBAR, there will be no Action items in the To Do list.)
    /// 
    /// This process is in the Registration activity.
    /// 
    /// The patient must be a PT 0.  The available PTs to activate to are 01, 02, 04, and 09.  
    /// The account is locked for the duration of the form execution.  If the user cancels, the account is 
    /// unlocked and - if any bed was reserved during the process - the bed is released.
    /// 
    /// The user may opt to edit the full details of the account by clicking the Edit more... button.  This
    /// brings them into account maintenance in the Registration activity.
    /// 
    /// TO DO:  release the bed reservation if one was made
    /// 
    /// </summary>
    [Serializable]
    public class ActivatePreRegistrationView : ControlView, IAlternateCareFacilityView
    {

        #region Delegates
        #endregion

        #region Events

        public event EventHandler OnRepeatActivity;
        public event EventHandler OnEditAccount;
        public event EventHandler OnCloseActivity;

        #endregion

        #region Event Handlers

        private void mtbAdmitTime_TextChanged( object sender, EventArgs e )
        {
        }

        private void ActivatePreRegistrationView_Leave( object sender, EventArgs e )
        {
            Dispose();
        }

        /// <summary>
        /// ActivatePreRegistrationView_Disposed - on disposing, remove any event handlers we have
        /// wired to rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivatePreRegistrationView_Disposed( object sender, EventArgs e )
        {
            // unwire the events

            unRegisterEvents();
        }

        // These are the event handlers for failed rules
        private void AdmitDateFutureDateEventHandler( object sender, EventArgs e )
        {
            setErrorBgColor( mtbAdmitDate );
            MessageBox.Show( UIErrorMessages.ACTIVATE_ADMIT_FUTURE_DATE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1 );
            mtbAdmitDate.Focus();
        }

        private void AdmitDateFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            setErrorBgColor( mtbAdmitDate );
            MessageBox.Show( UIErrorMessages.ACTIVATE_ADMIT_FUTURE_DATE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1 );
            mtbAdmitDate.Focus();
        }
        private void AdmitTimeRequiredEventHandler(object sender, EventArgs e)
        {
            setRequiredBgColor(mtbAdmitTime);
        }

        private void AdmitSourceRequiredEventHandler( object sender, EventArgs e )
        {
            setRequiredBgColor( cmbAdmitSource );
        }

        private void AdmitDateRequiredEventHandler( object sender, EventArgs e )
        {
            setRequiredBgColor( mtbAdmitDate );
        }
        private void AlternateCareFacilityRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAlternateCareFacility );
        }
        private void InvalidAdmitSourceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbAdmitSource );
        }

        /// <summary>
        /// mtbAdmitDate_Validating - the user tabbed out of the admit date text box, ensure that the data is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbAdmitDate_Validating( object sender, CancelEventArgs e )
        {
            AdmitDateValidating();
        }

        /// <summary>
        /// mtbAdmitTime_Validating - edit the admit time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbAdmitTime_Validating( object sender, CancelEventArgs e )
        {
            validateDateField = "TIME";
            string strDate = mtbAdmitDate.UnMaskedText.Trim();
            string strTime = mtbAdmitTime.UnMaskedText.Trim();
            if (strDate != string.Empty
                && strTime != "0000")
            {
                if (DateValidator.IsValidTime(mtbAdmitTime) == false)
                {
                    if (!dtAdmitDatePicker.Focused)
                    {
                        mtbAdmitTime.Focus();
                    }
                    return;
                }
            }
            Model.AdmitDate = (!string.IsNullOrEmpty(strDate) && !string.IsNullOrEmpty(strTime))
                                  ?
                                      DateTime.Parse(mtbAdmitDate.Text + " " + mtbAdmitTime.Text)
                                  : DateTime.MinValue;

            checkForRequiredFields();
        }

        /// <summary>
        /// dtAdmitDatePicker_ValueChanged - populate the admitdate text (mtb) with the datepicker value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtAdmitDatePicker_ValueChanged( object sender, EventArgs e )
        {
            mtbAdmitDate.Text = String.Format("{0:D2}{1:D2}{2:D4}", dtAdmitDatePicker.Value.Month,
                                            dtAdmitDatePicker.Value.Day, dtAdmitDatePicker.Value.Year);
            AdmitDateValidating();
        }

        /// <summary>
        /// cmbAdmitSource_SelectedIndexChanged - clear the required field indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void cmbAdmitSource_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                HandleAdmitSourceSelectedIndexChanged( cb.SelectedItem as AdmitSource );
            }
        }
        private void HandleAdmitSourceSelectedIndexChanged( AdmitSource newAdmitSource )
        {
            if ( newAdmitSource != null )
            {
                Model.AdmitSource = newAdmitSource;
            }
          
            AlternateCareFacilityPresenter.HandleAlternateCareFacility();
            EvaluateAdmitSourceRules();
            checkForRequiredFields();
        }
        
        private void cmbAdmitSource_Validating( object sender, CancelEventArgs e )
        {
                HandleAdmitSourceSelectedIndexChanged( cmbAdmitSource.SelectedItem as AdmitSource );

            
                UIColors.SetNormalBgColor( cmbAdmitSource );
                Refresh();
                AlternateCareFacilityPresenter.HandleAlternateCareFacility();
                EvaluateAdmitSourceRules();

        }
        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbAlternateCareFacility control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmbAlternateCareFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
           
        }
        private void cmbAlternateCareFacility_Validating( object sender, CancelEventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
            EvaluateAdmitSourceRules();
        }
        /// <summary>
        /// btnModifyMore_Click - the user will be transferred to the account maint form in Registration activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifyMore_Click( object sender, EventArgs e )
        {
            populateModelFromForm();

            if ( !checkForRequiredFields() )
            {
                displayRequiredFields();
                return;
            }

            if ( Model != null )
            {
                LooseArgs args = new LooseArgs( Model );
                SearchEventAggregator.GetInstance().RaiseEditRegistrationEvent( this, args );
            }
        }
        private void EvaluateAdmitSourceRules()
        {
                RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), Model );
        }
        /// <summary>
        /// btnOK_Click - the user has indicated that they are ready to activate the account.  Ensure that
        /// all required fields are entered and valid, and persist the changes.  If successful, display the
        /// confirmation screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click( object sender, EventArgs e )
        {
            populateModelFromForm();

            if ( !checkForRequiredFields() )
            {
                displayRequiredFields();
                return;
            }

            //Raise Activitycomplete event
            Model.Patient.SetPatientContextHeaderData();
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this,
                new LooseArgs( Model ) );

            saveModel();

            DisplayConfirmationScreen();
        }

        // if some required field is empty, display an error messgage
        private void displayRequiredFields()
        {
            StringBuilder sb = new StringBuilder();

            foreach ( LeafAction la in RuleEngine.GetFailedActions() )
            {
                if ( la.IsComposite )
                {
                    CompositeAction ca = ( CompositeAction )la;

                    foreach ( LeafAction cla in ca.Constituents )
                    {
                        // refactor to remove 'magic number'
                        if ( cla.Severity == 4 )
                        {
                            sb.Append( cla.Description + "\r\n" );
                        }
                    }
                }
            }

            string inValidCodesSummary = sb.ToString();

            if ( inValidCodesSummary != string.Empty )
            {
                RequiredFieldsDialog requiredFieldsDialog = new RequiredFieldsDialog();
                requiredFieldsDialog.ErrorText = inValidCodesSummary;
                requiredFieldsDialog.UpdateView();

                try
                {
                    requiredFieldsDialog.ShowDialog( this );
                    RuleEngine.GetInstance().ClearActions();
                }
                finally
                {
                    requiredFieldsDialog.Dispose();
                }
            }
        }

        /// <summary>
        /// checkForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private bool checkForRequiredFields()
        {
            if (btnCancel.Focused)
            {
                return true;
            }

            if (isAdmitDateValid())
            {
                SetAdmitDateNormalBgColor();
            }
            else
            {
                return false;
            }
            if (iSAdmitTimeValid())
            {
                SetAdmitTimeNormalBgColor();
            }
            else
            {
                return false;
            }

            bool[] rc = { true, true, true, true, true, true, true, true, true, true };

            rc[0] = RuleEngine.GetInstance().EvaluateRule(typeof(AdmitDateFutureDate), Model);
            rc[1] = RuleEngine.GetInstance().EvaluateRule(typeof(AdmitDateFiveDaysPast), Model);
            rc[2] = RuleEngine.GetInstance().EvaluateRule(typeof(AdmitDateRequired), Model);
            rc[3] = RuleEngine.GetInstance().EvaluateRule(typeof(PatientTypeRequired), Model);
            rc[4] = RuleEngine.GetInstance().EvaluateRule(typeof(LocationRequired), Model);
            rc[5] = RuleEngine.GetInstance().EvaluateRule(typeof(HospitalServiceRequired), Model);
            rc[6] = RuleEngine.GetInstance().EvaluateRule(typeof(AdmitSourceRequired), Model);
            rc[7] = RuleEngine.GetInstance().EvaluateRule(typeof(AccomodationRequired), Model);
            rc[8] = RuleEngine.GetInstance().EvaluateRule(typeof(AdmitTimeRequired), Model);
            rc[9] = RuleEngine.GetInstance().EvaluateRule( typeof( AlternateCareFacilityRequired ), Model );

            // return true if all rules passed
            if ( rc[0] && rc[1] && rc[2] && rc[3] && rc[4] && rc[5] && rc[6] && rc[7] && rc[8] && rc[9] )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// registerConfirmationView1_EditAccount - fire the edit account event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_EditAccount( object sender, EventArgs e )
        {
            if ( Model != null )
            {
                //Raise OnEditAccount event
                OnEditAccount( this, new LooseArgs( Model ) );
            }
        }

        /// <summary>
        /// registerConfirmationView1_CloseActivity - fire the close activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_CloseActivity( object sender, EventArgs e )
        {
            OnCloseActivity( this, new LooseArgs( Model ) );
        }

        /// <summary>
        /// registerConfirmationView1_RepeatActivity - fire the repeat activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_RepeatActivity( object sender, EventArgs e )
        {
            //Raise RepeatActivity event.
            OnRepeatActivity( this, new LooseArgs( new Patient() ) );
        }

        /// <summary>
        /// btnCancel_Click - close this form (and cleanup)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model ) ) )
            {

                Dispose();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// UpdateView - Build the dropdowns from reference data, populate 
        /// the form from Model, wire-up the LocationView and the RegulatorView.
        /// </summary>
        public override void UpdateView()
        {
            Cursor = Cursors.WaitCursor;
            AlternateCareFacilityPresenter = new AlternateCareFacilityPresenter( this, new AlternateCareFacilityFeatureManager() );
           
            if ( Model == null )
            {
                return;
            }

            if ( Model.KindOfVisit.Code == VisitType.INPATIENT ) //CR0231
            {
                Model.Clinics.Clear();
            }

            Cursor = Cursors.Arrow;
            RuleEngine.LoadRules( Model );

            buildDropDowns();
            populateFromModel();

            // default the AdmitDate on the Model so that the rules have the correct value to audit against

            Model.AdmitDate = DateTime.Parse( mtbAdmitDate.Text + " " + mtbAdmitTime.Text );

            // get rid of the PT (no longer valid)

            Model.KindOfVisit = new VisitType();

            // get rid of the HospitalService code (as it is no longer valid for the original patient type)

            patientTypeHSVLocationView.Model = Model;
            patientTypeHSVLocationView.UpdateView();

            regulatoryView1.Model = Model;
            regulatoryView1.UpdateView();

            patientContextView1.Model = Model.Patient;
            patientContextView1.Account = Model;
            patientContextView1.UpdateView();

            // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

            StackTrace st = new StackTrace();
            string stackOutput = StackTracer.LogTraceLog( st, btnOK.InvokeRequired );
            // this is being logged using the BreadCrumbLogger in namespace UI.Logging
            BreadCrumbLogger.GetInstance.Log( String.Format( stackOutput ) );

            // ********************************************************************************************************
            // wire up the event handlers
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateFutureDate ), Model, new EventHandler( AdmitDateFutureDateEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateFiveDaysPast ), Model, new EventHandler( AdmitDateFiveDaysPastEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitDateRequired ), Model, new EventHandler( AdmitDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitSourceRequired ), Model, new EventHandler( AdmitSourceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent(typeof(AdmitTimeRequired), Model, new EventHandler(AdmitTimeRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent( typeof( AlternateCareFacilityRequired ), new EventHandler( AlternateCareFacilityRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidAdmitSourceCode ), Model, new EventHandler( InvalidAdmitSourceCodeEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidAdmitSourceCodeChange ), Model, InvalidAdmitSourceCodeEventHandler );

            checkForRequiredFields();
        }
        public void PopulateAlternateCareFacility()
        {
            var alternateCareFacilityBroker = BrokerFactory.BrokerOfType<IAlternateCareFacilityBroker>();
            var allAlternateCareFacilities =
                alternateCareFacilityBroker.AllAlternateCareFacilities( User.GetCurrent().Facility.Oid );

            cmbAlternateCareFacility.Items.Clear();

            foreach ( var alternateCareFacility in allAlternateCareFacilities )
            {
                cmbAlternateCareFacility.Items.Add( alternateCareFacility );
            }

            // If the value is not in the list, add it as a valid choice. This will
            // prevent data loss in the event that the value stored with the account
            // is removed from the lookup table
            if ( !cmbAlternateCareFacility.Items.Contains( Model.AlternateCareFacility ) )
            {
                cmbAlternateCareFacility.Items.Add( Model.AlternateCareFacility );
            }

            cmbAlternateCareFacility.SelectedItem = Model.AlternateCareFacility;
        }

        public void ClearAlternateCareFacility()
        {
            if ( cmbAlternateCareFacility.Items.Count > 0 )
            {
                cmbAlternateCareFacility.SelectedIndex = 0;
            }
        }

        public void ShowAlternateCareFacilityDisabled()
        {
            UIColors.SetDisabledDarkBgColor( cmbAlternateCareFacility );
            cmbAlternateCareFacility.Enabled = false;
        }

        public void ShowAlternateCareFacilityEnabled()
        {
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            cmbAlternateCareFacility.Enabled = true;
        }

        public void ShowAlternateCareFacilityVisible()
        {
            cmbAlternateCareFacility.Visible = true;
            lblAlternateCareFacility.Visible = true;
        }

        public void ShowAlternateCareFacilityNotVisible()
        {
            cmbAlternateCareFacility.Visible = false;
            lblAlternateCareFacility.Visible = false;
        }
        #endregion

        #region Properties

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

        /// <summary>
        /// Model - Account from the selected account (FindAPatient)
        /// </summary>
        public new Account Model
        {
            get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        private IAlternateCareFacilityPresenter AlternateCareFacilityPresenter
        {
            get
            {
                return alternateCareFacilityPresenter;
            }

            set
            {
                alternateCareFacilityPresenter = value;
            }
        }
        #endregion

        #region Private Methods

        private void unRegisterEvents()
        {
            btnModifyMore.Click -= btnModifyMore_Click;
            cmbAdmitSource.SelectedIndexChanged -= cmbAdmitSource_SelectedIndexChanged;
            cmbAdmitSource.SelectedIndexChanged -= cmbAdmitSource_SelectedIndexChanged;
            mtbAdmitDate.Validating -= mtbAdmitDate_Validating;
            dtAdmitDatePicker.ValueChanged -= dtAdmitDatePicker_ValueChanged;
            Disposed += ActivatePreRegistrationView_Disposed;
            Leave += ActivatePreRegistrationView_Leave;

            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitSourceRequired ), Model, AdmitSourceRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent(typeof(AdmitTimeRequired), Model, AdmitTimeRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( AlternateCareFacilityRequired ), Model, AlternateCareFacilityRequiredEventHandler );
        }

        /// <summary>
        /// saveModel - persist changes to the Model (Account).
        /// </summary>
        private void saveModel()
        {
            IAccountBroker acctBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            if ( acctBroker == null )
            {
                MessageBox.Show( "Failed to get accountPBarBroker broker", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );
                return;
            }

            Account anAccount = Model;
            anAccount.Facility = User.GetCurrent().Facility;

            Activity currentActivity = new ActivatePreRegistrationActivity();
            currentActivity.AppUser = User.GetCurrent();

            AccountSaveResults results = acctBroker.Save( anAccount, currentActivity );
            results.SetResultsTo( Model );
        }

        /// <summary>
        /// getFacilityTime - return the current datetime stamp relative to the user's facility (from PBAR hub)
        /// </summary>
        /// <returns></returns>
        private DateTime getFacilityTime()
        {
            if ( facDateTime == DateTime.MinValue )
            {
                ITimeBroker broker = ProxyFactory.GetTimeBroker();
                facDateTime = broker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                             User.GetCurrent().Facility.DSTOffset );
            }

            return facDateTime;
        }

        /// <summary>
        /// displayConfirmationScreen - display a confirmation screen if the activate was successful
        /// </summary>
        private void DisplayConfirmationScreen()
        {
            pnlControls.Hide();
            btnOK.Hide();
            btnCancel.Hide();

            contextLabel.Description = "Register Inpatient or Outpatient";
            registerConfirmationView1.Model = Model;
            registerConfirmationView1.UpdateView();
            panelConfirmation.Show();
        }

        /// <summary>
        /// populateFromModel - set form field values based on the Model properties
        /// </summary>
        private void populateFromModel()
        {
            // admit date/time

            DateTime facDate = getFacilityTime();

            mtbAdmitDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", facDate.Month, DateTime.Now.Day, facDate.Year );

            if ( facDate.Hour != 0 ||
                facDate.Minute != 0 )
            {
                mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", facDate.Hour, facDate.Minute );
            }
            else
            {
                mtbAdmitTime.UnMaskedText = string.Empty;
            }

            mtbComments.Text = Model.ClinicalComments;
            mtbEmbosserCard.Text = Model.EmbosserCard;
        }

        /// <summary>
        /// populateModelFromForm - prior to invoking the broker to persist, populate the Model with the values the
        /// user entered on the form.
        /// </summary>
        private void populateModelFromForm()
        {
            Model.AdmitDate = DateTime.Parse( mtbAdmitDate.Text + " " + ( mtbAdmitTime.UnMaskedText == string.Empty ? "00:00" : mtbAdmitTime.Text ) );

            Model.AdmitSource = ( AdmitSource )cmbAdmitSource.SelectedItem;

            Model.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion =
                regulatoryView1.Model_Account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion;
            Model.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus =
                regulatoryView1.Model_Account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus;
            Model.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate =
                regulatoryView1.Model_Account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate;

            Model.ConfidentialityCode = regulatoryView1.Model_Account.ConfidentialityCode;
            Model.OptOutName = regulatoryView1.Model_Account.OptOutName;
            Model.OptOutLocation = regulatoryView1.Model_Account.OptOutLocation;
            Model.OptOutHealthInformation = regulatoryView1.Model_Account.OptOutHealthInformation;
            Model.OptOutReligion = regulatoryView1.Model_Account.OptOutReligion;
            Model.COSSigned = regulatoryView1.Model_Account.COSSigned;
            Model.FacilityDeterminedFlag = regulatoryView1.Model_Account.FacilityDeterminedFlag;

            Model.ClinicalComments = mtbComments.Text;
            Model.EmbosserCard = mtbEmbosserCard.Text;

            Model.ValuablesAreTaken = ( YesNoFlag )cmbValuablesCollected.SelectedItem;
        }

        /// <summary>
        /// buildDropDowns - invoke the appropriate broker to retrieve values for dropdownlist boxes.
        /// Build each list box based on the result set and any applicable rules
        /// </summary>
        private void buildDropDowns()
        {
            // patient type

            populateAdmitSources();
            PopulateValuablesCollectedList();
        }

        /// <summary>
        /// populateAdmitSources - build the admit source dropdown
        /// </summary>
        private void populateAdmitSources()
        {
            cmbAdmitSource.Items.Clear();
            cmbAdmitSource.ResetText();
            cmbAdmitSource.Items.Add( new AdmitSource() );

            AdmitSourceBrokerProxy brokerProxy = new AdmitSourceBrokerProxy();
            ArrayList allSources = ( ArrayList )brokerProxy.AllTypesOfAdmitSources( User.GetCurrent().Facility.Oid );

            for ( int i = 0; i < allSources.Count; i++ )
            {
                AdmitSource source = new AdmitSource();

                source = ( AdmitSource )allSources[i];
                cmbAdmitSource.Items.Add( source );
            }

            if ( Model.AdmitSource != null )
            {
                cmbAdmitSource.SelectedItem = Model.AdmitSource;
            }
            else
            {
                setRequiredBgColor( cmbAdmitSource );
            }
        }

        private void PopulateValuablesCollectedList()
        {
            cmbValuablesCollected.Items.Clear();

            YesNoFlag blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cmbValuablesCollected.Items.Add( blank );

            YesNoFlag yes = new YesNoFlag();
            yes.SetYes();
            cmbValuablesCollected.Items.Add( yes );

            YesNoFlag no = new YesNoFlag();
            no.SetNo();
            cmbValuablesCollected.Items.Add( no );

            if ( Model.ValuablesAreTaken != null )
            {
                if ( Model.ValuablesAreTaken.Code == YesNoFlag.CODE_YES )
                {
                    cmbValuablesCollected.SelectedIndex = 1;
                }
                else if ( Model.ValuablesAreTaken.Code == YesNoFlag.CODE_NO )
                {
                    cmbValuablesCollected.SelectedIndex = 2;
                }
                else
                {
                    cmbValuablesCollected.SelectedIndex = 0;
                }
            }
            else
            {
                cmbValuablesCollected.SelectedIndex = 2;
            }
        }

        private void SetAdmitDateNormalBgColor()
        {
            UIColors.SetNormalBgColor(mtbAdmitDate);
            Refresh();
        }
        private void SetAdmitDateErrBgColor()
        {
            mtbAdmitDate.Focus();
            admitMonth = admitDay = admitYear = -1;
            UIColors.SetErrorBgColor(mtbAdmitDate);
        }
        private void SetAdmitTimeErrBgColor()
        {
            mtbAdmitTime.Focus();
            admitMonth = admitDay = admitYear = -1;
            UIColors.SetErrorBgColor(mtbAdmitTime);
        }
      

        /// <summary>
        /// setRequiredBgColor - set the background color to the required field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setRequiredBgColor( Control field )
        {
            UIColors.SetRequiredBgColor( field );
            Refresh();
        }

        /// <summary>
        /// setErrorBgColor - set the background color to the error field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setErrorBgColor( Control field )
        {
            UIColors.SetErrorBgColor( field );
            Refresh();
        }

        /// <summary>
        /// AdmitDateValidating - Validates Admit date and display appropriate message
        /// </summary>
        private void AdmitDateValidating()
        {
            validateDateField = "DATE";
            if (dtAdmitDatePicker.Focused)
            {
                return;
            }

            if (!isLengthOfDateFieldValid())
            {
                return;
            }
            string strDate = mtbAdmitDate.UnMaskedText.Trim();
            string strTime = mtbAdmitTime.UnMaskedText.Trim();





            if (!string.IsNullOrEmpty(strDate))
            {
                Model.AdmitDate = (!string.IsNullOrEmpty(strTime))
                                      ?
                                          DateTime.Parse(mtbAdmitDate.Text + " " + mtbAdmitTime.Text)
                                      :
                                          DateTime.Parse(mtbAdmitDate.Text);

                AdmitDateToInsuranceValidation.CheckAdmitDateToInsurance(Model, Name);
            }
            else
            {
                Model.AdmitDate = DateTime.MinValue;
            }

            if (!dtAdmitDatePicker.Focused)
            {
                checkForRequiredFields();
            }
        }

        /// <summary>
        /// isLengthOfDateFieldValid - Checks for length of date field and dispays appropriate error message
        /// </summary>
        private bool isLengthOfDateFieldValid()
        {
            string dateString = mtbAdmitDate.UnMaskedText.Trim();
            if (dateString != string.Empty
                && dateString.Length != 0
                && mtbAdmitDate.Text.Length != 10)
            {
                DisplayDateTimeErrorMessage(UIErrorMessages.ADMIT_ERRMSG, "Date",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// SetAdmitTimeNormalBgColor - Sets admit time to normal color
        /// </summary>
        private void SetAdmitTimeNormalBgColor()
        {
            UIColors.SetNormalBgColor(mtbAdmitTime);
            Refresh();
        }

        /// <summary>
        /// DisplayDateTimeErrorMessage - Display error message 
        /// </summary>
        private void DisplayDateTimeErrorMessage(string errorMessage, string caption, MessageBoxButtons btn,
                                                MessageBoxIcon icon, MessageBoxDefaultButton defaultBtn)
        {
            if (iSValidatingDateField())
            {
                SetAdmitDateErrBgColor();
            }
            else
            {
                SetAdmitTimeErrBgColor();
            }
            MessageBox.Show(errorMessage, caption, btn, icon, defaultBtn);
        }

        /// <summary>
        /// Returns true if Date field is being validated.
        /// </summary>
        /// <returns></returns>
        private bool iSValidatingDateField()
        {
            return validateDateField == "DATE";
        }

        /// <summary>
        /// isAdmitDateValid - Checks if AdmitDate in Valid 
        /// </summary>
        private bool isAdmitDateValid()
        {
            if (dtAdmitDatePicker.Focused || mtbAdmitDate.UnMaskedText == String.Empty)
            {
                return true;
            }
            else if (!isLengthOfDateFieldValid())
            {
                return false;
            }

            SetDateTimeValuesFromUI();
            try
            {
                admitDate = new DateTime(admitYear, admitMonth, admitDay, admitHour, admitMinute, admitSecond);
          
                if (DateValidator.IsValidDate(admitDate) == false)
                {
                    DisplayDateTimeErrorMessage(UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                MessageBoxDefaultButton.Button1);


                    return false;
                }
                else
                {
                    Model.AdmitDate = admitDate;
                    return true;
                }
            }
            catch
            {
                // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                DisplayDateTimeErrorMessage(UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button1);
                return false;
            }
        }
        /// <summary>
        /// iSAdmitTimeValid - Checks if AdmitTime in Valid 
        /// </summary>
        private bool iSAdmitTimeValid()
        {
            
            if (mtbAdmitTime.UnMaskedText.Equals(String.Empty))
            {
                SetAdmitTimeNormalBgColor();

                return true;
            }
            else if (mtbAdmitTime.UnMaskedText.Length < 4)
            {
                DisplayDateTimeErrorMessage(UIErrorMessages.TIME_NOT_VALID_MSG, "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button1);
                return false;
            }
            SetDateTimeValuesFromUI();
            try
            {
                admitDate = new DateTime(admitYear, admitMonth, admitDay, admitHour, admitMinute, admitSecond);
                if (iSAdmitTimeInFuture())
                {
                    return false;
                }
                SetAdmitTimeNormalBgColor();
                Model.AdmitDate = admitDate;
                return true;
            }
            catch
            {
                // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                DisplayDateTimeErrorMessage(UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button1);
                return false;
            }
        }
        /// <summary>
        /// iSAdmitTimeInFuture - Checks if AdmitTime is in future 
        /// </summary>
        private bool iSAdmitTimeInFuture()
        {
            DateTime facilityDateTime = getFacilityTime();
            int originalAdmitHour = facilityDateTime.Hour;
            int originalAdmitMinute = facilityDateTime.Minute;
            int enteredHour = 0;
            int enteredMinute = 0;
             
            try
            {
                enteredHour = Convert.ToInt32(mtbAdmitTime.Text.Substring(0, 2));
            }
            catch
            {
                enteredHour = 0;
            }
            try
            {
                enteredMinute = Convert.ToInt32(mtbAdmitTime.Text.Substring(3, 2));
            }
            catch
            {
                enteredMinute = 0;
            }

            if (IsTodaysDate() && IsTimeInTheFuture(originalAdmitHour, originalAdmitMinute, enteredHour, enteredMinute))
            {
                DisplayDateTimeErrorMessage(UIErrorMessages.ADMIT_TIME_FUTURE_ERRMSG, "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button1);
                return true;
            }
            return false;
        }
        /// <summary>
        /// GetAdmitDateFromUI - Gets admit date from UI
        /// </summary>
        private DateTime GetAdmitDateFromUI()
        {
            DateTime theDate = DateTime.MinValue;

            if (mtbAdmitDate.UnMaskedText != string.Empty)
            {
                theDate = GetDateAndTimeFrom(mtbAdmitDate.UnMaskedText, mtbAdmitTime.UnMaskedText);
            }
            else
            {
                if (mtbAdmitTime.UnMaskedText != string.Empty)
                {
                    Model.AdmitDate = DateTime.MinValue;
                    string unFormattedDateString = GetUnFormattedDateString(Model.AdmitDate);
                    theDate = GetDateAndTimeFrom(unFormattedDateString, mtbAdmitTime.UnMaskedText);
                }
            }
            return theDate;
        }
        /// <summary>
        /// GetUnFormattedDateString - Get unformatted date string from the given datetime value
        /// </summary>
        private static string GetUnFormattedDateString(DateTime dateTime)
        {
            return String.Format("{0:D2}{1:D2}{2:D4}", dateTime.Month, dateTime.Day, dateTime.Year);
        }
        /// <summary>
        /// GetDateAndTimeFrom - Returns DateTime value given date string and Time string
        /// </summary>
        private DateTime GetDateAndTimeFrom(string dateText, string timeText)
        {
            DateTime theDate;
            admitMonth = Convert.ToInt32(dateText.Substring(0, 2));
            admitDay = Convert.ToInt32(dateText.Substring(2, 2));
            admitYear = Convert.ToInt32(dateText.Substring(4, 4));
            admitHour = 0;
            admitMinute = 0;

            if (timeText.Length == 4)
            {
                admitHour = Convert.ToInt32(timeText.Substring(0, 2));
                admitMinute = Convert.ToInt32(timeText.Substring(2, 2));
            }

            if ((admitHour >= 0 && admitHour <= 23) && (admitMinute >= 0 && admitMinute <= 59))
            {
                theDate = new DateTime(admitYear, admitMonth, admitDay, admitHour, admitMinute, 0);
            }
            else
            {
                theDate = new DateTime(admitYear, admitMonth, admitDay);
            }
            return theDate;
        }
        /// <summary>
        /// IsTodaysDate - Returns true if the Date entered in the UI is todays date
        /// </summary>IsTodaysDate
        private bool IsTodaysDate()
        {
            DateTime uiEnteredAdmitDate = GetAdmitDateFromUI();
            return uiEnteredAdmitDate.Date.Equals(DateTime.Today);
        }

        /// <summary>
        /// IsTodaysDate - Returns true if the Date is a future date
        /// </summary>IsTodaysDate
        private static bool IsTimeInTheFuture(int originalAdmitHour, int originalAdmitMinute, int enteredHour,
                                          int enteredMinute)
        {
            int todaysTotalSeconds = (originalAdmitHour * 60) + originalAdmitMinute;
            int enteredTotalSeconds = (enteredHour * 60) + enteredMinute;

            if (enteredTotalSeconds > todaysTotalSeconds)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// SetDateTimeValuesFromUI - Sets Date, Month , Year, Hours, Minutes and Seconds from UI
        /// </summary>IsTodaysDate
        private void SetDateTimeValuesFromUI()
        {
            if (!mtbAdmitDate.UnMaskedText.Equals(String.Empty))
            {
                admitMonth = Convert.ToInt32(mtbAdmitDate.Text.Substring(0, 2));
                admitDay = Convert.ToInt32(mtbAdmitDate.Text.Substring(3, 2));
                admitYear = Convert.ToInt32(mtbAdmitDate.Text.Substring(6, 4));
                admitSecond = 0;
            }
            else
            {
                admitMonth = DateTime.MinValue.Month;
                admitDay = DateTime.MinValue.Day;
                admitYear = DateTime.MinValue.Year;
            }
            if (mtbAdmitTime.UnMaskedText.Equals(String.Empty))
            {
                admitHour = 0;
                admitMinute = 0;
            }
            else
            {
                admitHour = Convert.ToInt32(mtbAdmitTime.Text.Substring(0, 2));
                admitMinute = Convert.ToInt32(mtbAdmitTime.Text.Substring(3, 2));
            }
        }
      

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureClinicalViewComments( mtbComments );
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ActivatePreRegistrationView ) );
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.cmbAlternateCareFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.patientTypeHSVLocationView = new PatientAccess.UI.CommonControls.PatientTypeHSVLocationView();
            this.mtbComments = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnModifyMore = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cmbValuablesCollected = new System.Windows.Forms.ComboBox();
            this.mtbEmbosserCard = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblValuablesCollected = new System.Windows.Forms.Label();
            this.lblEmbosserCard = new System.Windows.Forms.Label();
            this.lblComments = new System.Windows.Forms.Label();
            this.regulatoryView1 = new PatientAccess.UI.RegulatoryViews.ViewImpl.RegulatoryView();
            this.cmbAdmitSource = new System.Windows.Forms.ComboBox();
            this.lblAdmitSource = new System.Windows.Forms.Label();
            this.mtbAdmitTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.mtbAdmitDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dtAdmitDatePicker = new System.Windows.Forms.DateTimePicker();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnOK = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.panelConfirmation = new System.Windows.Forms.Panel();
            this.registerConfirmationView1 = new PatientAccess.UI.Registration.RegisterConfirmationView();
            this.pnlControls.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelConfirmation.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.contextLabel.Description = " Register Inpatient or Outpatient";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.Location = new System.Drawing.Point( 0, 0 );
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size( 1024, 23 );
            this.contextLabel.TabIndex = 0;
            this.contextLabel.TabStop = false;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size( 1006, 24 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // pnlControls
            // 
            this.pnlControls.BackColor = System.Drawing.Color.White;
            this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControls.Controls.Add( this.cmbAlternateCareFacility );
            this.pnlControls.Controls.Add( this.lblAlternateCareFacility );
            this.pnlControls.Controls.Add( this.patientTypeHSVLocationView );
            this.pnlControls.Controls.Add( this.mtbComments );
            this.pnlControls.Controls.Add( this.btnModifyMore );
            this.pnlControls.Controls.Add( this.cmbValuablesCollected );
            this.pnlControls.Controls.Add( this.mtbEmbosserCard );
            this.pnlControls.Controls.Add( this.lblValuablesCollected );
            this.pnlControls.Controls.Add( this.lblEmbosserCard );
            this.pnlControls.Controls.Add( this.lblComments );
            this.pnlControls.Controls.Add( this.regulatoryView1 );
            this.pnlControls.Controls.Add( this.cmbAdmitSource );
            this.pnlControls.Controls.Add( this.lblAdmitSource );
            this.pnlControls.Controls.Add( this.mtbAdmitTime );
            this.pnlControls.Controls.Add( this.lblTime );
            this.pnlControls.Controls.Add( this.lblStaticAdmitDate );
            this.pnlControls.Controls.Add( this.mtbAdmitDate );
            this.pnlControls.Controls.Add( this.dtAdmitDatePicker );
            this.pnlControls.Location = new System.Drawing.Point( 8, 56 );
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size( 1008, 528 );
            this.pnlControls.TabIndex = 3;
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point( 110, 469 );
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size( 192, 21 );
            this.cmbAlternateCareFacility.TabIndex = 9;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler( this.cmbAlternateCareFacility_SelectedIndexChanged );
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAlternateCareFacility_Validating );
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point( 8, 469 );
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size( 82, 27 );
            this.lblAlternateCareFacility.TabIndex = 8;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // patientTypeHSVLocationView
            // 
            this.patientTypeHSVLocationView.Location = new System.Drawing.Point( 3, 32 );
            this.patientTypeHSVLocationView.Model = null;
            this.patientTypeHSVLocationView.Name = "patientTypeHSVLocationView";
            this.patientTypeHSVLocationView.NursingStationCode = null;
            this.patientTypeHSVLocationView.Size = new System.Drawing.Size( 338, 277 );
            this.patientTypeHSVLocationView.TabIndex = 5;
            // 
            // mtbComments
            // 
            this.mtbComments.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComments.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbComments.Location = new System.Drawing.Point( 378, 384 );
            this.mtbComments.Mask = "";
            this.mtbComments.MaxLength = 120;
            this.mtbComments.Multiline = true;
            this.mtbComments.Name = "mtbComments";
            this.mtbComments.Size = new System.Drawing.Size( 360, 40 );
            this.mtbComments.TabIndex = 12;
            // 
            // btnModifyMore
            // 
            this.btnModifyMore.Location = new System.Drawing.Point( 378, 486 );
            this.btnModifyMore.Message = null;
            this.btnModifyMore.Name = "btnModifyMore";
            this.btnModifyMore.Size = new System.Drawing.Size( 86, 23 );
            this.btnModifyMore.TabIndex = 17;
            this.btnModifyMore.Text = "Modify More...";
            this.btnModifyMore.Click += new System.EventHandler( this.btnModifyMore_Click );
            // 
            // cmbValuablesCollected
            // 
            this.cmbValuablesCollected.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValuablesCollected.Location = new System.Drawing.Point( 502, 458 );
            this.cmbValuablesCollected.Name = "cmbValuablesCollected";
            this.cmbValuablesCollected.Size = new System.Drawing.Size( 46, 21 );
            this.cmbValuablesCollected.TabIndex = 16;
            // 
            // mtbEmbosserCard
            // 
            this.mtbEmbosserCard.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEmbosserCard.Location = new System.Drawing.Point( 502, 432 );
            this.mtbEmbosserCard.Mask = "";
            this.mtbEmbosserCard.MaxLength = 10;
            this.mtbEmbosserCard.Name = "mtbEmbosserCard";
            this.mtbEmbosserCard.Size = new System.Drawing.Size( 75, 20 );
            this.mtbEmbosserCard.TabIndex = 14;
            // 
            // lblValuablesCollected
            // 
            this.lblValuablesCollected.Location = new System.Drawing.Point( 378, 462 );
            this.lblValuablesCollected.Name = "lblValuablesCollected";
            this.lblValuablesCollected.Size = new System.Drawing.Size( 112, 13 );
            this.lblValuablesCollected.TabIndex = 15;
            this.lblValuablesCollected.Text = "Valuables collected:";
            // 
            // lblEmbosserCard
            // 
            this.lblEmbosserCard.Location = new System.Drawing.Point( 378, 436 );
            this.lblEmbosserCard.Name = "lblEmbosserCard";
            this.lblEmbosserCard.Size = new System.Drawing.Size( 100, 13 );
            this.lblEmbosserCard.TabIndex = 13;
            this.lblEmbosserCard.Text = "Embosser card:";
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point( 378, 366 );
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size( 64, 23 );
            this.lblComments.TabIndex = 11;
            this.lblComments.Text = "Comments:";
            // 
            // regulatoryView1
            // 
            this.regulatoryView1.BackColor = System.Drawing.Color.White;
            this.regulatoryView1.Location = new System.Drawing.Point( 378, 9 );
            this.regulatoryView1.Model = null;
            this.regulatoryView1.Name = "regulatoryView1";
            this.regulatoryView1.Size = new System.Drawing.Size( 360, 359 );
            this.regulatoryView1.TabIndex = 10;
            // 
            // cmbAdmitSource
            // 
            this.cmbAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdmitSource.Location = new System.Drawing.Point( 110, 436 );
            this.cmbAdmitSource.Name = "cmbAdmitSource";
            this.cmbAdmitSource.Size = new System.Drawing.Size( 192, 21 );
            this.cmbAdmitSource.TabIndex = 7;
            this.cmbAdmitSource.SelectedIndexChanged += new System.EventHandler( this.cmbAdmitSource_SelectedIndexChanged );
            this.cmbAdmitSource.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAdmitSource_Validating );
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point( 8, 436 );
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size( 80, 22 );
            this.lblAdmitSource.TabIndex = 6;
            this.lblAdmitSource.Text = "Admit source:";
            // 
            // mtbAdmitTime
            // 
            this.mtbAdmitTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitTime.KeyPressExpression = "^([0-2]?|[0-1][0-9]?|[0-1][0-9][0-5]?|[0-1][0-9][0-5][0-9]?|2[0-3]?|2[0-3][0-5]?|" +
                "2[0-3][0-5][0-9]?)$";
            this.mtbAdmitTime.Location = new System.Drawing.Point( 243, 11 );
            this.mtbAdmitTime.Mask = "  :";
            this.mtbAdmitTime.MaxLength = 5;
            this.mtbAdmitTime.Name = "mtbAdmitTime";
            this.mtbAdmitTime.Size = new System.Drawing.Size( 35, 20 );
            this.mtbAdmitTime.TabIndex = 3;
            this.mtbAdmitTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbAdmitTime.TextChanged += new System.EventHandler( this.mtbAdmitTime_TextChanged );
            this.mtbAdmitTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitTime_Validating );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 211, 14 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 41, 13 );
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "Time:";
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.Location = new System.Drawing.Point( 10, 14 );
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size( 63, 13 );
            this.lblStaticAdmitDate.TabIndex = 10;
            this.lblStaticAdmitDate.Text = "Admit date:";
            // 
            // mtbAdmitDate
            // 
            this.mtbAdmitDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitDate.KeyPressExpression = resources.GetString( "mtbAdmitDate.KeyPressExpression" );
            this.mtbAdmitDate.Location = new System.Drawing.Point( 109, 12 );
            this.mtbAdmitDate.Mask = "  /  /";
            this.mtbAdmitDate.MaxLength = 10;
            this.mtbAdmitDate.Name = "mtbAdmitDate";
            this.mtbAdmitDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbAdmitDate.TabIndex = 0;
            this.mtbAdmitDate.ValidationExpression = resources.GetString( "mtbAdmitDate.ValidationExpression" );
            this.mtbAdmitDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitDate_Validating );
            // 
            // dtAdmitDatePicker
            // 
            this.dtAdmitDatePicker.Location = new System.Drawing.Point( 179, 12 );
            this.dtAdmitDatePicker.Name = "dtAdmitDatePicker";
            this.dtAdmitDatePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dtAdmitDatePicker.TabIndex = 1;
            this.dtAdmitDatePicker.ValueChanged += new System.EventHandler( this.dtAdmitDatePicker_ValueChanged );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 944, 592 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Location = new System.Drawing.Point( 864, 592 );
            this.btnOK.Message = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 75, 23 );
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BackColor = System.Drawing.Color.White;
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView1 );
            this.panelPatientContext.Location = new System.Drawing.Point( 8, 24 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 1008, 26 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // panelConfirmation
            // 
            this.panelConfirmation.BackColor = System.Drawing.Color.White;
            this.panelConfirmation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelConfirmation.Controls.Add( this.registerConfirmationView1 );
            this.panelConfirmation.Location = new System.Drawing.Point( 8, 56 );
            this.panelConfirmation.Name = "panelConfirmation";
            this.panelConfirmation.Size = new System.Drawing.Size( 1008, 528 );
            this.panelConfirmation.TabIndex = 7;
            // 
            // registerConfirmationView1
            // 
            this.registerConfirmationView1.BackColor = System.Drawing.Color.White;
            this.registerConfirmationView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerConfirmationView1.Location = new System.Drawing.Point( 0, 0 );
            this.registerConfirmationView1.Model = null;
            this.registerConfirmationView1.Name = "registerConfirmationView1";
            this.registerConfirmationView1.Size = new System.Drawing.Size( 1006, 526 );
            this.registerConfirmationView1.TabIndex = 0;
            this.registerConfirmationView1.RepeatActivity += new System.EventHandler( this.registerConfirmationView1_RepeatActivity );
            this.registerConfirmationView1.EditAccount += new System.EventHandler( this.registerConfirmationView1_EditAccount );
            this.registerConfirmationView1.CloseActivity += new System.EventHandler( this.registerConfirmationView1_CloseActivity );
            // 
            // ActivatePreRegistrationView
            // 
            this.AcceptButton = this.btnOK;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.btnOK );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.contextLabel );
            this.Controls.Add( this.pnlControls );
            this.Controls.Add( this.panelConfirmation );
            this.Name = "ActivatePreRegistrationView";
            this.Size = new System.Drawing.Size( 1024, 625 );
            this.Disposed += new System.EventHandler( this.ActivatePreRegistrationView_Disposed );
            this.pnlControls.ResumeLayout( false );
            this.pnlControls.PerformLayout();
            this.panelPatientContext.ResumeLayout( false );
            this.panelConfirmation.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Default constuctor
        /// </summary>
        public ActivatePreRegistrationView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            ConfigureControls();

            // TODO: Add any initialization after the InitializeComponent call
            panelConfirmation.Hide();
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

        private PatientContextView patientContextView1;
        private UserContextView contextLabel;
        private RegulatoryView regulatoryView1;
        private RegisterConfirmationView registerConfirmationView1;

        private IContainer components = null;

        private DateTimePicker dtAdmitDatePicker;

        private MaskedEditTextBox mtbAdmitDate;
        private MaskedEditTextBox mtbAdmitTime;
        private MaskedEditTextBox mtbComments;
        private MaskedEditTextBox mtbEmbosserCard;

        private Label lblStaticAdmitDate;
        private Label lblTime;
        private Label lblAdmitSource;
        private Label lblComments;
        private Label lblEmbosserCard;
        private Label lblValuablesCollected;
        private ComboBox cmbAdmitSource;
        private ComboBox cmbValuablesCollected;

        private LoggingButton btnCancel;
        private LoggingButton btnOK;
        private LoggingButton btnModifyMore;

        private Panel panelPatientContext;
        private Panel panelConfirmation;
        private Panel pnlControls;

        private DateTime facDateTime;
        private DateTime admitDate;

        private int admitMonth;
        private int admitDay;
        private int admitYear;
        private int admitHour;
        private int admitMinute;
        private int admitSecond;
        private string validateDateField = "DATE";
        private RuleEngine i_RuleEngine;
        private PatientAccessComboBox cmbAlternateCareFacility;
        private Label lblAlternateCareFacility;
        private PatientTypeHSVLocationView patientTypeHSVLocationView;

        private IAlternateCareFacilityPresenter alternateCareFacilityPresenter;
        #endregion

        #region Constants
        #endregion
    }
}

