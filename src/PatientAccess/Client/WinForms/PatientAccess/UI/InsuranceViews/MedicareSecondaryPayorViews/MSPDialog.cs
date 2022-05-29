using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for MSPDialog.
    /// </summary>
    public class MSPDialog : TimeOutFormView
    {
        #region Events
        public event EventHandler MspButtonClickEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// linkLabelStateIndex is used to clear the LinkLabels to the right of the label associated
        /// with the currently visible state form if the user moves back to a previously viewed form
        /// and makes a change to one or more of the controls on that form.  The Welcome screen doesn't
        /// have a LinkLabel associated with it, so if the user hasn't clicked "Don't show this again"
        /// on the Welcome screen the index starts at -1.  Without the Welcome screen the first screen
        /// is Special Programs which does have a LinkLabel, so initialize the index to zero.
        /// </summary>
        private void MSPDialog_Load(object sender, EventArgs e)
        {
            linkLabelStateIndex = showWelcomeScreen ? -1 : 0;
        }

        /// <summary>
        /// Destroys the state machine and the ControlViews created by the states.
        /// </summary>
        private void MSPDialog_Closing(object sender, CancelEventArgs e)
        {
            //Hide();
            fsm.Dispose();
        }

        /// <summary>
        /// Back button sets the FSM to the previous state and displays it's view.
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            RemoveStateView();
            fsm.SetState( fsm.GetState().GetPreviousState(), true );
            SetButtonStates();
            AddStateView();
            linkLabelStateIndex--;
            fsm.GetState().GetView().UpdateView();

            if( showWelcomeScreen && fsm.GetStateString().Equals( "WelcomeScreenState" ) )
            {
                checkBox.Visible = true;
                HideLinkLabels();
            }
        }

        /// <summary>
        /// Cancels the wizard.  Closing event handler cleans up the FSM.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
