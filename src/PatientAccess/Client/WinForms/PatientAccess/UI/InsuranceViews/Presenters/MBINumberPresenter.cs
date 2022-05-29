using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses; 
using PatientAccess.UI.InsuranceViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.InsuranceViews.Presenters
{
    public class MBINumberPresenter
    {
        private readonly IMBINumberView MBINumberView;
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public MBINumberPresenter(IMBINumberView MBIView, IMessageBoxAdapter messageBoxAdapter, Coverage coverage,
            RuleEngine ruleEngine)
        {

            Guard.ThrowIfArgumentIsNull( MBIView,  "MBINumberView");
            Guard.ThrowIfArgumentIsNull(coverage, "Coverage");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");
            MBINumberView = MBIView;
            Coverage = coverage;
            MessageBoxAdapter = messageBoxAdapter;
            RuleEngine = ruleEngine;
        }
       
        private static void ShowInvalidMBIMessage()
        {
            MessageBox.Show(UIErrorMessages.MBI_FORMAT_ERRMSG, "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
        }
       
        private RuleEngine RuleEngine { get; set; }
        private Coverage Coverage { get; set; }
        
        public void UpdateView()
        {
            var mbi = String.Empty;
            if (Coverage != null)
            {
                if (Coverage.GetType() == typeof (GovernmentMedicareCoverage))
                {
                    mbi = ((GovernmentMedicareCoverage) Coverage).MBINumber;
                }

                else if (Coverage.GetType() == typeof (CommercialCoverage) ||
                         Coverage.GetType() == typeof (OtherCoverage) ||
                         Coverage.GetType() == typeof (GovernmentOtherCoverage))
                {

                    mbi = ((CoverageForCommercialOther) Coverage).MBINumber;
                }

                {
                    MBINumberView.MBINumber = mbi;
                }
            }
        }
        public void SetMBINumberStateForFinancialClass()
        {
            Account anAccount = MBINumberView.Account;
            if (anAccount != null
                && anAccount.FinancialClass != null
                && anAccount.FinancialClass.IsSignedOverMedicare())
            {
                MBINumberView.EnbleMBINumber();
            }
            else
            {
                MBINumberView.DisableMBINumber();
            }
        }
        public void UpdateModel()
        {
            if (Coverage == null) return;
            if (Coverage.GetType() == typeof (GovernmentMedicareCoverage))
            {
                ((GovernmentMedicareCoverage) Coverage).MBINumber = MBINumberView.MBINumber;
            }

            else if (Coverage.GetType() == typeof (CommercialCoverage) ||
                     Coverage.GetType() == typeof (OtherCoverage) ||
                     Coverage.GetType() == typeof (GovernmentOtherCoverage))
            {

                ((CoverageForCommercialOther) Coverage).MBINumber = MBINumberView.MBINumber;
            }
        }

        public void ValidateMBINumber()
        {
            if (ValidateMBINUmber())
            {
                UpdateModel();
                CheckForRequiredFields();
            }
            else
            {

                MBINumberView.SetMBINumberError();
                ShowInvalidMBIMessage();
                MBINumberView.setFocusToMBINumber();
            }
        }

        private void CheckForRequiredFields()
        {
            MBINumberView.SetMBINumberNormalColor();
            MBINumberView.SetHICNumberNormalColor();
            RuleEngine.GetInstance().EvaluateRule(typeof(MBINumberRequired), Coverage);
            RuleEngine.GetInstance().EvaluateRule(typeof(MBINumberPreferred), Coverage );
        }

        public bool ValidateMBINUmber()
        {
            var mbiNumberEntered = MBINumberView.MBINumber ;
            if (String.IsNullOrEmpty(mbiNumberEntered))
            {
                return true;
            }
            if (mbiNumberEntered.Length != 11)
            {
                return false;
            }
            else
            {
                char[] mbiCharacters = mbiNumberEntered.ToCharArray();
                if (
                    !Regex.IsMatch(mbiCharacters[0].ToString(), REGEX_1To9) ||
                    !Regex.IsMatch(mbiCharacters[1].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[2].ToString(), REGEX_0To9AToZ) ||
                    !Regex.IsMatch(mbiCharacters[3].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[4].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[5].ToString(), REGEX_0To9AToZ) ||
                    !Regex.IsMatch(mbiCharacters[6].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[7].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[8].ToString(), REGEX_AToZ) ||
                    !Regex.IsMatch(mbiCharacters[9].ToString(), REGEX_0To9) ||
                    !Regex.IsMatch(mbiCharacters[10].ToString(), REGEX_0To9)
                    )
                {
                    return false;
                }
            }
            return true;
        }

        private const string REGEX_0To9 = "[0-9]";
        private const string REGEX_1To9 = "[1-9]";
        private const string REGEX_AToZ = "[AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
        private const string REGEX_0To9AToZ = "[0-9AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
    }
}