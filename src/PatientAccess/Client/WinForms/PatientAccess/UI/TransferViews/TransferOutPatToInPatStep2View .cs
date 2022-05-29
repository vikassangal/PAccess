using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.TransferViews
{
    public class TransferOutPatToInPatStep2View : ControlView
    {

        #region Events
        public event EventHandler CancelButtonClicked;
        public event EventHandler BackButtonClicked;
        public event EventHandler NextButtonClicked;
        #endregion

        #region Event Handlers
        private void btnNext_Click( object sender, EventArgs e )
        {
            //Raise NextButtonClicked event.
            NextButtonClicked( this, new LooseArgs( Model ) );
        }

        private void btnBack_Click( object sender, EventArgs e )
        {
            BackButtonClicked( this, new LooseArgs( Model ) );
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model ) ) )
            {
                TransferOutPatToInPatView_LeaveView( sender, e );
                CancelButtonClicked( this, EventArgs.Empty );
            }
        }

        private void Application_Idle( object sender, EventArgs e )
        {
        }

        private void TransferOutPatToInPatView_LeaveView( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;
        }

        private void cboReligion_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void cboPlaceOfWorship_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void cboSmoker_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void cboValuablesCollected_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void cboConfidentialStatus_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void mtbComments_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void mtbEmbosserCard_Enter( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if ( Model != null )
            {
                btnNext.Enabled = true;

                if ( Model.Patient != null )
                {
                    patientContextView1.Model = Model.Patient;
                    patientContextView1.Account = Model;
                    patientContextView1.UpdateView();
                }

                physicianSelectionView1.Model = Model;
                physicianSelectionView1.UpdateView();

                PopulateReligions();
                PopulatePlaceOfWorships();
                PopulateSmokerList();
                PopulateValuablesCollectedList();
                PopulateConfidentialStatus();
                mtbComments.UnMaskedText = Model.ClinicalComments;
                mtbEmbosserCard.UnMaskedText = Model.EmbosserCard;

                physicianSelectionView1.SetDefaultFocus();
            }
        }

        public override void UpdateModel()
        {
            physicianSelectionView1.UpdateModel();
            Model.Patient.Religion = ( Religion )cboReligion.SelectedItem;
            Model.Patient.PlaceOfWorship = ( PlaceOfWorship )cboPlaceOfWorship.SelectedItem;
            Model.Smoker = ( YesNoFlag )cboSmoker.SelectedItem;

            Model.ValuablesAreTaken = ( YesNoFlag )cboValuablesCollected.SelectedItem;

            Model.ConfidentialityCode = ( ConfidentialCode )cboConfidentialStatus.SelectedItem;
            Model.ClinicalComments = mtbComments.UnMaskedText;
            Model.EmbosserCard = mtbEmbosserCard.UnMaskedText;
        }
        #endregion

        #region Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region private method

        private void PopulateReligions()
        {
            IReligionBroker broker = BrokerFactory.BrokerOfType<IReligionBroker>();
            ArrayList religionCollection = ( ArrayList )broker.AllReligions( User.GetCurrent().Facility.Oid );

            cboReligion.Items.Clear();

            Religion religion = new Religion();
            for ( int i = 0; i < religionCollection.Count; i++ )
            {
                religion = ( Religion )religionCollection[i];

                cboReligion.Items.Add( religion );

            }
            cboReligion.SelectedItem = Model.Patient.Religion;
        }

        private void PopulatePlaceOfWorships()
        {
            IReligionBroker broker = BrokerFactory.BrokerOfType<IReligionBroker>();
            ArrayList placesOfWorship = ( ArrayList )broker.AllPlacesOfWorshipFor( Model.Facility.Oid );

            cboPlaceOfWorship.Items.Clear();

            PlaceOfWorship place = new PlaceOfWorship();
            for ( int i = 0; i < placesOfWorship.Count; i++ )
            {
                place = ( PlaceOfWorship )placesOfWorship[i];

                cboPlaceOfWorship.Items.Add( place );

            }
            cboPlaceOfWorship.SelectedItem = Model.Patient.PlaceOfWorship != null ? Model.Patient.PlaceOfWorship : null;
        }

        private void PopulateSmokerList()
        {
            cboSmoker.Items.Clear();

            YesNoFlag blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboSmoker.Items.Add( blank );

            YesNoFlag yes = new YesNoFlag();
            yes.SetYes();
            cboSmoker.Items.Add( yes );

            YesNoFlag no = new YesNoFlag();
            no.SetNo();
            cboSmoker.Items.Add( no );

            if ( Model.Smoker != null )
            {
                cboSmoker.SelectedItem = Model.Smoker;
            }
            else
            {
                cboSmoker.SelectedIndex = 0;
            }
        }

        private void PopulateValuablesCollectedList()
        {
            cboValuablesCollected.Items.Clear();

            YesNoFlag blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboValuablesCollected.Items.Add( blank );

            YesNoFlag yes = new YesNoFlag();
            yes.SetYes();
            cboValuablesCollected.Items.Add( yes );

            YesNoFlag no = new YesNoFlag();
            no.SetNo();
            cboValuablesCollected.Items.Add( no );

            if ( Model.ValuablesAreTaken != null )
            {
                if ( Model.ValuablesAreTaken.Code == YesNoFlag.CODE_YES )
                {
                    cboValuablesCollected.SelectedIndex = 1;
                }
                else if ( Model.ValuablesAreTaken.Code == YesNoFlag.CODE_NO )
                {
                    cboValuablesCollected.SelectedIndex = 2;
                }
                else
                {
                    cboValuablesCollected.SelectedIndex = 0;
                }
            }
            else
            {
                cboValuablesCollected.SelectedIndex = 2;
            }
        }

        private void PopulateConfidentialStatus()
        {
            IConfidentialCodeBroker broker = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
            cboConfidentialStatus.Items.Clear();

            foreach ( ConfidentialCode obj in broker.ConfidentialCodesFor( User.GetCurrent().Facility.Oid ) )
            {
                cboConfidentialStatus.Items.Add( obj );
            }

            cboConfidentialStatus.SelectedItem = Model.ConfidentialityCode;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureClinicalViewComments( mtbComments );
        }
        #endregion

        #region Private Properties
        #endregion

        public TransferOutPatToInPatStep2View()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            ConfigureControls();

            EnableThemesOn( this );
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

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTransferOutPatToInPat = new System.Windows.Forms.Panel();
            this.mtbComments = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbEmbosserCard = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.physicianSelectionView1 = new PatientAccess.UI.CommonControls.PhysicianSelectionView();
            this.cboReligion = new System.Windows.Forms.ComboBox();
            this.lblReligion = new System.Windows.Forms.Label();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.cboPlaceOfWorship = new System.Windows.Forms.ComboBox();
            this.lblPlaceOfWorship = new System.Windows.Forms.Label();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblEmbosserCard = new System.Windows.Forms.Label();
            this.lblSmoker = new System.Windows.Forms.Label();
            this.cboSmoker = new System.Windows.Forms.ComboBox();
            this.cboValuablesCollected = new System.Windows.Forms.ComboBox();
            this.lblValuablesCollected = new System.Windows.Forms.Label();
            this.cboConfidentialStatus = new System.Windows.Forms.ComboBox();
            this.lblConfidentialStatus = new System.Windows.Forms.Label();
            this.btnNext = new LoggingButton();
            this.btnCancel = new LoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.btnBack = new LoggingButton();
            this.panelTransferOutPatToInPat.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferOutPatToInPat
            // 
            this.panelTransferOutPatToInPat.BackColor = System.Drawing.Color.White;
            this.panelTransferOutPatToInPat.Controls.Add( this.mtbComments );
            this.panelTransferOutPatToInPat.Controls.Add( this.mtbEmbosserCard );
            this.panelTransferOutPatToInPat.Controls.Add( this.physicianSelectionView1 );
            this.panelTransferOutPatToInPat.Controls.Add( this.cboReligion );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblReligion );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblStep2 );
            this.panelTransferOutPatToInPat.Controls.Add( this.cboPlaceOfWorship );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblPlaceOfWorship );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblComments );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblEmbosserCard );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblSmoker );
            this.panelTransferOutPatToInPat.Controls.Add( this.cboSmoker );
            this.panelTransferOutPatToInPat.Controls.Add( this.cboValuablesCollected );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblValuablesCollected );
            this.panelTransferOutPatToInPat.Controls.Add( this.cboConfidentialStatus );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblConfidentialStatus );
            this.panelTransferOutPatToInPat.Location = new System.Drawing.Point( 8, 64 );
            this.panelTransferOutPatToInPat.Name = "panelTransferOutPatToInPat";
            this.panelTransferOutPatToInPat.Size = new System.Drawing.Size( 1008, 520 );
            this.panelTransferOutPatToInPat.TabIndex = 2;
            // 
            // mtbComments
            // 
            this.mtbComments.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComments.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbComments.Location = new System.Drawing.Point( 583, 209 );
            this.mtbComments.Mask = "";
            this.mtbComments.MaxLength = 120;
            this.mtbComments.Multiline = true;
            this.mtbComments.Name = "mtbComments";
            this.mtbComments.Size = new System.Drawing.Size( 281, 52 );
            this.mtbComments.TabIndex = 6;
            this.mtbComments.Enter += new System.EventHandler( this.mtbComments_Enter );
            this.mtbComments.prePasteEdit = CommonFormatting.PreFilter;
            // 
            // mtbEmbosserCard
            // 
            this.mtbEmbosserCard.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEmbosserCard.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.mtbEmbosserCard.Location = new System.Drawing.Point( 669, 273 );
            this.mtbEmbosserCard.Mask = "";
            this.mtbEmbosserCard.MaxLength = 10;
            this.mtbEmbosserCard.Multiline = true;
            this.mtbEmbosserCard.Name = "mtbEmbosserCard";
            this.mtbEmbosserCard.Size = new System.Drawing.Size( 105, 20 );
            this.mtbEmbosserCard.TabIndex = 7;
            this.mtbEmbosserCard.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbEmbosserCard.Enter += new System.EventHandler( this.mtbEmbosserCard_Enter );
            // 
            // physicianSelectionView1
            // 
            this.physicianSelectionView1.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView1.Location = new System.Drawing.Point( 16, 40 );
            this.physicianSelectionView1.Model = null;
            this.physicianSelectionView1.Name = "physicianSelectionView1";
            this.physicianSelectionView1.Size = new System.Drawing.Size( 559, 279 );
            this.physicianSelectionView1.TabIndex = 0;
            // 
            // cboReligion
            // 
            this.cboReligion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReligion.Location = new System.Drawing.Point( 690, 44 );
            this.cboReligion.Name = "cboReligion";
            this.cboReligion.Size = new System.Drawing.Size( 171, 21 );
            this.cboReligion.TabIndex = 1;
            this.cboReligion.Enter += new System.EventHandler( this.cboReligion_Enter );
            // 
            // lblReligion
            // 
            this.lblReligion.Location = new System.Drawing.Point( 584, 46 );
            this.lblReligion.Name = "lblReligion";
            this.lblReligion.Size = new System.Drawing.Size( 101, 23 );
            this.lblReligion.TabIndex = 58;
            this.lblReligion.Text = "Religion:";
            // 
            // lblStep2
            // 
            this.lblStep2.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.lblStep2.Location = new System.Drawing.Point( 16, 8 );
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Size = new System.Drawing.Size( 168, 23 );
            this.lblStep2.TabIndex = 56;
            this.lblStep2.Text = "Step 2 of 3: Physicians";
            // 
            // cboPlaceOfWorship
            // 
            this.cboPlaceOfWorship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlaceOfWorship.Location = new System.Drawing.Point( 690, 72 );
            this.cboPlaceOfWorship.Name = "cboPlaceOfWorship";
            this.cboPlaceOfWorship.Size = new System.Drawing.Size( 224, 21 );
            this.cboPlaceOfWorship.TabIndex = 2;
            this.cboPlaceOfWorship.Enter += new System.EventHandler( this.cboPlaceOfWorship_Enter );
            // 
            // lblPlaceOfWorship
            // 
            this.lblPlaceOfWorship.Location = new System.Drawing.Point( 583, 78 );
            this.lblPlaceOfWorship.Name = "lblPlaceOfWorship";
            this.lblPlaceOfWorship.Size = new System.Drawing.Size( 94, 23 );
            this.lblPlaceOfWorship.TabIndex = 37;
            this.lblPlaceOfWorship.Text = "Place of worship:";
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point( 581, 190 );
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size( 83, 15 );
            this.lblComments.TabIndex = 60;
            this.lblComments.Text = "Comments:";
            // 
            // lblEmbosserCard
            // 
            this.lblEmbosserCard.Location = new System.Drawing.Point( 581, 278 );
            this.lblEmbosserCard.Name = "lblEmbosserCard";
            this.lblEmbosserCard.Size = new System.Drawing.Size( 88, 23 );
            this.lblEmbosserCard.TabIndex = 34;
            this.lblEmbosserCard.Text = "Embosser card:";
            // 
            // lblSmoker
            // 
            this.lblSmoker.Location = new System.Drawing.Point( 583, 107 );
            this.lblSmoker.Name = "lblSmoker";
            this.lblSmoker.Size = new System.Drawing.Size( 86, 26 );
            this.lblSmoker.TabIndex = 58;
            this.lblSmoker.Text = "Smoker:";
            // 
            // cboSmoker
            // 
            this.cboSmoker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmoker.Location = new System.Drawing.Point( 690, 101 );
            this.cboSmoker.Name = "cboSmoker";
            this.cboSmoker.Size = new System.Drawing.Size( 52, 21 );
            this.cboSmoker.TabIndex = 3;
            this.cboSmoker.Enter += new System.EventHandler( this.cboSmoker_Enter );
            // 
            // cboValuablesCollected
            // 
            this.cboValuablesCollected.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValuablesCollected.Location = new System.Drawing.Point( 690, 129 );
            this.cboValuablesCollected.Name = "cboValuablesCollected";
            this.cboValuablesCollected.Size = new System.Drawing.Size( 52, 21 );
            this.cboValuablesCollected.TabIndex = 4;
            this.cboValuablesCollected.Enter += new System.EventHandler( this.cboValuablesCollected_Enter );
            // 
            // lblValuablesCollected
            // 
            this.lblValuablesCollected.Location = new System.Drawing.Point( 583, 134 );
            this.lblValuablesCollected.Name = "lblValuablesCollected";
            this.lblValuablesCollected.Size = new System.Drawing.Size( 107, 23 );
            this.lblValuablesCollected.TabIndex = 58;
            this.lblValuablesCollected.Text = "Valuables collected:";
            // 
            // cboConfidentialStatus
            // 
            this.cboConfidentialStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConfidentialStatus.Location = new System.Drawing.Point( 688, 156 );
            this.cboConfidentialStatus.Name = "cboConfidentialStatus";
            this.cboConfidentialStatus.Size = new System.Drawing.Size( 131, 21 );
            this.cboConfidentialStatus.TabIndex = 5;
            this.cboConfidentialStatus.Enter += new System.EventHandler( this.cboConfidentialStatus_Enter );
            // 
            // lblConfidentialStatus
            // 
            this.lblConfidentialStatus.Location = new System.Drawing.Point( 581, 161 );
            this.lblConfidentialStatus.Name = "lblConfidentialStatus";
            this.lblConfidentialStatus.Size = new System.Drawing.Size( 104, 20 );
            this.lblConfidentialStatus.TabIndex = 58;
            this.lblConfidentialStatus.Text = "Confidential status:";
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Location = new System.Drawing.Point( 939, 593 );
            this.btnNext.Name = "btnNext";
            this.btnNext.TabIndex = 10;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += new System.EventHandler( this.btnNext_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point( 773, 593 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.panelUserContext.Controls.Add( this.userContextView1 );
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point( 0, 0 );
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size( 1024, 22 );
            this.panelUserContext.TabIndex = 0;
            // 
            // userContextView1
            // 
            this.userContextView1.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView1.Description = "Transfer Outpatient to Inpatient";
            this.userContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView1.Model = null;
            this.userContextView1.Name = "userContextView1";
            this.userContextView1.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView1.TabIndex = 0;
            this.userContextView1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.patientContextView1 );
            this.panel1.Location = new System.Drawing.Point( 8, 32 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1008, 24 );
            this.panel1.TabIndex = 5;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size( 1008, 53 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnBack.Location = new System.Drawing.Point( 859, 593 );
            this.btnBack.Name = "btnBack";
            this.btnBack.TabIndex = 9;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler( this.btnBack_Click );
            // 
            // TransferOutPatToInPatStep2View
            // 
            this.AcceptButton = this.btnNext;
            this.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.Controls.Add( this.btnBack );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnNext );
            this.Controls.Add( this.panelTransferOutPatToInPat );
            this.Controls.Add( this.panelUserContext );
            this.Name = "TransferOutPatToInPatStep2View";
            this.Size = new System.Drawing.Size( 1024, 632 );
            this.Leave += new System.EventHandler( this.TransferOutPatToInPatView_LeaveView );
            this.panelTransferOutPatToInPat.ResumeLayout( false );
            this.panelUserContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Data Elements
        private Panel panelTransferOutPatToInPat;
        private LoggingButton btnCancel;
        private Panel panelUserContext;
        private Panel panel1;
        private UserContextView userContextView1;
        private PatientContextView patientContextView1;
        private LoggingButton btnNext;
        private LoggingButton btnBack;
        private Label lblStep2;
        private Label lblComments;
        private Label lblPlaceOfWorship;
        private Label lblEmbosserCard;
        private Label lblReligion;
        private Label lblSmoker;
        private Label lblValuablesCollected;
        private Label lblConfidentialStatus;
        private PhysicianSelectionView physicianSelectionView1;
        private ComboBox cboPlaceOfWorship;
        private ComboBox cboReligion;
        private ComboBox cboSmoker;
        private ComboBox cboValuablesCollected;
        public MaskedEditTextBox mtbEmbosserCard;
        private ComboBox cboConfidentialStatus;
        public MaskedEditTextBox mtbComments;
        private IContainer components = null;

        #endregion


        #region Constants

        #endregion
    }
}

