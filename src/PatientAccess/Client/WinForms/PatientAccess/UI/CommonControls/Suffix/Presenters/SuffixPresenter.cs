using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls.Suffix.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CommonControls.Suffix.Presenters
{
    public class SuffixPresenter
    {
        private readonly ISuffixView SuffixView;
        private readonly Account Account;
        private readonly string BLANK_SUFFIX = String.Empty;

        public SuffixPresenter(ISuffixView suffixView, Account account , String context )
        {
            Guard.ThrowIfArgumentIsNull(suffixView, "SuffixView");
            Guard.ThrowIfArgumentIsNull(account, "Account"); 
            SuffixView = suffixView;
            SuffixView.SuffixPresenter = this;
            Account = account;
            SuffixContext = context;
        }

        public string SuffixContext { get; set; }

        public void UpdateView()
        {
            PopulateSuffixValues();
            string suffixForContext = String.Empty;
            switch (SuffixContext)
            {
                case "Patient":
                    suffixForContext = Account.Patient.Name.Suffix;
                    break;
                case "Guarantor":
                    suffixForContext = Account.Guarantor.Name.Suffix;
                    break;
                case "PrimaryInsured":
                    suffixForContext = Account.PrimaryInsured.Name.Suffix;
                    break;
                case "SecondaryInsured":
                    suffixForContext = Account.SecondaryInsured.Name.Suffix;
                    break;
            }
            SuffixView.UpdateSuffix(suffixForContext);
        }
       
        private void PopulateSuffixValues()
        {
            var suffixBroker = BrokerFactory.BrokerOfType<ISuffixBroker>();
            ICollection<string> suffixCollection = suffixBroker.AllSuffixCodes(User.GetCurrent().Facility.Oid);
         
            SuffixView.ClearItems();
            if (suffixCollection == null)
            {
                MessageBox.Show("No suffixCodes were found!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (string suffix in suffixCollection)
            {
                SuffixView.AddSuffix(suffix);
            }
        }


        public void UpdateSuffix(string suffix)
        {
            switch (SuffixContext)
            {
                case "Patient":
                    Account.Patient.Name.Suffix = suffix;
                    break;
                case "Guarantor":
                    Account.Guarantor.Name.Suffix = suffix;
                    break;
                case "PrimaryInsured":
                    Account.PrimaryInsured.Name.Suffix = suffix;
                    break;
                case "SecondaryInsured":
                    Account.SecondaryInsured.Name.Suffix = suffix;
                    break;
            }

        }

        public void ClearSuffix()
        {
            UpdateSuffix(BLANK_SUFFIX);
            SuffixView.ClearSuffix();
        }
    }
}