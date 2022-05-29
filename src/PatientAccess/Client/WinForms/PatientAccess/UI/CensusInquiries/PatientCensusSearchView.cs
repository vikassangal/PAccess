using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for PatientCensusSearchView.
    /// </summary>
    [Serializable]
    public class PatientCensusSearchView : ControlView
    {        

        #region Events

        public event EventHandler AccountsFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler ResetView;
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;

        #endregion

        #region Event Handlers

        private void 
            PatientCensusSearchView_Load( object sender, EventArgs e )
        {
            if( this.ParentForm != null && FirstTimeLayout )
            {
                this.lastNameTextbox.Select();
                FirstTimeLayout = false;
            }
        }

        private void firstNameTextbox_Enter( object sender, EventArgs e )
        {
            this.firstNameTextbox.SelectionStart = 
                this.firstNameTextbox.TextLength;
        }


        private void firstNameTextbox_Leave( object sender, EventArgs e )
        {
            this.firstNameTextbox.Text = 
                this.firstNameTextbox.Text.TrimEnd( null );
        }


        private void 
            firstNameTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForDataEntered();
        }


        private void lastNameTextbox_Enter( object sender, EventArgs e )
        {
            this.lastNameTextbox.SelectionStart = 
                this.lastNameTextbox.TextLength;
        }


        private void lastNameTextbox_Leave( object sender, EventArgs e )
        {
            this.lastNameTextbox.Text = 
                this.lastNameTextbox.Text.TrimEnd( null );
        }


        private void 
            lastNameTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForDataEntered();
        }


        private void 
            accountTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForDataEntered();
        }

        private void accountTextbox_Validating(object sender, CancelEventArgs e)
        {
            if( this.accountTextbox.Text != String.Empty )
            {
                //Check to see if search data entered is valid.
                ValidationResult result = Validate();

                if( !result.IsValid )
                {
                    this.ResetView( this, null );

                    string errorMsg = result.Message;
                    if( errorMsg != String.Empty )
                    {
                        SearchError( result.AspectInError );
                        MessageBox.Show( errorMsg, 
                            String.Empty,
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Exclamation, 
                            MessageBoxDefaultButton.Button1 );
                        accountNumberIsInvalid = true;
                        ResetBackGroundColor();
                    }
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            this.ResetView( this, null );
            ResetBackGroundColor();
            SearchData();
        }


        private void resetButton_Click(object sender, EventArgs e)
        {
            ResetBackGroundColor();
            ResetBackGroundColor();
            this.accountTextbox.ResetText();
            this.firstNameTextbox.ResetText();
            this.lastNameTextbox.ResetText();
            
            this.searchButton.Enabled = false;

            this.ResetView( this, null );
            this.Invalidate();
            this.Update();
        }

        private void censusView_Close(object sender, EventArgs e)
        {
            CancelBackgroundWorker();
        }

        // encapsulate cancellation functionality so it can be reused
        private void CancelBackgroundWorker()
        {
            if (this.backgroundWorker != null)
                this.backgroundWorker.CancelAsync();
        }

        #endregion

        #region Methods

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        
        private void InitializeComponent()
        {
            this.patientCensusSearchPanel = new System.Windows.Forms.Panel();
            this.resetButton = new LoggingButton();
            this.searchButton = new LoggingButton();
            this.accountTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.firstNameTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lastNameTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.accountLabel = new System.Windows.Forms.Label();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.searchCriteriaMessageLabel = new System.Windows.Forms.Label();
            this.patientCensusSearchPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patientCensusSearchPanel
            // 
            this.patientCensusSearchPanel.Controls.Add(this.resetButton);
            this.patientCensusSearchPanel.Controls.Add(this.searchButton);
            this.patientCensusSearchPanel.Controls.Add(this.accountTextbox);
            this.patientCensusSearchPanel.Controls.Add(this.firstNameTextbox);
            this.patientCensusSearchPanel.Controls.Add(this.lastNameTextbox);
            this.patientCensusSearchPanel.Controls.Add(this.accountLabel);
            this.patientCensusSearchPanel.Controls.Add(this.firstNameLabel);
            this.patientCensusSearchPanel.Controls.Add(this.lastNameLabel);
            this.patientCensusSearchPanel.Controls.Add(this.searchCriteriaMessageLabel);
            this.patientCensusSearchPanel.Location = new System.Drawing.Point(0, 0);
            this.patientCensusSearchPanel.Name = "patientCensusSearchPanel";
            this.patientCensusSearchPanel.Size = new System.Drawing.Size(900, 70);
            this.patientCensusSearchPanel.TabIndex = 0;
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(799, 42);
            this.resetButton.Name = "resetButton";
            this.resetButton.TabIndex = 5;
            this.resetButton.Text = "Rese&t";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Enabled = false;
            this.searchButton.Location = new System.Drawing.Point(720, 42);
            this.searchButton.Name = "searchButton";
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Sear&ch";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // accountTextbox
            // 
            this.accountTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.accountTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.accountTextbox.KeyPressExpression = "^\\d*$";
            this.accountTextbox.Location = new System.Drawing.Point(630, 43);
            this.accountTextbox.Mask = "";
            this.accountTextbox.MaxLength = 9;
            this.accountTextbox.Name = "accountTextbox";
            this.accountTextbox.Size = new System.Drawing.Size(62, 20);
            this.accountTextbox.TabIndex = 3;
            this.accountTextbox.ValidationExpression = "^\\d*$";
            this.accountTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.accountTextbox_Validating);
            this.accountTextbox.TextChanged += new System.EventHandler(this.accountTextbox_TextChanged);
            // 
            // firstNameTextbox
            // 
            this.firstNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.firstNameTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.firstNameTextbox.KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.firstNameTextbox.Location = new System.Drawing.Point(393, 43);
            this.firstNameTextbox.Mask = "";
            this.firstNameTextbox.MaxLength = 15;
            this.firstNameTextbox.Name = "firstNameTextbox";
            this.firstNameTextbox.Size = new System.Drawing.Size(160, 20);
            this.firstNameTextbox.TabIndex = 2;
            this.firstNameTextbox.ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.firstNameTextbox.Leave += new System.EventHandler(this.firstNameTextbox_Leave);
            this.firstNameTextbox.TextChanged += new System.EventHandler(this.firstNameTextbox_TextChanged);
            this.firstNameTextbox.Enter += new System.EventHandler(this.firstNameTextbox_Enter);
            // 
            // lastNameTextbox
            // 
            this.lastNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.lastNameTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.lastNameTextbox.KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.lastNameTextbox.Location = new System.Drawing.Point(58, 43);
            this.lastNameTextbox.Mask = "";
            this.lastNameTextbox.MaxLength = 25;
            this.lastNameTextbox.Name = "lastNameTextbox";
            this.lastNameTextbox.Size = new System.Drawing.Size(248, 20);
            this.lastNameTextbox.TabIndex = 1;
            this.lastNameTextbox.ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9,\\- ]*$";
            this.lastNameTextbox.Leave += new System.EventHandler(this.lastNameTextbox_Leave);
            this.lastNameTextbox.TextChanged += new System.EventHandler(this.lastNameTextbox_TextChanged);
            this.lastNameTextbox.Enter += new System.EventHandler(this.lastNameTextbox_Enter);
            // 
            // accountLabel
            // 
            this.accountLabel.Location = new System.Drawing.Point(579, 45);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(48, 16);
            this.accountLabel.TabIndex = 0;
            this.accountLabel.Text = "Account:";
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.Location = new System.Drawing.Point(335, 45);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(62, 16);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "First name:";
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.Location = new System.Drawing.Point(0, 45);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(64, 16);
            this.lastNameLabel.TabIndex = 0;
            this.lastNameLabel.Text = "Last name:";
            // 
            // searchCriteriaMessageLabel
            // 
            this.searchCriteriaMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.searchCriteriaMessageLabel.Location = new System.Drawing.Point(0, 19);
            this.searchCriteriaMessageLabel.Name = "searchCriteriaMessageLabel";
            this.searchCriteriaMessageLabel.Size = new System.Drawing.Size(848, 16);
            this.searchCriteriaMessageLabel.TabIndex = 0;
            this.searchCriteriaMessageLabel.Text = "Search by Name, Account, or both. When searching by First Name, Last Name is requ" +
                "ired. Partial search is supported for Last and First Names, but not for Account." +
                "";
            // 
            // PatientCensusSearchView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.patientCensusSearchPanel);
            this.Name = "PatientCensusSearchView";
            this.Size = new System.Drawing.Size(910, 70);
            this.Load += new System.EventHandler(this.PatientCensusSearchView_Load);
            this.patientCensusSearchPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        #region Properties
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

        #region Private Methods

        private void ResetBackGroundColor()
        {
            UIColors.SetNormalBgColor( firstNameTextbox );
            UIColors.SetNormalBgColor( lastNameTextbox );
            UIColors.SetNormalBgColor( accountTextbox );
        }


        private void SearchError( string AspectInError )
        {
            switch( AspectInError )
            {
                case "FirstName":
                    this.firstNameTextbox.BackColor = 
                        Color.Red ;  
                    this.firstNameTextbox.Select();
                    break;
                case "AccountNumber":
                    this.accountTextbox.BackColor = 
                        Color.Red ;                       
                    this.accountTextbox.Select();
                    break;
                case "LastName":
                    this.lastNameTextbox.BackColor = 
                        Color.Red ;  
                    this.lastNameTextbox.Select();
                    break;
                default:
                    break;
            }
        }


        private void CheckForDataEntered()
        {
            if( this.accountTextbox.Text == String.Empty && 
                this.lastNameTextbox.Text == String.Empty &&
                this.firstNameTextbox.Text == String.Empty )
            {
                this.searchButton.Enabled = false;
            }
            else
            {
                this.searchButton.Enabled = true;
                if( this.ParentForm != null )
                {
                    this.AcceptButton = searchButton;
                }
            }
        }


        private new ValidationResult Validate()
        {
            if( this.accountTextbox.Text != String.Empty )
            {
                string accountNumber = this.accountTextbox.Text;

                if( accountNumber.Length < 2 )
                {                    
                    return new ValidationResult( 
                        false, 
                        UIErrorMessages.ERR_MSG_INVALID_ACCOUNT_NUMBER, 
                        ACCOUNT_NUMBER );
                }
                
                if( Account.IsValidAccountNumber( User.GetCurrent().Facility, long.Parse(this.accountTextbox.Text) ) )
                {
                    return new ValidationResult( true );
                }
                else
                {                    
                    return new ValidationResult( 
                        false, 
                        UIErrorMessages.ERR_MSG_INVALID_ACCOUNT_NUMBER, 
                        ACCOUNT_NUMBER );
                }                
            }

            if( this.lastNameTextbox.Text != String.Empty )
            {
                return new ValidationResult( true );
            }

            if( this.firstNameTextbox.Text != String.Empty && 
                this.lastNameTextbox.Text == String.Empty )
            {
                return new ValidationResult( 
                    false, 
                    ERR_MSG_FIRTSTNAME_ONLY, LAST_NAME );
            }

            return new ValidationResult( false );
        }

        private void SearchData()
        {
            if( accountNumberIsInvalid )
            {
                this.accountTextbox.Select();
                accountNumberIsInvalid = false;
                return;
            }
            else
            {
                if( this.backgroundWorker == null
                    ||
                    ( this.backgroundWorker != null
                    && !this.backgroundWorker.IsBusy )
                    )
                {
                    this.BeforeWork();

                    this.backgroundWorker = new BackgroundWorker();
                    this.backgroundWorker.WorkerSupportsCancellation = true;

                    backgroundWorker.DoWork +=
                        new DoWorkEventHandler(DoSearchData);
                    backgroundWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(AfterWork);

                    backgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            if( this.BeforeWorkEvent != null )
            {
                this.BeforeWorkEvent( this, null );
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;
            
            if( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                
                this.ResetView( this, null );
                
                string errorMsg = this.result.Message;

                if( errorMsg != String.Empty )
                {
                    SearchError(this.result.AspectInError);

                    MessageBox.Show( errorMsg,
                        String.Empty,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }

                ResetBackGroundColor();
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    MessageBox.Show(UIErrorMessages.TIMEOUT_CENSUS_REPORT_DISPLAY);
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                if( this.accountProxiesCollection != null )
                {
                    if( this.accountProxiesCollection.Count != 0 &&
                        AccountsFound != null )
                    {
                        AccountsFound(
                            this,
                            new LooseArgs( this.accountProxiesCollection ) );
                    }
                    else
                    {
                        if( NoAccountsFound != null )
                        {
                            NoAccountsFound(
                                this,
                                new LooseArgs( this ) );
                        }
                    }
                }

                //Reset Background colors on successful search criteria.
                ResetBackGroundColor();
            }

            if( this.AfterWorkEvent != null )
            {
                this.AfterWorkEvent( this, null );
            }
                
            this.Cursor = Cursors.Default;
        }


        private void DoSearchData( object sender, DoWorkEventArgs e )
        {
            IHDIService aService = BrokerFactory.BrokerOfType<IHDIService>();

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //Check to see if search data entered is valid.
            this.result = Validate();

            if( this.result.IsValid )
            {

                long accountNumber = 0;
                string lastName = this.lastNameTextbox.Text.Trim();
                string firstName = this.firstNameTextbox.Text.Trim();

                if( this.accountTextbox.Text.Trim().Length > 0 )
                {
                    accountNumber = Convert.ToInt64( this.accountTextbox.Text );
                }
                if( lastName.Length > 0 )
                {
                    lastName = lastName.Replace( " ", "" );
                }
                if( firstName.Length > 0 )
                {
                    firstName = firstName.Replace( " ", "" ); 
                }

                // Poll the cancellationPending property, if true set e.Cancel to true and return.
                // Rationale: permit user cancellation of bgw. 
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //Create PatientSearchCriteria and pass in all args.
                PatientCensusSearchCriteria patientCriteria =
                    new PatientCensusSearchCriteria(
                    lastName,
                    firstName,
                    accountNumber,
                    User.GetCurrent().Facility );

                IAccountBroker accountBroker =
                    BrokerFactory.BrokerOfType<IAccountBroker>();

                this.accountProxiesCollection = null;

                this.accountProxiesCollection = accountBroker.
                    AccountsMatching( patientCriteria );
            }
            else
            {
                // the logic here means: if the operation did not succeed, then consider
                // this a user cancellation as opposed to an error.
                e.Cancel = true ;
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PatientCensusSearchView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            this.searchButton.Enabled = false;

            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null)
                {
                    components.Dispose();
                }
                // cancel launched background worker
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }


        #endregion

        #region Data Elements

        private Container                     components = null;

        private BackgroundWorker              backgroundWorker;
        private LoggingButton resetButton;
        private LoggingButton                                       searchButton;
        private ValidationResult                                    result = null;
        private ICollection                                         accountProxiesCollection = null;

        private Label                          searchCriteriaMessageLabel;
        private Label                          lastNameLabel;
        private Label                          firstNameLabel;
        private Label                          accountLabel;

        private MaskedEditTextBox            lastNameTextbox;
        private MaskedEditTextBox            firstNameTextbox;
        private MaskedEditTextBox            accountTextbox;
        
        private Panel                          patientCensusSearchPanel;

        private string                                              validationResult = String.Empty;
        private string                                              aspectInError = String.Empty;

        private bool                                                i_FirstTimeLayout = true;
        private bool                                                accountNumberIsInvalid = false;

        #endregion

        #region Constants
        private const string
            ERR_MSG_FIRTSTNAME_ONLY    = "When searching by first name, " + 
            "last name is required. Either " + 
            "provide a last name, or remove " + 
            "the first name.";
        private const string
            LAST_NAME                   = "LastName";
        private const string
            ACCOUNT_NUMBER              = "AccountNumber";
        #endregion
    }
}