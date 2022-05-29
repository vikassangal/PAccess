
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.Utilities;
using System.Data;

namespace PatientAccess.UI.InterfacilityTransfer
{
    public class InterfacilityTransferPresenter
    {
        private readonly InterfacilityPannel i_InterfacilityPannel;
        private Account Account { get; set; }
        public InterfacilityTransferPresenter(InterfacilityPannel interfacilityPannel, Account account)
        {
            Guard.ThrowIfArgumentIsNull(interfacilityPannel, "interfacilityPannel");
            Guard.ThrowIfArgumentIsNull(account, "account");

            i_InterfacilityPannel = interfacilityPannel;
            Account = account;

        }

        public void UpdateView()
        {

        }

        public void ShowInterfacilityTrasfer()
        {
            ShareHIEDataFeatureManager shareHieDataFeatureManager = new ShareHIEDataFeatureManager();
            if (shareHieDataFeatureManager.IsShareHieDataEnabledforaccount(Account))
            {


            }
        }

        public void ShowTrasferlogDetail()
        {

        }

        public void VisibilityDropdown()
        {
            //interfacilityPannel.con
            //cmb_ift_account.Visible = flag;
            //cmb_ift_hospital.Visible = flag;
        }

        private DataTable PopulateFromFacility()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HospitalName");
            dt.Columns.Add("HSPCode");

            DataRow dataRow = dt.NewRow();
            dataRow[0] = "NORTH CENTRAL BAPTIST MDL";
            dataRow[1] = "TAA";
            dt.Rows.Add(dataRow);
            dataRow = dt.NewRow();
            dataRow[0] = "NORTHEAST BAPTIST MODEL";
            dataRow[1] = "TAB";
            dt.Rows.Add(dataRow);
            dataRow = dt.NewRow();
            dataRow[0] = "SAINT LUKE MODEL";
            dataRow[1] = "TAC";
            dt.Rows.Add(dataRow);
            dataRow = dt.NewRow();
            dataRow[0] = "BAPTIST MC  MODEL";
            dataRow[1] = "TAD";
            dt.Rows.Add(dataRow);
            dataRow = dt.NewRow();
            dataRow[0] = "MT BAPTIST MODEL";
            dataRow[1] = "TAE";
            dt.Rows.Add(dataRow);
            dt.AcceptChanges();

            return dt;
        }

        private DataTable PopulateFromAccount()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AccountNumber");

            return dt;
        }

        public InterfacilityPannel interfacilityPannel { get; set; }
    }
}
