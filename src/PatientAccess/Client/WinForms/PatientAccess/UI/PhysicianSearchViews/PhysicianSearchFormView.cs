using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.PhysicianSearchViews
{
    /// <summary>
    /// Summary description for PhysicianSearchFormView.
    /// </summary>
    [Serializable]
    public class PhysicianSearchFormView : TimeOutFormView
    {
        #region Event Handlers

        protected override void OnClosing( CancelEventArgs e )
        {
            if ( !physicianSearchTabView1.recordNonStaffPhysicianView1.RunRules() )
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing( e );
        }

        private void physicianSearchTabView1_SetCurrentTabPage( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            string tabPage = args.Context as string;

            CallingObject = tabPage;

            UpdateOKButton();
        }

        private void physicianSearchTabView1_EnableOkButton( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            yesNo = args.Context as YesNoFlag;
            if ( yesNo != null )
            {
                if ( yesNo.Description == "Yes" && RelationshipChecked )
                {
                    btnOk.Enabled = true;
                }
                else
                {
                    btnOk.Enabled = false;
                }
            }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            if ( CallingObject == "NONSTAFF" || CallingObject == "VIEWDETAIL" )
            {
                VerifyNonstaffPhysician();
            }
            else
            {
                VerifyPhysician();
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            Dispose();
        }

        private void ResetButton_Clicked( object sender, EventArgs e )
        {
            physicianRelationshipView1.UpdateView();
            PhysicianSelected = false;
            btnOk.Enabled = false;
        }

        private void PhysicianNotFound( object sender, EventArgs e )
        {
            PhysicianSelected = false;
            UpdateOKButton();
        }

        private void PhysicianFound( object sender, EventArgs e )
        {
            PhysicianSelected = true;

            LooseArgs args = ( LooseArgs )e;
            SelectedPhysician = args.Context as Physician;

            UpdateOKButton();
        }

        private void Relationship_Checked( object sender, EventArgs e )
        {
            RelationshipChecked = true;
            UpdateOKButton();
        }

        private void NoRelationship_Checked( object sender, EventArgs e )
        {
            RelationshipChecked = false;
            UpdateOKButton();
        }
        #endregion

        #region Public Methods
        public override void UpdateView()
        {
            physicianSearchTabView1.Model = Model;
            physicianSearchTabView1.CallingObject = CallingObject;
            physicianSearchTabView1.PhysicianRelationshipToView = PhysicianRelationshipToView;
            physicianSearchTabView1.UpdateView();
            physicianRelationshipView1.Model = Model;
            physicianRelationshipView1.CallingObject = CallingObject;
            physicianRelationshipView1.PhysicianRelationshipToView = PhysicianRelationshipToView;
            physicianRelationshipView1.UpdateView();
        }

        public static PhysicianSearchFormView GetPhysicianSearchForm( AccountView.ScreenIndexes screenIndex, Account account )
        {
            var physicianSearchForm = new PhysicianSearchFormView();

            switch ( screenIndex )
            {
                case AccountView.ScreenIndexes.REFERRINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.REFERRING_PHYSICIAN;
                    break;
                case AccountView.ScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ADMITTING_PHYSICIAN;
                    break;
                case AccountView.ScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ATTENDING_PHYSICIAN;
                    break;
                case AccountView.ScreenIndexes.OPERATINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.OPERATING_PHYSICIAN;
                    break;
                case AccountView.ScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.PRIMARYCARE_PHYSICIAN;
                    break;
            }

            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.Model = account;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = PhysicianSearchTabView.NON_STAFF_PHYSICIAN_TAB;
            return physicianSearchForm;
        }

        public static PhysicianSearchFormView GetPhysicianSearchForm( ShortAccountView.ShortRegistrationScreenIndexes screenIndex, Account account )
        {
            var physicianSearchForm = new PhysicianSearchFormView();

            switch ( screenIndex )
            {
                case ShortAccountView.ShortRegistrationScreenIndexes.REFERRINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.REFERRING_PHYSICIAN;
                    break;
                case ShortAccountView.ShortRegistrationScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ADMITTING_PHYSICIAN;
                    break;
                case ShortAccountView.ShortRegistrationScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ATTENDING_PHYSICIAN;
                    break;
                case ShortAccountView.ShortRegistrationScreenIndexes.OPERATINGNONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.OPERATING_PHYSICIAN;
                    break;
                case ShortAccountView.ShortRegistrationScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN:
                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.PRIMARYCARE_PHYSICIAN;
                    break;
            }

            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.Model = account;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = PhysicianSearchTabView.NON_STAFF_PHYSICIAN_TAB;
            return physicianSearchForm;
        }

        #endregion

        #region Public Properties
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

        public bool OperatingPhysicianSelectionVisibility
        {
            get { return physicianRelationshipView1.OperatingPhysicianSelectionVisibility; }
            set { physicianRelationshipView1.OperatingPhysicianSelectionVisibility = value; }
        }

        public bool AttendingPhysicianSelectionVisibility
        {
            get { return physicianRelationshipView1.AttendingPhysicianSelectionVisibility; }
            set { physicianRelationshipView1.AttendingPhysicianSelectionVisibility = value; }
        }

        public bool PrimaryCarePhysicianSelectionVisibility
        {
            get { return physicianRelationshipView1.PrimaryCarePhysicianSelectionVisibility; }
            set { physicianRelationshipView1.PrimaryCarePhysicianSelectionVisibility = value; }
        }

        #endregion

        #region Private Methods
        private void UpdateOKButton()
        {
            if ( !string.IsNullOrEmpty( CallingObject ) && yesNo != null && RelationshipChecked )
            {
                if ( yesNo.Description == "Yes" )
                    btnOk.Enabled = true;
                else
                    btnOk.Enabled = false;
            }
            else
            {
                if ( PhysicianSelected && RelationshipChecked )
                    btnOk.Enabled = true;
                else
                    btnOk.Enabled = false;
            }
        }

        private void VerifyNonstaffPhysician()
        {
            Physician nonstaffPhysician = physicianSearchTabView1.NonstaffPhysician();

            if ( !physicianSearchTabView1.recordNonStaffPhysicianView1.IsUPINValid() )
            {
                return;
            }

            if ( !physicianSearchTabView1.recordNonStaffPhysicianView1.IsPhoneNumberValid() )
            {
                return;
            }

            if ( physicianRelationshipView1.cbxRef.Checked )
            {
                PhysicianService.AssignPhysician( nonstaffPhysician, PhysicianRole.Referring(), Model );
            }

            if ( physicianRelationshipView1.cbxAdm.Checked )
            {
                PhysicianService.AssignPhysician( nonstaffPhysician, PhysicianRole.Admitting(), Model );
            }

            if ( physicianRelationshipView1.cbxAtt.Checked )
            {
                PhysicianService.AssignPhysician( nonstaffPhysician, PhysicianRole.Attending(), Model );
            }

            if ( physicianRelationshipView1.cbxOpr.Checked )
            {
                PhysicianService.AssignPhysician( nonstaffPhysician, PhysicianRole.Operating(), Model );
            }

            if ( physicianRelationshipView1.cbxPcp.Checked )
            {
                PhysicianService.AssignPhysician( nonstaffPhysician, PhysicianRole.PrimaryCare(), Model );
            }

            DialogResult = DialogResult.OK;
        }

        private void VerifyPhysician()
        {
            string errorMsg = "The selected physician is invalid for one or more of the\nfollowing specified physician relationship type(s) in this\n activity:";
            const string errorMsg2 = "\n\nUncheck the boxes for the invalid physician relationship\ntype(s), or select a different physician, or record a nonstaff\nphysician.";
            bool errOccur = false;

            if ( physicianRelationshipView1.cbxRef.Checked )
            {
                try
                {
                    PhysicianService.AssignPhysician( SelectedPhysician, PhysicianRole.Referring(), Model );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref ";
                }
            }

            if ( physicianRelationshipView1.cbxAdm.Checked )
            {
                try
                {
                    PhysicianService.AssignPhysician( SelectedPhysician, PhysicianRole.Admitting(), Model );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm ";
                }
            }

            if ( physicianRelationshipView1.cbxAtt.Checked )
            {
                try
                {
                    PhysicianService.AssignPhysician( SelectedPhysician, PhysicianRole.Attending(), Model );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Att ";
                }
            }

            if ( physicianRelationshipView1.cbxOpr.Checked )
            {
                try
                {
                    PhysicianService.AssignPhysician( SelectedPhysician, PhysicianRole.Operating(), Model );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Opr ";
                }
            }

            if ( physicianRelationshipView1.cbxPcp.Checked )
            {
                try
                {
                    PhysicianService.AssignPhysician( SelectedPhysician, PhysicianRole.PrimaryCare(), Model );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        PCP ";
                }
            }

            if ( errOccur )
            {
                errorMsg += errorMsg2;

                MessageBox.Show( errorMsg, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
            }

            if ( !errOccur )
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ManualInitialization()
        {
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.SizeGripStyle = SizeGripStyle.Hide;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.physicianSearchTabView1 = new PatientAccess.UI.PhysicianSearchViews.PhysicianSearchTabView();
            this.btnCancel = new LoggingButton();
            this.btnOk = new LoggingButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.physicianRelationshipView1 = new PatientAccess.UI.PhysicianSearchViews.PhysicianRelationshipView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianSearchTabView1
            // 
            this.physicianSearchTabView1.BackColor = Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.physicianSearchTabView1.CallingObject = null;
            this.physicianSearchTabView1.Location = new System.Drawing.Point( 9, 13 );
            this.physicianSearchTabView1.Model = null;
            this.physicianSearchTabView1.Name = "physicianSearchTabView1";
            this.physicianSearchTabView1.PhysicianRelationshipToView = null;
            this.physicianSearchTabView1.Size = new System.Drawing.Size( 927, 394 );
            this.physicianSearchTabView1.TabIndex = 1;
            this.physicianSearchTabView1.PhysicianFound += new System.EventHandler( this.PhysicianFound );
            this.physicianSearchTabView1.EnableOkButton += new System.EventHandler( this.physicianSearchTabView1_EnableOkButton );
            this.physicianSearchTabView1.ResetButtonClicked += new System.EventHandler( this.ResetButton_Clicked );
            this.physicianSearchTabView1.PhysicianNotFound += new System.EventHandler( this.PhysicianNotFound );
            //this.physicianSearchTabView1.EnableDetailsButton += new System.EventHandler(this.physicianSearchTabView1_EnableDetailsButton);
            this.physicianSearchTabView1.SetCurrentTabPage += new EventHandler( physicianSearchTabView1_SetCurrentTabPage );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = SystemColors.Control;
            
            this.btnCancel.Location = new System.Drawing.Point( 861, 510 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // btnOk
            // 
            this.btnOk.BackColor = SystemColors.Control;
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point( 779, 510 );
            this.btnOk.Name = "btnOk";
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.White;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;
            this.panel1.Controls.Add( this.physicianRelationshipView1 );
            this.panel1.Location = new System.Drawing.Point( 9, 417 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 927, 82 );
            this.panel1.TabIndex = 2;
            // 
            // physicianRelationshipView1
            // 
            this.physicianRelationshipView1.BackColor = Color.White;
            this.physicianRelationshipView1.CallingObject = null;
            this.physicianRelationshipView1.Dock = DockStyle.Fill;
            this.physicianRelationshipView1.Location = new System.Drawing.Point( 0, 0 );
            this.physicianRelationshipView1.Model = null;
            this.physicianRelationshipView1.Name = "physicianRelationshipView1";
            this.physicianRelationshipView1.PhysicianRelationshipToView = null;
            this.physicianRelationshipView1.Size = new System.Drawing.Size( 925, 80 );
            this.physicianRelationshipView1.TabIndex = 2;
            this.physicianRelationshipView1.RelationshipChecked += new System.EventHandler( this.Relationship_Checked );
            this.physicianRelationshipView1.NoRelationshipChecked += new System.EventHandler( this.NoRelationship_Checked );
            // 
            // PhysicianSearchFormView
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.BackColor = Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size( 945, 544 );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.btnOk );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.physicianSearchTabView1 );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhysicianSearchFormView";
            this.ShowInTaskbar = false;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Select Physician";
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion

        #region Private Properties
        private bool PhysicianSelected
        {
            get
            {
                return i_PhysicianSelected;
            }
            set
            {
                i_PhysicianSelected = value;
            }
        }

        private bool RelationshipChecked
        {
            get
            {
                return i_RelationshipChecked;
            }
            set
            {
                i_RelationshipChecked = value;
            }
        }

        private Physician SelectedPhysician
        {
            get
            {
                return i_SelectedPhysician;
            }
            set
            {
                i_SelectedPhysician = value;
            }
        }

        public string CallingObject
        {
            private get
            {
                return i_CallingObject;
            }
            set
            {
                i_CallingObject = value;
            }
        }

        public string PhysicianRelationshipToView
        {
            get
            {
                return i_PhysicianRelationshipToView;
            }
            set
            {
                i_PhysicianRelationshipToView = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public PhysicianSearchFormView()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            ManualInitialization();

            EnableThemesOn( this );

            // Set the form icon here, otherwise if added to InitializeComponents 
            // the code will get replaced by the IDE if opened in the designer.
            // Adding the icon in the designer drastically changed the code.
            ResourceManager resources = new ResourceManager( typeof( PhysicianSearchFormView ) );
            Icon = ( ( Icon )( resources.GetObject( "$this.Icon" ) ) );
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
        private Container components = null;
        public PhysicianSearchTabView physicianSearchTabView1;
        private LoggingButton btnCancel;
        private LoggingButton btnOk;
        private Panel panel1;
        private PhysicianRelationshipView physicianRelationshipView1;
        private bool i_PhysicianSelected;
        private bool i_RelationshipChecked;
        private Physician i_SelectedPhysician;
        private string i_CallingObject = string.Empty;
        private string i_PhysicianRelationshipToView;
        private YesNoFlag yesNo;
        #endregion

        #region Constants
        #endregion
    }
}
