using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace PatientAccess.UI.PhysicianSearchViews
{
    [Serializable]
    public class PhysicianRelationshipView : ControlView
    {
        #region Event Handlers
        public event EventHandler RelationshipChecked;
        public event EventHandler NoRelationshipChecked;
        #endregion

        #region Methods
        public override void UpdateView()
        {
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            if ( primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate( Model.AccountCreatedDate ) )
            {
                cbxPcp.Text = PRIMARYCAREPHYSICIAN_LABEL;
            }
            else
            {
                cbxPcp.Text = OTHERPHYSICIAN_LABEL;
            }
            var primaryCarePhysicianForPremseFeatureManager = new PrimaryCarePhysicianForPreMseFeatureManager( ConfigurationManager.AppSettings );
            if ( primaryCarePhysicianForPremseFeatureManager.IsEnabledFor( Model.AccountCreatedDate ) )
            {
                enablePrimaryCarePhysicianForPre_MSE = true;
            }
            else
            {
                enablePrimaryCarePhysicianForPre_MSE = false;
            }
            if ( PhysicianRelationshipToView != String.Empty )
            {
                if ( PhysicianRelationshipToView == PhysicianRelationship.REFERRING_PHYSICIAN )
                {
                    cbxRef.Checked = true;
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.ADMITTING_PHYSICIAN )
                {
                    cbxAdm.Checked = true;
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.ATTENDING_PHYSICIAN )
                {
                    cbxAtt.Checked = true;
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.OPERATING_PHYSICIAN )
                {
                    cbxOpr.Checked = true;
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.PRIMARYCARE_PHYSICIAN )
                {
                    cbxPcp.Checked = true;
                }
            }
            CheckPatientTypeAndFinClass();
        }
        
        public override void UpdateModel()
        {
        }

        #endregion

        #region Properties
        public new Account Model
        {
            private get
            {
                return (Account)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public string CallingObject
        {
            get
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
            private get
            {
                return i_PhysicianRelationshipToView;
            }
            set
            {
                i_PhysicianRelationshipToView = value;
            }
        }

        public bool OperatingPhysicianSelectionVisibility 
        {
            get { return cbxOpr.Visible; }
            set { cbxOpr.Visible = value; }
        }
        public bool AttendingPhysicianSelectionVisibility
        {
            get { return cbxAtt.Visible; }
            set { cbxAtt.Visible = value; }
        }
        public bool PrimaryCarePhysicianSelectionVisibility
        {
            get { return cbxPcp.Visible; }
            set { cbxPcp.Visible = value; }
        }

        #endregion

        #region Private Methods
      
        private void cbxRef_CheckedChanged( object sender, EventArgs e )
        {
            HasRelationshipChecked();
        }

        private void cbxAdm_CheckedChanged( object sender, EventArgs e )
        {
            HasRelationshipChecked();
        }

        private void cbxAtt_CheckedChanged( object sender, EventArgs e )
        {
            HasRelationshipChecked();
        }

        private void cbxOpr_CheckedChanged( object sender, EventArgs e )
        {
            HasRelationshipChecked();
        }

        private void cbxPcp_CheckedChanged( object sender, EventArgs e )
        {
            HasRelationshipChecked();
        }

        private void HasRelationshipChecked()
        {
            if ( cbxRef.Checked ||
                cbxAdm.Checked ||
                cbxAtt.Checked ||
                cbxOpr.Checked ||
                cbxPcp.Checked )
            {
                RelationshipChecked( this, new LooseArgs( this ) );
            }
            else
            {
                NoRelationshipChecked( this, new LooseArgs( this ) );
            }
        }

        private void CheckPatientTypeAndFinClass()
        {
            if (  Model.IsEDorUrgentCarePremseAccount )
            {
                cbxOpr.Enabled = false;
                cbxPcp.Enabled = enablePrimaryCarePhysicianForPre_MSE;
            }
            else
            {
                cbxOpr.Enabled = true;
                cbxPcp.Enabled = true;
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPhysicianRel = new System.Windows.Forms.Label();
            this.cbxRef = new System.Windows.Forms.CheckBox();
            this.cbxPcp = new System.Windows.Forms.CheckBox();
            this.cbxOpr = new System.Windows.Forms.CheckBox();
            this.cbxAtt = new System.Windows.Forms.CheckBox();
            this.cbxAdm = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.label1.Location = new System.Drawing.Point( 20, 7 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 1137, 17 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Physician Relationship";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 20, 29 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 648, 17 );
            this.label2.TabIndex = 0;
            this.label2.Text = "For the specified physician above, check one or more boxes for the physician rela" +
                "tionship between the physician and the patient.";
            // 
            // lblPhysicianRel
            // 
            this.lblPhysicianRel.Location = new System.Drawing.Point( 534, 55 );
            this.lblPhysicianRel.Name = "lblPhysicianRel";
            this.lblPhysicianRel.Size = new System.Drawing.Size( 156, 17 );
            this.lblPhysicianRel.TabIndex = 0;
            this.lblPhysicianRel.Text = "Specify physician relationship:";
            // 
            // cbxRef
            // 
            this.cbxRef.Location = new System.Drawing.Point( 685, 50 );
            this.cbxRef.Name = "cbxRef";
            this.cbxRef.Size = new System.Drawing.Size( 41, 24 );
            this.cbxRef.TabIndex = 1;
            this.cbxRef.Text = "Ref";
            this.cbxRef.CheckedChanged += new System.EventHandler( this.cbxRef_CheckedChanged );
            // 
            // cbxPcp
            // 
            //this.cbxPcp.Enabled = false;
            this.cbxPcp.Location = new System.Drawing.Point( 881, 50 );
            this.cbxPcp.Name = "cbxPcp";
            this.cbxPcp.Size = new System.Drawing.Size( 54, 24 );
            this.cbxPcp.TabIndex = 5;
            this.cbxPcp.Text = "PCP";
            this.cbxPcp.CheckedChanged += new System.EventHandler( this.cbxPcp_CheckedChanged );
            // 
            // cbxOpr
            // 
            this.cbxOpr.Enabled = false;
            this.cbxOpr.Location = new System.Drawing.Point( 831, 50 );
            this.cbxOpr.Name = "cbxOpr";
            this.cbxOpr.Size = new System.Drawing.Size( 44, 24 );
            this.cbxOpr.TabIndex = 4;
            this.cbxOpr.Text = "Opr";
            this.cbxOpr.CheckedChanged += new System.EventHandler( this.cbxOpr_CheckedChanged );
            // 
            // cbxAtt
            // 
            this.cbxAtt.Location = new System.Drawing.Point( 784, 50 );
            this.cbxAtt.Name = "cbxAtt";
            this.cbxAtt.Size = new System.Drawing.Size( 41, 24 );
            this.cbxAtt.TabIndex = 3;
            this.cbxAtt.Text = "Att";
            this.cbxAtt.CheckedChanged += new System.EventHandler( this.cbxAtt_CheckedChanged );
            // 
            // cbxAdm
            // 
            this.cbxAdm.Location = new System.Drawing.Point( 732, 50 );
            this.cbxAdm.Name = "cbxAdm";
            this.cbxAdm.Size = new System.Drawing.Size( 46, 24 );
            this.cbxAdm.TabIndex = 2;
            this.cbxAdm.Text = "Adm";
            this.cbxAdm.CheckedChanged += new System.EventHandler( this.cbxAdm_CheckedChanged );
            // 
            // PhysicianRelationshipView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.cbxAdm );
            this.Controls.Add( this.cbxAtt );
            this.Controls.Add( this.cbxOpr );
            this.Controls.Add( this.cbxPcp );
            this.Controls.Add( this.cbxRef );
            this.Controls.Add( this.lblPhysicianRel );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Name = "PhysicianRelationshipView";
            this.Size = new System.Drawing.Size( 938, 85 );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PhysicianRelationshipView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
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
        private Label label1;
        private Label label2;
        private Label lblPhysicianRel;

        public CheckBox cbxRef;
        public CheckBox cbxOpr;
        public CheckBox cbxAtt;
        public CheckBox cbxAdm;
        public CheckBox cbxPcp;

        private string i_CallingObject;
        private string i_PhysicianRelationshipToView;
        private bool enablePrimaryCarePhysicianForPre_MSE;

        #endregion

        #region Constants

        private const string PRIMARYCAREPHYSICIAN_LABEL = "PCP";
        private const string OTHERPHYSICIAN_LABEL = "Oth";
        #endregion
    }
}
