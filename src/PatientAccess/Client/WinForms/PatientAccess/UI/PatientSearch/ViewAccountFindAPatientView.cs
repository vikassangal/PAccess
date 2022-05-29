using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.HistoricalAccountViews;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Summary description for FindAPatientView.
    /// </summary>
    [Serializable]
    public class ViewAccountFindAPatientView : ControlView
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
                ValidateDobEntry();
                if (i_ValidDobEntry)
                {
                    //Call FindMatchingPatients method to perform search.
                    FindMatchingPatients();
                }
            }
            
            this.ParentForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Reset All text entry fields and clear listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
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

            this.btnSearch.Enabled = false;
            //Reset the combobox.
            cmbGender.SelectedItem = null;

            this.SearchReset(this, null);
            this.Invalidate();
            this.Update();

            //this.SetFocus();

            i_Reset = false;

            this.mtbDobM.Refresh();
            this.mtbDobY.Refresh();
            this.mtbDobD.Refresh();
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
            if (mtbDobM.Text != String.Empty && mtbDobM.Text != "0" && mtbDobM.TextLength == 1)
            {
                mtbDobM.Text = "0" + mtbDobM.Text;
            }
            else if (mtbDobM.Text != String.Empty && Convert.ToInt16(mtbDobM.Text) == 0)
            {
                mtbDobM.Text = String.Empty;
            }

            if (mtbDobY.TextLength == 4 || mtbDobM.Text != string.Empty && !(mtbDobY.TextLength > 0 && mtbDobY.TextLength < 4))
            {
                SetDobNormalBgColor();
            }

            CheckForDataEntered();
        }

        private void mtbDobD_Validating(object sender, CancelEventArgs e)
        {
            if (mtbDobD.Text != String.Empty && mtbDobD.Text != "0" && mtbDobD.TextLength == 1)
            {
                mtbDobD.Text = "0" + mtbDobD.Text;
            }
            else if (mtbDobD.Text != String.Empty && Convert.ToInt16(mtbDobD.Text) == 0)
            {
                mtbDobD.Text = String.Empty;
            }
        }

        private void mtbDobY_Validating(object sender, CancelEventArgs e)
        {
            if (mtbDobY.TextLength > 0 && mtbDobY.TextLength < 4)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                mtbDobY.Focus();
                //SetDobNormalBgColor(); 
                return;
            }
            else
            {
                SetDobNormalBgColor();
            }

            if (!(mtbDobM.Text == String.Empty && mtbDobY.Text == String.Empty))
            {
                SetDobNormalBgColor();
            }
            else if (mtbDobM.Text == String.Empty && mtbDobD.Text == String.Empty && mtbDobY.Text == String.Empty)
            {
                SetDobNormalBgColor();
            }

            CheckForDataEntered();
        }

        private void axMskSsn_Validating(object sender, CancelEventArgs e)
        {
            ValidateSsn();
        }

        private void mtbLastName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces
            this.mtbLastName.Text = this.mtbLastName.Text.TrimEnd(null);
        }

        private void mtbFirstName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces
            this.mtbFirstName.Text = this.mtbFirstName.Text.TrimEnd(null);
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
                    && (aType == typeof (RegistrationActivity)
                        || aType == typeof (PreRegistrationActivity)
                        || aType == typeof (PreMSERegisterActivity)
                        || aType == typeof (UCCPreMSERegistrationActivity)))
                {
                    this.axMskSsn.Focus();
                }
                else
                {
                    this.mtbAccount.Focus();
                }

            }
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
            private get
            {
                return i_CurrentActivity;
            }
            set
            {
                i_CurrentActivity = value;
            }
        }

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
            }

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
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text
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
                    MessageBox.Show(errorMsg, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    SearchError(result.AspectInError);
                }
            }

            this.Cursor = Cursors.Default;

        }

        private void CheckForDataEntered()
        {
            if (this.mtbMrn.Text == String.Empty && this.mtbAccount.Text == String.Empty &&
                this.axMskSsn.UnMaskedText.Length < SocialSecurityNumber.MAX_VALUE.Length && this.mtbLastName.Text == String.Empty &&
                this.mtbFirstName.Text == String.Empty && this.cmbGender.Text.Equals(String.Empty) &&
                mtbDobM.Text == String.Empty && mtbDobY.Text == String.Empty)
            {
                this.btnSearch.Enabled = false;
            }
            else
            {
                this.btnSearch.Enabled = true;
                if (this.ParentForm != null)
                {
                    base.AcceptButton = btnSearch;
                }
            }
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
                ssn,
                gender,
                i_MonthOfBirth,
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text
                );

            //Check to see if search data entered is valid.
            ValidationResult result = patientCriteria.Validate();
            if (!result.IsValid && !ssn.IsComplete)
            {
                string errorMsg = PatientSearchCriteria.ERR_MSG_PARTIAL_SSN;
                UIColors.SetErrorBgColor(axMskSsn);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK,
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

            string dobMonth = mtbDobM.Text; ;
            string dobDay = mtbDobD.Text;
            string dobYear = mtbDobY.Text;

            string createDate;

            i_ValidDobEntry = false;

            if (dobYear.Length > 0 && dobYear.Length < 4)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            else if (dobDay != String.Empty && dobYear == String.Empty && dobMonth == String.Empty)
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_FORMAT_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }

            if ((dobYear != String.Empty && Convert.ToInt16(dobYear) > currentYear) || (dobYear != string.Empty &&
                Convert.ToInt16(dobYear) == currentYear && dobMonth != String.Empty &&
                Convert.ToInt16(dobMonth) > currentDate.Month))
            {
                SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
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
                        MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                    }
                    else if (DateValidator.IsValidDate(dateOfBirth) == false)
                    {   // Invalid leap year will trigger this
                        SetDobErrBgColor();
                        MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                    }
                }
                catch
                {   // DateTime throws ArgumentOutOfRange exception when there's an
                    // invalid year, month, or day.
                    SetDobErrBgColor();
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            SetDobNormalBgColor();
            i_ValidDobEntry = true;
        }

        private void PopulateSearchArgs()
        {
            i_Ssn = axMskSsn.UnMaskedText;

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
                i_YearOfBirth,
                this.mtbMrn.Text,
                this.mtbAccount.Text
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize FindAPatientView, call FindAPatientView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblMrn = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSsn = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSearch = new ClickOnceLoggingButton();
            this.btnReset = new LoggingButton();
            this.mtbMrn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbAccount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
            this.mtbDobM = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobD = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobY = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSlash1 = new System.Windows.Forms.Label();
            this.lblSlash2 = new System.Windows.Forms.Label();
            this.axMskSsn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.SuspendLayout();
            // 
            // lblInstructions
            // 
            this.lblInstructions.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblInstructions.Location = new System.Drawing.Point(1, 19);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(941, 28);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = @"The more information provided, the narrower the search. Partial searches are not supported for MRN, Account, or SSN. When searching by Last Name, at least one other field is required. When searching by First Name, Gender, or Date of Birth, either include all three of these, or add at least one of the following: Last Name, MRN, Account, or SSN.";
            // 
            // lblMrn
            // 
            this.lblMrn.Location = new System.Drawing.Point(8, 62);
            this.lblMrn.Name = "lblMrn";
            this.lblMrn.Size = new System.Drawing.Size(40, 16);
            this.lblMrn.TabIndex = 0;
            this.lblMrn.Text = "MRN:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(115, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account:";
            // 
            // lblSsn
            // 
            this.lblSsn.Location = new System.Drawing.Point(239, 62);
            this.lblSsn.Name = "lblSsn";
            this.lblSsn.Size = new System.Drawing.Size(32, 16);
            this.lblSsn.TabIndex = 0;
            this.lblSsn.Text = "SSN:";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point(8, 102);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(62, 16);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last name:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(238, 102);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(62, 16);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First name:";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.Location = new System.Drawing.Point(457, 99);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(84, 21);
            this.cmbGender.TabIndex = 6;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(409, 102);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(46, 16);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.Location = new System.Drawing.Point(552, 102);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(71, 16);
            this.lblDateOfBirth.TabIndex = 0;
            this.lblDateOfBirth.Text = "Date of birth:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(631, 123);
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
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Sear&ch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(883, 145);
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 11;
            this.btnReset.Text = "Rese&t";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // mtbMrn
            // 
            this.mtbMrn.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMrn.KeyPressExpression = "^\\d*$";
            this.mtbMrn.Location = new System.Drawing.Point(43, 59);
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
            this.mtbAccount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAccount.KeyPressExpression = "^\\d*$";
            this.mtbAccount.Location = new System.Drawing.Point(168, 59);
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
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point(70, 99);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(157, 20);
            this.mtbLastName.TabIndex = 4;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbLastName_Validating);
            this.mtbLastName.TextChanged += new System.EventHandler(this.mtbLastName_TextChanged);
            this.mtbLastName.Enter += new System.EventHandler(this.mtbLastName_Enter);
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point(301, 99);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 15;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(97, 20);
            this.mtbFirstName.TabIndex = 5;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbFirstName_Validating);
            this.mtbFirstName.TextChanged += new System.EventHandler(this.mtbFirstName_TextChanged);
            this.mtbFirstName.Enter += new System.EventHandler(this.mtbFirstName_Enter);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // mtbDobM
            // 
            this.mtbDobM.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDobM.KeyPressExpression = "^0[0-9]*|1[0-2]*|[0-9]$";
            this.mtbDobM.Location = new System.Drawing.Point(627, 99);
            this.mtbDobM.Mask = "";
            this.mtbDobM.MaxLength = 2;
            this.mtbDobM.Name = "mtbDobM";
            this.mtbDobM.Size = new System.Drawing.Size(18, 20);
            this.mtbDobM.TabIndex = 7;
            this.mtbDobM.ValidationExpression = "^0[0-9]|1[0-2]|[0-9]$";
            this.mtbDobM.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobM_Validating);
            this.mtbDobM.TextChanged += new System.EventHandler(this.mtbDobM_TextChanged);
            this.mtbDobM.Enter += new System.EventHandler(this.mtbDobM_Enter);
            // 
            // mtbDobD
            // 
            this.mtbDobD.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDobD.KeyPressExpression = "^[0-2][0-9]*|3[0-1]*|[0-9]$";
            this.mtbDobD.Location = new System.Drawing.Point(657, 99);
            this.mtbDobD.Mask = "";
            this.mtbDobD.MaxLength = 2;
            this.mtbDobD.Name = "mtbDobD";
            this.mtbDobD.Size = new System.Drawing.Size(18, 20);
            this.mtbDobD.TabIndex = 8;
            this.mtbDobD.ValidationExpression = "^[0-2][0-9]|3[0-1]|[0-9]$";
            this.mtbDobD.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobD_Validating);
            this.mtbDobD.TextChanged += new System.EventHandler(this.mtbDobD_TextChanged);
            this.mtbDobD.Enter += new System.EventHandler(this.mtbDobD_Enter);
            // 
            // mtbDobY
            // 
            this.mtbDobY.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDobY.KeyPressExpression = "^\\d*$";
            this.mtbDobY.Location = new System.Drawing.Point(688, 99);
            this.mtbDobY.Mask = "";
            this.mtbDobY.MaxLength = 4;
            this.mtbDobY.Name = "mtbDobY";
            this.mtbDobY.Size = new System.Drawing.Size(30, 20);
            this.mtbDobY.TabIndex = 9;
            this.mtbDobY.ValidationExpression = "^\\d*$";
            this.mtbDobY.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDobY_Validating);
            this.mtbDobY.TextChanged += new System.EventHandler(this.mtbDobY_TextChanged);
            this.mtbDobY.Enter += new System.EventHandler(this.mtbDobY_Enter);
            // 
            // lblSlash1
            // 
            this.lblSlash1.Location = new System.Drawing.Point(649, 102);
            this.lblSlash1.Name = "lblSlash1";
            this.lblSlash1.Size = new System.Drawing.Size(7, 18);
            this.lblSlash1.TabIndex = 13;
            this.lblSlash1.Text = "/";
            // 
            // lblSlash2
            // 
            this.lblSlash2.Location = new System.Drawing.Point(680, 102);
            this.lblSlash2.Name = "lblSlash2";
            this.lblSlash2.Size = new System.Drawing.Size(6, 17);
            this.lblSlash2.TabIndex = 14;
            this.lblSlash2.Text = "/";
            // 
            // axMskSsn
            // 
            this.axMskSsn.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.axMskSsn.KeyPressExpression = "^\\d*$";
            this.axMskSsn.Location = new System.Drawing.Point(271, 59);
            this.axMskSsn.Mask = "   -  -";
            this.axMskSsn.MaxLength = 11;
            this.axMskSsn.Name = "axMskSsn";
            this.axMskSsn.Size = new System.Drawing.Size(75, 20);
            this.axMskSsn.TabIndex = 3;
            this.axMskSsn.ValidationExpression = "^\\d*$";
            this.axMskSsn.Validating += new System.ComponentModel.CancelEventHandler(this.axMskSsn_Validating);
            this.axMskSsn.TextChanged += new System.EventHandler(this.axMskSsn_Change);
            // 
            // FindAPatientView
            // 
            this.BackColor = System.Drawing.Color.White;
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
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.label6);
            this.Name = "FindAPatientView";
            this.Size = new System.Drawing.Size(990, 176);
            this.Load += new System.EventHandler(this.FindAPatientView_Load);
            this.ResumeLayout(false);

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
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construction for FindAPatientView, 
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public ViewAccountFindAPatientView()
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

        private Label lblInstructions;
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
        private bool i_FirstTimeLayout = true;
        private bool i_Reset = false;
        private bool i_ValidDobEntry = true;
        private bool i_ValidSsn = true;
        private List<PatientSearchResult> collectionOfPatients = null;
        private PatientSearchResponse patientSearchResponse = null;

        private string i_Ssn;

        #endregion

        #region Constants
        #endregion
    }
}
