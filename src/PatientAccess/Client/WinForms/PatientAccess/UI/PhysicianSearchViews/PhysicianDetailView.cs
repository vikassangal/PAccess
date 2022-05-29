using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Remoting;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for PhysicianDetailView.
	/// </summary>
	public class PhysicianDetailView : TimeOutFormView
    {
		
        #region EventHandler

        
        private void closeButton_Click(  object sender, EventArgs e )
        {
            this.Close();
        }


        private void PhysicianDetail_Load(  object sender, EventArgs e )
        {
            physicianNumber = this.SelectPhysicians;
            GetPhysicianDetails();
            
        }


        private void printButton_Click( object sender, EventArgs e )
        {
            PhysicianReport physicianReport = new PhysicianReport();
            physicianReport.Model = physicianDetail;
            physicianReport.PrintPreview();

        }

        #endregion

        #region Construction And Finalization
        
        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicianDetailView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used. 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PhysicianDetailView ) );
            this.physicianDetailPanel = new System.Windows.Forms.Panel();
            this.NPIValueLabel = new System.Windows.Forms.Label();
            this.NPIlabel = new System.Windows.Forms.Label();
            this.pinLabel = new System.Windows.Forms.Label();
            this.dateExcludedValueLabel = new System.Windows.Forms.Label();
            this.admAttValueLabel = new System.Windows.Forms.Label();
            this.dateInactivatedValueLabel = new System.Windows.Forms.Label();
            this.dateActivatedValueLabel = new System.Windows.Forms.Label();
            this.activeInactiveValueLabel = new System.Windows.Forms.Label();
            this.admPrivilegesValueLabel = new System.Windows.Forms.Label();
            this.statusValueLabel = new System.Windows.Forms.Label();
            this.upinValueLabel = new System.Windows.Forms.Label();
            this.medicalGroupNumberValueLabel = new System.Windows.Forms.Label();
            this.stateLicenseNumberValueLabel = new System.Windows.Forms.Label();
            this.federalLicenseNumberValueLabel = new System.Windows.Forms.Label();
            this.numberValuelabel = new System.Windows.Forms.Label();
            this.dateExcludedLabel = new System.Windows.Forms.Label();
            this.admAttLabel = new System.Windows.Forms.Label();
            this.dateInactivatedLabel = new System.Windows.Forms.Label();
            this.dateActivatedLabel = new System.Windows.Forms.Label();
            this.activeInactiveLabel = new System.Windows.Forms.Label();
            this.admPrivilegesLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.upinLabel = new System.Windows.Forms.Label();
            this.medicalGroupnumberLabel = new System.Windows.Forms.Label();
            this.stateLicensenumberLabel = new System.Windows.Forms.Label();
            this.federalLicensenumberLabel = new System.Windows.Forms.Label();
            this.numberLabel = new System.Windows.Forms.Label();
            this.pinValueLabel = new System.Windows.Forms.Label();
            this.pagerValueLabel = new System.Windows.Forms.Label();
            this.cellValueLabel = new System.Windows.Forms.Label();
            this.phoneValueLabel = new System.Windows.Forms.Label();
            this.addressValueLabel = new System.Windows.Forms.Label();
            this.titleValueLabel = new System.Windows.Forms.Label();
            this.nameValueLabel = new System.Windows.Forms.Label();
            this.pagerlabel = new System.Windows.Forms.Label();
            this.specialityValuelabel = new System.Windows.Forms.Label();
            this.specialityLabel = new System.Windows.Forms.Label();
            this.cellLabel = new System.Windows.Forms.Label();
            this.phonelabel = new System.Windows.Forms.Label();
            this.addrssLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.physicianNameLabel = new System.Windows.Forms.Label();
            this.closeButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.printButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.physicianParentDetailpanel = new System.Windows.Forms.Panel();
            this.physicianDetailPanel.SuspendLayout();
            this.physicianParentDetailpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianDetailPanel
            // 
            this.physicianDetailPanel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.physicianDetailPanel.BackColor = System.Drawing.Color.White;
            this.physicianDetailPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.physicianDetailPanel.Controls.Add( this.NPIValueLabel );
            this.physicianDetailPanel.Controls.Add( this.NPIlabel );
            this.physicianDetailPanel.Controls.Add( this.pinLabel );
            this.physicianDetailPanel.Controls.Add( this.dateExcludedValueLabel );
            this.physicianDetailPanel.Controls.Add( this.admAttValueLabel );
            this.physicianDetailPanel.Controls.Add( this.dateInactivatedValueLabel );
            this.physicianDetailPanel.Controls.Add( this.dateActivatedValueLabel );
            this.physicianDetailPanel.Controls.Add( this.activeInactiveValueLabel );
            this.physicianDetailPanel.Controls.Add( this.admPrivilegesValueLabel );
            this.physicianDetailPanel.Controls.Add( this.statusValueLabel );
            this.physicianDetailPanel.Controls.Add( this.upinValueLabel );
            this.physicianDetailPanel.Controls.Add( this.medicalGroupNumberValueLabel );
            this.physicianDetailPanel.Controls.Add( this.stateLicenseNumberValueLabel );
            this.physicianDetailPanel.Controls.Add( this.federalLicenseNumberValueLabel );
            this.physicianDetailPanel.Controls.Add( this.numberValuelabel );
            this.physicianDetailPanel.Controls.Add( this.dateExcludedLabel );
            this.physicianDetailPanel.Controls.Add( this.admAttLabel );
            this.physicianDetailPanel.Controls.Add( this.dateInactivatedLabel );
            this.physicianDetailPanel.Controls.Add( this.dateActivatedLabel );
            this.physicianDetailPanel.Controls.Add( this.activeInactiveLabel );
            this.physicianDetailPanel.Controls.Add( this.admPrivilegesLabel );
            this.physicianDetailPanel.Controls.Add( this.statusLabel );
            this.physicianDetailPanel.Controls.Add( this.upinLabel );
            this.physicianDetailPanel.Controls.Add( this.medicalGroupnumberLabel );
            this.physicianDetailPanel.Controls.Add( this.stateLicensenumberLabel );
            this.physicianDetailPanel.Controls.Add( this.federalLicensenumberLabel );
            this.physicianDetailPanel.Controls.Add( this.numberLabel );
            this.physicianDetailPanel.Controls.Add( this.pinValueLabel );
            this.physicianDetailPanel.Controls.Add( this.pagerValueLabel );
            this.physicianDetailPanel.Controls.Add( this.cellValueLabel );
            this.physicianDetailPanel.Controls.Add( this.phoneValueLabel );
            this.physicianDetailPanel.Controls.Add( this.addressValueLabel );
            this.physicianDetailPanel.Controls.Add( this.titleValueLabel );
            this.physicianDetailPanel.Controls.Add( this.nameValueLabel );
            this.physicianDetailPanel.Controls.Add( this.pagerlabel );
            this.physicianDetailPanel.Controls.Add( this.specialityValuelabel );
            this.physicianDetailPanel.Controls.Add( this.specialityLabel );
            this.physicianDetailPanel.Controls.Add( this.cellLabel );
            this.physicianDetailPanel.Controls.Add( this.phonelabel );
            this.physicianDetailPanel.Controls.Add( this.addrssLabel );
            this.physicianDetailPanel.Controls.Add( this.titleLabel );
            this.physicianDetailPanel.Controls.Add( this.physicianNameLabel );
            this.physicianDetailPanel.Location = new System.Drawing.Point( 10, 37 );
            this.physicianDetailPanel.Name = "physicianDetailPanel";
            this.physicianDetailPanel.Size = new System.Drawing.Size( 598, 301 );
            this.physicianDetailPanel.TabIndex = 0;
            // 
            // NPIValueLabel
            // 
            this.NPIValueLabel.AutoSize = true;
            this.NPIValueLabel.Location = new System.Drawing.Point( 457, 116 );
            this.NPIValueLabel.Name = "NPIValueLabel";
            this.NPIValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.NPIValueLabel.TabIndex = 44;
            // 
            // NPIlabel
            // 
            this.NPIlabel.AutoSize = true;
            this.NPIlabel.Location = new System.Drawing.Point( 312, 116 );
            this.NPIlabel.Name = "NPIlabel";
            this.NPIlabel.Size = new System.Drawing.Size( 28, 13 );
            this.NPIlabel.TabIndex = 43;
            this.NPIlabel.Text = "NPI:";
            // 
            // pinLabel
            // 
            this.pinLabel.AutoSize = true;
            this.pinLabel.Location = new System.Drawing.Point( 168, 156 );
            this.pinLabel.Name = "pinLabel";
            this.pinLabel.Size = new System.Drawing.Size( 28, 13 );
            this.pinLabel.TabIndex = 42;
            this.pinLabel.Text = "PIN:";
            // 
            // dateExcludedValueLabel
            // 
            this.dateExcludedValueLabel.AutoSize = true;
            this.dateExcludedValueLabel.Location = new System.Drawing.Point( 457, 276 );
            this.dateExcludedValueLabel.Name = "dateExcludedValueLabel";
            this.dateExcludedValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.dateExcludedValueLabel.TabIndex = 41;
            // 
            // admAttValueLabel
            // 
            this.admAttValueLabel.AutoSize = true;
            this.admAttValueLabel.Location = new System.Drawing.Point( 457, 256 );
            this.admAttValueLabel.Name = "admAttValueLabel";
            this.admAttValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.admAttValueLabel.TabIndex = 40;
            // 
            // dateInactivatedValueLabel
            // 
            this.dateInactivatedValueLabel.AutoSize = true;
            this.dateInactivatedValueLabel.Location = new System.Drawing.Point( 457, 216 );
            this.dateInactivatedValueLabel.Name = "dateInactivatedValueLabel";
            this.dateInactivatedValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.dateInactivatedValueLabel.TabIndex = 39;
            // 
            // dateActivatedValueLabel
            // 
            this.dateActivatedValueLabel.AutoSize = true;
            this.dateActivatedValueLabel.Location = new System.Drawing.Point( 457, 196 );
            this.dateActivatedValueLabel.Name = "dateActivatedValueLabel";
            this.dateActivatedValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.dateActivatedValueLabel.TabIndex = 38;
            // 
            // activeInactiveValueLabel
            // 
            this.activeInactiveValueLabel.AutoSize = true;
            this.activeInactiveValueLabel.Location = new System.Drawing.Point( 457, 176 );
            this.activeInactiveValueLabel.Name = "activeInactiveValueLabel";
            this.activeInactiveValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.activeInactiveValueLabel.TabIndex = 37;
            // 
            // admPrivilegesValueLabel
            // 
            this.admPrivilegesValueLabel.AutoSize = true;
            this.admPrivilegesValueLabel.Location = new System.Drawing.Point( 457, 236 );
            this.admPrivilegesValueLabel.Name = "admPrivilegesValueLabel";
            this.admPrivilegesValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.admPrivilegesValueLabel.TabIndex = 36;
            // 
            // statusValueLabel
            // 
            this.statusValueLabel.AutoSize = true;
            this.statusValueLabel.Location = new System.Drawing.Point( 457, 156 );
            this.statusValueLabel.Name = "statusValueLabel";
            this.statusValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.statusValueLabel.TabIndex = 35;
            // 
            // upinValueLabel
            // 
            this.upinValueLabel.AutoSize = true;
            this.upinValueLabel.Location = new System.Drawing.Point( 457, 96 );
            this.upinValueLabel.Name = "upinValueLabel";
            this.upinValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.upinValueLabel.TabIndex = 34;
            // 
            // medicalGroupNumberValueLabel
            // 
            this.medicalGroupNumberValueLabel.AutoSize = true;
            this.medicalGroupNumberValueLabel.Location = new System.Drawing.Point( 457, 76 );
            this.medicalGroupNumberValueLabel.Name = "medicalGroupNumberValueLabel";
            this.medicalGroupNumberValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.medicalGroupNumberValueLabel.TabIndex = 33;
            // 
            // stateLicenseNumberValueLabel
            // 
            this.stateLicenseNumberValueLabel.AutoSize = true;
            this.stateLicenseNumberValueLabel.Location = new System.Drawing.Point( 457, 56 );
            this.stateLicenseNumberValueLabel.Name = "stateLicenseNumberValueLabel";
            this.stateLicenseNumberValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.stateLicenseNumberValueLabel.TabIndex = 32;
            // 
            // federalLicenseNumberValueLabel
            // 
            this.federalLicenseNumberValueLabel.AutoSize = true;
            this.federalLicenseNumberValueLabel.Location = new System.Drawing.Point( 457, 36 );
            this.federalLicenseNumberValueLabel.Name = "federalLicenseNumberValueLabel";
            this.federalLicenseNumberValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.federalLicenseNumberValueLabel.TabIndex = 31;
            // 
            // numberValuelabel
            // 
            this.numberValuelabel.AutoSize = true;
            this.numberValuelabel.Location = new System.Drawing.Point( 457, 16 );
            this.numberValuelabel.Name = "numberValuelabel";
            this.numberValuelabel.Size = new System.Drawing.Size( 0, 13 );
            this.numberValuelabel.TabIndex = 30;
            // 
            // dateExcludedLabel
            // 
            this.dateExcludedLabel.AutoSize = true;
            this.dateExcludedLabel.Location = new System.Drawing.Point( 312, 276 );
            this.dateExcludedLabel.Name = "dateExcludedLabel";
            this.dateExcludedLabel.Size = new System.Drawing.Size( 79, 13 );
            this.dateExcludedLabel.TabIndex = 29;
            this.dateExcludedLabel.Text = "Date excluded:";
            this.dateExcludedLabel.Click += new System.EventHandler( this.dateExcludedLabel_Click );
            // 
            // admAttLabel
            // 
            this.admAttLabel.AutoSize = true;
            this.admAttLabel.Location = new System.Drawing.Point( 312, 256 );
            this.admAttLabel.Name = "admAttLabel";
            this.admAttLabel.Size = new System.Drawing.Size( 138, 13 );
            this.admAttLabel.TabIndex = 28;
            this.admAttLabel.Text = "Excluded from Adm and Att:";
            // 
            // dateInactivatedLabel
            // 
            this.dateInactivatedLabel.AutoSize = true;
            this.dateInactivatedLabel.Location = new System.Drawing.Point( 312, 216 );
            this.dateInactivatedLabel.Name = "dateInactivatedLabel";
            this.dateInactivatedLabel.Size = new System.Drawing.Size( 88, 13 );
            this.dateInactivatedLabel.TabIndex = 27;
            this.dateInactivatedLabel.Text = "Date inactivated:";
            // 
            // dateActivatedLabel
            // 
            this.dateActivatedLabel.AutoSize = true;
            this.dateActivatedLabel.Location = new System.Drawing.Point( 312, 196 );
            this.dateActivatedLabel.Name = "dateActivatedLabel";
            this.dateActivatedLabel.Size = new System.Drawing.Size( 80, 13 );
            this.dateActivatedLabel.TabIndex = 26;
            this.dateActivatedLabel.Text = "Date activated:";
            // 
            // activeInactiveLabel
            // 
            this.activeInactiveLabel.AutoSize = true;
            this.activeInactiveLabel.Location = new System.Drawing.Point( 312, 176 );
            this.activeInactiveLabel.Name = "activeInactiveLabel";
            this.activeInactiveLabel.Size = new System.Drawing.Size( 83, 13 );
            this.activeInactiveLabel.TabIndex = 25;
            this.activeInactiveLabel.Text = "Active/Inactive:";
            // 
            // admPrivilegesLabel
            // 
            this.admPrivilegesLabel.AutoSize = true;
            this.admPrivilegesLabel.Location = new System.Drawing.Point( 312, 236 );
            this.admPrivilegesLabel.Name = "admPrivilegesLabel";
            this.admPrivilegesLabel.Size = new System.Drawing.Size( 78, 13 );
            this.admPrivilegesLabel.TabIndex = 24;
            this.admPrivilegesLabel.Text = "Adm privileges:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point( 312, 156 );
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size( 40, 13 );
            this.statusLabel.TabIndex = 23;
            this.statusLabel.Text = "Status:";
            // 
            // upinLabel
            // 
            this.upinLabel.AutoSize = true;
            this.upinLabel.Location = new System.Drawing.Point( 312, 96 );
            this.upinLabel.Name = "upinLabel";
            this.upinLabel.Size = new System.Drawing.Size( 36, 13 );
            this.upinLabel.TabIndex = 22;
            this.upinLabel.Text = "UPIN:";
            // 
            // medicalGroupnumberLabel
            // 
            this.medicalGroupnumberLabel.AutoSize = true;
            this.medicalGroupnumberLabel.Location = new System.Drawing.Point( 312, 76 );
            this.medicalGroupnumberLabel.Name = "medicalGroupnumberLabel";
            this.medicalGroupnumberLabel.Size = new System.Drawing.Size( 115, 13 );
            this.medicalGroupnumberLabel.TabIndex = 21;
            this.medicalGroupnumberLabel.Text = "Medical group number:";
            // 
            // stateLicensenumberLabel
            // 
            this.stateLicensenumberLabel.AutoSize = true;
            this.stateLicensenumberLabel.Location = new System.Drawing.Point( 312, 56 );
            this.stateLicensenumberLabel.Name = "stateLicensenumberLabel";
            this.stateLicensenumberLabel.Size = new System.Drawing.Size( 109, 13 );
            this.stateLicensenumberLabel.TabIndex = 20;
            this.stateLicensenumberLabel.Text = "State license number:";
            // 
            // federalLicensenumberLabel
            // 
            this.federalLicensenumberLabel.AutoSize = true;
            this.federalLicensenumberLabel.Location = new System.Drawing.Point( 312, 36 );
            this.federalLicensenumberLabel.Name = "federalLicensenumberLabel";
            this.federalLicensenumberLabel.Size = new System.Drawing.Size( 119, 13 );
            this.federalLicensenumberLabel.TabIndex = 19;
            this.federalLicensenumberLabel.Text = "Federal license number:";
            // 
            // numberLabel
            // 
            this.numberLabel.AutoSize = true;
            this.numberLabel.Location = new System.Drawing.Point( 312, 16 );
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size( 47, 13 );
            this.numberLabel.TabIndex = 18;
            this.numberLabel.Text = "Number:";
            // 
            // pinValueLabel
            // 
            this.pinValueLabel.AutoSize = true;
            this.pinValueLabel.Location = new System.Drawing.Point( 196, 156 );
            this.pinValueLabel.Name = "pinValueLabel";
            this.pinValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.pinValueLabel.TabIndex = 15;
            // 
            // pagerValueLabel
            // 
            this.pagerValueLabel.AutoSize = true;
            this.pagerValueLabel.Location = new System.Drawing.Point( 91, 156 );
            this.pagerValueLabel.Name = "pagerValueLabel";
            this.pagerValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.pagerValueLabel.TabIndex = 13;
            // 
            // cellValueLabel
            // 
            this.cellValueLabel.AutoSize = true;
            this.cellValueLabel.Location = new System.Drawing.Point( 91, 136 );
            this.cellValueLabel.Name = "cellValueLabel";
            this.cellValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.cellValueLabel.TabIndex = 12;
            // 
            // phoneValueLabel
            // 
            this.phoneValueLabel.AutoSize = true;
            this.phoneValueLabel.Location = new System.Drawing.Point( 91, 116 );
            this.phoneValueLabel.Name = "phoneValueLabel";
            this.phoneValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.phoneValueLabel.TabIndex = 11;
            // 
            // addressValueLabel
            // 
            this.addressValueLabel.Location = new System.Drawing.Point( 91, 64 );
            this.addressValueLabel.Name = "addressValueLabel";
            this.addressValueLabel.Size = new System.Drawing.Size( 152, 56 );
            this.addressValueLabel.TabIndex = 10;
            // 
            // titleValueLabel
            // 
            this.titleValueLabel.AutoSize = true;
            this.titleValueLabel.Location = new System.Drawing.Point( 91, 44 );
            this.titleValueLabel.Name = "titleValueLabel";
            this.titleValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.titleValueLabel.TabIndex = 9;
            // 
            // nameValueLabel
            // 
            this.nameValueLabel.AutoSize = true;
            this.nameValueLabel.Location = new System.Drawing.Point( 91, 24 );
            this.nameValueLabel.Name = "nameValueLabel";
            this.nameValueLabel.Size = new System.Drawing.Size( 0, 13 );
            this.nameValueLabel.TabIndex = 8;
            // 
            // pagerlabel
            // 
            this.pagerlabel.AutoSize = true;
            this.pagerlabel.Location = new System.Drawing.Point( 24, 156 );
            this.pagerlabel.Name = "pagerlabel";
            this.pagerlabel.Size = new System.Drawing.Size( 38, 13 );
            this.pagerlabel.TabIndex = 7;
            this.pagerlabel.Text = "Pager:";
            // 
            // specialityValuelabel
            // 
            this.specialityValuelabel.Location = new System.Drawing.Point( 91, 196 );
            this.specialityValuelabel.Name = "specialityValuelabel";
            this.specialityValuelabel.Size = new System.Drawing.Size(193, 33);
            this.specialityValuelabel.TabIndex = 6;
            // 
            // specialityLabel
            // 
            this.specialityLabel.AutoSize = true;
            this.specialityLabel.Location = new System.Drawing.Point( 24, 196 );
            this.specialityLabel.Name = "specialityLabel";
            this.specialityLabel.Size = new System.Drawing.Size( 53, 13 );
            this.specialityLabel.TabIndex = 5;
            this.specialityLabel.Text = "Specialty:";
            // 
            // cellLabel
            // 
            this.cellLabel.AutoSize = true;
            this.cellLabel.Location = new System.Drawing.Point( 24, 136 );
            this.cellLabel.Name = "cellLabel";
            this.cellLabel.Size = new System.Drawing.Size( 27, 13 );
            this.cellLabel.TabIndex = 4;
            this.cellLabel.Text = "Cell:";
            // 
            // phonelabel
            // 
            this.phonelabel.AutoSize = true;
            this.phonelabel.Location = new System.Drawing.Point( 24, 116 );
            this.phonelabel.Name = "phonelabel";
            this.phonelabel.Size = new System.Drawing.Size( 41, 13 );
            this.phonelabel.TabIndex = 3;
            this.phonelabel.Text = "Phone:";
            // 
            // addrssLabel
            // 
            this.addrssLabel.AutoSize = true;
            this.addrssLabel.Location = new System.Drawing.Point( 24, 64 );
            this.addrssLabel.Name = "addrssLabel";
            this.addrssLabel.Size = new System.Drawing.Size( 48, 13 );
            this.addrssLabel.TabIndex = 2;
            this.addrssLabel.Text = "Address:";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point( 24, 44 );
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size( 30, 13 );
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "Title:";
            // 
            // physicianNameLabel
            // 
            this.physicianNameLabel.AutoSize = true;
            this.physicianNameLabel.Location = new System.Drawing.Point( 24, 24 );
            this.physicianNameLabel.Name = "physicianNameLabel";
            this.physicianNameLabel.Size = new System.Drawing.Size( 38, 13 );
            this.physicianNameLabel.TabIndex = 0;
            this.physicianNameLabel.Text = "Name:";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.closeButton.BackColor = System.Drawing.Color.White;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point( 544, 359 );
            this.closeButton.Message = null;
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size( 75, 23 );
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler( this.closeButton_Click );
            // 
            // printButton
            // 
            this.printButton.BackColor = System.Drawing.Color.White;
            this.printButton.Location = new System.Drawing.Point( 536, 10 );
            this.printButton.Message = null;
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size( 75, 23 );
            this.printButton.TabIndex = 2;
            this.printButton.Text = "Pri&nt";
            this.printButton.UseVisualStyleBackColor = false;
            this.printButton.Click += new System.EventHandler( this.printButton_Click );
            // 
            // physicianParentDetailpanel
            // 
            this.physicianParentDetailpanel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.physicianParentDetailpanel.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.physicianParentDetailpanel.Controls.Add( this.printButton );
            this.physicianParentDetailpanel.Controls.Add( this.physicianDetailPanel );
            this.physicianParentDetailpanel.Location = new System.Drawing.Point( 7, 12 );
            this.physicianParentDetailpanel.Name = "physicianParentDetailpanel";
            this.physicianParentDetailpanel.Size = new System.Drawing.Size( 622, 341 );
            this.physicianParentDetailpanel.TabIndex = 0;
            // 
            // PhysicianDetailView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 637, 391 );
            this.Controls.Add( this.physicianParentDetailpanel );
            this.Controls.Add( this.closeButton );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhysicianDetailView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Physician Details";
            this.Load += new System.EventHandler( this.PhysicianDetail_Load );
            this.physicianDetailPanel.ResumeLayout( false );
            this.physicianDetailPanel.PerformLayout();
            this.physicianParentDetailpanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        
        public long SelectPhysicians
        {
            private get
            { 
                return i_SelectedPhysician;
            }
            set
            {
                i_SelectedPhysician = value;

            }
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Gets all the Details of a Physician
        /// </summary>
        private void GetPhysicianDetails()
        {
            this.Cursor = Cursors.WaitCursor;

            //Check to see if search data entered is valid.
            try
            {
                IPhysicianBroker broker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
                Physician physician = broker.PhysicianDetails(
                    User.GetCurrent().Facility.Oid, physicianNumber );
                physicianDetail = physician;
                if(  physician != null )
                {
                    this.nameValueLabel.Text = physician.Name.AsFormattedName();
                    this.numberValuelabel.Text = physician.PhysicianNumber.ToString();
                    if( physician.Address != null )
                    {
                        this.addressValueLabel.Text = physician.Address.OneLineAddressLabel();
                    }
                    if( physician.PhoneNumber != null )
                    {
                        this.phoneValueLabel.Text = physician.PhoneNumber.AsFormattedString();
                    }
                    if( physician.CellPhoneNumber != null )
                    {
                        this.cellValueLabel.Text = physician.CellPhoneNumber.AsFormattedString();
                    }
                    if( physician.PagerNumber != null )
                    {
                        this.pagerValueLabel.Text = physician.PagerNumber.AsFormattedString();
                    }
                    if (physician.Specialization != null)
                    {
                        this.specialityValuelabel.Text = physician.Specialization.AsFormattedSpeciality;
                    }
                    if( physician.FederalLicense != null )
                    {
                        this.federalLicenseNumberValueLabel.Text = physician.FederalLicense.Number;
                    }
                    if( physician.StateLicense != null )
                    {
                        this.stateLicenseNumberValueLabel.Text = physician.StateLicense;
                    }
                    this.statusValueLabel.Text = physician.Status;
                    this.admPrivilegesValueLabel.Text = physician.AdmittingPrivileges;
                    this.activeInactiveValueLabel.Text = physician.ActiveInactiveFlag;
                    this.admAttValueLabel.Text = physician.ExcludedStatus;
                    this.titleValueLabel.Text = physician.Title;
                    this.medicalGroupNumberValueLabel.Text = physician.MedicalGroupNumber;
                    this.pinValueLabel.Text = physician.PIN.ToString();
                    this.upinValueLabel.Text = physician.UPIN;
                    this.NPIValueLabel.Text = physician.NPI;
                    if( physician.DateExcluded.Equals(  DateTime.MinValue ) )
                    {
                        this.dateExcludedValueLabel.Text = "";
                    }
                    else
                    {
                        this.dateExcludedValueLabel.Text = physician.DateExcluded.
                            Date.ToString( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );
                    }
                    if( physician.DateInactivated.Equals(  DateTime.MinValue ) )
                    {
                        this.dateInactivatedValueLabel.Text = "";
                    }
                    else
                    {
                        this.dateInactivatedValueLabel.Text = physician.DateInactivated.
                            Date.ToString( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );
                    }
                    if( physician.DateActivated.Equals(  DateTime.MinValue ) )
                    {
                        this.dateActivatedValueLabel.Text  = "";
                    }
                    else
                    {
                        this.dateActivatedValueLabel.Text = physician.DateActivated
                            .Date.ToString( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );
                    }
                }
            }
            catch (RemotingTimeoutException)
            {
                MessageBox.Show(UIErrorMessages.TIMEOUT_PHYSICIAN_REPORT_DISPLAY);
                this.Close();
            }
            finally
            {
                if (!this.IsDisposed && !this.Disposing)
                    this.Cursor = Cursors.Default;
            }
        }
            

        #endregion

        #region Data Elements

        private Container components = null;
        private Panel physicianDetailPanel;
        private Panel physicianParentDetailpanel;
        private LoggingButton closeButton;
        private Label physicianNameLabel;
        private Label titleLabel;
        private Label addrssLabel;
        private Label phonelabel;
        private Label cellLabel;
        private Label specialityLabel;
        private Label specialityValuelabel;
        private Label pagerlabel;
        private Label nameValueLabel;
        private Label titleValueLabel;
        private Label addressValueLabel;
        private Label phoneValueLabel;
        private Label cellValueLabel;
        private Label pagerValueLabel;
        private Label pinLabel;
        private Label pinValueLabel;
        private Label numberLabel;
        private Label federalLicensenumberLabel;
        private Label stateLicensenumberLabel;
        private Label medicalGroupnumberLabel;
        private Label upinLabel;
        private Label statusLabel;
        private Label admPrivilegesLabel;
        private Label activeInactiveLabel;
        private Label dateActivatedLabel;
        private Label dateInactivatedLabel;
        private Label admAttLabel;
        private Label dateExcludedLabel;
        private Label numberValuelabel;
        private Label federalLicenseNumberValueLabel;
        private Label stateLicenseNumberValueLabel;
        private Label medicalGroupNumberValueLabel;
        private Label upinValueLabel;
        private Label statusValueLabel;
        private Label admPrivilegesValueLabel;
        private Label activeInactiveValueLabel;
        private Label dateActivatedValueLabel;
        private Label dateInactivatedValueLabel;
        private Label admAttValueLabel;
        private Label dateExcludedValueLabel;
        private LoggingButton printButton;
     
        private long i_SelectedPhysician;
        private long physicianNumber;
        private Physician physicianDetail;

        #endregion
        private Label NPIlabel;
        private Label NPIValueLabel;
       
        #region Constants
        private const string PBAR_DOWN_MESSAGE = "The activity cannot proceed" +
            " because the system is unavailable. Please try again later.";
        #endregion

        private void dateExcludedLabel_Click( object sender, EventArgs e )
        {

        }

    }
}