//            Hide();
//            Close();
        }

        /// <summary>
        /// Closes the wizard.  Closing event handler cleans up the FSM.
        /// </summary>
        private void btnFinish_Click(object sender, EventArgs e)
        {
            if( ShowingSummary == false )
            {
                userPreferences.ShowMSPWelcomeScreen = checkBox.Checked ? false : true;
                userPreferences.Save( userPreferences );
            }
//            Hide();
//            Close();
        }

        /// <summary>
        /// Next button sets the FSM to the next logical state 
        /// based on the user's inputs and displays it's view
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if( showWelcomeScreen )
            {
                checkBox.Visible = false;
                RedisplayLinkLabels();
            }
            RemoveStateView();
            fsm.ConsumeEvent( fsm.GetState().Response );
            SetButtonStates();
            AddStateView();
            SetNextLinkLabel( fsm.GetStateString() );
            linkLabelStateIndex++;
        }

        /// <summary>
        /// LinkLabel click sets the FSM to the state associated with the label
        /// and displays the state's view.  If the user makes a change from that
        /// screen, all subsequent views are disposed and re-instantiated.
        /// </summary>
        private void linkLabel_Click(object sender, EventArgs e)
        {
            LinkLabel linkLabel = sender as LinkLabel;

            // Save LinkLabel index value. If user makes a change to the
            // form, we must clear all the LinkLabels to the right of this one.
            if( linkLabel.Tag != null )
            {
                linkLabelStateIndex = (int) linkLabel.Tag;
            }

            if( fsm.GetState().GetStateString().Equals( linkLabel.Text ) )
            {   // Prevent screen flicker from resetting current form
                return;
            }

            RemoveStateView();
            if( linkLabel.Text != string.Empty && linkLabel.Text != null )
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "Select {0}", linkLabel.Text ), 
                    this.Model_Account );
            }

            switch( linkLabel.Text )
            {
                case "Special Programs":
                    // Manually force the state machine back to SpecialProgram state.
                    // 'true' means set Forms' Tag property to watch for form changes
                    fsm.SetState( SpecialProgramState.stateExemplar, true );
                    break;

                case "Liability Insurer":
                    fsm.SetState( LiabilityInsurerState.stateExemplar, true );
                    break;

                case "Medicare Entitlement":
                    fsm.SetState( MedicareEntitlementState.stateExemplar, true );
                    break;

                case "Entitlement by ESRD":
                    fsm.SetState( ESRDEntitlementPage1State.stateExemplar, true );
                    break;

                case "Entitlement by Disability":
                    fsm.SetState( DisabilityEntitlementPage1State.stateExemplar, true );
                    break;

                case "Entitlement by Age":
                    fsm.SetState( AgeEntitlementPage1State.stateExemplar, true );
                    break;

                case "Summary":
                    fsm.SetState( SummaryPageState.stateExemplar, false );
                    break;
            }
            if( fsm.GetPreviousState() != null )
            {   // Prevent control selections from disappearing if state is re-entered.
                fsm.GetPreviousState().FormWasChanged = false;
            }
            SetButtonStates();
            AddStateView();
            fsm.GetState().GetView().UpdateView();
        }
        #endregion

        #region Methods
        /// <summary>
        /// When the user clicks and edit button on a wizard screen this method
        /// is called which raises an event on the parent form, FinancialClassesView,
        /// which closes the wizard and through a series of events, ultimately
        /// displays the proper tab page on the AccountView.
        /// </summary>
        public void RaiseTabSelectedEvent( int index )
        {
            if( MspButtonClickEvent != null )
            {
                MspButtonClickEvent( this, new LooseArgs( index ) );
            }

            this.Close();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Add the next state form to the dialog's control collection.
        /// </summary>
        private void AddStateView()
        {
            SuspendLayout();
            panelMain.Controls.Add( fsm.GetStateView() );
            ResumeLayout( false );

            if( fsm.GetStateView() != null )
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "loading {0}", fsm.GetStateView().Name ) );
            }
        }

        /// <summary>
        /// When the user clicks on a LinkLabel the labels to the right of the clicked 
        /// one must be hidden. The labels Control.Tag value holds a unique number.
        /// The labels whose Tag value is greater than the clicked tag's value gets hidden.
        /// </summary>
        public void ClearLinkLabels()
        {
            foreach( Control control in panelLinks.Controls )
            {
                if( control is LinkLabel && control.Tag != null &&
                    ((int) control.Tag > linkLabelStateIndex) )
                {
                    control.Tag = null;
                    control.Visible = false;
                    linkLabelIndex--;
                }
            }
        }

        /// <summary>
        /// If the Welcome screen is not disabled, hide all the LinkLabels if the user returns to it.
        /// </summary>
        private void HideLinkLabels()
        {
            foreach( Control control in panelLinks.Controls )
            {
                if( control is LinkLabel && control.Tag != null &&
                    ((int) control.Tag > linkLabelStateIndex) )
                {
                    control.Visible = false;
                }
            }
        }

        /// <summary>
        /// If the Welcome screen is not disabled, redisplay all the LinkLabels if the user leaves it.
        /// </summary>
        private void RedisplayLinkLabels()
        {
            // Redisplay the LinkLabels in the correct order
            for( int ix = 0; ix < 8; ix++ )
            {
                foreach( Control control in panelLinks.Controls )
                {
                    if( control is LinkLabel && control.Tag != null && ix == (int) control.Tag )
                    {
                        control.Visible = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the current state Form the the dialog's control collection.
        /// </summary>
        private void RemoveStateView()
        {
            SuspendLayout();
            panelMain.Controls.Remove( fsm.GetStateView() );
            ResumeLayout( false );
        }

        /// <summary>
        /// Set the properties of the buttons appropriate to the current state.
        /// </summary>
        private void SetButtonStates()
        {
            btnBack.Enabled = fsm.GetState().IsIdleState == false;

            if( fsm.GetState().IsEndState )
            {
                AcceptButton = btnFinish;
                btnContinue.Visible = false;
                btnCancel.Location = new Point( 376, 592 );
                btnBack.Location = new Point( 461, 592 );
                if( ShowingSummary )
                {
                    btnBack.Enabled = false;
                }
                btnNext.Location = new Point( 541, 592 );
                btnFinish.Visible = true;
                btnFinish.Focus();
            }
            else
            {
                btnCancel.Location = new Point( 323, 592 );
                btnBack.Location = new Point( 408, 592 );
                btnNext.Location = new Point( 488, 592 );
                btnContinue.Visible = true;
                btnFinish.Visible = false;
            }
        }

        /// <summary>
        /// When Next button is clicked, the LinkLabel whose text matches the
        /// state's text string is found and displayed.  Since we can't know
        /// the order that the states, the Point locations are stored in a
        /// list and the index is set to the label's Control.Tag to order them
        /// for clearing multiple labels when the user jumps back.
        /// </summary>
        private void SetNextLinkLabel( string labelName )
        {
            // The state name matches the name of a link label.
            // Find linkLabel with matching name
            Point point = new Point(0);

            if( fsm.GetState().FormWasChanged )
            {
                ClearLinkLabels();
            }

            foreach( Control control in panelLinks.Controls )
            {
                if( control.Text.Equals( labelName ) )
                {
                    LinkLabel linkLabel = control as LinkLabel;
                    if( linkLabel.Visible )
                    {
                        return;
                    }
                    linkLabelIndex += 1;
                    linkLabel.Tag = linkLabelIndex;

                    if( linkLabelIndex == 0 )
                    {
                        point = new Point( 0, 0 );
                    }
                    else
                    {
                        // Find the previous LinkLabel and measure it's string length
                        // to determine the location of the next LinkLabel
                        foreach( Control ctrl in panelLinks.Controls )
                        {
                            LinkLabel label = ctrl as LinkLabel;
                            
                            if( label.Tag != null && label.Tag.Equals( linkLabelIndex - 1 ) )
                            {
                                Graphics g = CreateGraphics();
                                SizeF size = g.MeasureString( label.Text, this.Font ).ToSize();
                                Point lastLocation = new Point( label.Location.X + (int) size.Width + 15,
                                                                label.Location.Y );
                                point = new Point( lastLocation.X, 0 );
                                break;
                            }
                        }
                    }
                    linkLabel.Location = new Point( point.X, point.Y );
                    linkLabel.TabIndex = btnFinish.TabIndex + linkLabelIndex + 1;
                    linkLabel.Visible = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Override WndProc to handle messages sent from the state forms to set the proper button states.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc( ref Message m )
        {

            switch( m.Msg )
            {
                case WM_ACTIVATE:
                {
                    // Don't allow windows to handle this message.  When the Welcome
                    // screen has it's "Don't show this again" checkbox checked, the 
                    // Special Programs screen is the first screen loaded.  The first
                    // control on it that can receive focus is the first radio button 
                    // which automatically goes to the checked state.  Disabling 
                    // WM_ACTIVATE prevents this from happening.
                    if( ((int)m.WParam & 0xFFFF) != WA_INACTIVE )
                    {
                        return;
                    }
                    break;
                } 
                case CONTINUE_BUTTON_DISABLED:
                    if( continueEnabled )
                    {
                        btnContinue.Enabled = false;
                        continueEnabled = false;
                    }
                    break;

                case CONTINUE_BUTTON_ENABLED:
                    if( continueEnabled == false )
                    {
                        btnContinue.Enabled = true;
                        continueEnabled = true;
                    }
                    break;

                case CONTINUE_BUTTON_FOCUS:
                    if( btnContinue.CanFocus )
                    {
//                        btnContinue.Focus();  CAUSES INTERMITTENT THREAD HANG
                    }
                    break;

                case NEXT_BUTTON_DISABLED:
                    if( nextEnabled )
                    {
                        btnNext.Enabled = false;
                        nextEnabled = false;
                    }
                    break;

                case NEXT_BUTTON_ENABLED:
                    if( nextEnabled == false )
                    {
                        btnNext.Enabled = true;
                        nextEnabled = true;
                    }
                    break;

                case NEXT_BUTTON_FOCUS:
                    if( btnNext.CanFocus )
                    {
//                        btnNext.Focus();  CAUSES INTERMITTENT THREAD HANG
                    }
                    break;
            }
            base.WndProc( ref m );
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Creates the Finite State Machine and gets the Account data into the FSM,
        /// sets the FSM to the idle state, and displays the state's view.
        /// </summary>
        public MSPDialog( Account account, MedicareSecondaryPayor msp, bool showSummary )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            EnableThemesOn( this );
            this.btnCancel.Message = "MSP cancelled";
            this.btnFinish.Message = "MSP completed";
            fsm = new FiniteStateMachine( this );
            // ShowingSummary property enables SummaryPageView to know  
            // which data model to use on the MSP Summary data analysis.
            // (Normally it uses a MedicareSecondaryPayor created on the stack.)
            ShowingSummary = showSummary;

            if( showSummary )
            {
                checkBox.Visible = false;
                fsm.SetSummaryState( account );
                SummaryPageState.stateExemplar.View.UpdateView();
                SetButtonStates();
                AddStateView();
            }
            else
            {
                // UserPreferences in Isolated Storage determines if Welcome screen is shown
                userPreferences = UserPreference.Load();
                showWelcomeScreen = userPreferences.ShowMSPWelcomeScreen;
                checkBox.Visible = showWelcomeScreen;
                fsm.SetIdleState( showWelcomeScreen, account, msp );
                SetButtonStates();
                AddStateView();

                if( showWelcomeScreen == false )
                {   // Display the Special Program linklabel
                    SetNextLinkLabel( fsm.GetStateString() );
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private Account Model_Account
        {
            get
            {
                return (Account)this.Model;
            }
        }

        [Browsable(false)]
        public bool ShowingSummary
        {
            get
            {
                return i_displaySummary;
            }
            private set
            {
                btnNext.Enabled = false;
                i_displaySummary = value;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnContinue = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnNext = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnBack = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelLinks = new System.Windows.Forms.Panel();
            this.linkESRD = new System.Windows.Forms.LinkLabel();
            this.linkEntitlement = new System.Windows.Forms.LinkLabel();
            this.linkSummary = new System.Windows.Forms.LinkLabel();
            this.linkAge = new System.Windows.Forms.LinkLabel();
            this.linkDiability = new System.Windows.Forms.LinkLabel();
            this.linkLiability = new System.Windows.Forms.LinkLabel();
            this.linkSpecial = new System.Windows.Forms.LinkLabel();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.btnFinish = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelLinks.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Black;
            this.panelMain.Location = new System.Drawing.Point(16, 56);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(682, 522);
            this.panelMain.TabIndex = 1;
            this.panelMain.TabStop = true;
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnContinue.Enabled = false;
            this.btnContinue.Location = new System.Drawing.Point(573, 592);
            this.btnContinue.Message = null;
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(125, 23);
            this.btnContinue.TabIndex = 6;
            this.btnContinue.Text = "&Continue to Summary";
            this.btnContinue.Visible = false;
            this.btnContinue.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(520, 592);
            this.btnNext.Message = null;
            this.btnNext.Name = "btnNext";
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "&Next>";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(437, 592);
            this.btnBack.Message = null;
            this.btnBack.Name = "btnBack";
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "<&Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(352, 592);
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelLinks
            // 
            this.panelLinks.Controls.Add(this.linkESRD);
            this.panelLinks.Controls.Add(this.linkEntitlement);
            this.panelLinks.Controls.Add(this.linkSummary);
            this.panelLinks.Controls.Add(this.linkAge);
            this.panelLinks.Controls.Add(this.linkDiability);
            this.panelLinks.Controls.Add(this.linkLiability);
            this.panelLinks.Controls.Add(this.linkSpecial);
            this.panelLinks.Location = new System.Drawing.Point(16, 16);
            this.panelLinks.Name = "panelLinks";
            this.panelLinks.Size = new System.Drawing.Size(650, 24);
            this.panelLinks.TabIndex = 7;
            // 
            // linkESRD
            // 
            this.linkESRD.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkESRD.Location = new System.Drawing.Point(176, 0);
            this.linkESRD.Name = "linkESRD";
            this.linkESRD.Size = new System.Drawing.Size(112, 23);
            this.linkESRD.TabIndex = 0;
            this.linkESRD.TabStop = true;
            this.linkESRD.Text = "Entitlement by ESRD";
            this.linkESRD.Visible = false;
            this.linkESRD.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkEntitlement
            // 
            this.linkEntitlement.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkEntitlement.Location = new System.Drawing.Point(272, 0);
            this.linkEntitlement.Name = "linkEntitlement";
            this.linkEntitlement.Size = new System.Drawing.Size(112, 23);
            this.linkEntitlement.TabIndex = 0;
            this.linkEntitlement.TabStop = true;
            this.linkEntitlement.Text = "Medicare Entitlement";
            this.linkEntitlement.Visible = false;
            this.linkEntitlement.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkSummary
            // 
            this.linkSummary.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkSummary.Location = new System.Drawing.Point(584, 0);
            this.linkSummary.Name = "linkSummary";
            this.linkSummary.Size = new System.Drawing.Size(58, 23);
            this.linkSummary.TabIndex = 0;
            this.linkSummary.TabStop = true;
            this.linkSummary.Text = "Summary";
            this.linkSummary.Visible = false;
            this.linkSummary.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkAge
            // 
            this.linkAge.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkAge.Location = new System.Drawing.Point(486, 0);
            this.linkAge.Name = "linkAge";
            this.linkAge.TabIndex = 0;
            this.linkAge.TabStop = true;
            this.linkAge.Text = "Entitlement by Age";
            this.linkAge.Visible = false;
            this.linkAge.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkDiability
            // 
            this.linkDiability.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkDiability.Location = new System.Drawing.Point(368, 0);
            this.linkDiability.Name = "linkDiability";
            this.linkDiability.Size = new System.Drawing.Size(125, 23);
            this.linkDiability.TabIndex = 0;
            this.linkDiability.TabStop = true;
            this.linkDiability.Text = "Entitlement by Disability";
            this.linkDiability.Visible = false;
            this.linkDiability.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkLiability
            // 
            this.linkLiability.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLiability.Location = new System.Drawing.Point(97, 0);
            this.linkLiability.Name = "linkLiability";
            this.linkLiability.Size = new System.Drawing.Size(85, 23);
            this.linkLiability.TabIndex = 0;
            this.linkLiability.TabStop = true;
            this.linkLiability.Text = "Liability Insurer";
            this.linkLiability.Visible = false;
            this.linkLiability.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // linkSpecial
            // 
            this.linkSpecial.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkSpecial.Location = new System.Drawing.Point(0, 0);
            this.linkSpecial.Name = "linkSpecial";
            this.linkSpecial.Size = new System.Drawing.Size(94, 23);
            this.linkSpecial.TabIndex = 0;
            this.linkSpecial.TabStop = true;
            this.linkSpecial.Text = "Special Programs";
            this.linkSpecial.Visible = false;
            this.linkSpecial.Click += new System.EventHandler(this.linkLabel_Click);
            // 
            // checkBox
            // 
            this.checkBox.Location = new System.Drawing.Point(16, 592);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(136, 24);
            this.checkBox.TabIndex = 2;
            this.checkBox.Text = "Don\'t show this again";
            // 
            // btnFinish
            // 
            this.btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnFinish.Location = new System.Drawing.Point(626, 592);
            this.btnFinish.Message = null;
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.TabIndex = 6;
            this.btnFinish.Text = "Fini&sh";
            this.btnFinish.Visible = false;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // MSPDialog
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(716, 627);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.checkBox);
            this.Controls.Add(this.panelLinks);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MSPDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medicare Secondary Payor";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MSPDialog_Closing);
            this.Load += new System.EventHandler(this.MSPDialog_Load);
            this.panelLinks.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container         components = null;
        public  LoggingButton             btnNext;
        private LoggingButton             btnBack;
        private LoggingButton             btnCancel;
        public  LoggingButton             btnContinue;
        private LoggingButton             btnFinish;

        private CheckBox           checkBox;
        
        private Panel              panelMain;
        private Panel              panelLinks;
        
        private LinkLabel          linkSpecial;
        private LinkLabel          linkLiability;
        private LinkLabel          linkEntitlement;
        private LinkLabel          linkESRD;
        private LinkLabel          linkDiability;
        private LinkLabel          linkAge;
        private LinkLabel          linkSummary;

        private UserPreference     userPreferences;

        private FiniteStateMachine                      fsm;
        private bool                                    i_displaySummary;
        private static bool                              showWelcomeScreen;
        private bool                                    continueEnabled;
        private bool                                    nextEnabled;
        private int                                     linkLabelIndex = -1;
        private int                                     linkLabelStateIndex;
        #endregion

        #region Constants
        const Int32                                     WM_ACTIVATE              = 0x006;
        const Int32                                     WA_INACTIVE              = 0;
        const Int32                                     WM_USER                  = 0x400;
        const Int32                                     CONTINUE_BUTTON_DISABLED = WM_USER + 1;
        const Int32                                     CONTINUE_BUTTON_ENABLED  = WM_USER + 2;
        const Int32                                     CONTINUE_BUTTON_FOCUS    = WM_USER + 3;
        const Int32                                     NEXT_BUTTON_DISABLED     = WM_USER + 4;
        const Int32                                     NEXT_BUTTON_ENABLED      = WM_USER + 5;
        const Int32                                     NEXT_BUTTON_FOCUS        = WM_USER + 6;
        #endregion
    }
}
