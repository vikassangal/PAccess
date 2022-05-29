using System;
using System.Data;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.InterFacilityTransfer;
using PatientAccess.BrokerInterfaces;
using System.Collections;

namespace PatientAccess.UI.InterfacilityTransfer
{
    public partial class InterfacilityPannel : UserControl
    {
        #region Event Handlers
        #endregion
        #region Methods
        public void UpdateView()
        {            
            if (TranferToAccount.FromAccountNumber==0)
            {
                VisibilityPanel(false);
                VisibilityLable(false);
                VisibilityDropdown(false);
                //SetDropdownValue();
            }
            else
            {
                VisibilityPanel(true);
                VisibilityLable(true);
                VisibilityDropdown(false);
                SetLabelValue(TranferToAccount);
            }
        }
        public void VisibilityPanel(bool flag)
        {
            pnl_ift.Visible = flag;           
        }

        public void VisibilityLable(bool flag)
        {
            lbl_ift_account_view.Visible = flag;
            lbl_ift_hospital_view.Visible = flag;
        }

        public void VisibilityDropdown(bool flag)
        {
            cmb_ift_account.Visible = flag;
            cmb_ift_hospital.Visible = flag;
            lblNote.Visible = true;
        }
        public void Reset()
        {
            lbl_ift_account_view.Text = string.Empty;
            lbl_ift_hospital_view.Text = string.Empty;
            cmb_ift_hospital.SelectedIndex = 0;
            cmb_ift_account.SelectedIndex = 0;
        }

