using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.QuickAccountCreation.Presenters;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.CptCodes.ViewImpl;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    partial class PAIWalkinAccountCreationView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PAIWalkinAccountCreationView));
            this.lblBirthGender = new System.Windows.Forms.Label();
            this.grpPatientName = new System.Windows.Forms.GroupBox();
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMI = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticLastName = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.mtbAdmitDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.mtbAdmitTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblDOB = new System.Windows.Forms.Label();
            this.mtbDateOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.lblPatientAge = new System.Windows.Forms.Label();
            this.grpPatientStay = new System.Windows.Forms.GroupBox();
            this.cmbAppointment = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAppointment = new System.Windows.Forms.Label();
            this.mtbProcedure = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblProcedure = new System.Windows.Forms.Label();
            this.mtbComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblComplaint = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.patientTypeHSVLocationView = new PatientAccess.UI.QuickAccountCreation.ViewImpl.QuickPatientTypeHSVLocationView();
            this.physicianSelectionView1 = new PatientAccess.UI.QuickAccountCreation.ViewImpl.QuickPhysicianSelectionView();
            this.insuranceView = new PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl.PAIWalkinInsuranceView();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.cptCodesView = new PatientAccess.UI.CptCodes.ViewImpl.CptCodesView();
            this.birthGenderView = new PatientAccess.UI.DemographicsViews.ViewImpl.GenderView();
            this.genderView = new PatientAccess.UI.DemographicsViews.ViewImpl.GenderView();
            this.grpPatientName.SuspendLayout();
            this.grpPatientStay.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientName
            // 
            this.grpPatientName.Controls.Add(this.mtbMiddleInitial);
            this.grpPatientName.Controls.Add(this.lblStaticMI);
            this.grpPatientName.Controls.Add(this.mtbFirstName);
            this.grpPatientName.Controls.Add(this.lblFirstName);
            this.grpPatientName.Controls.Add(this.mtbLastName);
            this.grpPatientName.Controls.Add(this.lblStaticLastName);
            this.grpPatientName.Location = new System.Drawing.Point(8, 4);
            this.grpPatientName.Name = "grpPatientName";
            this.grpPatientName.Size = new System.Drawing.Size(653, 51);
            this.grpPatientName.TabIndex = 0;
            this.grpPatientName.TabStop = false;
            this.grpPatientName.Text = "Patient name";
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.Location = new System.Drawing.Point(533, 21);
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size(18, 20);
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.Validating += new System.ComponentModel.CancelEventHandler(this.MiddleInitialOnValidating);
            // 
            // lblStaticMI
            // 
            this.lblStaticMI.Location = new System.Drawing.Point(510, 24);
            this.lblStaticMI.Name = "lblStaticMI";
            this.lblStaticMI.Size = new System.Drawing.Size(21, 23);
            this.lblStaticMI.TabIndex = 0;
            this.lblStaticMI.Text = "MI:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point(335, 21);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(162, 20);
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.FirstNameOnValidating);
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(304, 24);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(30, 23);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point(38, 21);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(255, 20);
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler(this.LastNameOnValidating);
            // 
            // lblStaticLastName
            // 
            this.lblStaticLastName.Location = new System.Drawing.Point(9, 24);
            this.lblStaticLastName.Name = "lblStaticLastName";
            this.lblStaticLastName.Size = new System.Drawing.Size(29, 23);
            this.lblStaticLastName.TabIndex = 0;
            this.lblStaticLastName.Text = "Last:";
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.Location = new System.Drawing.Point(9, 24);
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size(63, 23);
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Admit date:";
            // 
            // mtbAdmitDate
            // 
            this.mtbAdmitDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitDate.KeyPressExpression = "^\\d*$";
            this.mtbAdmitDate.Location = new System.Drawing.Point(80, 21);
            this.mtbAdmitDate.Mask = "  /  /";
            this.mtbAdmitDate.MaxLength = 10;
            this.mtbAdmitDate.Name = "mtbAdmitDate";
            this.mtbAdmitDate.Size = new System.Drawing.Size(70, 20);
            this.mtbAdmitDate.TabIndex = 0;
            this.mtbAdmitDate.ValidationExpression = resources.GetString("mtbAdmitDate.ValidationExpression");
            this.mtbAdmitDate.Validating += new System.ComponentModel.CancelEventHandler(this.AdmitDateOnValidating);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point(150, 21);
            this.dateTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateTimePicker.TabIndex = 1;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler(this.DateTimePickerOnCloseUp);
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(175, 24);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(40, 23);
            this.lblTime.TabIndex = 1;
            this.lblTime.Text = "Time:";
            // 
            // mtbAdmitTime
            // 
            this.mtbAdmitTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdmitTime.KeyPressExpression = "^\\d*$";
            this.mtbAdmitTime.Location = new System.Drawing.Point(209, 21);
            this.mtbAdmitTime.Mask = "  :";
            this.mtbAdmitTime.MaxLength = 5;
            this.mtbAdmitTime.Name = "mtbAdmitTime";
            this.mtbAdmitTime.Size = new System.Drawing.Size(48, 20);
            this.mtbAdmitTime.TabIndex = 2;
            this.mtbAdmitTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbAdmitTime.Validating += new System.ComponentModel.CancelEventHandler(this.AdmitTimeOnValidating);
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(8, 69);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(60, 23);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            this.lblBirthGender.Location = new System.Drawing.Point(180, 70);
            this.lblBirthGender.Name = "lblBirthGender";
            this.lblBirthGender.Size = new System.Drawing.Size(77, 27);
            this.lblBirthGender.TabIndex = 14;
            this.lblBirthGender.Text = "Birth Sex:";
            // 
            // birthGenderView
            // 
            this.birthGenderView.GenderViewPresenter = null;
            this.birthGenderView.Location = new System.Drawing.Point(240, 63);
            this.birthGenderView.Model = null;
            this.birthGenderView.Name = "birthGenderView";
            this.birthGenderView.Size = new System.Drawing.Size(91, 28);
            this.birthGenderView.TabIndex = 3;
            
            // 
            // genderView
            // 
            this.genderView.GenderViewPresenter = null;
            this.genderView.Location = new System.Drawing.Point(81, 63);
            this.genderView.Model = null;
            this.genderView.Name = "genderView";
            this.genderView.Size = new System.Drawing.Size(89, 25);
            this.genderView.TabIndex = 2;
            // 
            // lblDOB
            // 
            this.lblDOB.Location = new System.Drawing.Point(8, 94);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(60, 23);
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Text = "DOB:";
            // 
            // mtbDateOfBirth
            // 
            this.mtbDateOfBirth.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDateOfBirth.KeyPressExpression = "^\\d*$";
            this.mtbDateOfBirth.Location = new System.Drawing.Point(83, 94);
            this.mtbDateOfBirth.Mask = "  /  /";
            this.mtbDateOfBirth.MaxLength = 10;
            this.mtbDateOfBirth.Name = "mtbDateOfBirth";
            this.mtbDateOfBirth.Size = new System.Drawing.Size(70, 20);
            this.mtbDateOfBirth.TabIndex = 3;
            this.mtbDateOfBirth.ValidationExpression = resources.GetString("mtbDateOfBirth.ValidationExpression");
            this.mtbDateOfBirth.Validating += new System.ComponentModel.CancelEventHandler(this.DateOfBirthOnValidating);
            // 
            // lblStaticAge
            // 
            this.lblStaticAge.Location = new System.Drawing.Point(166, 94);
            this.lblStaticAge.Name = "lblStaticAge";
            this.lblStaticAge.Size = new System.Drawing.Size(40, 23);
            this.lblStaticAge.TabIndex = 0;
            this.lblStaticAge.Text = "Age:";
            // 
            // lblPatientAge
            // 
            this.lblPatientAge.Location = new System.Drawing.Point(194, 94);
            this.lblPatientAge.Name = "lblPatientAge";
            this.lblPatientAge.Size = new System.Drawing.Size(95, 23);
            this.lblPatientAge.TabIndex = 0;
            // 
            // grpPatientStay
            // 
            this.grpPatientStay.Controls.Add(this.cmbAppointment);
            this.grpPatientStay.Controls.Add(this.lblAppointment);
            this.grpPatientStay.Controls.Add(this.mtbAdmitDate);
            this.grpPatientStay.Controls.Add(this.mtbAdmitTime);
            this.grpPatientStay.Controls.Add(this.lblTime);
            this.grpPatientStay.Controls.Add(this.lblStaticAdmitDate);
            this.grpPatientStay.Controls.Add(this.dateTimePicker);
            this.grpPatientStay.Location = new System.Drawing.Point(672, 4);
            this.grpPatientStay.Name = "grpPatientStay";
            this.grpPatientStay.Size = new System.Drawing.Size(300, 74);
            this.grpPatientStay.TabIndex = 1;
            this.grpPatientStay.TabStop = false;
            this.grpPatientStay.Text = "Patient stay information";
            // 
            // cmbAppointment
            // 
            this.cmbAppointment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAppointment.Location = new System.Drawing.Point(80, 48);
            this.cmbAppointment.Name = "cmbAppointment";
            this.cmbAppointment.Size = new System.Drawing.Size(204, 21);
            this.cmbAppointment.TabIndex = 3;
            this.cmbAppointment.SelectedIndexChanged += new System.EventHandler(this.AppointmentOnSelectedIndexChanged);
            this.cmbAppointment.Validating += new System.ComponentModel.CancelEventHandler(this.AppointmentOnValidating);
            // 
            // lblAppointment
            // 
            this.lblAppointment.Location = new System.Drawing.Point(9, 49);
            this.lblAppointment.Name = "lblAppointment";
            this.lblAppointment.Size = new System.Drawing.Size(68, 23);
            this.lblAppointment.TabIndex = 0;
            this.lblAppointment.Text = "Appointment:";
            // 
            // mtbProcedure
            // 
            this.mtbProcedure.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbProcedure.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbProcedure.Location = new System.Drawing.Point(239, 199);
            this.mtbProcedure.Mask = "";
            this.mtbProcedure.MaxLength = 74;
            this.mtbProcedure.Multiline = true;
            this.mtbProcedure.Name = "mtbProcedure";
            this.mtbProcedure.Size = new System.Drawing.Size(285, 48);
            this.mtbProcedure.TabIndex = 9;
            this.mtbProcedure.Validating += new System.ComponentModel.CancelEventHandler(this.ProcedureOnValidating);
            // 
            // lblProcedure
            // 
            this.lblProcedure.Location = new System.Drawing.Point(239, 185);
            this.lblProcedure.Name = "lblProcedure";
            this.lblProcedure.Size = new System.Drawing.Size(75, 11);
            this.lblProcedure.TabIndex = 12;
            this.lblProcedure.Text = "Procedure:";
            

            // 
            // mtbComplaint
            // 
            this.mtbComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComplaint.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbComplaint.Location = new System.Drawing.Point(239, 134);
            this.mtbComplaint.Mask = "";
            this.mtbComplaint.MaxLength = 74;
            this.mtbComplaint.Multiline = true;
            this.mtbComplaint.Name = "mtbComplaint";
            this.mtbComplaint.Size = new System.Drawing.Size(285, 48);
            this.mtbComplaint.TabIndex = 8;
            this.mtbComplaint.Validating += new System.ComponentModel.CancelEventHandler(this.ComplaintOnValidating);
            // 
            // lblComplaint
            // 
            this.lblComplaint.Location = new System.Drawing.Point(239, 118);
            this.lblComplaint.Name = "lblComplaint";
            this.lblComplaint.Size = new System.Drawing.Size(88, 14);
            this.lblComplaint.TabIndex = 7;
            this.lblComplaint.Text = "Chief complaint:";
            // 
            // groupBox
            // 
            this.groupBox.Location = new System.Drawing.Point(595, 78);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(377, 173);
            this.groupBox.TabIndex = 10;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Patient Insurance Information";
            // 
            // patientTypeHSVLocationView
            // 
            this.patientTypeHSVLocationView.Location = new System.Drawing.Point(295, 58);
            this.patientTypeHSVLocationView.Model = null;
            this.patientTypeHSVLocationView.Name = "patientTypeHSVLocationView";
            this.patientTypeHSVLocationView.Size = new System.Drawing.Size(300, 61);
            this.patientTypeHSVLocationView.TabIndex = 6;
            // 
            // physicianSelectionView1
            // 
            this.physicianSelectionView1.AdmPhysicianNumber = "";
            this.physicianSelectionView1.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView1.Location = new System.Drawing.Point(8, 251);
            this.physicianSelectionView1.Model = null;
            this.physicianSelectionView1.Name = "physicianSelectionView1";
            this.physicianSelectionView1.RefPhysicianNumber = "";
            this.physicianSelectionView1.Size = new System.Drawing.Size(948, 151);
            this.physicianSelectionView1.TabIndex = 6;
            // 
            // insuranceView
            // 
            this.insuranceView.Account = null;
            this.insuranceView.BackColor = System.Drawing.Color.White;
            this.insuranceView.InsuranceDetailsDialog = null;
            this.insuranceView.Location = new System.Drawing.Point(598, 94);
            this.insuranceView.Model = null;
            this.insuranceView.ModelCoverage = null;
            this.insuranceView.Name = "insuranceView";
            this.insuranceView.Padding = new System.Windows.Forms.Padding(3);
            this.insuranceView.Size = new System.Drawing.Size(364, 153);
            this.insuranceView.TabIndex = 11;
            this.insuranceView.CoverageResetClickedEvent += new PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl.CoverageDelegate(this.CatchCoverageResetClickedEvent);
            this.insuranceView.PlanSelectedEvent += new PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl.CoverageDelegate(this.CatchPlanSelectedEvent);
            this.insuranceView.CoverageUpdatedEvent += new PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl.CoveragesDelegate(this.CatchCoverageEvent);
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point(8, 126);
            this.ssnView.Model = null;
            this.ssnView.ModelAccount = null;
            this.ssnView.Name = "ssnView";
            this.ssnView.Size = new System.Drawing.Size(193, 77);
            this.ssnView.SsnContext = SsnViewContext.PAIWalkinAccountCreationView;
            this.ssnView.TabIndex = 4;
            this.ssnView.ssnNumberChanged += new System.EventHandler(this.SsnViewOnSsnNumberChanged);
            // 
            // cptCodesView
            // 
            this.cptCodesView.CptCodesPresenter = null;
            this.cptCodesView.Location = new System.Drawing.Point(8, 209);
            this.cptCodesView.Model = null;
            this.cptCodesView.Name = "cptCodesView";
            this.cptCodesView.Size = new System.Drawing.Size(210, 25);
            this.cptCodesView.TabIndex = 5;
            // 
            // PAIWalkinAccountCreationView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cptCodesView);
            this.Controls.Add(this.ssnView);
            this.Controls.Add(this.patientTypeHSVLocationView);
            this.Controls.Add(this.physicianSelectionView1);
            this.Controls.Add(this.insuranceView);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.mtbProcedure);
            this.Controls.Add(this.lblProcedure);
            this.Controls.Add(this.mtbComplaint);
            this.Controls.Add(this.lblComplaint);
            this.Controls.Add(this.grpPatientStay);
            this.Controls.Add(this.lblPatientAge);
            this.Controls.Add(this.lblStaticAge);
            this.Controls.Add(this.mtbDateOfBirth);
            this.Controls.Add(this.lblDOB);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.grpPatientName);
            this.Controls.Add(this.birthGenderView);
            this.Controls.Add(this.lblBirthGender);
            this.Controls.Add(this.genderView);
            this.Name = "PAIWalkinAccountCreationView";
            this.Size = new System.Drawing.Size(1024, 447);
            this.Enter += new System.EventHandler(this.PAIWalkinAccountCreationViewOnEnter);
            this.Leave += new System.EventHandler(this.PAIWalkinAccountCreationViewOnLeave);
            this.Disposed += new System.EventHandler(this.PAIWalkinAccountCreationViewOnDisposed);
            this.grpPatientName.ResumeLayout(false);
            this.grpPatientName.PerformLayout();
            this.grpPatientStay.ResumeLayout(false);
            this.grpPatientStay.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        #region Data Elements

        private PatientAccessComboBox cmbAppointment;
        private GroupBox grpPatientStay;
        private GroupBox grpPatientName;
        private DateTimePicker dateTimePicker;
        private Label lblDOB;
        private Label lblFirstName;
        private Label lblGender;
        private Label lblPatientAge;
        private Label lblStaticAge;
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
        private MaskedEditTextBox mtbProcedure;
        private Label lblProcedure;
        private MaskedEditTextBox mtbComplaint;
        private Label lblComplaint;
        private GroupBox groupBox;
        private QuickPatientTypeHSVLocationView patientTypeHSVLocationView;
        private QuickPhysicianSelectionView physicianSelectionView1;
        private PAIWalkinInsuranceView insuranceView;
        private SSNControl ssnView;
        private CptCodesView cptCodesView;
        private DemographicsViews.ViewImpl.GenderView genderView;
        private Label lblBirthGender;
        private DemographicsViews.ViewImpl.GenderView birthGenderView;
        #endregion
      

    }
}
