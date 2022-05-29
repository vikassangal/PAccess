using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.EmploymentViews
{
    /// <summary>
    /// Summary description for FormSelectEmployer.
    /// </summary>
    public class FormSelectEmployer : TimeOutFormView
    {
        #region Event Handlers
        private void button_OK_Click( object sender, EventArgs e )
        {
            if( this.tabControl1.SelectedTab != this.tabPageEmployer )
            {
                this.Model = this.coveredGroupsView1.Model_Employer;                            
            }
            else
            {
                this.Model = this.SelectedEmployer;
            }
        }

        private void selectEmployerAddress1_AddressSelected(object sender, EventArgs e)
        {
            LooseArgs args = ( LooseArgs ) e;
            bool state = Convert.ToBoolean( args.Context );
            this.button_OK.Enabled = state;
        }

        private void selectAnEmployer_SelectedEmployerChanged( object sender, EventArgs e )
        {
            OnSelectedEmployerChanged();
        }

        private void FormSelectEmployer_Load(object sender, EventArgs e)
        {
            selectAnEmployer.Activity = this.Activity;
            selectAnEmployer.UpdateView();
        }

        private void tabControl1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( i_AllowCoveredgroupTabSelect && tabControl1.SelectedTab == this.tabPageCoveredGroups )
            {
                tabControl1.SelectedTab = this.tabPageCoveredGroups;
                button_OK.Enabled = true;
            }
            else
            {
                tabControl1.SelectedTab = this.tabPageEmployer;
                if (selectEmployerAddress1 != null && selectEmployerAddress1.IsAddressSelected())
                    button_OK.Enabled = true;
                else
                    button_OK.Enabled = false;
            }
        } 

        private void coveredGroupsView1_SelectedCoveredGroupChanged( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            CoveredGroup cgp =  args.Context as CoveredGroup; 

            if( cgp.ContactPoints.Count > 0 )
            {
                foreach( ContactPoint cp in cgp.ContactPoints )
                {
                    if( cp != null && cp.TypeOfContactPoint != null
                        && cp.TypeOfContactPoint.Oid == TypeOfContactPoint.NewEmployerContactPointType().Oid )
                    {
                         this.SelectedEmployerContactPoint =  cp;
                         break;
                    }                    
                }
            }          

            this.button_OK.Enabled = true;            
            this.AcceptButton = this.button_OK;
        }

        private void selectEmployerAddress1_NoAddressChecked(object sender, EventArgs e)
        {
            this.button_OK.Enabled = true;        
        }

        private void selectEmployerAddress1_NoAddressUnchecked(object sender, EventArgs e)
        {
            this.button_OK.Enabled = false;     
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
			if( this.tabControl1.SelectedTab ==  this.tabPageCoveredGroups )
            {
                this.coveredGroupsView1.Account = Account;
                this.coveredGroupsView1.UpdateView();
            }
        }

        private void OnSelectedEmployerChanged()
		{
			selectEmployerAddress1.Model = selectAnEmployer.SelectedEmployer;
            //button_OK.Enabled = selectEmployerAddress1.SelectedAddress != null;
            button_OK.Enabled = selectEmployerAddress1.IsAddressSelected();
			selectEmployerAddress1.UpdateView();
			selectEmployerAddress1.ButtonEnabled = selectAnEmployer.EmployersFound;
			selectAnEmployer.UpdateView();
		}
        /// <summary>
        /// Set Covered groups as default tab
        /// </summary>
        public void SetCoveredGroupsAsDefaultTabPage()
        {
            //Coveredgroups - Default tab for the insured screen.
            i_AllowCoveredgroupTabSelect = true;
			//Show the CoveredGroups tab.
			this.tabControl1.TabPages.Add( this.tabPageCoveredGroups );
            tabControl1.SelectedTab = this.tabPageCoveredGroups;
            tabControl1.TabPages[1].Focus(); 
        }
        #endregion

        #region Properties
        public Employer SelectedEmployer
        {
            get
            {
                return selectAnEmployer.SelectedEmployer;
            }
        }

        public Address SelectedAddress
        {
            get
            {
                return selectEmployerAddress1.SelectedAddress;
            }
        }
        public ContactPoint SelectedContactPoint
        {
            get
            {
                return selectEmployerAddress1.SelectedContactPoint;
            }
        }
        public bool SelectNoAddressChecked
        {
            get
            {
                return selectEmployerAddress1.SelectNoAddressChecked;
            }
        }
        public ContactPoint SelectedEmployerContactPoint
        {
            get
            {
                return i_selectedEmployerContactPoint;
            }
            private set
            {
                i_selectedEmployerContactPoint = value;
            }
        }
        
        public Activity Activity
        {
            private get
            {
                return i_activity;
            }
            set
            {
                i_activity = value;
            }
        }  
        public Employer Model_Employer
        {
            get
            {
                return (Employer)this.Model;
            }
        }

        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value; 
            }
        }
        #endregion

        #region Private Methods
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectAnEmployer = new PatientAccess.UI.EmploymentViews.SelectAnEmployer();
            this.selectEmployerAddress1 = new PatientAccess.UI.EmploymentViews.SelectEmployerAddress();
            this.panelEmpAddress = new System.Windows.Forms.Panel();
            this.panelLineLabel = new System.Windows.Forms.Panel();
            this.employeerLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.employerAddressesLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageEmployer = new System.Windows.Forms.TabPage();
            this.tabPageCoveredGroups = new System.Windows.Forms.TabPage();
            this.coveredGroupsView1 = new PatientAccess.UI.EmploymentViews.CoveredGroupsView();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.button_Cancel = new LoggingButton();
            this.button_OK = new LoggingButton();
            this.panelEmpAddress.SuspendLayout();
            this.panelLineLabel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageEmployer.SuspendLayout();
            this.tabPageCoveredGroups.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectAnEmployer
            // 
            this.selectAnEmployer.Activity = null;
            this.selectAnEmployer.Location = new System.Drawing.Point(8, 23);
            this.selectAnEmployer.Model = null;
            this.selectAnEmployer.Name = "selectAnEmployer";
            this.selectAnEmployer.SelectedEmployer = null;
            this.selectAnEmployer.Size = new System.Drawing.Size(390, 206);
            this.selectAnEmployer.TabIndex = 0;
            this.selectAnEmployer.SelectedEmployerChanged += new System.EventHandler(this.selectAnEmployer_SelectedEmployerChanged);
            
            // 
            // selectEmployerAddress1
            // 
            this.selectEmployerAddress1.Location = new System.Drawing.Point(2, 7);
            this.selectEmployerAddress1.Model = null;
            this.selectEmployerAddress1.Model_Employer = null;
            this.selectEmployerAddress1.Name = "selectEmployerAddress1";
            this.selectEmployerAddress1.SelectNoAddressChecked = false;
            this.selectEmployerAddress1.Size = new System.Drawing.Size(396, 159);
            this.selectEmployerAddress1.TabIndex = 0;
            this.selectEmployerAddress1.NoAddressChecked += new System.EventHandler(this.selectEmployerAddress1_NoAddressChecked);
            this.selectEmployerAddress1.NoAddressUnchecked += new System.EventHandler(this.selectEmployerAddress1_NoAddressUnchecked);
            // 
            // panelEmpAddress
            // 
            this.panelEmpAddress.Controls.Add(this.selectEmployerAddress1);
            this.panelEmpAddress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelEmpAddress.DockPadding.Top = 5;
            this.panelEmpAddress.Location = new System.Drawing.Point(0, 248);
            this.panelEmpAddress.Name = "panelEmpAddress";
            this.panelEmpAddress.Size = new System.Drawing.Size(405, 170);
            this.panelEmpAddress.TabIndex = 1;
            // 
            // panelLineLabel
            // 
            this.panelLineLabel.Controls.Add(this.employeerLineLabel);
            this.panelLineLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLineLabel.Location = new System.Drawing.Point(0, 0);
            this.panelLineLabel.Name = "panelLineLabel";
            this.panelLineLabel.Size = new System.Drawing.Size(405, 232);
            this.panelLineLabel.TabIndex = 0;
            // 
            // employeerLineLabel
            // 
            this.employeerLineLabel.Caption = "Employers";
            this.employeerLineLabel.Location = new System.Drawing.Point(14, 9);
            this.employeerLineLabel.Name = "employeerLineLabel";
            this.employeerLineLabel.Size = new System.Drawing.Size(382, 15);
            this.employeerLineLabel.TabIndex = 15;
            this.employeerLineLabel.TabStop = false;
            // 
            // employerAddressesLineLabel
            // 
            this.employerAddressesLineLabel.Caption = "Employer Addresses";
            this.employerAddressesLineLabel.Location = new System.Drawing.Point(14, 233);
            this.employerAddressesLineLabel.Name = "employerAddressesLineLabel";
            this.employerAddressesLineLabel.Size = new System.Drawing.Size(384, 15);
            this.employerAddressesLineLabel.TabIndex = 16;
            this.employerAddressesLineLabel.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageEmployer);
            this.tabControl1.Controls.Add(this.tabPageCoveredGroups);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(413, 444);
            this.tabControl1.TabIndex = 17;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageEmployer
            // 
            this.tabPageEmployer.BackColor = System.Drawing.Color.White;
            this.tabPageEmployer.Controls.Add(this.employerAddressesLineLabel);
            this.tabPageEmployer.Controls.Add(this.selectAnEmployer);
            this.tabPageEmployer.Controls.Add(this.panelEmpAddress);
            this.tabPageEmployer.Controls.Add(this.panelLineLabel);
            this.tabPageEmployer.Location = new System.Drawing.Point(4, 22);
            this.tabPageEmployer.Name = "tabPageEmployer";
            this.tabPageEmployer.Size = new System.Drawing.Size(405, 418);
            this.tabPageEmployer.TabIndex = 1;
            this.tabPageEmployer.Text = "Employer";
            // 
            // tabPageCoveredGroups
            // 
            this.tabPageCoveredGroups.BackColor = System.Drawing.Color.White;
            this.tabPageCoveredGroups.Controls.Add(this.coveredGroupsView1);
            this.tabPageCoveredGroups.Location = new System.Drawing.Point(4, 22);
            this.tabPageCoveredGroups.Name = "tabPageCoveredGroups";
            this.tabPageCoveredGroups.Size = new System.Drawing.Size(405, 418);
            this.tabPageCoveredGroups.TabIndex = 0;
            this.tabPageCoveredGroups.Text = "Covered Groups";
            // 
            // coveredGroupsView1
            // 
            this.coveredGroupsView1.Account = null;
            this.coveredGroupsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coveredGroupsView1.DockPadding.All = 10;
            this.coveredGroupsView1.Location = new System.Drawing.Point(0, 0);
            this.coveredGroupsView1.Model = null;
            this.coveredGroupsView1.Model_Employer = null;
            this.coveredGroupsView1.Name = "coveredGroupsView1";
            this.coveredGroupsView1.Size = new System.Drawing.Size(405, 418);
            this.coveredGroupsView1.TabIndex = 0;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.button_Cancel);
            this.panelButtons.Controls.Add(this.button_OK);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(5, 457);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(413, 30);
            this.panelButtons.TabIndex = 2;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(327, 4);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Cancel";
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(244, 4);
            this.button_OK.Name = "button_OK";
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // FormSelectEmployer
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(423, 492);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.tabControl1);
            this.DockPadding.All = 5;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectEmployer";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Employer";
            this.Load += new System.EventHandler(this.FormSelectEmployer_Load);
            this.panelEmpAddress.ResumeLayout(false);
            this.panelLineLabel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageEmployer.ResumeLayout(false);
            this.tabPageCoveredGroups.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Constructors and Finalization
        public FormSelectEmployer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

			if( !i_AllowCoveredgroupTabSelect )
			{
				this.tabControl1.TabPages.RemoveAt(1);
			}

            OnSelectedEmployerChanged();
            base.EnableThemesOn( this );

            //default selected tab
            this.tabControl1.SelectedIndex = 1;  

            this.coveredGroupsView1.SelectedCoveredGroupChanged +=
                new EventHandler(coveredGroupsView1_SelectedCoveredGroupChanged);

            this.selectEmployerAddress1.AddressSelected += 
                new EventHandler(selectEmployerAddress1_AddressSelected);
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

        #region Data Elements
        private Container                         components = null;

        private LoggingButton                             button_Cancel;
        private LoggingButton                             button_OK;

        private Panel                              panelEmpAddress;
        private Panel                              panelLineLabel;
        private Panel                              panelButtons;

        private TabControl                         tabControl1;

        private TabPage                            tabPageCoveredGroups;
        private TabPage                            tabPageEmployer;

        private SelectAnEmployer       selectAnEmployer;
        private SelectEmployerAddress  selectEmployerAddress1;

        private LineLabel               employeerLineLabel;
        private LineLabel               employerAddressesLineLabel;

        private CoveredGroupsView      coveredGroupsView1;

        private bool                                                    i_AllowCoveredgroupTabSelect;
//        private InsurancePlan i_InsurancePlan = null;
        private Account                                                 i_Account = null;
        private Activity                                                i_activity;
        private ContactPoint i_selectedEmployerContactPoint             = new EmployerContactPoint();                                
        #endregion

        #region Constants
        #endregion       
    }
}
