using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.DemographicsViews.ViewImpl;
using PatientAccess.UI.HelperClasses;
using Peradigm.Framework.Domain.Collections;
using SortOrder = Peradigm.Framework.Domain.Collections.SortOrder;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.DemographicsViews
{
    partial class DemographicsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton btnEditAKA;

        private PatientAccessComboBox cmbEthnicity;
        private PatientAccessComboBox cmbMaritalStatus;

        private PatientAccessComboBox cmbAppointment;
        private GroupBox grpPatientStay;
        private GroupBox grpPatientName;

        private DateTimePicker dateTimePicker;

      
        private GenderView genderView;
        private GenderView birthGenderView;
        private Label lblAKA;
        private Label lblDOB;
        private Label lblEthnicity;
        private Label lblEthnicity2;
        private Label lblFirstName;
        
        private Label lblMaritalStatus;
        private Label lblNationalID;
        private Label lblPatientAge;
        private Label lblRace;
        private Label lblRace2;
        private Label lblStaticAge;
        private Label lblStaticAKA;
        private Label lblStaticLastName; 
        private Label lblStaticMI;
        private Label lblStaticAdmitDate;

        private Label lblTime;
        private Label lblAppointment;

        private MaskedEditTextBox mtbAdmitDate;
        private MaskedEditTextBox mtbAdmitTime;
        private MaskedEditTextBox mtbDateOfBirth;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbMiddleInitial; 
        private MaskedEditTextBox mtbNationalID;
        private MaskedEditTextBox mtbPreopDate;
        private AddressView patientMailingAddrView;
        private AddressView patientPhysicalAddrView;
        private SSNControl ssnView;
        private Label label1;
        private DateTimePicker dateTimePicker_Preop;
        private EthnicityView ethnicityView;
        private EthnicityView ethnicity2View;

        private Label lblPatientGender;
        private Label lblBirthGender;

        private Label lblBirthTime;
        private MaskedEditTextBox mtbBirthTime;
        private SuffixView suffixView;
        private RaceView raceView;
        private RaceView race2View;
        private LoggingButton btnEditRace;
        private Label lblAdditionalRace;
       
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( DemographicsView ) );
            this.grpPatientName = new System.Windows.Forms.GroupBox();
            this.btnEditAKA = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAKA = new System.Windows.Forms.Label();
            this.lblStaticAKA = new System.Windows.Forms.Label(); 
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMI = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticLastName = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.mtbAdmitDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbPreopDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.mtbAdmitTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblEthnicity2 = new System.Windows.Forms.Label();

            this.lblDOB = new System.Windows.Forms.Label();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.lblRace = new System.Windows.Forms.Label();
            this.lblRace2 = new System.Windows.Forms.Label();
            this.lblEthnicity = new System.Windows.Forms.Label();
            this.mtbDateOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblNationalID = new System.Windows.Forms.Label();
            this.mtbNationalID = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.lblPatientAge = new System.Windows.Forms.Label();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.grpPatientStay = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_Preop = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAppointment = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAppointment = new System.Windows.Forms.Label();
            this.patientPhysicalAddrView = new PatientAccess.UI.AddressViews.AddressView();
            this.patientMailingAddrView = new PatientAccess.UI.AddressViews.AddressView();
            this.cmbEthnicity = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbMaritalStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblPatientGender = new System.Windows.Forms.Label();
            this.lblBirthGender = new System.Windows.Forms.Label();
            this.ethnicityView = new EthnicityView();
            this.ethnicity2View = new EthnicityView();

            this.genderView = new GenderView();
            this.birthGenderView = new GenderView();
            this.lblBirthTime = new System.Windows.Forms.Label();
            this.mtbBirthTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.suffixView = new SuffixView();
            this.raceView = new RaceView();
            this.race2View = new RaceView();
            this.btnEditRace = new LoggingButton();
            this.lblAdditionalRace = new Label();
            this.grpPatientName.SuspendLayout();
            this.grpPatientStay.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientName
            // 
            this.grpPatientName.Controls.Add( this.btnEditAKA );
            this.grpPatientName.Controls.Add( this.lblAKA );
            this.grpPatientName.Controls.Add( this.lblStaticAKA ); 
            this.grpPatientName.Controls.Add( this.suffixView );
            this.grpPatientName.Controls.Add( this.mtbMiddleInitial );
            this.grpPatientName.Controls.Add( this.lblStaticMI );
            this.grpPatientName.Controls.Add( this.mtbFirstName );
            this.grpPatientName.Controls.Add( this.lblFirstName );
            this.grpPatientName.Controls.Add( this.mtbLastName );
            this.grpPatientName.Controls.Add( this.lblStaticLastName );
            this.grpPatientName.Location = new System.Drawing.Point( 8, 8 );
            this.grpPatientName.Name = "grpPatientName";
            this.grpPatientName.Size = new System.Drawing.Size( 670, 90 );
            this.grpPatientName.TabIndex = 0;
            this.grpPatientName.TabStop = false;
            this.grpPatientName.Text = "Patient name";
            // 
            // btnEditAKA
            // 
            this.btnEditAKA.Location = new System.Drawing.Point( 462, 56 );
            this.btnEditAKA.Message = null;
            this.btnEditAKA.Name = "btnEditAKA";
            this.btnEditAKA.Size = new System.Drawing.Size( 90, 23 );
            this.btnEditAKA.TabIndex = 5;
            this.btnEditAKA.Text = "Manage A&KA...";
            this.btnEditAKA.Click += new System.EventHandler( this.btnEditAKA_Click );
            // 
            // lblAKA
            // 
            this.lblAKA.Location = new System.Drawing.Point( 38, 57 );
            this.lblAKA.Name = "lblAKA";
            this.lblAKA.Size = new System.Drawing.Size( 410, 23 );
            this.lblAKA.TabIndex = 0;
            this.lblAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticAKA
            // 
            this.lblStaticAKA.Location = new System.Drawing.Point( 9, 57 );
            this.lblStaticAKA.Name = "lblStaticAKA";
            this.lblStaticAKA.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticAKA.TabIndex = 0;
            this.lblStaticAKA.Text = "AKA:";
            this.lblStaticAKA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.Location = new System.Drawing.Point( 533, 21 );
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size( 18, 20 );
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMiddleInitial_Validating );
            // 
            // lblStaticMI
            // 
            this.lblStaticMI.Location = new System.Drawing.Point( 510, 24 );
            this.lblStaticMI.Name = "lblStaticMI";
            this.lblStaticMI.Size = new System.Drawing.Size( 21, 23 );
            this.lblStaticMI.TabIndex = 0;
            this.lblStaticMI.Text = "MI:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point( 335, 21 );
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size( 162, 20 );
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFirstName_Validating );
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 304, 24 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 30, 23 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point( 38, 21 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 255, 20 );
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // lblStaticLastName
            // 
            this.lblStaticLastName.Location = new System.Drawing.Point( 9, 24 );
            this.lblStaticLastName.Name = "lblStaticLastName";
            this.lblStaticLastName.Size = new System.Drawing.Size( 29, 23 );
            this.lblStaticLastName.TabIndex = 0;
            this.lblStaticLastName.Text = "Last:";
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.Location = new System.Drawing.Point( 9, 24 );
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size( 63, 23 );
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Admit date:";
            // 
            // mtbAdmitDate
            // 
            this.mtbAdmitDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitDate.KeyPressExpression = "^\\d*$";
            this.mtbAdmitDate.Location = new System.Drawing.Point( 80, 21 );
            this.mtbAdmitDate.Mask = "  /  /";
            this.mtbAdmitDate.MaxLength = 10;
            this.mtbAdmitDate.Name = "mtbAdmitDate";
            this.mtbAdmitDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbAdmitDate.TabIndex = 0;
            this.mtbAdmitDate.ValidationExpression = resources.GetString( "mtbAdmitDate.ValidationExpression" );
            this.mtbAdmitDate.TextChanged += new System.EventHandler( this.mtbAdmitDate_TextChanged );
            this.mtbAdmitDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitDate_Validating );
            // 
            // mtbPreopDate
            // 
            this.mtbPreopDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPreopDate.KeyPressExpression = "^\\d*$";
            this.mtbPreopDate.Location = new System.Drawing.Point( 80, 50 );
            this.mtbPreopDate.Mask = "  /  /";
            this.mtbPreopDate.MaxLength = 10;
            this.mtbPreopDate.Name = "mtbPreopDate";
            this.mtbPreopDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbPreopDate.TabIndex = 2;
            this.mtbPreopDate.ValidationExpression = resources.GetString( "mtbPreopDate.ValidationExpression" );
            this.mtbPreopDate.TextChanged += new System.EventHandler( this.mtbPreopDate_TextChanged );
            this.mtbPreopDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPreopDate_Validating );
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point( 150, 21 );
            this.dateTimePicker.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 175, 24 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 40, 23 );
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time:";
            // 
            // mtbAdmitTime
            // 
            this.mtbAdmitTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitTime.KeyPressExpression = "^\\d*$";
            this.mtbAdmitTime.Location = new System.Drawing.Point( 209, 21 );
            this.mtbAdmitTime.Mask = "  :";
            this.mtbAdmitTime.MaxLength = 5;
            this.mtbAdmitTime.Name = "mtbAdmitTime";
            this.mtbAdmitTime.Size = new System.Drawing.Size( 48, 20 );
            this.mtbAdmitTime.TabIndex = 1;
            this.mtbAdmitTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbAdmitTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAdmitTime_Validating );
           
            // 
            // lblDOB
            // 
            this.lblDOB.Location = new System.Drawing.Point( 8, 134 );
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size( 60, 23 );
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Text = "DOB:";
            // 
            // lblMaritalStatus
            // 
            this.lblMaritalStatus.Location = new System.Drawing.Point( 8, 186 );
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size( 75, 23 );
            this.lblMaritalStatus.TabIndex = 0;
            this.lblMaritalStatus.Text = "Marital status:";
            // 
            // lblRace
            // 
            this.lblRace.Location = new System.Drawing.Point( 8, 216 );
            this.lblRace.Name = "lblRace";
            this.lblRace.Size = new System.Drawing.Size(45, 23);
            this.lblRace.TabIndex = 0;
            this.lblRace.Text = "Race:";
            // 
            // lblRace2
            // 
            this.lblRace2.Location = new System.Drawing.Point(220, 216);
            this.lblRace2.Name = "lblRace2";
            this.lblRace2.Size = new System.Drawing.Size(60, 23);
            this.lblRace2.TabIndex = 0;
            this.lblRace2.Text = "Race 2:";
            // 
            // lblEthnicity
            // 
            this.lblEthnicity.Location = new System.Drawing.Point( 8, 245 );
            this.lblEthnicity.Name = "lblEthnicity";
            this.lblEthnicity.Size = new System.Drawing.Size( 60, 23 );
            this.lblEthnicity.TabIndex = 0;
            this.lblEthnicity.Text = "Ethnicity:";
            // 
            // lblEthnicity2
            // 
            this.lblEthnicity2.Location = new System.Drawing.Point(220, 245);
            this.lblEthnicity2.Name = "lblEthnicity2";
            this.lblEthnicity2.Size = new System.Drawing.Size(60, 23);
            this.lblEthnicity2.TabIndex = 0;
            this.lblEthnicity2.Text = "Ethnicity 2:";

            // 
            // mtbDateOfBirth
            // 
            this.mtbDateOfBirth.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDateOfBirth.KeyPressExpression = "^\\d*$";
            this.mtbDateOfBirth.Location = new System.Drawing.Point( 83, 132 );
            this.mtbDateOfBirth.Mask = "  /  /";
            this.mtbDateOfBirth.MaxLength = 10;
            this.mtbDateOfBirth.Name = "mtbDateOfBirth";
            this.mtbDateOfBirth.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDateOfBirth.TabIndex = 10;
            this.mtbDateOfBirth.ValidationExpression = resources.GetString( "mtbDateOfBirth.ValidationExpression" );
            this.mtbDateOfBirth.TextChanged += new System.EventHandler(this.mtbDateOfBirth_TextChanged);
            this.mtbDateOfBirth.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDateOfBirth_Validating );
            // 
            // lblNationalID
            // 
            this.lblNationalID.Location = new System.Drawing.Point( 8, 345 );
            this.lblNationalID.Name = "lblNationalID";
            this.lblNationalID.Size = new System.Drawing.Size( 70, 23 );
            this.lblNationalID.TabIndex = 0;
            this.lblNationalID.Text = "National ID:";
            // 
            // mtbNationalID
            // 
            this.mtbNationalID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbNationalID.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbNationalID.Location = new System.Drawing.Point( 83, 345 );
            this.mtbNationalID.Mask = "";
            this.mtbNationalID.MaxLength = 12;
            this.mtbNationalID.Name = "mtbNationalID";
            this.mtbNationalID.Size = new System.Drawing.Size( 95, 20 );
            this.mtbNationalID.TabIndex = 18;
            this.mtbNationalID.Validating += new System.ComponentModel.CancelEventHandler( this.mtbNationalID_Validating );
            // 
            // lblStaticAge
            // 
            this.lblStaticAge.Location = new System.Drawing.Point( 166, 138 );
            this.lblStaticAge.Name = "lblStaticAge";
            this.lblStaticAge.Size = new System.Drawing.Size( 40, 23 );
            this.lblStaticAge.TabIndex = 0;
            this.lblStaticAge.Text = "Age:";
            // 
            // lblPatientAge
            // 
            this.lblPatientAge.Location = new System.Drawing.Point( 194, 138 );
            this.lblPatientAge.Name = "lblPatientAge";
            this.lblPatientAge.Size = new System.Drawing.Size( 95, 23 );
            this.lblPatientAge.TabIndex = 0;
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point( 8, 265 );
            this.ssnView.Model = null;
            this.ssnView.ModelAccount = null;
            this.ssnView.Name = "ssnView";
            this.ssnView.Size = new System.Drawing.Size( 265, 72 );
            this.ssnView.SsnContext = SsnViewContext.DemographicsView;
            this.ssnView.TabIndex = 17;
            this.ssnView.ssnNumberChanged += new System.EventHandler( this.ssnView_ssnNumberChanged );
            // 
            // grpPatientStay
            // 
            this.grpPatientStay.Controls.Add( this.mtbPreopDate );
            this.grpPatientStay.Controls.Add( this.dateTimePicker_Preop );
            this.grpPatientStay.Controls.Add( this.label1 );
            this.grpPatientStay.Controls.Add( this.cmbAppointment );
            this.grpPatientStay.Controls.Add( this.lblAppointment );
            this.grpPatientStay.Controls.Add( this.mtbAdmitDate );
            this.grpPatientStay.Controls.Add( this.mtbAdmitTime );
            this.grpPatientStay.Controls.Add( this.lblTime );
            this.grpPatientStay.Controls.Add( this.lblStaticAdmitDate );
            this.grpPatientStay.Controls.Add( this.dateTimePicker );
            this.grpPatientStay.Location = new System.Drawing.Point( 688, 8 );
            this.grpPatientStay.Name = "grpPatientStay";
            this.grpPatientStay.Size = new System.Drawing.Size( 300, 118 );
            this.grpPatientStay.TabIndex = 1;
            this.grpPatientStay.TabStop = false;
            this.grpPatientStay.Text = "Patient stay information";
            // 
            // dateTimePicker_Preop
            // 
            this.dateTimePicker_Preop.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker_Preop.Checked = false;
            this.dateTimePicker_Preop.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_Preop.Location = new System.Drawing.Point( 150, 50 );
            this.dateTimePicker_Preop.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker_Preop.Name = "dateTimePicker_Preop";
            this.dateTimePicker_Preop.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker_Preop.TabIndex = 3;
            this.dateTimePicker_Preop.TabStop = false;
            this.dateTimePicker_Preop.CloseUp += new System.EventHandler( this.dateTimePicker_Preop_CloseUp );
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 9, 52 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 68, 23 );
            this.label1.TabIndex = 9;
            this.label1.Text = "Pre-op date:";
            // 
            // cmbAppointment
            // 
            this.cmbAppointment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppointment.Location = new System.Drawing.Point( 80, 79 );
            this.cmbAppointment.Name = "cmbAppointment";
            this.cmbAppointment.Size = new System.Drawing.Size( 204, 21 );
            this.cmbAppointment.TabIndex = 4;
            this.cmbAppointment.SelectedIndexChanged += new System.EventHandler( this.cmbAppointment_SelectedIndexChanged );
            this.cmbAppointment.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAppointment_Validating );
            // 
            // lblAppointment
            // 
            this.lblAppointment.Location = new System.Drawing.Point( 9, 80 );
            this.lblAppointment.Name = "lblAppointment";
            this.lblAppointment.Size = new System.Drawing.Size( 68, 23 );
            this.lblAppointment.TabIndex = 0;
            this.lblAppointment.Text = "Appointment:";
            // 
            // patientPhysicalAddrView
            // 
            this.patientPhysicalAddrView.Context = null;
            this.patientPhysicalAddrView.EditAddressButtonText = "Edit Address...";
            this.patientPhysicalAddrView.IsAddressWithCounty = true;
            this.patientPhysicalAddrView.IsAddressWithStreet2 = true;
            this.patientPhysicalAddrView.KindOfTargetParty = null;
            this.patientPhysicalAddrView.Location = new System.Drawing.Point(722, 127);
            this.patientPhysicalAddrView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONE;
            this.patientPhysicalAddrView.Model = null;
            this.patientPhysicalAddrView.Model_ContactPoint = null;
            this.patientPhysicalAddrView.Name = "patientPhysicalAddrView";
            this.patientPhysicalAddrView.PatientAccount = null;
            this.patientPhysicalAddrView.ShowStatus = false;
            this.patientPhysicalAddrView.Size = new System.Drawing.Size(265, 178);
            this.patientPhysicalAddrView.TabIndex = 23;
            this.patientPhysicalAddrView.AddressChanged += new System.EventHandler( this.PatientPhysicalAddressView_AddressChangedEventHandler );
            this.patientPhysicalAddrView.PhoneNumberChanged += new System.EventHandler( this.patientPhysicalAddrView_PhoneNumberChanged );
            // 
            // patientMailingAddrView
            // 
            this.patientMailingAddrView.Context = null;
            this.patientMailingAddrView.EditAddressButtonText = "Edit Address...";
            this.patientMailingAddrView.IsAddressWithCounty = false;
            this.patientMailingAddrView.IsAddressWithStreet2 = true;
            this.patientMailingAddrView.KindOfTargetParty = null;
            this.patientMailingAddrView.Location = new System.Drawing.Point( 455, 128 );
            this.patientMailingAddrView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONECELL;
            this.patientMailingAddrView.Model = null;
            this.patientMailingAddrView.Model_ContactPoint = null;
            this.patientMailingAddrView.Name = "patientMailingAddrView";
            this.patientMailingAddrView.PatientAccount = null;
            this.patientMailingAddrView.ShowStatus = false;
            this.patientMailingAddrView.Size = new System.Drawing.Size( 265, 245 );
            this.patientMailingAddrView.TabIndex = 19;
            this.patientMailingAddrView.AddressChanged += new System.EventHandler( this.PatientMailingAddressView_AddressChangedEventHandler );
            this.patientMailingAddrView.AreaCodeChanged += new System.EventHandler( this.patientMailingAddrView_AreaCodeChanged );
            this.patientMailingAddrView.PhoneNumberChanged += new System.EventHandler( this.patientMailingAddrView_PhoneNumberChanged );
            this.patientMailingAddrView.CellPhoneNumberChanged += new System.EventHandler( this.patientMailingAddrView_CellPhoneNumberChanged );
           // this.patientMailingAddrView.EmailChanged += new System.EventHandler( this.patientMailingAddrView_EmailChanged );
            
            //
            // ethnicityView
            //
            this.ethnicityView.Location = new System.Drawing.Point(83, 242);
            this.ethnicityView.Name = "ethnicityView";
            this.ethnicityView.TabIndex = 15;
            this.ethnicityView.Size = new System.Drawing.Size(135, 25);
            //
            // ethnicity2View
            //
            this.ethnicity2View.Location = new System.Drawing.Point(280, 242);
            this.ethnicity2View.Name = "ethnicity2View";
            this.ethnicity2View.TabIndex = 16;
            this.ethnicity2View.Size = new System.Drawing.Size(135, 25);

            //
            // raceView
            //
            this.raceView.Location = new System.Drawing.Point(83, 213);
            this.raceView.Name = "raceView";
            this.raceView.TabIndex = 13;
            this.raceView.Size = new System.Drawing.Size(135, 25);
            this.raceView.Leave += new EventHandler(raceView_Leave);
            //
            // race2View
            //
            this.race2View.Location = new System.Drawing.Point(270, 213);
            this.race2View.Name = "race2View";
            this.race2View.TabIndex = 14;
            this.race2View.Size = new System.Drawing.Size(135, 25);
            this.race2View.Leave += new EventHandler(race2View_Leave);
            //
            //lblAdditionalRace
            //
            this.lblAdditionalRace.Location = new System.Drawing.Point(407, 180);
            this.lblAdditionalRace.Name = "lblAdditionalRace";
            this.lblAdditionalRace.Text = "Additional\nRace";
            this.lblAdditionalRace.TabIndex = 14;
            this.lblAdditionalRace.Size = new System.Drawing.Size(100, 100);
            //
            // btnEditRace
            //
            this.btnEditRace.Location = new System.Drawing.Point(407, 213);
            this.btnEditRace.Name = "btnEditRace";
            this.btnEditRace.Text = "Edit";
            this.btnEditRace.TabIndex = 14;
            this.btnEditRace.Size = new System.Drawing.Size(36, 23);
            this.btnEditRace.Click += new System.EventHandler( btnEditRace_Click );
            // 
            // cmbMaritalStatus
            // 
            this.cmbMaritalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaritalStatus.Location = new System.Drawing.Point( 83, 184 );
            this.cmbMaritalStatus.Name = "cmbMaritalStatus";
            this.cmbMaritalStatus.Size = new System.Drawing.Size( 100, 21 );
            this.cmbMaritalStatus.TabIndex = 12;
            this.cmbMaritalStatus.SelectedIndexChanged += new System.EventHandler( this.cmbMaritalStatus_SelectedIndexChanged );
            this.cmbMaritalStatus.Validating += new System.ComponentModel.CancelEventHandler( this.cmbMaritalStatus_Validating );

            // lblPatientGender
            // 
            this.lblPatientGender.Location = new System.Drawing.Point(8, 108);
            this.lblPatientGender.Name = "lblPatientGender";
            this.lblPatientGender.Size = new System.Drawing.Size(60, 23);
            this.lblPatientGender.TabIndex = 0;
            this.lblPatientGender.Text = "Gender:";
            // 
            // Gender View
            // 
            this.genderView.Location = new System.Drawing.Point(80, 102);
            this.genderView.Model = null;
            this.genderView.Name = "genderView";
            this.genderView.Size = new System.Drawing.Size(100, 30);
            this.genderView.TabIndex = 9;
            this.genderView.GenderViewPresenter = null;
            // 
            // lblBirthGender
            // 
            this.lblBirthGender.Location = new System.Drawing.Point(236, 108);
            this.lblBirthGender.Name = "lblBirthGender";
            this.lblBirthGender.Size = new System.Drawing.Size(60, 23);
            this.lblBirthGender.TabIndex = 0;
            this.lblBirthGender.Text = "Birth Sex:";

            // 
            // Birth Gender View
            // 
            this.birthGenderView.Location = new System.Drawing.Point(300, 102);
            this.birthGenderView.Model = null;
            this.birthGenderView.Name = "birthGenderView";
            this.birthGenderView.Size = new System.Drawing.Size(100, 30);
            this.birthGenderView.TabIndex = 9;
            this.birthGenderView.GenderViewPresenter = null;
          
            // 
            // lblBirthTime
            // 
            this.lblBirthTime.Location = new System.Drawing.Point( 8, 160 );
            this.lblBirthTime.Name = "lblBirthTime";
            this.lblBirthTime.Size = new System.Drawing.Size( 60, 23 );
            this.lblBirthTime.TabIndex = 24;
            this.lblBirthTime.Text = "Birth Time:";
            // 
            // mtbBirthTime
            // 
            this.mtbBirthTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbBirthTime.KeyPressExpression = "^\\d*$";
            this.mtbBirthTime.Location = new System.Drawing.Point( 83, 158 );
            this.mtbBirthTime.Mask = "  :";
            this.mtbBirthTime.MaxLength = 5;
            this.mtbBirthTime.Name = "mtbBirthTime";
            this.mtbBirthTime.Size = new System.Drawing.Size( 48, 20 );
            this.mtbBirthTime.TabIndex = 11;
            this.mtbBirthTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbBirthTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbBirthTime_Validating );
            //
            // suffixView
            // 
            this.suffixView.Location = new System.Drawing.Point(566, 19);
            this.suffixView.Name = "suffixView";
            this.suffixView.Size = new System.Drawing.Size(100, 27);
            this.suffixView.TabIndex = 4;
            this.suffixView.Visible = true;
            // 
            // DemographicsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.mtbBirthTime );
            this.Controls.Add( this.lblBirthTime );
            this.Controls.Add( this.grpPatientStay );
            this.Controls.Add( this.patientPhysicalAddrView );
            this.Controls.Add( this.patientMailingAddrView );
            this.Controls.Add( this.ssnView );
            this.Controls.Add( this.mtbNationalID );
            this.Controls.Add( this.lblPatientAge );
            this.Controls.Add( this.lblStaticAge );
            this.Controls.Add( this.lblNationalID ); 

            this.Controls.Add( this.raceView );
            this.Controls.Add( this.race2View );
            this.Controls.Add( this.cmbMaritalStatus );
            this.Controls.Add( this.mtbDateOfBirth );

            this.Controls.Add(this.ethnicityView);
            this.Controls.Add(this.ethnicity2View);
            this.Controls.Add(this.lblEthnicity2);

            this.Controls.Add(this.lblPatientGender);
            this.Controls.Add(this.lblBirthGender);

             
            this.Controls.Add(this.genderView);
            this.Controls.Add(this.birthGenderView);
            this.Controls.Add( this.lblEthnicity );
            this.Controls.Add( this.lblRace );
            this.Controls.Add( this.lblRace2 );
            this.Controls.Add( this.btnEditRace );
            this.Controls.Add( this.lblAdditionalRace );
            this.Controls.Add( this.lblMaritalStatus );
            this.Controls.Add( this.lblDOB );
      
            this.Controls.Add( this.grpPatientName );
            this.Name = "DemographicsView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Enter += new System.EventHandler( this.DemographicsView_Enter );
            this.Leave += new System.EventHandler( this.DemographicsView_Leave );
            this.Disposed += new System.EventHandler( this.DemographicsView_Disposed );
            this.grpPatientName.ResumeLayout( false );
            this.grpPatientName.PerformLayout();
            this.grpPatientStay.ResumeLayout( false );
            this.grpPatientStay.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();
          }
        
        #endregion
    }
}