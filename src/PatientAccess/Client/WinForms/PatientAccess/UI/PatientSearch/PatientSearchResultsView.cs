using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;
using PatientAccess.UI.Registration;
using PatientAccess.Utilities;

namespace PatientAccess.UI.PatientSearch 
{


    [Serializable]
    public class PatientSearchResultsView : ControlView
    {
        #region Events
        public event EventHandler<PatientSelectedEventArgs> PatientSelectedFromSearchResult;
        #endregion

        #region Event Handlers
        //HACK: Button Hidden under grid that is set up to be an ACCEPT button.  Fixed problem of re-entering control not getting ENTER key event
        private void button2_Click( object sender, EventArgs e )
        {
            GetSelectedPatientFromPatientSearchResult();
        }

        private void CreateNewPatientButtonClick(object sender, EventArgs e)
        {
            this.btnCreateNewPatient.Enabled = false;
            CreateAndLoadNewPatient();
        }

        private void SearchEMPIButtonClick(object sender, EventArgs e)
        {
          ParentSearchView.FindAPatientView.SearchEMPI();
          if (ParentSearchView.FindAPatientView.ResestSearchView )
          {
              ParentSearchView.FindAPatientView.ResestSearchCriteria();
              return;
          }
          btnSearchEMPI.Enabled = false;
        }
       
        private void foundPatientsView_DoubleClick( object sender, EventArgs e )
        {
            GetSelectedPatientFromPatientSearchResult();
        }

        private void foundPatientsView_KeyDown( object sender, KeyEventArgs e )
        {
            if( e.KeyCode == Keys.Enter )
            {
                GetSelectedPatientFromPatientSearchResult();
            }
        }

        private void PatientSearchResultsView_Load(object sender, EventArgs e)
        {
            this.label2.Visible                 = false;
            this.label_NoPatientsFound.Visible  = false;

            btnCreateNewPatient.Visible         = true;

            if( this.i_SearchByType == HISTORICAL_PATIENT_SEARCH )
            {
                this.searchResultsOptionsPanel.Size = new Size(984, 57);
                this.label_NoPatientsFound.Location = new Point(8,10);
                this.searchResultpanel.Size = new Size(964, 250);
                this.searchResultpanel.Location = new Point(0,50);
                this.btnCreateNewPatient.Visible = false;
                this.chAddress.Width = 280;
            }
            EMPIFeatureManager = new EMPIFeatureManager();
            EMPIFeatureEnabled = EMPIFeatureManager.IsEMPIFeatureEnabledForSearch(CurrentActivity);
            EMPIFeatureEnabledForFacility = EMPIFeatureManager.IsEMPIFeatureEnabledForFacility();
            this.label1.Text = EMPIFeatureEnabled ? SEARCH_TIP_MSG : NON_EMPI_SEARCH_TIP_MESSAGE;
            // Only certain activities can display the CreateNewPatient button,
            // if this is not one of them, do not show it.
            if( this.CurrentActivity != null && this.CurrentActivity.CanCreateNewPatient() )
            {
                 this.btnCreateNewPatient.Visible = true;
            }
            else if ( this.CurrentActivity != null && ( this.CurrentActivity.GetType() == typeof( AdmitNewbornActivity ) 
                            || this.CurrentActivity.GetType() == typeof( PreAdmitNewbornActivity ) ) )
            {
                this.label1.Text = MOTHER_NOT_FOUND_MSG;
                this.btnCreateNewPatient.Visible = false;
            }
            else
            {
                this.btnCreateNewPatient.Visible = false;
            }
        }
        #endregion

        #region Methods


