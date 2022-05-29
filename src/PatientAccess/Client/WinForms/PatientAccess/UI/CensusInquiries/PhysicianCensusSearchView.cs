using System;
using System.Collections;
using System.ComponentModel;
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
    /// Census by Physician search view
    /// </summary>
    [Serializable]
    public class PhysicianCensusSearchView : ControlView
    {
       
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.physicianSearchPanel = new System.Windows.Forms.Panel();
            this.resetButton = new LoggingButton();
            this.searchButton = new LoggingButton();
            this.numberTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.firstNameTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lastNameTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.numberLabel = new System.Windows.Forms.Label();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.physicianSearchPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianSearchPanel
            // 
            this.physicianSearchPanel.Controls.Add(this.resetButton);
            this.physicianSearchPanel.Controls.Add(this.searchButton);
            this.physicianSearchPanel.Controls.Add(this.numberTextbox);
            this.physicianSearchPanel.Controls.Add(this.firstNameTextbox);
            this.physicianSearchPanel.Controls.Add(this.lastNameTextbox);
            this.physicianSearchPanel.Controls.Add(this.numberLabel);
            this.physicianSearchPanel.Controls.Add(this.firstNameLabel);
            this.physicianSearchPanel.Controls.Add(this.lastNameLabel);
            this.physicianSearchPanel.Controls.Add(this.instructionsLabel);
            this.physicianSearchPanel.Location = new System.Drawing.Point(0, 0);
            this.physicianSearchPanel.Name = "physicianSearchPanel";
            this.physicianSearchPanel.Size = new System.Drawing.Size(416, 167);
            this.physicianSearchPanel.TabIndex = 0;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(323, 135);
            this.resetButton.Name = "resetButton";
            this.resetButton.TabIndex = 5;
            this.resetButton.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(243, 135);
            this.searchButton.Name = "searchButton";
            this.searchButton.TabIndex = 4;
            this.searchButton.Text = "Sear&ch";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // numberTextbox
            // 
            this.numberTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.numberTextbox.KeyPressExpression = "^[0-9]*$";
            this.numberTextbox.Location = new System.Drawing.Point(126, 98);
            this.numberTextbox.Mask = "";
            this.numberTextbox.MaxLength = 5;
            this.numberTextbox.Name = "numberTextbox";
            this.numberTextbox.Size = new System.Drawing.Size(40, 20);
            this.numberTextbox.TabIndex = 3;
            this.numberTextbox.ValidationExpression = "^[0-9]*$";
            this.numberTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.numberTextbox_Validating);
            this.numberTextbox.TextChanged += new System.EventHandler(this.numberTextbox_TextChanged);
            this.numberTextbox.Enter += new System.EventHandler(this.numberTextbox_Enter);
            // 
            // firstNameTextbox
            // 
            this.firstNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.firstNameTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.firstNameTextbox.KeyPressExpression = "^[a-zA-z][a-zA-Z0-9\\-\\ ]*$";
            this.firstNameTextbox.Location = new System.Drawing.Point(126, 73);
            this.firstNameTextbox.Mask = "";
            this.firstNameTextbox.MaxLength = 15;
            this.firstNameTextbox.Name = "firstNameTextbox";
            this.firstNameTextbox.Size = new System.Drawing.Size(176, 20);
            this.firstNameTextbox.TabIndex = 2;
            this.firstNameTextbox.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\-\\ ]*$";
            this.firstNameTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.firstNameTextbox_Validating);
            this.firstNameTextbox.TextChanged += new System.EventHandler(this.firstNameTextbox_TextChanged);
            this.firstNameTextbox.Enter += new System.EventHandler(this.firstNameTextbox_Enter);
            // 
            // lastNameTextbox
            // 
            this.lastNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.lastNameTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.lastNameTextbox.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\-\\ ]*$";
            this.lastNameTextbox.Location = new System.Drawing.Point(126, 48);
            this.lastNameTextbox.Mask = "";
            this.lastNameTextbox.MaxLength = 25;
            this.lastNameTextbox.Name = "lastNameTextbox";
            this.lastNameTextbox.Size = new System.Drawing.Size(272, 20);
            this.lastNameTextbox.TabIndex = 1;
            this.lastNameTextbox.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\-\\ ]*$";
            this.lastNameTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.lastNameTextbox_Validating);
            this.lastNameTextbox.TextChanged += new System.EventHandler(this.lastNameTextbox_TextChanged);
            this.lastNameTextbox.Enter += new System.EventHandler(this.lastNameTextbox_Enter);
            // 
            // numberLabel
            // 
            this.numberLabel.Location = new System.Drawing.Point(7, 101);
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size(112, 16);
            this.numberLabel.TabIndex = 0;
            this.numberLabel.Text = "Physician number:";
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.Location = new System.Drawing.Point(7, 76);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(112, 16);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "Physician first name:";
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.Location = new System.Drawing.Point(7, 51);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(112, 16);
            this.lastNameLabel.TabIndex = 0;
            this.lastNameLabel.Text = "Physician last name:";
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.Location = new System.Drawing.Point(7, 7);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(416, 33);
            this.instructionsLabel.TabIndex = 0;
            this.instructionsLabel.Text = "Search by Name, Number, or both. When searching by First Name, Last Name is requi" +
                "red. Partial search is supported for Last and First Names, but not for Number.";
            // 
            // PhysicianCensusSearchView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.physicianSearchPanel);
            this.Name = "PhysicianCensusSearchView";
            this.Size = new System.Drawing.Size(416, 167);
            this.physicianSearchPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers
        public event EventHandler FocusOutOfBtnReset;
        public event EventHandler AcceptButtonChanged;
		public event EventHandler PhysiciansFound;
        public event EventHandler NoPhysiciansFound;
        public event EventHandler PhysicianSearchReset;
        public event EventHandler PreviousSelectedPhysicianReset;


        private void searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                IHDIService aService = BrokerFactory.BrokerOfType<IHDIService>();

                if( Validate() )
                {
                    if( PhysicianSearchReset != null )
                    {
                        PhysicianSearchReset( sender, e );
                    }
                    long number;
                    if( numberTextbox.Text == string.Empty )
                    {
                        number = Convert.ToInt64( null );
                    }
                    else
                    {
                        number = Convert.ToInt64( numberTextbox.Text );
                    }
            
                    this.Cursor = Cursors.WaitCursor;
                    PhysicianSearchCriteria physicianCriteria = new 
                        PhysicianSearchCriteria( User.GetCurrent().Facility,
                        firstNameTextbox.Text, 
                        lastNameTextbox.Text, 
                        number );
                    IPhysicianBroker broker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                    ICollection physiciansCollection = 
                        broker.PhysiciansMatching( physicianCriteria );

                    if( PhysiciansFound != null && physiciansCollection.Count > 0 )
                    {
                        PhysiciansFound( this, 
                            new LooseArgs( physiciansCollection ) );
                    } 
                    else
                    {
                        if( NoPhysiciansFound != null )
                        {
                            NoPhysiciansFound( sender, e );
                        }
                    }
                }
            }
            catch (RemotingTimeoutException)
            {
                MessageBox.Show(UIErrorMessages.TIMEOUT_CENSUS_REPORT_DISPLAY);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void resetButton_Click( object sender, EventArgs e )
        {
            firstNameTextbox.ResetText();
            lastNameTextbox.ResetText();
            numberTextbox.ResetText();
            searchButton.Enabled = false;
            if( PhysicianSearchReset != null )
            {
                PhysicianSearchReset( sender, e );
            }
            if( PreviousSelectedPhysicianReset != null )
            {
                PreviousSelectedPhysicianReset( sender, e );
            }
        }

		private void numberTextbox_Enter( object sender, EventArgs e )
		{
			if( this.searchButton.Enabled )
			{
				this.AcceptButton = this.searchButton;
			}
		}
	
		private void lastNameTextbox_Enter( object sender, EventArgs e )
		{
			if( this.searchButton.Enabled )
			{
				this.AcceptButton = this.searchButton;
			}
		}

		private void firstNameTextbox_Enter( object sender, EventArgs e )
		{
			if( this.searchButton.Enabled )
			{
				this.AcceptButton = this.searchButton;
			}
		}

        private void lastNameTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForData();
			if( ( this.searchButton.Enabled == false ) && ( AcceptButtonChanged != null ) )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}
        }

        private void firstNameTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForData();
			if( ( this.searchButton.Enabled == false ) && ( AcceptButtonChanged != null ) )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}
        }

        private void numberTextbox_TextChanged( object sender, EventArgs e )
        {
            CheckForData();
			if( ( this.searchButton.Enabled == false ) && ( AcceptButtonChanged != null ) )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}
        }

		private void lastNameTextbox_Validating(object sender, CancelEventArgs e)
		{
			if( AcceptButtonChanged != null )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}		
		}

		private void firstNameTextbox_Validating(object sender, CancelEventArgs e)
		{
			if( AcceptButtonChanged != null )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}		
		}

		private void numberTextbox_Validating(object sender, CancelEventArgs e)
		{
			if( AcceptButtonChanged != null )
			{
				AcceptButtonChanged( this, new LooseArgs( this ) );
			}
		}

        private void resetButton_LostFocus(object sender, EventArgs e)
        {
            if( FocusOutOfBtnReset != null )
            {
                this.FocusOutOfBtnReset( null, EventArgs.Empty );
            }            
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        
        private void CheckForData()
        {
            if( ( lastNameTextbox.Text.Trim().Length == 0 ) &&
                ( firstNameTextbox.Text.Trim().Length == 0 ) &&
                ( numberTextbox.Text.Trim().Length == 0 ) )
            {
                searchButton.Enabled = false;
            }
            else
            {
                searchButton.Enabled = true;
				if( this.ParentForm != null )
				{
					this.AcceptButton = this.searchButton;
				}
            }
        }

        private new bool Validate()
        {
            if( ( firstNameTextbox.Text.Trim().Length > 0 ) &&
                ( lastNameTextbox.Text.Trim().Length == 0 ) )
            {
                MessageBox.Show( UIErrorMessages.FIRSTNAME_WITHOUT_LASTNAME_ERRMSG,
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation );
                lastNameTextbox.SelectAll();
                lastNameTextbox.Focus();
                return false;
            }

            return true;
        }

        private void WireUpMiscEvents()
        {
            this.resetButton.LostFocus += new EventHandler(resetButton_LostFocus);
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PhysicianCensusSearchView()
        {
            this.InitializeComponent();
            this.WireUpMiscEvents();
            base.EnableThemesOn( this );
            this.searchButton.Enabled = false;
        }

        #endregion

        #region Data Elements

        private Panel physicianSearchPanel;
        private LoggingButton searchButton;
        private LoggingButton resetButton;
        private Label numberLabel;
        private MaskedEditTextBox numberTextbox;
        private MaskedEditTextBox firstNameTextbox;
        private Label firstNameLabel;
        private Label lastNameLabel;
        private Label instructionsLabel;
        private MaskedEditTextBox lastNameTextbox;
 
        #endregion

        #region Constants
        #endregion

    }
}