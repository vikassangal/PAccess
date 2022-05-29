using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.HistoricalAccountViews;
using PatientAccess.Utilities;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Summary description for FindAPatientView.
    /// </summary>
    [Serializable]
    public class FindAPatientView : ControlView
    {
        #region Events
        public event EventHandler PatientsFound;
        public event EventHandler SearchReset;
        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>Valid
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // TLG 06/25/2007 - reset the results view so that 'View Accounts' button 
            // cannot be clicked while the search is in progress (happens when the 
            // button was enabled from a previous search)
            this.SearchReset(this, null);

            ValidateSsn();
            this.ParentForm.Cursor = Cursors.WaitCursor;
            if (i_ValidSsn)
            {
                var i_phoneNumberComplete = IsPhoneNumberComplete();
                ValidateDobEntry();
                if (i_ValidDobEntry && i_phoneNumberComplete)
                {
                    //Call FindMatchingPatients method to perform search.
                    FindMatchingPatients();
                }
            }
            
            this.ParentForm.Cursor = Cursors.Default;
            EMPISearched = false;
        }

        /// <summary>
        /// Reset All text entry fields and clear listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResestSearchCriteria();
        }

        public void ResestSearchCriteria()
        {
            i_Reset = true;

            ResetBackGroundColor();

            mtbDobM.Text = String.Empty;
            mtbDobD.Text = String.Empty;
            mtbDobY.Text = String.Empty;

            this.mtbAccount.ResetText();
            this.mtbFirstName.ResetText();
            this.mtbLastName.ResetText();
            this.mtbMrn.ResetText();
            this.axMskSsn.ResetText();
            this.mtbDobM.ResetText();
            this.mtbDobY.ResetText();
            this.mtbDobD.ResetText();
            this.btnSearch.Enabled = false;
            //Reset the combobox.
            cmbGender.SelectedItem = null;
            phoneNumberControl.Model = new PhoneNumber(); 
            this.SearchReset(this, null);
            this.Invalidate();
            this.Update();

            //this.SetFocus();

            i_Reset = false;

            this.mtbDobM.Refresh();
            this.mtbDobY.Refresh();
            this.mtbDobD.Refresh();

            SetPreferredColor();
        }

        /// <summary>
        /// Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindAPatientView_Load(object sender, EventArgs e)
        {
            InitializeGendersComboBox();

            // TODO:  Evaluate.  This may not be the best place to do this... but it works.
            if (this.ParentForm != null && FirstTimeLayout)
            {
                //this.ParentForm.AcceptButton = btnSearch;
                base.AcceptButton = btnSearch;
                FirstTimeLayout = false;
            }
            SetPreferredColor();
            this.SetFocus();
            base.AcceptButton = btnSearch;
        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (cmb != null && cmb.SelectedItem != null && !cmb.SelectedItem.Equals(String.Empty))
            {
                Gender aGender = (Gender)((DictionaryEntry)cmb.SelectedItem).Key;
                /**
                 * Now that we have the ReferenceValue representing the gender,
                 * we can do something useful, like update the SearchCriteria object
                 **/
            }

            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void mtbMrn_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void mtbAccount_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void mtbLastName_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void mtbFirstName_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void axMskSsn_Change(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void mtbDobM_Enter(object sender, EventArgs e)
        {
            mtbDobM.SelectionStart = mtbDobM.TextLength;
        }

        private void mtbDobD_Enter(object sender, EventArgs e)
        {
            mtbDobD.SelectionStart = mtbDobD.TextLength;
        }

        private void mtbDobY_Enter(object sender, EventArgs e)
        {
            mtbDobY.SelectionStart = mtbDobY.TextLength;
        }

        private void mtbDobM_TextChanged(object sender, EventArgs e)
       {
            if (mtbDobM.TextLength == 2)
            {
                mtbDobD.Focus();
            }
            CheckForDataEntered();
        }

        private void mtbDobD_TextChanged(object sender, EventArgs e)
        {
            if (mtbDobD.TextLength == 2)
            {
                mtbDobY.Focus();
            }
        }

        private void mtbDobY_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void mtbDobM_Validating(object sender, CancelEventArgs e)
        {
            ValidateMonth();
        }

        private void ValidateMonth()
        {
            if (mtbDobM.Text != String.Empty && mtbDobM.Text != "0" && mtbDobM.TextLength == 1)
            {
                mtbDobM.Text = "0" + mtbDobM.Text;
            }
            else if (mtbDobM.Text != String.Empty && Convert.ToInt16(mtbDobM.Text) == 0)
            {
                mtbDobM.Text = String.Empty;
            }
            SetPreferredColor(mtbDobM);
            CheckForDataEntered();
        }

        private void mtbDobD_Validating(object sender, CancelEventArgs e)
        {
            ValidateDate();
        }

        private void ValidateDate()
        {
            if (mtbDobD.Text != String.Empty && mtbDobD.Text != "0" && mtbDobD.TextLength == 1)
            {
                mtbDobD.Text = "0" + mtbDobD.Text;
            }
            else if (mtbDobD.Text != String.Empty && Convert.ToInt16(mtbDobD.Text) == 0)
            {
                mtbDobD.Text = String.Empty;
            }
            SetPreferredColor(mtbDobD);
        }

        private void mtbDobY_Validating(object sender, CancelEventArgs e)
        {
            if (mtbDobY.TextLength > 0 && mtbDobY.TextLength < 4)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG, UIErrorMessages.ERROR,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                mtbDobY.Focus();
                return;
            }
            ValidateMonth();
            ValidateDate();
            SetPreferredColor(mtbDobY);
            CheckForDataEntered();
        }

        private void axMskSsn_Validating(object sender, CancelEventArgs e)
        {
            ValidateSsn();
            SetPreferredColor(axMskSsn);
        }

        private void mtbLastName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces
            this.mtbLastName.Text = this.mtbLastName.Text.TrimEnd(null);
            SetPreferredColor(mtbLastName);
        }

        private void SetPreferredColor(MaskedEditTextBox mtb)
        {
            if (mtb.UnMaskedText.Trim().Length == 0)
            {
                UIColors.SetPreferredBgColor(mtb);
            }
            else
            {
                UIColors.SetNormalBgColor(mtb);
            }
        }

        private void mtbFirstName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces
            this.mtbFirstName.Text = this.mtbFirstName.Text.TrimEnd(null);
            SetPreferredColor(mtbFirstName);
        }

        private void mtbLastName_Enter(object sender, EventArgs e)
        {
            this.mtbLastName.SelectionStart = this.mtbLastName.TextLength;
        }

        private void mtbFirstName_Enter(object sender, EventArgs e)
        {
            this.mtbFirstName.SelectionStart = this.mtbFirstName.TextLength;
        }
        #endregion

        #region Methods

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if (i_ValidSearchArgs & !i_Reset)
            {
                this.btnSearch.Enabled = true;
            }
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            if ((axMskSsn.UnMaskedText.Length == 9 || axMskSsn.UnMaskedText.Length == 0) && !i_Reset && i_ValidDobEntry)
            {
                PopulateSearchArgs();
            }
            else
            {
                i_ValidSearchArgs = false;
            }
        }

        /// <summary>
        /// Set default focus.
        /// </summary>
        public void SetFocus()
        {
            Type aType = null;

            if (!this.DesignMode)
            {
                if (this.ParentForm.GetType() == typeof(HistoricalPatientSearch))
                {
                }
                else if (this.Parent != null
                    && this.Parent.Parent != null
                    && this.Parent.Parent.Parent != null
                    && this.Parent.Parent.Parent.Parent != null
                    && this.Parent.Parent.Parent.Parent.Parent != null
                    && this.Parent.Parent.Parent.Parent.Parent.Parent != null)
                {
                    if (((MasterPatientIndexView)this.Parent.Parent.Parent.Parent.Parent.Parent).CurrentActivity != null)
                    {
                        aType = ((MasterPatientIndexView)this.Parent.Parent.Parent.Parent.Parent.Parent).CurrentActivity.GetType();
                    }
                }

                if (aType != null
                    && (aType == typeof(RegistrationActivity)
                    || aType == typeof(PreRegistrationActivity)
                    || aType == typeof(PreMSERegisterActivity)
                    || aType == typeof(UCCPreMSERegistrationActivity)))
                {
                    this.axMskSsn.Focus();
                }
                else
                {
                    this.mtbAccount.Focus();
                }

            }
        }
        public void ShowNewbornSearchLabel()
        {
            lblNewbornSearch.Visible = false;
        }
        #endregion

        #region Properties
        public ClickOnceLoggingButton SearchButton
        {
            get
            {
                return btnSearch;
            }
            set
            {
                btnSearch = value;
            }
        }

        internal PatientSearchCriteria CriteriaForCurrentSearch
        {
            get
            {
                return i_CriteriaForCurrentSearch;
            }
            private set
            {
                i_CriteriaForCurrentSearch = value;
            }
        }

        public Activity CurrentActivity
        {
            private get { return i_CurrentActivity; }
            set { i_CurrentActivity = value; }
        }

        public PatientSearchResponse PatientSearchResponse
        {
            get { return this.patientSearchResponse; }
        }

        public bool ResestSearchView { get; set; }

        #endregion

        #region Private Methods
        /// <summary>
        /// Validate the search criteria entered by the user.
        /// Messagebox will be displayed if error is found.
        /// </summary>

        private void BeforeWork()
        {
            if (this.ParentForm.GetType() == typeof(HistoricalPatientSearch))
            {
                ((HistoricalPatientSearch)this.ParentForm).ShowPanel();
            }
            else
            {
                ((SearchView)this.Parent.Parent.Parent.Parent.Parent).ShowPanel();
            }
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
           
            if (this.IsDisposed || this.Disposing)
                return;

            this.btnSearch.Enabled = true;

            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    MessageBox.Show(UIErrorMessages.PATIENT_SEARCH_TIMEOUT_MSG);
                    this.SearchReset(this, null);
                    DisplaySearchView();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                if (PatientsFound != null)
                {
                    // Raise PatientsFound Event
                    PatientsFound(this, new LooseArgs(collectionOfPatients));
                }

                DisplaySearchView();
                if (IsEMPIFeatureEnabled() && collectionOfPatients.Count == 0 &&
                    ( HasFullName || HasDateOfBirthAndName || HasPhoneNumber || HasSocialSecurityNumber ) )
                {
                    ((SearchView) this.Parent.Parent.Parent.Parent.Parent).PatientSearchResultsView
                        .DisableSearchEMPIButton();
                    btnSearch.Enabled = false;
                    SearchEMPI();
                    if (ResestSearchView)
                    {
                        ResestSearchCriteria();
                        return;
                    }
                    btnSearch.Enabled = true;
                    if (EMPISearched)
                    {
                        ((SearchView) this.Parent.Parent.Parent.Parent.Parent).PatientSearchResultsView
                            .DisableSearchEMPIButton();
                    }
                    else
                    {
                        ((SearchView) this.Parent.Parent.Parent.Parent.Parent).PatientSearchResultsView
                            .EnableSearchEMPIButton();
                    }
                }
            }

            this.Cursor = Cursors.Default;

            //Reset Background colors on successful search criteria.
            ResetBackGroundColor();
        }

        public void SearchEMPI()
        {
            ResestSearchView = false;
            GetPatientSearchResultsFromEMPI();

            if (HasRequiredFieldsForEMPISearch && !PatientSearchResponse.HasEMPIResults)
            {
                switch (PatientSearchResponse.EMPIResultStatus)
                {
                    case EMPIResultStatus.NOEMPIRESULTSFOUND:
                        MessageBox.Show(UIErrorMessages.NO_EMPI_PATIENTS_FOUND, UIErrorMessages.ERROR,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                        break;
                    case EMPIResultStatus.SYSTEMDOWN:
                        MessageBox.Show(UIErrorMessages.EMPI_SYSTEM_UNAVAILABLE, UIErrorMessages.SYSTEM_UNAVAILABLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                        break;
                    case EMPIResultStatus.TIMEOUT:
                        MessageBox.Show(UIErrorMessages.EMPI_SYSTEM_TIMEDOUT, UIErrorMessages.ERROR,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                        break;
                }
            }
            EMPIPatientsFound();
        }

        private void EMPIPatientsFound()
        {
            if (PatientsFound != null)
            {
                // Raise PatientsFound Event
                PatientsFound(this, new LooseArgs(PatientSearchResponse.PatientSearchResults));
            }

            DisplaySearchView();
            this.Cursor = Cursors.Default;
            //Reset Background colors on successful search criteria.
            ResetBackGroundColor();
        }

        private void DoFindMatchingPatients(object sender, DoWorkEventArgs e)
        {
            // Set an instance level reference to the PatientSearchCriteria used
            // to retrieve the this set of results.
            this.CriteriaForCurrentSearch = this.patientCriteria;

            if (this.CurrentActivity != null &&
                this.CurrentActivity.GetType().Equals(typeof(ViewAccountActivity)))
            {
                IPatientBroker broker =
                    BrokerFactory.BrokerOfType<IPatientBroker>();

                //Call PatientsMatching on PBAR broker 
                //(returns patients, purged and non purged accounts).
                this.patientSearchResponse = broker.GetPatientSearchResponseFor(patientCriteria);
                collectionOfPatients = patientSearchResponse.PatientSearchResults;
            }
            else
            {
                // poll CancellationPending and set e.Cancel to true and return 
                if (this.backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                PatientBrokerProxy broker =
                    new PatientBrokerProxy();
                this.patientSearchResponse = broker.GetPatientSearchResponseFor(patientCriteria);
                collectionOfPatients = patientSearchResponse.PatientSearchResults;
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        public List<PatientSearchResult> GetPatientSearchResultsFromEMPI()
        {

            if ( IsEMPIFeatureEnabled() )
            {
                if (!HasRequiredFieldsForEMPISearch )
                {
                    ShowEMPISearchRequiredFieldsDialog();
                }
                if (HasRequiredFieldsForEMPISearch)
                {
                    var empiBroker = BrokerFactory.BrokerOfType<IEMPIBroker>();
                    var EMPIPatientSearchResponse = empiBroker.SearchEMPI(patientCriteria,
                        User.GetCurrent().Facility);
                    if (EMPIPatientSearchResponse != null && EMPIPatientSearchResponse.PatientSearchResults != null &&
                        EMPIPatientSearchResponse.PatientSearchResults.Count > 0)
                    {
                        patientSearchResponse = EMPIPatientSearchResponse;
                        collectionOfPatients = patientSearchResponse.PatientSearchResults;
                    }
                    if (EMPIPatientSearchResponse != null)
                    {
                        patientSearchResponse.EMPIResultStatus = EMPIPatientSearchResponse.EMPIResultStatus;
                    }
                }
                EMPISearched = true; 
            }
            return collectionOfPatients;
        }

        public bool EMPISearched { get; set; }

        public bool IsPatientSearchWithOnlyMRN
        {
            get
            {
                return (!String.IsNullOrEmpty(mtbMrn.Text.Trim()) && !HasRequiredFieldsForEMPISearch &&
                        String.IsNullOrEmpty(mtbLastName.Text) && String.IsNullOrEmpty(mtbFirstName.Text) &&
                        !HasGender);
            }
        }
        public bool IsPatientSearchWithMRN
        {
            get { return (!String.IsNullOrEmpty(mtbMrn.Text.Trim())); }
        }
        public bool HasFullName
        {
            get
            {
                return ( ! String.IsNullOrEmpty(mtbLastName.Text) && !String.IsNullOrEmpty(mtbFirstName.Text) );
            }
        }

        private void ShowEMPISearchRequiredFieldsDialog()
        {
            var empiSearchFieldsDialog = new EMPISearchFieldsDialog {SearchCriteria = patientCriteria};

            try
            {
                empiSearchFieldsDialog.ShowDialog();

                if (empiSearchFieldsDialog.DialogResult == DialogResult.OK)
                {

                    patientCriteria.DayOfBirth = empiSearchFieldsDialog.SearchCriteria.DayOfBirth;
                    patientCriteria.MonthOfBirth = empiSearchFieldsDialog.SearchCriteria.MonthOfBirth;
                    patientCriteria.YearOfBirth = empiSearchFieldsDialog.SearchCriteria.YearOfBirth;
                    patientCriteria.PhoneNumber.AreaCode =
                        empiSearchFieldsDialog.SearchCriteria.PhoneNumber.AreaCode;
                    patientCriteria.PhoneNumber.Number =
                        empiSearchFieldsDialog.SearchCriteria.PhoneNumber.Number;
                    patientCriteria.SocialSecurityNumber =
                        new SocialSecurityNumber(
                            empiSearchFieldsDialog.SearchCriteria.SocialSecurityNumber
                                .UnformattedSocialSecurityNumber);
                    PopulateSearchView();
                  
                }
                else if (empiSearchFieldsDialog.DialogResult == DialogResult.Cancel)
                {
                    ResestSearchView = true;
                }
                
            }
            finally
            {
                empiSearchFieldsDialog.Dispose();
            }
        }

        private void PopulateSearchView()
        {
            if(patientCriteria != null)
            {
                
                var DOB = patientCriteria.DateOfBirth;
                if (  DOB != DateTime.MinValue )
                {
                    mtbDobM.Text = String.Format("{0:D2}", DOB.Month);
                    mtbDobY.Text = String.Format("{0:D4}", DOB.Year);
                    mtbDobD.Text = String.Format("{0:D2}", DOB.Day);
                }
                if (patientCriteria.SocialSecurityNumber != null)
                {
                    axMskSsn.UnMaskedText = patientCriteria.SocialSecurityNumber.UnformattedSocialSecurityNumber;
                }
                if (patientCriteria.PhoneNumber != null)
                {
                    phoneNumberControl.AreaCode = patientCriteria.PhoneNumber.AreaCode;
                    phoneNumberControl.PhoneNumber = patientCriteria.PhoneNumber.Number;
                }
                SetPreferredColor();
            }
        }

        private bool HasRequiredFieldsForEMPISearch
        {
            get { return (HasPhoneNumber || HasDateOfBirth || HasSocialSecurityNumber ); }
        }

        public bool HasDateOfBirthAndName
        {
            get
            {
                return (HasDateOfBirth &&
                        (!String.IsNullOrEmpty(mtbLastName.Text) || !String.IsNullOrEmpty(mtbFirstName.Text))
                    );
            }
        }
        public bool HasGender
        {
            get
            {
                return ( cmbGender.SelectedItem != null &&
                        ((DictionaryEntry) cmbGender.SelectedItem).Value.ToString() != string.Empty);
            }
        }
        /// <summary>
        /// FindMatchingPatients
        /// </summary>
        private void FindMatchingPatients()
        {
            this.Cursor = Cursors.WaitCursor;
            Gender gender = new Gender();
            if (cmbGender.SelectedItem != null && ((DictionaryEntry)cmbGender.SelectedItem).Value.ToString() != string.Empty)
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            PopulateSearchArgs();

            //Create PatientSearchCriteria and pass in all args.
            patientCriteria = new PatientSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbFirstName.Text,
                this.mtbLastName.Text,
                i_Ssn,
                gender,
                i_MonthOfBirth,
                i_DayOfBirth,
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text,
                i_PhoneNumber
                );
            if (!string.IsNullOrEmpty(this.mtbDobD.Text))
            {
                patientCriteria.DayOfBirth = long.Parse(mtbDobD.Text);
            }

            //Check to see if search data entered is valid.
            ValidationResult result = patientCriteria.Validate();

            if (result.IsValid)
            {
                if (this.backgroundWorker == null
                    ||
                    (this.backgroundWorker != null
                    && !this.backgroundWorker.IsBusy)
                    )
                {

                    this.BeforeWork();

                    this.backgroundWorker = new BackgroundWorker();
                    this.backgroundWorker.WorkerSupportsCancellation = true;

                    backgroundWorker.DoWork += DoFindMatchingPatients;
                    backgroundWorker.RunWorkerCompleted += AfterWork;

                    backgroundWorker.RunWorkerAsync();

                }
            }
            else
            {
                this.SearchReset(this, null);
                string errorMsg = result.Message;
                if (errorMsg != String.Empty)
                {
                    MessageBox.Show(errorMsg, UIErrorMessages.ERROR,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    SearchError(result.AspectInError);
                }
            }

            this.Cursor = Cursors.Default;

        }



        private void CheckForDataEntered()
        {
            if (ShouldEnableSearchButton)
            {
                this.btnSearch.Enabled = true;
                if (this.ParentForm != null)
                {
                    base.AcceptButton = btnSearch;
                }
            }
            else
            {

                this.btnSearch.Enabled = false;
            }
        }

        private bool HasDateOfBirth
        {
            get
            {
                return !String.IsNullOrEmpty(mtbDobM.Text) &&
                       !String.IsNullOrEmpty(mtbDobD.Text) &&
                       !String.IsNullOrEmpty(mtbDobY.Text);
            }
        }

        public bool HasPhoneNumber
        {
            get { return phoneNumberControl.AreaCode.Length + phoneNumberControl.PhoneNumber.Length == 10;  }
        }
        private bool IsPhoneNumberEmpty
        {
            get { return phoneNumberControl.AreaCode.Length + phoneNumberControl.PhoneNumber.Length == 0    ; }
        }

        public bool HasSocialSecurityNumber 
        {
            get
            {
                return patientCriteria.SocialSecurityNumber.IsComplete &&
                       !SocialSecurityNumber.DEFAULT_SSN_NUMBERS.Contains(
                           patientCriteria.SocialSecurityNumber.UnformattedSocialSecurityNumber);
            }
        }

        private bool ShouldEnableSearchButton
        {
            get
           {
                return (!String.IsNullOrEmpty(mtbMrn.Text) ||
                        !String.IsNullOrEmpty(mtbAccount.Text) ||
                        axMskSsn.UnMaskedText.Length == SocialSecurityNumber.MAX_VALUE.Length ||
                        HasPhoneNumber ||
                        (!String.IsNullOrEmpty(mtbLastName.Text) &&
                         (HasDateOfBirth || !String.IsNullOrEmpty(mtbFirstName.Text) || GenderSelected())) ||
                        (HasDateOfBirth && !String.IsNullOrEmpty(mtbFirstName.Text) && GenderSelected())
                    );

            }
        }

        private bool GenderSelected()
        {
            return (cmbGender.SelectedItem != null && !cmbGender.Text.Equals(String.Empty));
        }

        private void ResetBackGroundColor()
        {
            //Set background color back to white.

            UIColors.SetNormalBgColor(axMskSsn);
            UIColors.SetNormalBgColor(mtbFirstName);
            UIColors.SetNormalBgColor(mtbLastName);
            UIColors.SetNormalBgColor(mtbMrn);
            UIColors.SetNormalBgColor(mtbAccount);
            UIColors.SetNormalBgColor(cmbGender);
            SetDobNormalBgColor();
            phoneNumberControl.SetNormalColor();
            SetPreferredColor();
        }

        private void SearchError(string AspectInError)
        {
            switch (AspectInError)
            {
                case "FirstName":
                    this.mtbFirstName.Select();
                    break;
                case "LastName":
                    this.mtbLastName.Select();
                    break;
                case "Mrn":
                    this.mtbMrn.Select();
                    break;
                case "Account":
                    this.mtbAccount.Select();
                    break;
                case "Gender":
                    this.cmbGender.Select();
                    break;
                default:
                    break;
            }
        }

        private void ValidateSsn()
        {
            i_ValidSsn = true;
            if (this.axMskSsn.UnMaskedText.Length == 0)
            {
                UIColors.SetNormalBgColor(axMskSsn);
                return;
            }

            PopulateSearchArgs();

            Gender gender = null;
            if (cmbGender.SelectedItem != null && !cmbGender.Text.Equals(String.Empty)) //cmbGender.SelectedItem != ""
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            SocialSecurityNumber ssn = new SocialSecurityNumber(i_Ssn);

            //Create PatientSearchCriteria and pass in all args.
            PatientSearchCriteria patientCriteria = new PatientSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbFirstName.Text,
                this.mtbLastName.Text,
                i_Ssn,
                gender,
                i_MonthOfBirth,
                i_DayOfBirth,
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text,
                i_PhoneNumber
                );

            //Check to see if search data entered is valid.
            ValidationResult result = patientCriteria.Validate();
            if (!result.IsValid && !ssn.IsComplete)
            {
                string errorMsg = PatientSearchCriteria.ERR_MSG_PARTIAL_SSN;
                UIColors.SetErrorBgColor(axMskSsn);
                MessageBox.Show(errorMsg, UIErrorMessages.ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                i_ValidSsn = false;
                axMskSsn.Focus();
                // this.axMskSsn.ResetText();
            }
            else
            {
                UIColors.SetNormalBgColor(axMskSsn);
            }
            CheckForDataEntered();
            //UIColors.SetNormalBgColor( axMskSsn );
        }

        private void ValidateDobEntry()
        {
            DateTime currentDate = DateTime.Today;

            int currentYear = currentDate.Year;

            string dobMonth = mtbDobM.Text; 
            string dobDay = mtbDobD.Text;
            string dobYear = mtbDobY.Text;
            var DOBLength = dobDay.Length + dobMonth.Length + dobYear.Length;
            string createDate;

            i_ValidDobEntry = false;

            if (dobYear.Length > 0 && dobYear.Length < 4)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG,
                    UIErrorMessages.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            else if (DOBLength > 0 && DOBLength < 8)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_INCOMPLETE_ERRMSG, UIErrorMessages.ERROR,
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }


            if ((dobYear != String.Empty && Convert.ToInt16(dobYear) > currentYear) || (dobYear != string.Empty &&
                Convert.ToInt16(dobYear) == currentYear && dobMonth != String.Empty &&
                Convert.ToInt16(dobMonth) > currentDate.Month))
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG,
                    UIErrorMessages.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }

            if (dobMonth != String.Empty && dobDay != String.Empty && dobYear != String.Empty)
            {
                createDate = dobMonth + "/" + dobDay + "/" + dobYear;
                try
                {
                    DateTime dateOfBirth = Convert.ToDateTime(createDate);

                    if (dateOfBirth > DateTime.Today)
                    {
                        SetDobErrBgColor();
                        MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, UIErrorMessages.ERROR,
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                    }
                    else if (DateValidator.IsValidDate(dateOfBirth) == false)
                    {   // Invalid leap year will trigger this
                        SetDobErrBgColor();
                        MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, UIErrorMessages.ERROR,
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                    }
                }
                catch
                {   // DateTime throws ArgumentOutOfRange exception when there's an
                    // invalid year, month, or day.
                    SetDobErrBgColor();
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, UIErrorMessages.ERROR,
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            SetPreferredColor();
            i_ValidDobEntry = true;
        }

        private void PopulateSearchArgs()
        {
            i_Ssn = axMskSsn.UnMaskedText;
            i_PhoneNumber = phoneNumberControl.Model;
            if (mtbDobY.TextLength == 0)
            {
                i_YearOfBirth = PatientSearchCriteria.NO_YEAR;
            }
            else
            {
                i_YearOfBirth = Convert.ToInt64(mtbDobY.Text);
            }

            if (mtbDobM.TextLength == 0)
            {
                i_MonthOfBirth = PatientSearchCriteria.NO_MONTH;
            }
            else
            {
                i_MonthOfBirth = Convert.ToInt64(mtbDobM.Text);
            }
            if (mtbDobD.TextLength == 0)
            {
                i_DayOfBirth = PatientSearchCriteria.NO_DAY;
            }
            else
            {
                i_DayOfBirth = Convert.ToInt64(mtbDobD.Text);
            }
        }

        protected void ValidateSearchArgs()
        {
            Gender gender = null;

            string selected = cmbGender.SelectedItem as String;
            if (selected == null)
            {
                selected = String.Empty;
            }

            if (cmbGender.SelectedItem != null && selected != String.Empty)
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            //Create PatientSearchCriteria and pass in all args.
            PatientSearchCriteria patientCriteria = new PatientSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbFirstName.Text,
                this.mtbLastName.Text,
                i_Ssn,
                gender,
                i_MonthOfBirth,
                i_DayOfBirth,
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text,
                this.phoneNumberControl.Model
                );

            //Check to see if search data entered is valid.
            ValidationResult result = patientCriteria.Validate();
            if (result.IsValid)
            {
                i_ValidSearchArgs = true;
            }
            else
            {
                i_ValidSearchArgs = false;
            }
        }

        private void SetDobErrBgColor()
        {
            mtbDobM.BackColor = Color.FromArgb(255, 36, 36);
            mtbDobD.BackColor = Color.FromArgb(255, 36, 36);
            mtbDobY.BackColor = Color.FromArgb(255, 36, 36);
        }

        private void SetDobNormalBgColor()
        {
            UIColors.SetNormalBgColor(mtbDobM);
            UIColors.SetNormalBgColor(mtbDobD);
            UIColors.SetNormalBgColor(mtbDobY);
            this.Refresh();
        }

        private void InitializeGendersComboBox()
        {
            if (base.IsInRuntimeMode)
            {
                if (cmbGender.Items.Count == 0)
                {
                    //IGenderBroker broker = BrokerFactory.BrokerOfType<IGenderBroker >();
                    DemographicsBrokerProxy broker = new DemographicsBrokerProxy();

                    ICollection genders = broker.AllTypesOfGenders(User.GetCurrent().Facility.Oid);

                    cmbGender.ValueMember = "Key";
                    cmbGender.DisplayMember = "Value"; 

                    foreach (Gender gender in genders)
                    {
                        cmbGender.Items.Add(gender.AsDictionaryEntry());
                    }
                }
            }
        }

        private void DisplaySearchView()
        {
            if (this.ParentForm != null
                && this.ParentForm.GetType() == typeof(HistoricalPatientSearch))
            {
                ((HistoricalPatientSearch)this.ParentForm).HidePanel();
            }
            else
            {
                if (this.Parent != null)
                {
                    ((SearchView)this.Parent.Parent.Parent.Parent.Parent).HidePanel();
                }
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );
        }

        private void SetPreferredColor()
        {
            SetPhoneNumberPreferredColor();
            SetPreferredColor(mtbLastName);
            SetPreferredColor(mtbFirstName);
            SetPreferredColor(axMskSsn);
            SetPreferredColor(mtbDobY);
            SetPreferredColor(mtbDobM);
            SetPreferredColor(mtbDobD);
        }
            
        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize FindAPatientView, call FindAPatientView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindAPatientView));
            this.lblMrn = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSsn = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSearch = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.mtbMrn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbAccount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.mtbDobM = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobD = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobY = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSlash1 = new System.Windows.Forms.Label();
            this.lblSlash2 = new System.Windows.Forms.Label();
            this.axMskSsn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblNewbornSearch = new System.Windows.Forms.Label();
            this.lblOr1 = new System.Windows.Forms.Label();
            this.lblOr2 = new System.Windows.Forms.Label();
            this.lblOr3 = new System.Windows.Forms.Label();
            this.lblAnd1 = new System.Windows.Forms.Label();
            this.lblOr4 = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblFulllastName = new System.Windows.Forms.Label();
            this.lblFullFirstName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMrn
            // 
            this.lblMrn.Location = new System.Drawing.Point(27, 17);
            this.lblMrn.Name = "lblMrn";
            this.lblMrn.Size = new System.Drawing.Size(40, 16);
            this.lblMrn.TabIndex = 0;
            this.lblMrn.Text = "MRN:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(27, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account:";
            // 
            // lblSsn
            // 
            this.lblSsn.Location = new System.Drawing.Point(27, 74);
            this.lblSsn.Name = "lblSsn";
            this.lblSsn.Size = new System.Drawing.Size(32, 16);
            this.lblSsn.TabIndex = 0;
            this.lblSsn.Text = "SSN:";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point(27, 113);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(60, 14);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last name:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(282, 113);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(60, 14);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First name:";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.Location = new System.Drawing.Point(371, 17);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(84, 21);
            this.cmbGender.TabIndex = 6;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(323, 17);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(46, 16);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.Location = new System.Drawing.Point(527, 115);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(71, 16);
            this.lblDateOfBirth.TabIndex = 0;
            this.lblDateOfBirth.Text = "Date of birth:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(606, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 19);
            this.label6.TabIndex = 0;
            this.label6.Text = "mm/dd/yyyy";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(801, 145);
            this.btnSearch.Message = null;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 15;
            this.btnSearch.Text = "Sear&ch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(883, 145);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "Rese&t";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // mtbMrn
            // 
            this.mtbMrn.KeyPressExpression = "^\\d*$";
            this.mtbMrn.Location = new System.Drawing.Point(96, 17);
            this.mtbMrn.Mask = "";
            this.mtbMrn.MaxLength = 9;
            this.mtbMrn.Name = "mtbMrn";
            this.mtbMrn.Size = new System.Drawing.Size(62, 20);
            this.mtbMrn.TabIndex = 1;
            this.mtbMrn.ValidationExpression = "^\\d*$";
            this.mtbMrn.TextChanged += new System.EventHandler(this.mtbMrn_TextChanged);
            // 
            // mtbAccount
            // 
            this.mtbAccount.KeyPressExpression = "^\\d*$";
            this.mtbAccount.Location = new System.Drawing.Point(96, 44);
            this.mtbAccount.Mask = "";
            this.mtbAccount.MaxLength = 9;
            this.mtbAccount.Name = "mtbAccount";
            this.mtbAccount.Size = new System.Drawing.Size(62, 20);
            this.mtbAccount.TabIndex = 2;
            this.mtbAccount.ValidationExpression = "^\\d*$";
            this.mtbAccount.TextChanged += new System.EventHandler(this.mtbAccount_TextChanged);
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.Location = new System.Drawing.Point(96, 111);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(157, 20);
            this.mtbLastName.TabIndex = 4;
            this.mtbLastName.TextChanged += new System.EventHandler(this.mtbLastName_TextChanged);
            this.mtbLastName.Enter += new System.EventHandler(this.mtbLastName_Enter);
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbLastName_Validating);
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.Location = new System.Drawing.Point(345, 111);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 15;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(97, 20);
            this.mtbFirstName.TabIndex = 5;
            this.mtbFirstName.TextChanged += new System.EventHandler(this.mtbFirstName_TextChanged);
            this.mtbFirstName.Enter += new System.EventHandler(this.mtbFirstName_Enter);
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbFirstName_Validating);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // mtbDobM
            // 
            this.mtbDobM.KeyPressExpression = "^0[0-9]*|1[0-2]*|[0-9]$";
            this.mtbDobM.Location = new System.Drawing.Point(602, 111);
            this.mtbDobM.Mask = "";
            this.mtbDobM.MaxLength = 2;
            this.mtbDobM.Name = "mtbDobM";
            this.mtbDobM.Size = new System.Drawing.Size(18, 20);
            this.mtbDobM.TabIndex = 7;
            this.mtbDobM.ValidationExpression = "^0[0-9]|1[0-2]|[0-9]$";
            this.mtbDobM.TextChanged += new System.EventHandler(this.mtbDobM_TextChanged);
            this.mtbDobM.Enter += new System.EventHandler(this.mtbDobM_Enter);
            this.mtbDobM.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobM_Validating);
            // 
            // mtbDobD
            // 
            this.mtbDobD.KeyPressExpression = "^[0-2][0-9]*|3[0-1]*|[0-9]$";
            this.mtbDobD.Location = new System.Drawing.Point(632, 111);
            this.mtbDobD.Mask = "";
            this.mtbDobD.MaxLength = 2;
            this.mtbDobD.Name = "mtbDobD";
            this.mtbDobD.Size = new System.Drawing.Size(18, 20);
            this.mtbDobD.TabIndex = 8;
            this.mtbDobD.ValidationExpression = "^[0-2][0-9]|3[0-1]|[0-9]$";
            this.mtbDobD.TextChanged += new System.EventHandler(this.mtbDobD_TextChanged);
            this.mtbDobD.Enter += new System.EventHandler(this.mtbDobD_Enter);
            this.mtbDobD.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobD_Validating);
            // 
            // mtbDobY
            // 
            this.mtbDobY.KeyPressExpression = "^\\d*$";
            this.mtbDobY.Location = new System.Drawing.Point(663, 111);
            this.mtbDobY.Mask = "";
            this.mtbDobY.MaxLength = 4;
            this.mtbDobY.Name = "mtbDobY";
            this.mtbDobY.Size = new System.Drawing.Size(30, 20);
            this.mtbDobY.TabIndex = 9;
            this.mtbDobY.ValidationExpression = "^\\d*$";
            this.mtbDobY.TextChanged += new System.EventHandler(this.mtbDobY_TextChanged);
            this.mtbDobY.Enter += new System.EventHandler(this.mtbDobY_Enter);
            this.mtbDobY.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobY_Validating);
            // 
            // lblSlash1
            // 
            this.lblSlash1.Location = new System.Drawing.Point(624, 111);
            this.lblSlash1.Name = "lblSlash1";
            this.lblSlash1.Size = new System.Drawing.Size(7, 18);
            this.lblSlash1.TabIndex = 0;
            this.lblSlash1.Text = "/";
            // 
            // lblSlash2
            // 
            this.lblSlash2.Location = new System.Drawing.Point(655, 111);
            this.lblSlash2.Name = "lblSlash2";
            this.lblSlash2.Size = new System.Drawing.Size(6, 17);
            this.lblSlash2.TabIndex = 0;
            this.lblSlash2.Text = "/";
            // 
            // axMskSsn
            // 
            this.axMskSsn.KeyPressExpression = "^\\d*$";
            this.axMskSsn.Location = new System.Drawing.Point(96, 74);
            this.axMskSsn.Mask = "   -  -";
            this.axMskSsn.MaxLength = 11;
            this.axMskSsn.Name = "axMskSsn";
            this.axMskSsn.Size = new System.Drawing.Size(75, 20);
            this.axMskSsn.TabIndex = 3;
            this.axMskSsn.ValidationExpression = "^\\d*$";
            this.axMskSsn.TextChanged += new System.EventHandler(this.axMskSsn_Change);
            this.axMskSsn.Validating += new System.ComponentModel.CancelEventHandler(this.axMskSsn_Validating);
            // 
            // lblNewbornSearch
            // 
            this.lblNewbornSearch.Location = new System.Drawing.Point(552, 17);
            this.lblNewbornSearch.Name = "lblNewbornSearch";
            this.lblNewbornSearch.Size = new System.Drawing.Size(418, 16);
            this.lblNewbornSearch.TabIndex = 15;
            this.lblNewbornSearch.Text = "To register a Newborn, perform search using the birth mother\'s registered inform" +
    "ation.";
            // 
            // lblOr1
            // 
            this.lblOr1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr1.Location = new System.Drawing.Point(180, 17);
            this.lblOr1.Name = "lblOr1";
            this.lblOr1.Size = new System.Drawing.Size(40, 16);
            this.lblOr1.TabIndex = 19;
            this.lblOr1.Text = "OR";
            // 
            // lblOr2
            // 
            this.lblOr2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr2.Location = new System.Drawing.Point(180, 47);
            this.lblOr2.Name = "lblOr2";
            this.lblOr2.Size = new System.Drawing.Size(40, 16);
            this.lblOr2.TabIndex = 20;
            this.lblOr2.Text = "OR";
            // 
            // lblOr3
            // 
            this.lblOr3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr3.Location = new System.Drawing.Point(180, 74);
            this.lblOr3.Name = "lblOr3";
            this.lblOr3.Size = new System.Drawing.Size(40, 16);
            this.lblOr3.TabIndex = 21;
            this.lblOr3.Text = "OR";
            // 
            // lblAnd1
            // 
            this.lblAnd1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnd1.Location = new System.Drawing.Point(473, 115);
            this.lblAnd1.Name = "lblAnd1";
            this.lblAnd1.Size = new System.Drawing.Size(40, 16);
            this.lblAnd1.TabIndex = 22;
            this.lblAnd1.Text = "AND";
            // 
            // lblOr4
            // 
            this.lblOr4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr4.Location = new System.Drawing.Point(712, 115);
            this.lblOr4.Name = "lblOr4";
            this.lblOr4.Size = new System.Drawing.Size(40, 16);
            this.lblOr4.TabIndex = 23;
            this.lblOr4.Text = "OR";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point(96, 137);
            this.phoneNumberControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.phoneNumberControl.Model = ((PatientAccess.Domain.Parties.PhoneNumber)(resources.GetObject("phoneNumberControl.Model")));
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size(94, 27);
            this.phoneNumberControl.TabIndex = 10;
            this.phoneNumberControl.AreaCodeChanged += new System.EventHandler(this.phoneNumberControl_AreaCodeChanged);
            this.phoneNumberControl.PhoneNumberChanged += new System.EventHandler(this.phoneNumberControl_PhoneNumberChanged);
            this.phoneNumberControl.Leave += new System.EventHandler(this.PhoneNumberControl_Leave);
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point(28, 144);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(40, 16);
            this.lblPhone.TabIndex = 24;
            this.lblPhone.Text = "Phone:";
            // 
            // lblFulllastName
            // 
            this.lblFulllastName.Location = new System.Drawing.Point(27, 100);
            this.lblFulllastName.Name = "lblFulllastName";
            this.lblFulllastName.Size = new System.Drawing.Size(30, 12);
            this.lblFulllastName.TabIndex = 26;
            this.lblFulllastName.Text = "Full";
            // 
            // lblFullFirstName
            // 
            this.lblFullFirstName.Location = new System.Drawing.Point(282, 100);
            this.lblFullFirstName.Name = "lblFullFirstName";
            this.lblFullFirstName.Size = new System.Drawing.Size(30, 12);
            this.lblFullFirstName.TabIndex = 27;
            this.lblFullFirstName.Text = "Full";
            // 
            // FindAPatientView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblFullFirstName);
            this.Controls.Add(this.lblFulllastName);
            this.Controls.Add(this.phoneNumberControl);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.lblOr4);
            this.Controls.Add(this.lblAnd1);
            this.Controls.Add(this.lblOr3);
            this.Controls.Add(this.lblOr2);
            this.Controls.Add(this.lblOr1);
            this.Controls.Add(this.lblNewbornSearch);
            this.Controls.Add(this.axMskSsn);
            this.Controls.Add(this.lblSlash2);
            this.Controls.Add(this.lblSlash1);
            this.Controls.Add(this.mtbDobY);
            this.Controls.Add(this.mtbDobD);
            this.Controls.Add(this.mtbDobM);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.mtbFirstName);
            this.Controls.Add(this.mtbLastName);
            this.Controls.Add(this.mtbAccount);
            this.Controls.Add(this.mtbMrn);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblDateOfBirth);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblSsn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMrn);
            this.Controls.Add(this.label6);
            this.Name = "FindAPatientView";
            this.Size = new System.Drawing.Size(990, 176);
            this.Load += new System.EventHandler(this.FindAPatientView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Private Properties
        private bool FirstTimeLayout
        {
            get
            {
                return i_FirstTimeLayout;
            }
            set
            {
                i_FirstTimeLayout = value;
            }
        }
        private IEMPIFeatureManager EMPIFeatureManager { get; set; }
       

        public bool HasAccountAndDateOfBirthOnly
        {
            get
            {
                return !String.IsNullOrEmpty(patientCriteria.AccountNumber) &&
                       patientCriteria.DateOfBirth != DateTime.MinValue &&
                       !HasPhoneNumber && !HasSocialSecurityNumber &&
                       !HasFullName;
            }
        }

        public bool HasMRNAndDateOfBirthOnly
        {
            get
            {
                return !String.IsNullOrEmpty(patientCriteria.MedicalRecordNumber) &&
                       patientCriteria.DateOfBirth != DateTime.MinValue &&
                       !HasPhoneNumber && !HasSocialSecurityNumber &&
                       !HasFullName;
            }
        }

        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construction for FindAPatientView, 
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public FindAPatientView()
        {
            InitializeComponent();

            ConfigureControls();

            this.btnSearch.Enabled = false;
            this.btnSearch.Message = "search for patient";
        }
        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // cancel the background worker here...
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.CancelAsync();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Data Elements
        private IContainer components = null;

        private PatientSearchCriteria patientCriteria;
        private PatientSearchCriteria i_CriteriaForCurrentSearch;
        private Activity i_CurrentActivity;
        private BackgroundWorker backgroundWorker;

        private ClickOnceLoggingButton btnSearch;
        private LoggingButton btnReset;
        private ComboBox cmbGender;
        private Label lblMrn;
        private Label label1;
        private Label lblSsn;
        private Label lblLastName;
        private Label lblFirstName;
        private Label lblGender;
        private Label lblDateOfBirth;
        private Label label6;
        private Label lblSlash1;
        private Label lblSlash2;

        private ErrorProvider errorProvider1;

        private MaskedEditTextBox mtbMrn;
        private MaskedEditTextBox mtbAccount;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbDobM;
        private MaskedEditTextBox mtbDobD;
        private MaskedEditTextBox mtbDobY;
        private MaskedEditTextBox axMskSsn;

        private bool i_ValidSearchArgs;
        private long i_YearOfBirth;
        private long i_MonthOfBirth;
        private long i_DayOfBirth;
        private bool i_FirstTimeLayout = true;
        private bool i_Reset = false;
        private bool i_ValidDobEntry = true;
        private bool i_ValidSsn = true;
        private List<PatientSearchResult> collectionOfPatients = null;
        private PatientSearchResponse patientSearchResponse = null;
        private Label lblNewbornSearch;
        private Label lblAnd1;
        private Label lblOr3;
        private Label lblOr2;
        private Label lblOr1;
        private Label lblOr4;
        private PhoneNumberControl phoneNumberControl;
        private Label lblPhone;
        private Label lblFullFirstName;
        private Label lblFulllastName;

        private string i_Ssn;
        private PhoneNumber i_PhoneNumber; 
        #endregion

        private void phoneNumberControl_AreaCodeChanged(object sender, EventArgs e)
        {
            if (phoneNumberControl.AreaCode != String.Empty && Convert.ToInt16(phoneNumberControl.AreaCode) == 0)
            {
                phoneNumberControl.AreaCode = String.Empty; 
            }
            ValidatePhoneNumber();
        }

        private void phoneNumberControl_PhoneNumberChanged(object sender, EventArgs e)
        {
            if (phoneNumberControl.PhoneNumber != String.Empty && Convert.ToInt32(phoneNumberControl.PhoneNumber) == 0)
            {
                phoneNumberControl.PhoneNumber = String.Empty;
            }
            ValidatePhoneNumber();
        }

        private void ValidatePhoneNumber()
        {
            SetPhoneNumberPreferredColor();
            CheckForDataEntered();
        }

        private void PhoneNumberControl_Leave(object sender, EventArgs e)
        {
            if (phoneNumberControl.PhoneNumber != String.Empty && Convert.ToInt32(phoneNumberControl.PhoneNumber) == 0)
            {
                phoneNumberControl.PhoneNumber = String.Empty;
            }
            if (phoneNumberControl.PhoneNumber.Length > 0 && phoneNumberControl.PhoneNumber.Length != 7)
            {
                phoneNumberControl.FocusPhoneNumber();
            }
            ValidatePhoneNumber();
        }

        private bool IsPhoneNumberComplete()
        {
            if (!IsPhoneNumberEmpty)
            {
                if (!HasPhoneNumber)
                {
                    SetPhoneNumberErrorColor();
                    MessageBox.Show(UIErrorMessages.PHONE_NUMBER_INCOMPLETE, UIErrorMessages.ERROR,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    phoneNumberControl.Focus();
                    return false;
                }
            }
            return true;
        }

        private void SetPhoneNumberPreferredColor()
        {
            phoneNumberControl.SetNormalColor();
            if (phoneNumberControl.PhoneNumber.Length == 0)
            {
                phoneNumberControl.SetPhoneNumberPreferredColor();
            }
            else if (phoneNumberControl.PhoneNumber.Length != 7)
            {
                phoneNumberControl.SetNumberErrorColor();
            }
            if (phoneNumberControl.AreaCode.Length == 0)
            {
                phoneNumberControl.SetAreaCodePreferredColor(); 
            }
        }

        private void SetPhoneNumberErrorColor()
        {
            phoneNumberControl.SetPhoneNumberErrorColor();
        }

        private bool IsEMPIFeatureEnabled()
        {
               EMPIFeatureManager = new EMPIFeatureManager();
            return (EMPIFeatureManager.IsEMPIFeatureEnabledForSearch(CurrentActivity));
        }

      
    }
}