        /// <summary>
        /// Gets the selected patient from patient search result.
        /// </summary>
        public void GetSelectedPatientFromPatientSearchResult()
        {

            if( IsPatientSelected() )
            {

                // Copy to a temporary variable to be thread-safe.
                EventHandler<PatientSelectedEventArgs> patientSelectedHandler = 
                    PatientSelectedFromSearchResult;

                PatientSearchResult searchResult = 
                    foundPatientsView.SelectedItems[0].Tag as PatientSearchResult;

                IPatientBroker patientPBARBroker = 
                    BrokerFactory.BrokerOfType<IPatientBroker>();

                this.Cursor = Cursors.WaitCursor;
                Patient patient = new Patient();
                if (EMPIResults)
                {
                    patient = GetSelectedPatientToCreateEMPIPatientFromSearchResult();
                    var empiServiceBroker = BrokerFactory.BrokerOfType<IEMPIBroker>();
                    var patientFacilityCode = empiServiceBroker.GetPBARFacilityCode(patient.Facility.Code);
                    if (User.GetCurrent().Facility.Code == patientFacilityCode)
                    {
                        patient.Facility = User.GetCurrent().Facility;
                        IAccountBroker acctBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                        var accounts = acctBroker.AccountsFor(patient);
                        patient.AddAccounts(accounts);
                    }
                }
                else
                {
                     patient = patientPBARBroker.PatientFrom(searchResult);
                }

                this.Cursor = Cursors.Default;
                var aPatient = patient;
                if (EMPIFeatureManager.IsEMPIFeatureEnabled(CurrentActivity))
                {
                    aPatient = GetEMPIPatient(patient);
                }
                patient.SetPatientContextHeaderData();

                BreadCrumbLogger.GetInstance.Log( String.Format( "{0} Patient selected from the search results", 
                    patient.Name.AsFormattedName() ) );

                string searchAccountNumber = string.Empty;

                if( this.ParentSearchView != null &&
                    this.ParentSearchView.FindAPatientView != null &&
                    this.ParentSearchView.FindAPatientView.CriteriaForCurrentSearch != null )
                {
                    searchAccountNumber = 
                        this.ParentSearchView.FindAPatientView.CriteriaForCurrentSearch.AccountNumber;
                }

                if( patientSelectedHandler != null )
                {
                    if (searchAccountNumber.Length != 0)
                    {
                        //Raise PatientSelected Event with Patient and AccountNumber.
                        patientSelectedHandler(
                            this, new PatientSelectedEventArgs( patient, searchAccountNumber ) );
                    }//if
                    else
                    {
                        //Raise PatientSelected Event with only Patient.
                        patientSelectedHandler(
                            this, new PatientSelectedEventArgs( patient ) );
                    }//else
                }

            }
        }

        private Patient GetEMPIPatient(Patient patient)
        {
            Guard.ThrowIfArgumentIsNull(patient, "Patient");
            var aPatient = patient;
            PatientSearchCriteria searchCriteria = null;
            if(    i_SearchByType == "FindAPatientView"  )
            {
               searchCriteria =  ParentSearchView.FindAPatientView.CriteriaForCurrentSearch;
                
            }
            if (searchCriteria != null)
            {
                searchCriteria.UpdateEMPIPatientDetails(aPatient,
                    CurrentActivity);

                aPatient = CurrentActivity.EmpiPatient.Patient;
                if (aPatient.HasAliases())
                {
                    var aka = ((Name) aPatient.Aliases[0]);
                    foreach (Name name in patient.Aliases)
                    {
                        patient.RemoveAlias(name);
                    }
                    patient.AddAlias(aka);
                }
                patient.FirstName = aPatient.FirstName;
                patient.LastName = aPatient.LastName;
                patient.MiddleInitial = aPatient.MiddleInitial;
                patient.DateOfBirth = aPatient.DateOfBirth;
                patient.Sex = aPatient.Sex;
                patient.SocialSecurityNumber = aPatient.SocialSecurityNumber;
                patient.MedicalRecordNumber = aPatient.MedicalRecordNumber;
            }
            return patient;
        }

        public void SetListViewDefaultItemSelected()
        {
            foreach (ListViewItem item in foundPatientsView.Items)
            {
                item.Selected = false;
            }
            if (this.foundPatientsView.Items.Count > 0)
            {
                this.foundPatientsView.Items[0].Selected = true;
            }
        }

        public void FocusOnGrid()
        {
            if( this.foundPatientsView.Items.Count > 0 )
            {
                this.foundPatientsView.Focus();
            }
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            
        }
       
        /// <summary>
        /// Update patient listview with data.
        /// </summary>
        public override void UpdateView()
        {
            EMPIFeatureManager = new EMPIFeatureManager();
            EMPIFeatureEnabledForFacility = EMPIFeatureManager.IsEMPIFeatureEnabledForFacility();
            if (this.PatientSearchResults.Count == 0)
            {
                DoNotShowPatientSearchResults();
            }
            else
            {
                ShowPatientSearchResults();
            }
        }

