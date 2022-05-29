using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for CensusView.
    /// </summary>
    public class CensusView : TimeOutFormView
    {

        #region Events
        #endregion

        # region Event Handlers

        private void CensusView_Closing(object sender, CancelEventArgs e)
        {
            // abort backgroundworkers before close
            CensusEventAggregator.GetInstance().RaiseCloseEvent(sender, e);
            CensusEventAggregator.GetInstance().RemoveAllListeners();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CensusView_Load(object sender, EventArgs e)
        {
            switch (this.SelectedMenu)
            {
                case PATIENT:
                    tabCensusEnquiry.SelectedTab = tabPatient;
                    break;
                case PHYSICIAN:
                    tabCensusEnquiry.SelectedTab = tabPhysician;
                    break;
                case NURSINGSTATION:
                    tabCensusEnquiry.SelectedTab = tabNursingStations;
                    break;
                case BLOODLESS:
                    tabCensusEnquiry.SelectedTab = tabBloodless;
                    break;
                case RELIGIONS:
                    tabCensusEnquiry.SelectedTab = tabReligions;
                    break;
                case ADT:
                    tabCensusEnquiry.SelectedTab = tabADT;
                    break;
                case PAYOR:
                    tabCensusEnquiry.SelectedTab = tabPayor;
                    break;
                default:
                    break;
            }
        }

        private void tabCensusEnquiry_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (tabCensusEnquiry.SelectedIndex)
            {
                case PATIENT:
                    this.censusbyPatientView.UpdateView();
                    break;
                case PHYSICIAN:
                    this.censusbyPhysicianView.UpdateView();
                    break;
                case NURSINGSTATION:
                    this.censusbyNursingStationView.UpdateView();
                    break;
                case BLOODLESS:
                    this.censusOfBloodlessPatientsView.UpdateView();
                    break;
                case RELIGIONS:
                    this.censusbyReligionView.UpdateView();
                    break;
                case ADT:
                    this.censusbyADT.UpdateView();
                    break;
                case PAYOR:
                    this.censusbyInsurance.UpdateView();
                    break;
                default:
                    break;
            }

        }
        private void CensusView_Deactivate(object sender, EventArgs e)
        {
            if (tabCensusEnquiry.SelectedIndex == BLOODLESS)
            {
                this.censusOfBloodlessPatientsView.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == RELIGIONS)
            {
                this.censusbyReligionView.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PATIENT)
            {
                this.censusbyPatientView.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == NURSINGSTATION)
            {
                this.censusbyNursingStationView.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == ADT)
            {
                this.censusbyADT.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PHYSICIAN)
            {
                this.censusbyPhysicianView.SetRowSelectionInActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PAYOR)
            {
                this.censusbyInsurance.SetRowSelectionInActiveAppearance();
            }

        }





        private void CensusView_Activated(object sender, EventArgs e)
        {
            if (tabCensusEnquiry.SelectedIndex == BLOODLESS)
            {
                this.censusOfBloodlessPatientsView.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == RELIGIONS)
            {
                this.censusbyReligionView.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PATIENT)
            {
                this.censusbyPatientView.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == NURSINGSTATION)
            {
                this.censusbyNursingStationView.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == ADT)
            {
                this.censusbyADT.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PHYSICIAN)
            {
                this.censusbyPhysicianView.SetRowSelectionActiveAppearance();
            }
            if (tabCensusEnquiry.SelectedIndex == PAYOR)
            {
                this.censusbyInsurance.SetRowSelectionActiveAppearance();
            }
        }

        private void FocusToBtnCloseHandler(object sender, EventArgs e)
        {
            //            this.censusbyPhysicianView.AcceptButton = null;
            if (!this.button_Close.ContainsFocus)
            {
                this.button_Close.Focus();
            }
        }
        
       #endregion

        # region Methods

        private void ParentAcceptButtonChanged(object sender, EventArgs e)
        {
            this.censusbyADT.AcceptButton = this.button_Close;
            this.censusbyInsurance.AcceptButton = this.button_Close;
            this.censusbyReligionView.AcceptButton = this.button_Close;
            this.censusbyPhysicianView.AcceptButton = this.button_Close;
        }
        # endregion

        # region Properties

        public int SelectedMenu
        {
            private get
            {
                return selected_Menu;
            }
            set
            {
                selected_Menu = value;

            }
        }


        # endregion

        # region Private Methods

        private void AuthorizeUser()
        {
            CheckUserHasPermissionToView(tabPatient, new CensusByPatientActivity().Description);
            CheckUserHasPermissionToView(tabNursingStations, new CensusByNursingStationActivity().Description);
            CheckUserHasPermissionToView(tabADT, new CensusByADTActivity().Description);
            CheckUserHasPermissionToView(tabPhysician, new CensusByPhysicianActivity().Description);
            CheckUserHasPermissionToView(tabBloodless, new CensusByBloodlessActivity().Description);
            CheckUserHasPermissionToView(tabReligions, new CensusByReligionActivity().Description);
            CheckUserHasPermissionToView(tabPayor, new CensusByPayorActivity().Description);
        }

        private void CheckUserHasPermissionToView(TabPage tabPage, string contextItem)
        {
            tabPage.Enabled = true;
            var patientAccessUser = User.GetCurrent();
            var securityUser = patientAccessUser.SecurityUser;
            var securityFrameworkFacility = new Peradigm.Framework.Domain.Parties.Facility(patientAccessUser.Facility.Code, patientAccessUser.Facility.Description);

            bool hasPermissionToView = true;

            hasPermissionToView = securityUser.HasPermissionTo(Privilege.Actions.View, contextItem, securityFrameworkFacility);
            if (!(hasPermissionToView))
            {
                tabPage.Enabled = false;
            }
        }

        # endregion

        #region Private Properties
        private void WireupMiscEvents()
        {
            censusbyPhysicianView.FocusToBtnClose += new EventHandler(this.FocusToBtnCloseHandler);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PanelCensusView = new System.Windows.Forms.Panel();
            this.label_OptoutReligion = new System.Windows.Forms.Label();
            this.label_OptoutLocation = new System.Windows.Forms.Label();
            this.label_OptoutHealthInfo = new System.Windows.Forms.Label();
            this.label_OptoutNameAndAllInformation = new System.Windows.Forms.Label();
            this.label_ConfidentialPatient = new System.Windows.Forms.Label();
            this.button_Close = new LoggingButton();
            this.tabCensusEnquiry = new System.Windows.Forms.TabControl();
            this.tabPatient = new System.Windows.Forms.TabPage();
            this.censusbyPatientView = new PatientAccess.UI.CensusInquiries.CensusbyPatientView();
            this.tabNursingStations = new System.Windows.Forms.TabPage();
            this.censusbyNursingStationView = new PatientAccess.UI.CensusInquiries.CensusbyNursingStationView();
            this.tabADT = new System.Windows.Forms.TabPage();
            this.censusbyADT = new PatientAccess.UI.CensusInquiries.CensusbyADT();
            this.tabPhysician = new System.Windows.Forms.TabPage();
            this.censusbyPhysicianView = new PatientAccess.UI.CensusInquiries.CensusbyPhysicianView();
            this.tabBloodless = new System.Windows.Forms.TabPage();
            this.censusOfBloodlessPatientsView = new PatientAccess.UI.CensusInquiries.CensusOfBloodlessPatientsView();
            this.tabReligions = new System.Windows.Forms.TabPage();
            this.censusbyReligionView = new PatientAccess.UI.CensusInquiries.CensusbyReligionView();
            this.tabPayor = new System.Windows.Forms.TabPage();
            this.censusbyInsurance = new PatientAccess.UI.CensusInquiries.CensusbyInsurancePlan();
            this.tabADT = new System.Windows.Forms.TabPage();
            this.censusbyADT = new PatientAccess.UI.CensusInquiries.CensusbyADT();
            this.PanelCensusView.SuspendLayout();
            this.tabCensusEnquiry.SuspendLayout();
            this.tabPatient.SuspendLayout();
            this.tabNursingStations.SuspendLayout();
            this.tabPhysician.SuspendLayout();
            this.tabBloodless.SuspendLayout();
            this.tabReligions.SuspendLayout();
            this.tabPayor.SuspendLayout();
            this.tabADT.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelCensusView
            // 
            this.PanelCensusView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.PanelCensusView.Controls.Add(this.label_OptoutReligion);
            this.PanelCensusView.Controls.Add(this.label_OptoutLocation);
            this.PanelCensusView.Controls.Add(this.label_OptoutHealthInfo);
            this.PanelCensusView.Controls.Add(this.label_OptoutNameAndAllInformation);
            this.PanelCensusView.Controls.Add(this.label_ConfidentialPatient);
            this.PanelCensusView.Controls.Add(this.button_Close);
            this.PanelCensusView.Controls.Add(this.tabCensusEnquiry);
            this.PanelCensusView.Location = new System.Drawing.Point(0, 0);
            this.PanelCensusView.Name = "PanelCensusView";
            this.PanelCensusView.Size = new System.Drawing.Size(940, 585);
            this.PanelCensusView.TabIndex = 0;
            // 
            // label_OptoutReligion
            // 
            this.label_OptoutReligion.Location = new System.Drawing.Point(598, 520);
            this.label_OptoutReligion.Name = "label_OptoutReligion";
            this.label_OptoutReligion.Size = new System.Drawing.Size(120, 16);
            this.label_OptoutReligion.TabIndex = 6;
            this.label_OptoutReligion.Text = "[r] Opt-out religion";
            // 
            // label_OptoutLocation
            // 
            this.label_OptoutLocation.Location = new System.Drawing.Point(464, 520);
            this.label_OptoutLocation.Name = "label_OptoutLocation";
            this.label_OptoutLocation.Size = new System.Drawing.Size(120, 16);
            this.label_OptoutLocation.TabIndex = 5;
            this.label_OptoutLocation.Text = "[l] Opt-out location";
            // 
            // label_OptoutHealthInfo
            // 
            this.label_OptoutHealthInfo.Location = new System.Drawing.Point(325, 520);
            this.label_OptoutHealthInfo.Name = "label_OptoutHealthInfo";
            this.label_OptoutHealthInfo.Size = new System.Drawing.Size(124, 16);
            this.label_OptoutHealthInfo.TabIndex = 4;
            this.label_OptoutHealthInfo.Text = "[h] Opt-out health info";
            // 
            // label_OptoutNameAndAllInformation
            // 
            this.label_OptoutNameAndAllInformation.Location = new System.Drawing.Point(160, 520);
            this.label_OptoutNameAndAllInformation.Name = "label_OptoutNameAndAllInformation";
            this.label_OptoutNameAndAllInformation.Size = new System.Drawing.Size(155, 16);
            this.label_OptoutNameAndAllInformation.TabIndex = 3;
            this.label_OptoutNameAndAllInformation.Text = "[n] Opt-out name and all info";
            // 
            // label_ConfidentialPatient
            // 
            this.label_ConfidentialPatient.Location = new System.Drawing.Point(24, 520);
            this.label_ConfidentialPatient.Name = "label_ConfidentialPatient";
            this.label_ConfidentialPatient.Size = new System.Drawing.Size(112, 16);
            this.label_ConfidentialPatient.TabIndex = 2;
            this.label_ConfidentialPatient.Text = "* Confidential patient";
            // 
            // button_Close
            // 
            this.button_Close.Location = new System.Drawing.Point(840, 518);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 21);
            this.button_Close.TabIndex = 1;
            this.button_Close.Text = "&Close";
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // tabCensusEnquiry
            // 
            this.tabCensusEnquiry.Controls.Add(this.tabPatient);
            this.tabCensusEnquiry.Controls.Add(this.tabNursingStations);
            this.tabCensusEnquiry.Controls.Add(this.tabADT);
            this.tabCensusEnquiry.Controls.Add(this.tabPhysician);
            this.tabCensusEnquiry.Controls.Add(this.tabBloodless);
            this.tabCensusEnquiry.Controls.Add(this.tabReligions);
            this.tabCensusEnquiry.Controls.Add(this.tabPayor);
            this.tabCensusEnquiry.Location = new System.Drawing.Point(7, 15);
            this.tabCensusEnquiry.Name = "tabCensusEnquiry";
            this.tabCensusEnquiry.SelectedIndex = 0;
            this.tabCensusEnquiry.Size = new System.Drawing.Size(926, 490);
            this.tabCensusEnquiry.TabIndex = 0;
            this.tabCensusEnquiry.SelectedIndexChanged +=
                new EventHandler(tabCensusEnquiry_SelectedIndexChanged);
            
            
            // 
            // tabPatient
            // 
            this.tabPatient.Controls.Add(this.censusbyPatientView);
            this.tabPatient.Location = new System.Drawing.Point(4, 22);
            this.tabPatient.Name = "tabPatient";
            this.tabPatient.Size = new System.Drawing.Size(926, 464);
            this.tabPatient.TabIndex = 0;
            this.tabPatient.Text = "Patient";
            // 
            // censusbyPatientView
            // 
            this.censusbyPatientView.BackColor = System.Drawing.Color.White;
            this.censusbyPatientView.Location = new System.Drawing.Point(0, 0);
            this.censusbyPatientView.Model = null;
            this.censusbyPatientView.Name = "censusbyPatientView";
            this.censusbyPatientView.Size = new System.Drawing.Size(926, 464);
            this.censusbyPatientView.TabIndex = 0;
            this.censusbyPatientView.AcceptButton = this.button_Close;
            // 
            // tabNursingStations
            // 
            this.tabNursingStations.Controls.Add(this.censusbyNursingStationView);
            this.tabNursingStations.Location = new System.Drawing.Point(4, 22);
            this.tabNursingStations.Name = "tabNursingStations";
            this.tabNursingStations.Size = new System.Drawing.Size(926, 464);
            this.tabNursingStations.TabIndex = 1;
            this.tabNursingStations.Text = "Nursing Station";

            this.censusbyNursingStationView.BackColor = System.Drawing.Color.White;
            this.censusbyNursingStationView.Location = new System.Drawing.Point(0, 0);
            this.censusbyNursingStationView.Model = null;
            this.censusbyNursingStationView.Name = "censusbyPatientView";
            this.censusbyNursingStationView.Size = new System.Drawing.Size(926, 464);
            this.censusbyNursingStationView.TabIndex = 0;
            this.censusbyNursingStationView.AcceptButton = this.button_Close;
            // 
            // tabADT
            // 
            this.tabADT.Controls.Add(this.censusbyADT);
            this.tabADT.Location = new System.Drawing.Point(4, 22);
            this.tabADT.Name = "tabADT";
            this.tabADT.Size = new System.Drawing.Size(926, 464);
            this.tabADT.TabIndex = 2;
            this.tabADT.Text = "A-D-T";
            // 
            // censusbyADT
            // 
            this.censusbyADT.Location = new System.Drawing.Point(0, 0);
            this.censusbyADT.Model = null;
            this.censusbyADT.Name = "censusbyADT";
            this.censusbyADT.Size = new System.Drawing.Size(926, 464);
            this.censusbyADT.TabIndex = 0;
            this.censusbyADT.AcceptButton = this.button_Close;
            this.censusbyADT.ParentAcceptButtonChanged +=
                new System.EventHandler(this.ParentAcceptButtonChanged);
            // 
            // tabPhysician
            // 
            this.tabPhysician.Controls.Add(this.censusbyPhysicianView);
            this.tabPhysician.Location = new System.Drawing.Point(4, 22);
            this.tabPhysician.Name = "tabPhysician";
            this.tabPhysician.Size = new System.Drawing.Size(926, 464);
            this.tabPhysician.TabIndex = 3;
            this.tabPhysician.Text = "Physician";
            // 
            // censusbyPhysicianView
            // 
            this.censusbyPhysicianView.BackColor = System.Drawing.Color.White;
            this.censusbyPhysicianView.Location = new System.Drawing.Point(0, 0);
            this.censusbyPhysicianView.Model = null;
            this.censusbyPhysicianView.Name = "censusbyPhysicianView";
            this.censusbyPhysicianView.Size = new System.Drawing.Size(920, 464);
            this.censusbyPhysicianView.TabIndex = 0;
            this.censusbyPhysicianView.AcceptButton = this.button_Close;
            this.censusbyPhysicianView.ParentAcceptButtonChanged +=
                new System.EventHandler(this.ParentAcceptButtonChanged);
            // 
            // tabBloodless
            // 
            this.tabBloodless.Controls.Add(this.censusOfBloodlessPatientsView);
            this.tabBloodless.Location = new System.Drawing.Point(4, 22);
            this.tabBloodless.Name = "tabBloodless";
            this.tabBloodless.Size = new System.Drawing.Size(926, 464);
            this.tabBloodless.TabIndex = 4;
            this.tabBloodless.Text = "Bloodless";
            // 
            // censusOfBloodlessPatientsView
            // 
            this.censusOfBloodlessPatientsView.BackColor = System.Drawing.Color.White;
            this.censusOfBloodlessPatientsView.Location = new System.Drawing.Point(0, 0);
            this.censusOfBloodlessPatientsView.Model = null;
            this.censusOfBloodlessPatientsView.Name = "censusOfBloodlessPatientsView";
            this.censusOfBloodlessPatientsView.Size = new System.Drawing.Size(920, 468);
            this.censusOfBloodlessPatientsView.TabIndex = 0;
            this.censusOfBloodlessPatientsView.AcceptButton = this.button_Close;
            // 
            // tabReligions
            // 
            this.tabReligions.Controls.Add(this.censusbyReligionView);
            this.tabReligions.Location = new System.Drawing.Point(4, 22);
            this.tabReligions.Name = "tabReligions";
            this.tabReligions.Size = new System.Drawing.Size(926, 464);
            this.tabReligions.TabIndex = 5;
            this.tabReligions.Text = "Religion";
            // 
            // censusbyReligionView
            // 
            this.censusbyReligionView.Location = new System.Drawing.Point(0, 0);
            this.censusbyReligionView.Model = null;
            this.censusbyReligionView.Name = "censusbyReligionView";
            this.censusbyReligionView.Size = new System.Drawing.Size(920, 464);
            this.censusbyReligionView.TabIndex = 0;
            this.censusbyReligionView.AcceptButton = this.button_Close;
            this.censusbyReligionView.ParentAcceptButtonChanged +=
                new System.EventHandler(this.ParentAcceptButtonChanged);

            // 
            // tabReligions
            // 
            this.tabPayor.Controls.Add(this.censusbyInsurance);
            this.tabPayor.Location = new System.Drawing.Point(4, 22);
            this.tabPayor.Name = "tabPayor";
            this.tabPayor.Size = new System.Drawing.Size(926, 464);
            this.tabPayor.TabIndex = 6;
            this.tabPayor.Text = "Payor";
            // 
            // censusbyReligionView
            // 
            this.censusbyInsurance.Location = new System.Drawing.Point(0, 0);
            this.censusbyInsurance.Model = null;
            this.censusbyInsurance.Name = "censusbyInsurance";
            this.censusbyInsurance.Size = new System.Drawing.Size(920, 464);
            this.censusbyInsurance.TabIndex = 0;
            this.censusbyInsurance.AcceptButton = this.button_Close;
            this.censusbyInsurance.ParentAcceptButtonChanged +=
                new System.EventHandler(this.ParentAcceptButtonChanged);

            // 
            // CensusView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(940, 553);
            this.Controls.Add(this.PanelCensusView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CensusView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Census";
            this.Load += new System.EventHandler(this.CensusView_Load);
            this.Activated += new System.EventHandler(this.CensusView_Activated);
            this.Deactivate += new System.EventHandler(this.CensusView_Deactivate);
            this.Closing += new CancelEventHandler(CensusView_Closing);

            this.PanelCensusView.ResumeLayout(false);
            this.tabCensusEnquiry.ResumeLayout(false);
            this.tabPatient.ResumeLayout(false);
            this.tabNursingStations.ResumeLayout(false);
            this.tabPhysician.ResumeLayout(false);
            this.tabBloodless.ResumeLayout(false);
            this.tabReligions.ResumeLayout(false);
            this.tabADT.ResumeLayout(false);
            this.tabPayor.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        # region Construction And Finalization

        public CensusView()
        {
            InitializeComponent();

            base.EnableThemesOn(this);

            this.WireupMiscEvents();

            this.AuthorizeUser();

            // Set the form icon here, otherwise if added to InitializeComponents 
            // the code will get replaced by the IDE if opened in the designer.
            // Adding the icon in the designer drastically changed the code.
            ResourceManager resources = new ResourceManager(typeof(CensusView));
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        # endregion

        # region Date Elements

        private TabPage tabPatient;
        private TabPage tabPhysician;
        private TabPage tabBloodless;
        private TabControl tabCensusEnquiry;
        private TabPage tabNursingStations;
        private TabPage tabReligions;
        private TabPage tabPayor;
        private TabPage tabADT;
        private Panel PanelCensusView;
        private LoggingButton button_Close;
        private Label label_ConfidentialPatient;
        private Label label_OptoutNameAndAllInformation;
        private Label label_OptoutHealthInfo;
        private Label label_OptoutLocation;
        private Label label_OptoutReligion;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private int selected_Menu;
        private CensusbyPatientView censusbyPatientView;
        private CensusbyNursingStationView censusbyNursingStationView;
        private CensusbyPhysicianView censusbyPhysicianView;
        private CensusOfBloodlessPatientsView censusOfBloodlessPatientsView;
        private CensusbyReligionView censusbyReligionView;
        private CensusbyADT censusbyADT;
        private CensusbyInsurancePlan censusbyInsurance;


        # endregion

        # region Constants
        public const int
            PATIENT = 0,
            NURSINGSTATION = 1,
            ADT = 2,
            PHYSICIAN = 3,
            BLOODLESS = 4,
            RELIGIONS = 5,
            PAYOR = 6;
        # endregion


    }
}