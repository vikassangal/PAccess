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
    public class MBIPresenter
    {
        private readonly IMBIView iMBIView;
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public MBIPresenter(IMBIView MBIView, IMessageBoxAdapter messageBoxAdapter, Coverage coverage,
            RuleEngine ruleEngine)
        {

            Guard.ThrowIfArgumentIsNull(MBIView, "MBINumberView");
            Guard.ThrowIfArgumentIsNull(coverage, "Coverage");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");
            iMBIView = MBIView;
            Coverage = coverage;
            MessageBoxAdapter = messageBoxAdapter;
            RuleEngine = ruleEngine;
        }

        private static void ShowInvalidMBIMessage()
        {
            MessageBox.Show(UIErrorMessages.MBI_FORMAT_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
        }
        private static void ShowTestMBIMessage()
        {
            MessageBox.Show(UIErrorMessages.MBI_TESTNUMBER_MESSAGE, "Warning",
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
                if (Coverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    mbi = ((GovernmentMedicareCoverage)Coverage).MBINumber;
                }

                else if (Coverage.GetType() == typeof(CommercialCoverage) ||
                         Coverage.GetType() == typeof(OtherCoverage) ||
                         Coverage.GetType() == typeof(GovernmentOtherCoverage))
                {

                    mbi = ((CoverageForCommercialOther)Coverage).MBINumber;
                }

                {
                    iMBIView.MBINumber = mbi;
                }
            }
        }
        public void SetMBINumberStateForFinancialClass()
        {
            Account anAccount = iMBIView.Account;
            if (anAccount != null
                && anAccount.FinancialClass != null
                && anAccount.FinancialClass.IsSignedOverMedicare())
            {
                iMBIView.EnbleMBINumber();
            }
            else
            {
                iMBIView.DisableMBINumber();
            }
        }
        public void UpdateModel()
        {
            if (Coverage == null) return;
            if (Coverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                ((GovernmentMedicareCoverage)Coverage).MBINumber = iMBIView.MBINumber;
            }

            else if (Coverage.GetType() == typeof(CommercialCoverage) ||
                     Coverage.GetType() == typeof(OtherCoverage) ||
                     Coverage.GetType() == typeof(GovernmentOtherCoverage))
            {

                ((CoverageForCommercialOther)Coverage).MBINumber = iMBIView.MBINumber;
            }
        }

        public bool ValidateMBINumber()
        {
            if (ValidateMBI())
            {
                UpdateModel();
                CheckForRequiredFields();
                return true;
            }
            else
            {
                if (TestMBI)
                {
                    HandleTestMBINumber();
                }
                else
                {

                    iMBIView.SetMBINumberError();
                    ShowInvalidMBIMessage();
                    iMBIView.setFocusToMBINumber();
                }

                return TestMBI = false;
            }
        }

        private void CheckForRequiredFields()
        {
            iMBIView.CheckForRequiredFields();

        }

        private static bool IsTestMBINumber(String mbiNumber)
        {
            return Coverage.TestMBINumbers.Contains(mbiNumber);
        }

        private void HandleTestMBINumber()
        {
            iMBIView.ResetMBINumber();
            ValidateMBINumber();
            iMBIView.setFocusToMBINumber();
            ShowTestMBIMessage();
        }

        public bool ValidateMBI()
        {
            var mbiNumberEntered = iMBIView.MBINumber;
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

            TestMBI = IsTestMBINumber(mbiNumberEntered);
            if (TestMBI)
            {
                return false;
            }
            return true;
        }

        private bool TestMBI;
        private const string REGEX_0To9 = "[0-9]";
        private const string REGEX_1To9 = "[1-9]";
        private const string REGEX_AToZ = "[AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
        private const string REGEX_0To9AToZ = "[0-9AC-HJKMNPQRT-Yac-hjkmnpqrt-y]";
       
    }
}