        private void ShowPatientSearchResults()
        {
            switch (i_SearchByType)
            {
                case "FindAPatientView":
                    DisplaySearchEMPIButton();
                    if (!EMPIResults)
                    {
                        PatientSearchResults.Sort();
                    }
                    foundPatientsView.Visible = true;
                    label_NoPatientsFound.Visible = false;
                    if (EMPIResults)
                    {
                        CreateEMPIPatientSearchResults();
                        btnCreateNewPatient.Enabled = true;
                        btnSearchEMPI.Enabled = false;
                    }
                    else
                    {
                        CreatePBARPatientSearchResults();
                        if (ParentSearchView != null)
                        {
                            btnSearchEMPI.Enabled = EMPIFeatureEnabled &&
                                                    !ParentSearchView.FindAPatientView.IsPatientSearchWithMRN &&
                                                    !ParentSearchView.FindAPatientView.HasAccountAndDateOfBirthOnly;
                            btnCreateNewPatient.Enabled = !EMPIFeatureEnabled ||
                                                          ParentSearchView.FindAPatientView.IsPatientSearchWithMRN ||
                                                          ParentSearchView.FindAPatientView.HasAccountAndDateOfBirthOnly ||
                                                          SearchedEMPI;
                        }
                    }
                    this.PopulatePatientSearchResultGrid();
                    if (!EMPIResults)
                    {
                      DefaultSort();
                    }
                    break;
                case "FindPatientByGuarantorView":
                     DoNotShowSearchEMPIButton();
                    PatientSearchResults.Sort();
                    btnCreateNewPatient.Enabled = true;
                    foundPatientsView.Visible = true;
                    label_NoPatientsFound.Visible = false;
                    CreatePBARPatientSearchResults();
                    this.PopulatePatientSearchResultGrid();
                    DefaultSort();
                    break;

                default:
                    DoNotShowSearchEMPIButton();
                    PatientSearchResults.Sort();
                    foundPatientsView.Visible = true;
                    label_NoPatientsFound.Visible = false;
                    CreatePBARPatientSearchResults();
                    this.PopulatePatientSearchResultGrid();
                    DefaultSort();
                    break;
            }
        }

        private void DisplaySearchEMPIButton()
        {

            if (EMPIFeatureEnabledForFacility && SearchByPatient)
            {
                ShowSearchEMPIButton();
            }
            else
            {
                DoNotShowSearchEMPIButton();
            }
        }

        private bool SearchByPatient
        {
            get { return i_SearchByType == "FindAPatientView"; }

        }

        private void DoNotShowPatientSearchResults()
        {
            foundPatientsView.Visible = false;
            label_NoPatientsFound.Visible = true;
            this.searchResultpanel.BorderStyle = BorderStyle.FixedSingle;
            switch (i_SearchByType)
            {
                case "FindAPatientView":
                    DisplaySearchEMPIButton();
                    label_NoPatientsFound.Text = PATIENT_SEARCH_ERRMSG;
                    break;
                case HISTORICAL_PATIENT_SEARCH:
                    DoNotShowSearchEMPIButton();
                    label_NoPatientsFound.Text = PATIENT_SEARCH_ERRMSG;
                    break;
                case "FindPatientByGuarantorView":
                    DoNotShowSearchEMPIButton();
                    label_NoPatientsFound.Text = GUARANTOR_SEARCH_ERRMSG;
                    break;
                default:
                    DoNotShowSearchEMPIButton();
                    label_NoPatientsFound.Text = string.Empty;
                    break;
            }
            CreatePBARPatientSearchResults();
            if (ParentSearchView != null)
            {
                if (EMPIFeatureEnabled && SearchByPatient)
                {
                    btnSearchEMPI.Enabled = !ParentSearchView.FindAPatientView.IsPatientSearchWithOnlyMRN &&
                                            !ParentSearchView.FindAPatientView.HasMRNAndDateOfBirthOnly &&
                                            !SearchedEMPI &&
                                            !ParentSearchView.FindAPatientView.HasAccountAndDateOfBirthOnly;

                    btnCreateNewPatient.Enabled = !btnSearchEMPI.Enabled;
                }
                else
                {
                    btnSearchEMPI.Enabled = false;
                    btnCreateNewPatient.Enabled = true;
                }
            }
        }

        private bool SearchedEMPI
        {
            get { return ParentSearchView.FindAPatientView.EMPISearched; }
        }

        private void CreatePBARPatientSearchResults()
        {
            this.foundPatientsView.Columns.Clear();
            this.foundPatientsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                {
                    this.chBmp,
                    this.chPatientName,
                    this.chGender,
                    this.chDob,
                    this.chSsn,
                    this.chMrn,
                    this.chAddress
                });
            
            this.chMrn.Text = "MRN";
        }