        public void SetLabelValue(InterFacilityTransferAccount interFacilityTransferAccount)
        {
            if (interFacilityTransferAccount.FromAccountNumber != 0 || interFacilityTransferAccount.ToAccountNumber != 0)
            {
                i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                
                if (interFacilityTransferAccount.Activity.IsQuickAccountCreationActivity())
                {
                    Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.FromFacilityOid);
                    lbl_ift_account_view.Text = interFacilityTransferAccount.FromAccountNumber.ToString();
                    lbl_ift_hospital_view.Text = facility.DisplayString;
                }
                else if (interFacilityTransferAccount.Activity.IsDischargeActivity())
                {
                    Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.ToFacilityOid);
                    lbl_ift_account_view.Text = interFacilityTransferAccount.ToAccountNumber.ToString();
                    lbl_ift_hospital_view.Text = facility.DisplayString;
                }
                else if(interFacilityTransferAccount.Activity.IsActivatePreAdmitNewbornActivity())
                {
                    
                }
                
            }
        }
        

        public void SetFacilityDropdownValue()
        {
            DataTable dtHospital = new DataTable();
            cmb_ift_hospital.Items.Insert(0, "Select");
            dtHospital = FillFromHospital(User.GetCurrent().Facility);
            cmb_ift_hospital.DataSource = dtHospital;
            cmb_ift_hospital.DisplayMember = "HSPCode";
            cmb_ift_hospital.ValueMember = "HSPCode";            
        }

        public void SetAccountDropdownValue()
        {
            DataTable dtHospital = new DataTable();
            //cmb_ift_hospital.Items.Insert(0, "Select");
            //dtHospital = FillFromHospital(User.GetCurrent().Facility);
            //cmb_ift_hospital.DataSource = dtHospital;
            //cmb_ift_hospital.DisplayMember = "HSPCode";
            //cmb_ift_hospital.ValueMember = "HSPCode";
            //

        }

        public DataTable FillFromHospital(Facility facility)
        {
            DataTable dtfromHospital = new DataTable();
            
            dtfromHospital.Columns.Add("HSPCode");
            dtfromHospital.Columns.Add("HospitalName");
            IInterfacilityTransferBroker interfacilityTransferBroker= BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
            ArrayList arrIFTRFacility = interfacilityTransferBroker.AllInterFacilityTransferHospitals(facility);
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility1;
            DataRow dr;
            dr = dtfromHospital.NewRow();
            dr[0] = "";
            dtfromHospital.Rows.Add(dr);
            if (arrIFTRFacility.Count > 0)
            {
                foreach (string list in arrIFTRFacility)
                {
                    if (list.ToString() != facility.Code)
                    {
                        facility1 = i_FacilityBroker.FacilityWith(list);
                        dr = dtfromHospital.NewRow();
                        dr[0] = list;
                        dr[1] = facility1.Description.ToString();
                        dtfromHospital.Rows.Add(dr);
                    }
                }
            }
            dtfromHospital.AcceptChanges();
            return dtfromHospital;
        }

        public void SetValueForConfirmationScreen(InterFacilityTransferAccount interFacilityTransferAccount)
        {
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            lblNote.Visible = false;
            cmb_ift_account.Visible = false;
            cmb_ift_hospital.Visible = false;
            lbl_ift_hospital_view.Visible = true;
            lbl_ift_account_view.Visible=true;
            if (interFacilityTransferAccount.Activity.IsQuickAccountCreationActivity() 
                || interFacilityTransferAccount.Activity.IsRegistrationActivity())
            {
                Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.FromFacilityOid);
                lbl_ift_account_view.Text = interFacilityTransferAccount.FromAccountNumber.ToString();
                lbl_ift_hospital_view.Text = facility.DisplayString;
            }
            else if (interFacilityTransferAccount.Activity.ContextDescription == "Discharge Patient")
            {
                Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.ToFacilityOid);
                lbl_ift_account_view.Text = interFacilityTransferAccount.ToAccountNumber.ToString();
                lbl_ift_hospital_view.Text = facility.DisplayString;
            }

            //lbl_HSVView.Text = interFacilityTransferAccount.HSV;
            //lbl_PTView.Text = interFacilityTransferAccount.PatientType;
            //lbl_physicianView.Text = interFacilityTransferAccount.ERPhysician;
        }
        public void SetValueForConfirmationScreen()
        {                       
            lblNote.Visible = false;
            cmb_ift_account.Visible = false;
            cmb_ift_hospital.Visible = false;
        }
        #endregion
        #region Properties       

        public string  SetAccountLabel
        {
            get
            {
                return lbl_ift_account.Text;
            }
            set
            {
                lbl_ift_account.Text = value;
            }
        }
        public string SetHospitalLabel
        {
            set
            {
                lbl_ift_hospital.Text = value;
            }
        }

        public string SetHSV
        {
            set
            {
                lbl_HSVView.Text = value;
            }
        }
                

        public string SetPT
        {
            set
            {
                lbl_PTView.Text = value;
            }
        }

        public InterFacilityTransferAccount TranferToAccount
        {
            get;
            set;
        }
        #endregion
        #region Private Methods

        public void CheckInTransferTabel(string selectedfacility)
        {
            //long ToAccount = 0;
            var interfacilityTransferBroker = BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
            //long HSPCode = Convert.ToInt32(selectedfacility);
            //ArrayList patient = interfacilityTransferBroker.GetAccountsForPatient(TranferToAccount, "ER");
            //foreach (object objAccount in patient)
            //{
            //    ToAccount = (long)objAccount;
            //}
            //var dtaccounts = interfacilityTransferBroker.Accountsfromtransferlogfordischarge(TranferToAccount, HSPCode, ToAccount);
            //return interfacilityTransferBroker.;
        }

        public void CheckInTransferTabel()
        {
            //long ToAccount = 0;
            var interfacilityTransferBroker = BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
            //long HSPCode = TranferToAccount.Facility.Oid;
            //ArrayList patient = interfacilityTransferBroker.GetAccountsForPatient(TranferToAccount, "ER");
            //foreach (object objAccount in patient)
            //{
            //    ToAccount = (long)objAccount;
            //}
            //InterFacilityTransferAccount dtaccounts = interfacilityTransferBroker.Accountsfromtransferlogfordischarge(TranferToAccount, HSPCode, ToAccount);
            //return interfacilityTransferBroker;
        }

        #endregion
        #region Construction and Finalization
        public InterfacilityPannel()
        {
            InitializeComponent();
            TranferToAccount=new InterFacilityTransferAccount();
        }

        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        //private InterfacilityTransferPresenter InterfacilityTransferPresenter = new InterfacilityTransferPresenter();
        IFacilityBroker i_FacilityBroker ;
        #endregion

        #region Constants
        #endregion

        private void cmb_ift_hospital_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_ift_hospital.SelectedValue.ToString() != "")
            {
                CheckInTransferTabel(cmb_ift_hospital.SelectedValue.ToString());
            }
        }
       
    }


}
