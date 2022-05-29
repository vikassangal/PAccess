using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
    public partial class SSNControl : ControlView, ISsnView
    {
        #region Events
        public event EventHandler ssnNumberChanged;
        #endregion

        #region Event Handlers

        private void mtbSsnNumber_Validating( object sender, CancelEventArgs e )
        {
            if ( mtbSSNNumber.UnMaskedText.Trim() != string.Empty
                && mtbSSNNumber.UnMaskedText.Length != 9 )
            {
                if ( cmbSSNStatus.Focused )
                {
                    // Don't report error if user is clicking on SSN Status ComboBox
                    return;
                }

                // Prevent cursor from advancing to the next control
                UIColors.SetErrorBgColor( mtbSSNNumber );
                MessageBox.Show( UIErrorMessages.SSN_INVALID, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbSSNNumber.Focus();
                return;
            }

            UIColors.SetNormalBgColor( mtbSSNNumber );

            // check if more than 1 patient is using the given SSN for demographics view
            if ( ( SsnContext == SsnViewContext.DemographicsView || 
                   SsnContext == SsnViewContext.PreMseDemographicsView ||
                   SsnContext == SsnViewContext.ShortDemographicsView ||
                   SsnContext == SsnViewContext.QuickAccountCreationView ||
                   SsnContext == SsnViewContext.PAIWalkinAccountCreationView ) &&
                    mtbSSNNumber.UnMaskedText != string.Empty &&
                    SsnFactory.IsKnownSSN(mtbSSNNumber.UnMaskedText))
            {
                if ( mtbSSNNumber.UnMaskedText.Equals( i_previousSsnEntered ) )
                {
                    // don't display the dialog again unless ssn number changed
                    return;
                }

                i_previousSsnEntered = mtbSSNNumber.UnMaskedText;

                Model_Person.SocialSecurityNumber = new SocialSecurityNumber( mtbSSNNumber.UnMaskedText );

                var storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                try
                {
                    var patientBroker = new PatientBrokerProxy();
                    var patientSearchCriteria = PatientSearchCriteria.Default;
                    patientSearchCriteria.SocialSecurityNumber = new SocialSecurityNumber( mtbSSNNumber.UnMaskedText );
                    patientSearchCriteria.HSPCode = User.GetCurrent().Facility.Code;
                    var patientSearchResponse = patientBroker.GetPatientSearchResponseFor( patientSearchCriteria );

                    Cursor = storedCursor;

                    if ( patientSearchResponse.PatientSearchResults.Count > 0 )
                    {
                        var currentPatient = Model_Person as Patient;

                        if ( currentPatient != null && currentPatient.MedicalRecordNumber != 0 )
                        {
                            patientSearchResponse.PatientSearchResults.RemoveAll(
                                result => result.MedicalRecordNumber.Equals(currentPatient.MedicalRecordNumber));
                        }

                        if ( patientSearchResponse.PatientSearchResults.Count > 0 )
                        {
                            // Display dialog box showing multiple patient       
                            // names & MRNs for the given social security number.
                            var dialog = new SSNInUseDialog( patientSearchResponse ) { Model = Model_Person };

                            try
                            {
                                dialog.ShowDialog( this );
                            }
                            finally
                            {
                                dialog.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    Cursor = storedCursor;
                }
            }
            else
            {
                i_previousSsnEntered = mtbSSNNumber.UnMaskedText;
                SetSsnStatusUsingValidatedSsn();
                mtbSSNNumber.Enabled = ( Model_Person.SocialSecurityNumber.SSNStatus.IsKnownSSNStatus );
            }

            RunRules();

            // refresh top panel for demographics view
            if ( ssnNumberChanged != null )
            {
                ssnNumberChanged( this, null );
            }
        }

        public void ResetSSNControl()
        {
            SsnControlPopulator = new SsnPopulatorFactory(ModelAccount).GetPopulator();
            SsnControlPopulator.Populate( this );
            SetSsnStatusUsingValidatedSsn();
        }

        public void SetSsnStatusUsingValidatedSsn()
        {
            var oldSsn = new SocialSecurityNumber( mtbSSNNumber.UnMaskedText );
            if (SsnContext == SsnViewContext.GuarantorView || SsnContext == SsnViewContext.ShortGuarantorView)
            {

                Model_Person.SocialSecurityNumber = SsnFactory.GetValidatedSocialSecurityNumberUsing(
                    oldSsn, User.GetCurrent().Facility.GetPersonState(), ModelAccount.AdmitDate);
            }
            else
            {

                Model_Person.SocialSecurityNumber = SsnFactory.GetValidatedSocialSecurityNumberUsing(
                    oldSsn, User.GetCurrent().Facility.GetPersonState(), ModelAccount.AdmitDate,
                    ModelAccount.Patient.AgeInYearsFor());
            }

            SetSsnAndSsnStatusFromModel();
        }

        /// <summary>
        /// This method is used to update the default Social Security Number on a Patient, based
        /// on the Admit Date, whose SSN Status (other than KNOWN) is already selected/available.
        /// Example: When Admit Date is changed on an account.
        /// </summary>
        public void UpdateDefaultSocialSecurityNumberForAdmitDate()
        {
            var personState = User.GetCurrent().Facility.GetPersonState();

            SsnFactory.UpdateSsn( ModelAccount.AdmitDate, SsnContext, personState, Model_Person );

            SetSsnAndSsnStatusFromModel();
        }

        public void AddSsnStatus ( SocialSecurityNumberStatus ssnStatus )
        {
            cmbSSNStatus.Items.Add( ssnStatus );
        }
        public void ClearSsnStatus()
        {
            cmbSSNStatus.Items.Clear();
        }
        public void DeselectSelectedStatus()
        {
            cmbSSNStatus.SelectedIndex = -1;
        }

        private void SetSsnAndSsnStatusFromModel()
        {
            cmbSSNStatus.SelectedIndexChanged -= cmbSsnStatus_SelectedIndexChanged;

            if ( StatusIsNotInDropdown() )
            {
                SetStatusToKnown();
            }

            else
            {
                SetStatusFromTheModel();    
            }

            cmbSSNStatus.SelectedIndexChanged += cmbSsnStatus_SelectedIndexChanged;
            mtbSSNNumber.UnMaskedText = Model_Person.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim();
            mtbSSNNumber.Enabled = IsKnownSelected();
        }

        private void SetStatusFromTheModel ()
        {
            cmbSSNStatus.SelectedIndex = cmbSSNStatus.FindString( Model_Person.SocialSecurityNumber.SSNStatus.Description );
        }

        private void SetStatusToKnown ()
        {
            cmbSSNStatus.SelectedIndex = cmbSSNStatus.FindString( SocialSecurityNumberStatus.KNOWN );
        }

        private bool StatusIsNotInDropdown ()
        {
            return cmbSSNStatus.FindString( Model_Person.SocialSecurityNumber.SSNStatus.Description ) == -1;
        }

        private bool IsKnownSelected()
        {
            if ( cmbSSNStatus.SelectedIndex > -1 )
            {
                return ( ( ( SocialSecurityNumberStatus )cmbSSNStatus.SelectedItem ).IsKnownSSNStatus );
            }

            return Model_Person.SocialSecurityNumber.SSNStatus.IsKnownSSNStatus;
        }

        private void cmbSsnStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( cmbSSNStatus.SelectedIndex > -1 )
            {
                var ssnStatus = ( SocialSecurityNumberStatus )cmbSSNStatus.SelectedItem;
                var ssnPresenter = new SsnPresenter( this, User.GetCurrent().Facility.GetPersonState(), ModelAccount, Model_Person ); 
                ssnPresenter.SsnStatusChanged( ssnStatus );
            }

            i_previousSsnEntered = mtbSSNNumber.UnMaskedText;

            RunRules();

            if (ssnNumberChanged != null)
            {
                ssnNumberChanged(this, null);
            }
        }

        public bool SsnEnabled
        {
            get { return mtbSSNNumber.Enabled; }
            set { mtbSSNNumber.Enabled = value; }
        }

        public string SsnText
        {
            get { return mtbSSNNumber.UnMaskedText; }
            set { mtbSSNNumber.UnMaskedText = value; }
        }

        private void cmbSsnStatus_Validating(object sender, EventArgs e)
        {
           cmbSsnStatus_SelectedIndexChanged( sender, e );
        }

        private void PersonSSNRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbSSNNumber );
            Refresh();
        }

        private void PersonSSNPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbSSNNumber );
            Refresh();
        }

        public void SetSSNNumberNormalColor()
        {
            UIColors.SetNormalBgColor( mtbSSNNumber );
        }

        /// <summary>
        /// Modify size of the control based on the location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SSNControl_Load( object sender, EventArgs e )
        {
            if ( SsnContext == SsnViewContext.DemographicsView || 
                 SsnContext == SsnViewContext.PreMseDemographicsView || 
                 SsnContext == SsnViewContext.ShortDemographicsView  )
            {
                grpSSN.Size = new Size( 265, 72 );
                Size = new Size( 265, 72 );
            }
        }

        #endregion

        #region Methods

        public void RegisterRules()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( PersonSSNRequired ), Model_Person, PersonSSNRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( PersonSSNPreferred ), Model_Person, PersonSSNPreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( SocialSecurityNumberRequired ), Model_Person, PersonSSNRequiredEventHandler );
        }

        public void RunRules()
        {

            RegisterRules();
           
            UIColors.SetNormalBgColor( mtbSSNNumber );
            if ( SsnContext == SsnViewContext.GuarantorView )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonSSNRequired ), Model_Person );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonSSNPreferred ), Model_Person );
            }
            else if ( SsnContext == SsnViewContext.PreMseDemographicsView )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonSSNPreferred ), Model_Person );
            }
            else if ( SsnContext == SsnViewContext.DemographicsView )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( SocialSecurityNumberRequired ), Model_Person );
            }
            else if ( SsnContext == SsnViewContext.ShortGuarantorView )
            {
                RuleEngine.GetInstance().EvaluateRule(typeof (PersonSSNRequired), Model_Person);
            }
            else if ( SsnContext == SsnViewContext.ShortDemographicsView )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( SocialSecurityNumberRequired ), Model_Person );
            }
            else if ( SsnContext == SsnViewContext.QuickAccountCreationView )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( SocialSecurityNumberRequired ), Model_Person );
            }
            else if (SsnContext == SsnViewContext.PAIWalkinAccountCreationView)
            {
                RuleEngine.GetInstance().EvaluateRule(typeof(SocialSecurityNumberRequired), Model_Person);
            }
        }

        public override void UpdateView()
        {
            SsnControlPopulator = new SsnPopulatorFactory(ModelAccount).GetPopulator(); 
            SsnControlPopulator.Populate( this );

            if ( Model_Person.SocialSecurityNumber == null ||
                Model_Person.SocialSecurityNumber.SSNStatus.Description == string.Empty )
            {
                Model_Person.SocialSecurityNumber = new SocialSecurityNumber
                    {
                        SSNStatus = ( new SSNBrokerProxy() ).SSNStatusWith(
                            User.GetCurrent().Facility.Oid, SocialSecurityNumberStatus.KNOWN )
                    };
            }

            mtbSSNNumber.UnMaskedText = Model_Person.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim();
            if ( mtbSSNNumber.UnMaskedText != string.Empty && mtbSSNNumber.Text.Length != 11 )
            {
                UIColors.SetErrorBgColor( mtbSSNNumber );
                mtbSSNNumber.Focus();
            }
            else
            {
                SetSsnStatusUsingValidatedSsn();
            }
            mtbSSNNumber.Enabled = ( Model_Person.SocialSecurityNumber.SSNStatus.IsKnownSSNStatus );
            RunRules();
        }        

        #endregion

        #region Properties

        public int SsnStatusCount
        {
            get { return cmbSSNStatus.Items.Count; }
        }

        public SsnViewContext SsnContext
        {
            get
            {
                return i_ssnContext;
            }
            set
            {
                i_ssnContext = value ;
            }
        }

        // This cannot be of type 'Patient' because the SSNControl is being used for 
        // the SSN entries of not just Patient but also Guarantor, and also be able to
        // record SSN for any other type of Person as the system demands in the future.
        private Person Model_Person
        {
            get
            {
                return Model as Person;
            }
            
        }

        public Account ModelAccount
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public SSNControl()
        {
            InitializeComponent();
            SsnFactory = new SSNFactory();
        }

        #endregion

        #region Data Elements
         
        private SsnViewContext i_ssnContext;
        private string i_previousSsnEntered = string.Empty;
        public ISsnFactory SsnFactory { get; set; }
        private Account i_Account = new Account();
        private ISsnViewPopulator SsnControlPopulator { get; set; }

        #endregion

        #region Constants

        #endregion
    }
}