        private void DoNotShowSearchEMPIButton()
        {
            btnCreateNewPatient.Location = new Point(0, 48);
            btnSearchEMPI.Location = new Point(139, 48);
            btnSearchEMPI.Visible = false;
            btnSearchEMPI.Enabled = false;
            btnCreateNewPatient.Focus(); 
        }

        private void CreateEMPIPatientSearchResults()
        { 
            if (EnableTestMode)
            {
                CreateEMPITestModePatientSearchResultsGrid();
            }
            else
            {
                CreateEMPIPatientSearchResultsGrid();
            }
            this.chPatientName.Width = 295;
        }

        private void ShowSearchEMPIButton()
        {
            btnSearchEMPI.Visible = true;
            btnSearchEMPI.Enabled = false;
            btnSearchEMPI.Location = new Point(0, 48);
            btnCreateNewPatient.Location = new Point(150, 48);
        }

        private void CreateEMPIPatientSearchResultsGrid()
        {
            this.foundPatientsView.Columns.Clear();
            this.foundPatientsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                {
                    this.chBmp,
                    this.chPatientName,
                    this.chGender,
                    this.chDob,
                    this.chSsn,
                    this.chMrn,
                     this.chHSPCode,
                    this.chAddress
                });
        }
        private void CreateEMPITestModePatientSearchResultsGrid()
        {
            this.foundPatientsView.Columns.Clear();
            this.foundPatientsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                {
                    this.chBmp,
                    this.chEMPIScore,
                    this.chPatientName,
                    this.chGender,
                    this.chDob,
                    this.chSsn,
                    this.chMrn,
                    chHSPCode,
                    this.chAddress
                });
            this.chEMPIScore.Text = "EMPI SCORE";
            this.chEMPIScore.Width = 75;
        }

        public Patient GetSelectedPatientToCreateEMPIPatientFromSearchResult()
        {
            var aPatient = new Patient();
            if (IsPatientSelected())
            {

                var EMPIMedicalRecordNumber = String.Empty;
                var EMPIFacilityCode = String.Empty;
                var EMPIPatientName = String.Empty;
                // Copy to a temporary variable to be thread-safe.
                if (EnableTestMode)
                {
                     EMPIMedicalRecordNumber = (String)foundPatientsView.SelectedItems[0].SubItems[8].Text;
                     EMPIFacilityCode = (String)foundPatientsView.SelectedItems[0].SubItems[6].Text;
                     EMPIPatientName = foundPatientsView.SelectedItems[0].SubItems[2].Text;
                }
                else
                {
                     EMPIMedicalRecordNumber = (String) foundPatientsView.SelectedItems[0].SubItems[5].Text;
                     EMPIFacilityCode = (String) foundPatientsView.SelectedItems[0].SubItems[6].Text;
                     EMPIPatientName = foundPatientsView.SelectedItems[0].SubItems[1].Text;
                }
                this.Cursor = Cursors.WaitCursor;

                if (!String.IsNullOrEmpty(EMPIMedicalRecordNumber))
                {
                    long empiMRN;
                    Int64.TryParse(EMPIMedicalRecordNumber, out empiMRN);
                    aPatient.MedicalRecordNumber = (int) empiMRN;
                }
                aPatient.Facility = new Facility {Code = EMPIFacilityCode};

                BreadCrumbLogger.GetInstance.Log(String.Format("{0} Patient selected from the search results",
                                                               EMPIPatientName));
            }
            return aPatient;
        }

        public virtual Patient SelectedEMPIPatient(PatientSearchResult patientResult)
        {
            if (patientResult == null)
            {
                throw new ArgumentNullException("patientResult", "Parameter PatientSearchResult should not be null.");
            }

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith(patientResult.HspCode);

            Name patientName = patientResult.Name;

            if (patientResult.Name.TypeOfName == TypeOfName.Alias)
            {
                // For an alias result, the real name is the only AKA entry
                patientName = patientResult.AkaNames[0];
            }

            Patient newPatient =
                new Patient(PersistentModel.NEW_OID,
                            PersistentModel.NEW_VERSION,
                            patientName,
                            patientResult.MedicalRecordNumber,
                            patientResult.DateOfBirth,
                            new SocialSecurityNumber(patientResult.SocialSecurityNumber),
                            patientResult.Gender,
                            facility);

            return newPatient;
        }

        public void FocusCreateButton()
        {
            btnCreateNewPatient.Focus(); 
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties

        public List<PatientSearchResult> PatientSearchResults
        {
            get
            {
                return this.Model as List<PatientSearchResult>;
            }
        }

        private bool EMPIFeatureEnabled { get; set; }
        public bool EMPIFeatureEnabledForFacility { get; set; }
        public string SearchByType
        {
            set
            {
                i_SearchByType = value;
            }
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Create a new Patient and Account from the current search criteria,
        /// then load the new account into the AccountView.
        /// </summary>
        private void CreateAndLoadNewPatient()
        {
            DialogResult result = DialogResult.None;

            //Raise activity start event for create new patient
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, EventArgs.Empty);

            Patient newPatient;

            if (this.ParentSearchView.SelectedTab.Name == FIND_BY_PATIENT)
            {
                newPatient =
                    this.ParentSearchView.FindAPatientView.CriteriaForCurrentSearch.CreateNewPatientWith(
                        this.CurrentActivity, User.GetCurrent().Facility);
            }
            else
            {
                PatientSearchCriteria patientCriteria = new PatientSearchCriteria(
                    User.GetCurrent().Facility.Code,
                    string.Empty,
                    string.Empty,
                    new SocialSecurityNumber(),
                    new Gender(),
                    0,
                    0,
                    string.Empty,
                    string.Empty
                    );

                newPatient = patientCriteria.CreateNewPatientWith(this.CurrentActivity, User.GetCurrent().Facility);
            }

            newPatient.IsNew = true;
            newPatient.SelectedAccount.IsNew = true;
            newPatient.SelectedAccount.Facility = User.GetCurrent().Facility;
            newPatient.SelectedAccount.ActionsLoader = new ActionLoader(newPatient.SelectedAccount);

            if (CurrentActivity.GetType() == typeof (ShortRegistrationActivity) ||
                CurrentActivity.GetType() == typeof (ShortPreRegistrationActivity))
            {
                newPatient.SelectedAccount.IsShortRegistered = true;
            }
            else if (CurrentActivity.GetType() == typeof (QuickAccountCreationActivity))
            {
                newPatient.SelectedAccount.IsQuickRegistered = true;
            }
            else if (CurrentActivity.GetType() == typeof (PAIWalkinOutpatientCreationActivity))
            {
                newPatient.SelectedAccount.IsPAIWalkinRegistered = true;
            }
            else if (CurrentActivity.GetType() == typeof(UCCPreMSERegistrationActivity))
            {
                newPatient.SelectedAccount.KindOfVisit = VisitType.UCCOutpatient;
                newPatient.SelectedAccount.FinancialClass = FinancialClass.MedScreenFinancialClass;
            }
            if (this.CurrentActivity.AssociatedActivityType == typeof (PreRegistrationWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (RegistrationWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (PreMSERegistrationWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (AdmitNewbornWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (PreAdmitNewbornWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (ShortRegistrationWithOfflineActivity)
                || this.CurrentActivity.AssociatedActivityType == typeof (ShortPreRegistrationWithOfflineActivity)
                )
            {
                // launch popup to collect AccountNumber and/or MedicalRecordNumber

                this.enterOfflineInfo = new EnterOfflineInfo();
                this.enterOfflineInfo.Model_Patient = newPatient;
                this.enterOfflineInfo.Model_IAccount = null;
                this.enterOfflineInfo.UpdateView();
                result = this.enterOfflineInfo.ShowDialog(this);

                if (result == DialogResult.Cancel)
                {
                    this.btnCreateNewPatient.Enabled = true;
                    return;
                }
            }

            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent(this,
                                                                          new LooseArgs(newPatient.SelectedAccount));
        }

        
        private bool IsPatientSelected()
        {
            foreach( ListViewItem item in foundPatientsView.Items )
            {
                if( item.Selected )
                {
                    return true;
                }
            }
            return false;
        }

        private ListViewSortManager FoundPatientsViewSortManager()
        {
            // Array of IComparer for each row.
            Type[] CompareTypes = new[] 
                {
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewDateSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewInt32Sort )
                };

            return new ListViewSortManager( foundPatientsView, CompareTypes );
        }

        private void DefaultSort()
        {
            SortManager.Sort( 1, SortOrder.Ascending );
        }
        private void DescendingSort()
        {
            SortManager.Sort(1, SortOrder.Descending);
        }

        private void PopulatePatientSearchResultGrid()
        {

            foundPatientsView.BeginUpdate();
            foundPatientsView.Items.Clear();


            if (this.EMPIResults)
            {
                foreach (PatientSearchResult aPatient in this.PatientSearchResults)
                {
                    this.foundPatientsView.Items.Add(new EMPIPatientSearchResultListViewItem(aPatient));
                }

                DescendingSort();
                if (!EnableTestMode)

                    foreach (ListViewItem item in foundPatientsView.Items)
                    {
                       item.SubItems.RemoveAt(1);
                    }

            }
            else
            {
                foreach (PatientSearchResult aPatient in this.PatientSearchResults)
                {
                    this.foundPatientsView.Items.Add(new PatientSearchResultListViewItem(aPatient));
                }

            }

            if (this.foundPatientsView.Items.Count > 0)
            {
                if (!EMPIResults)
                {
                    this.DefaultSort();
                }
                this.foundPatientsView.Focus();
                this.foundPatientsView.Items[0].Selected = true;
            }

            foundPatientsView.EndUpdate();

            base.DisableAcceptButton = true;

        }
 
        private void FoundPatientsView_GotFocus(object sender, EventArgs e )
        {
            CheckForSelectedItem();
        }

        private void CheckForSelectedItem()
        {
            bool rowSelected = false;
            int  totalNumberOfRows = foundPatientsView.Items.Count;

            foreach( ListViewItem item in foundPatientsView.Items )
            {
                if( item.Selected )
                {
                    rowSelected = true;
                    break;
                }
            }

            if( !rowSelected && totalNumberOfRows > 0 )
            {
                this.foundPatientsView.Items[0].Selected = true;
            }
        }


        #endregion

        #region Protected Properties

        #endregion

        #region Private Properties
        /// <summary>
        /// Navigates upwards to find instance of SearchView aggregating this
        /// </summary>
        private SearchView ParentSearchView
        {
            get
            {
                return this.Parent as SearchView;
            }
        }

        private IEMPIFeatureManager EMPIFeatureManager { get; set; }

        /// <summary>
        /// Returns the CurrentActivity from the ParentSearchView
        /// </summary>
        private Activity CurrentActivity
        {
            get
            {
                return this.ParentSearchView != null ? this.ParentSearchView.CurrentActivity : null;
            }
        }
        
        private ListViewSortManager SortManager
        {
            get
            {
                return i_SortManager;
            }
            set
            {
                i_SortManager = value;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize PatientSearchResultsView, call PatientSearchResultsView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientSearchResultsView));
            this.searchResultsOptionsPanel = new System.Windows.Forms.Panel();
            this.btnSearchEMPI = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnCreateNewPatient = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.button_DummyEnter = new PatientAccess.UI.CommonControls.LoggingButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPanelTitle = new System.Windows.Forms.Label();
            this.foundPatientsView = new System.Windows.Forms.ListView();
            this.chBmp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPatientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDob = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSsn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMrn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHSPCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imlAlias = new System.Windows.Forms.ImageList(this.components);
            this.chEMPIScore = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ttPatientName = new System.Windows.Forms.ToolTip(this.components);
            this.label_NoPatientsFound = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.searchResultpanel = new System.Windows.Forms.Panel();
            this.searchResultsOptionsPanel.SuspendLayout();
            this.searchResultpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchResultsOptionsPanel
            // 
            this.searchResultsOptionsPanel.BackColor = System.Drawing.Color.White;
            this.searchResultsOptionsPanel.Controls.Add(this.btnSearchEMPI);
            this.searchResultsOptionsPanel.Controls.Add(this.btnCreateNewPatient);
            this.searchResultsOptionsPanel.Controls.Add(this.button_DummyEnter);
            this.searchResultsOptionsPanel.Controls.Add(this.label1);
            this.searchResultsOptionsPanel.Controls.Add(this.lblPanelTitle);
            this.searchResultsOptionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchResultsOptionsPanel.Location = new System.Drawing.Point(0, 0);
            this.searchResultsOptionsPanel.Name = "searchResultsOptionsPanel";
            this.searchResultsOptionsPanel.Size = new System.Drawing.Size(984, 75);
            this.searchResultsOptionsPanel.TabIndex = 0;
            // 
            // btnSearchEMPI
            // 
            this.btnSearchEMPI.Location = new System.Drawing.Point(233, 48);
            this.btnSearchEMPI.Message = null;
            this.btnSearchEMPI.Name = "btnSearchEMPI";
            this.btnSearchEMPI.Size = new System.Drawing.Size(130, 23);
            this.btnSearchEMPI.TabIndex = 2;
            this.btnSearchEMPI.Text = "Search EMPI";
            this.btnSearchEMPI.Click += new System.EventHandler(this.SearchEMPIButtonClick);
            // 
            // btnCreateNewPatient
            // 
            this.btnCreateNewPatient.Location = new System.Drawing.Point(0, 48);
            this.btnCreateNewPatient.Message = null;
            this.btnCreateNewPatient.Name = "btnCreateNewPatient";
            this.btnCreateNewPatient.Size = new System.Drawing.Size(167, 23);
            this.btnCreateNewPatient.TabIndex = 1;
            this.btnCreateNewPatient.Text = "Cre&ate New Patient...";
            this.btnCreateNewPatient.Click += new System.EventHandler(this.CreateNewPatientButtonClick);
            // 
            // button_DummyEnter
            // 
            this.button_DummyEnter.Location = new System.Drawing.Point(184, 160);
            this.button_DummyEnter.Message = null;
            this.button_DummyEnter.Name = "button_DummyEnter";
            this.button_DummyEnter.Size = new System.Drawing.Size(75, 23);
            this.button_DummyEnter.TabIndex = 0;
            this.button_DummyEnter.TabStop = false;
            this.button_DummyEnter.Text = "Hidden";
            this.button_DummyEnter.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(952, 16);
            this.label1.TabIndex = 0;
            // 
            // lblPanelTitle
            // 
            this.lblPanelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPanelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPanelTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPanelTitle.Name = "lblPanelTitle";
            this.lblPanelTitle.Size = new System.Drawing.Size(984, 23);
            this.lblPanelTitle.TabIndex = 0;
            this.lblPanelTitle.Text = "  Search Results";
            // 
            // foundPatientsView
            // 
            this.foundPatientsView.BackColor = System.Drawing.SystemColors.Window;
            this.foundPatientsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chBmp,
            this.chPatientName,
            this.chGender,
            this.chDob,
            this.chSsn,
            this.chMrn,
            this.chAddress});
            this.foundPatientsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foundPatientsView.FullRowSelect = true;
            this.foundPatientsView.GridLines = true;
            this.foundPatientsView.HideSelection = false;
            this.foundPatientsView.Location = new System.Drawing.Point(0, 0);
            this.foundPatientsView.Name = "foundPatientsView";
            this.foundPatientsView.ShowItemToolTips = true;
            this.foundPatientsView.Size = new System.Drawing.Size(984, 211);
            this.foundPatientsView.SmallImageList = this.imlAlias;
            this.foundPatientsView.TabIndex = 1;
            this.foundPatientsView.UseCompatibleStateImageBehavior = false;
            this.foundPatientsView.View = System.Windows.Forms.View.Details;
            this.foundPatientsView.DoubleClick += new System.EventHandler(this.foundPatientsView_DoubleClick);
            this.foundPatientsView.GotFocus += new System.EventHandler(this.FoundPatientsView_GotFocus);
            this.foundPatientsView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.foundPatientsView_KeyDown);
            //this.foundPatientsView.Leave += new System.EventHandler(this.foundPatientsView_Leave);
            // 
            // chBmp
            // 
            this.chBmp.Text = "";
            this.chBmp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chBmp.Width = 22;
            // 
            // chPatientName
            // 
            this.chPatientName.Text = "Patient Name";
            this.chPatientName.Width = 300;
            // 
            // chGender
            // 
            this.chGender.Text = "Gender";
            this.chGender.Width = 71;
            // 
            // chDob
            // 
            this.chDob.Text = "DOB";
            this.chDob.Width = 80;
            // 
            // chSsn
            // 
            this.chSsn.Text = "SSN";
            this.chSsn.Width = 91;
            // 
            // chMrn
            // 
            this.chMrn.Text = "MRN";
            this.chMrn.Width = 91;
            // 
            // chHSPCode
            // 
            this.chHSPCode.Text = "Hospital Code";
            this.chHSPCode.Width = 91;
            // 
            // chAddress
            // 
            this.chAddress.Text = "Address";
            this.chAddress.Width = 315;
            // 
            // imlAlias
            // 
            this.imlAlias.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlAlias.ImageStream")));
            this.imlAlias.TransparentColor = System.Drawing.Color.Transparent;
            this.imlAlias.Images.SetKeyName(0, "");
            this.imlAlias.Images.SetKeyName(1, "");
            this.imlAlias.Images.SetKeyName(2, "");
            // 
            // ttPatientName
            // 
            this.ttPatientName.AutoPopDelay = 200;
            this.ttPatientName.InitialDelay = 200;
            this.ttPatientName.ReshowDelay = 200;
            // 
            // label_NoPatientsFound
            // 
            this.label_NoPatientsFound.Location = new System.Drawing.Point(15, 11);
            this.label_NoPatientsFound.Name = "label_NoPatientsFound";
            this.label_NoPatientsFound.Size = new System.Drawing.Size(416, 32);
            this.label_NoPatientsFound.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.label2.Location = new System.Drawing.Point(192, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(344, 72);
            this.label2.TabIndex = 0;
            this.label2.Text = "NOTE: Code in FoundPatientsViewSortManager() must be maintained to coorespond to " +
    "each Column.  If a Column is added, deleted, or changes type, this routine must " +
    "be updated";
            // 
            // searchResultpanel
            // 
            this.searchResultpanel.Controls.Add(this.label2);
            this.searchResultpanel.Controls.Add(this.foundPatientsView);
            this.searchResultpanel.Controls.Add(this.label_NoPatientsFound);
            this.searchResultpanel.Location = new System.Drawing.Point(0, 77);
            this.searchResultpanel.Name = "searchResultpanel";
            this.searchResultpanel.Size = new System.Drawing.Size(984, 211);
            this.searchResultpanel.TabIndex = 2;
            // 
            // PatientSearchResultsView
            // 
            this.AcceptButton = this.button_DummyEnter;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.searchResultpanel);
            this.Controls.Add(this.searchResultsOptionsPanel);
            this.Name = "PatientSearchResultsView";
            this.Size = new System.Drawing.Size(984, 307);
            this.Load += new System.EventHandler(this.PatientSearchResultsView_Load);
            this.searchResultsOptionsPanel.ResumeLayout(false);
            this.searchResultpanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construction for PatientSearchResultsView, 
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public PatientSearchResultsView()
        {
            InitializeComponent();

            SortManager = FoundPatientsViewSortManager();
        }
        #endregion

        #region Data Elements

        private IContainer              components;
        private Panel                   searchResultsOptionsPanel;
        private ListView                foundPatientsView;
        private ClickOnceLoggingButton  btnCreateNewPatient;
        private LoggingButton           button_DummyEnter;
        private ColumnHeader            chPatientName;
        private ColumnHeader            chEMPIScore;
        private ColumnHeader            chGender;
        private ColumnHeader            chDob;
        private ColumnHeader            chSsn;
        private ColumnHeader            chMrn;
        private ColumnHeader            chHSPCode;
        private ColumnHeader            chAddress;
        private ColumnHeader            chBmp;
        private ImageList               imlAlias;
        private Label                   lblPanelTitle;
        private Label                   label1;
        private Label                   label2;
        private Label                   label_NoPatientsFound;
        private ToolTip                 ttPatientName;
        private ListViewSortManager     i_SortManager;
        private string                  i_SearchByType;
        private Panel                   searchResultpanel;

        private EnterOfflineInfo        enterOfflineInfo ;
        public bool EMPIResults = false;
        private bool EnableTestMode
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings[ENABLE_TEST_MODE]); }
        }

        private const string ENABLE_TEST_MODE = "PatientAccess.EnableTestMode";
        #endregion

        #region Constants

        private const string PATIENT_SEARCH_ERRMSG      = @"No patients were found for the patient information entered.";
        private const string GUARANTOR_SEARCH_ERRMSG    = @"No patients were found for the guarantor information entered.";
        private const string SEARCH_TIP_MSG             = @"If no appropriate results found, try searching EMPI database for a patient located in the Enterprise.";
        private const string NON_EMPI_SEARCH_TIP_MESSAGE = @"If no appropriate result is found, verify the information or try modifying your search criteria above and search again. ";
        private const string MOTHER_NOT_FOUND_MSG       = @"If no appropriate result is found, modify your search criteria and try again. You must register the mother before registering a newborn.";
        private const string HISTORICAL_PATIENT_SEARCH  = @"HistoricalPatientSearch";
        private ClickOnceLoggingButton btnSearchEMPI;
        private const string FIND_BY_PATIENT            = @"findByPatientTab";

        #endregion

        public void EnableSearchEMPIButton()
        {
            btnSearchEMPI.Enabled = true;
        }
        public void DisableSearchEMPIButton()
        {
            btnSearchEMPI.Enabled = false;
        }
    }
}
