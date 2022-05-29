using System;
using System.Collections.Generic;
using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.CptCodes.Views;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.CptCodes.ViewImpl
{
    public partial class CptCodesDetailsView : TimeOutFormView, ICptCodesDetailsView
    {

        #region Properties
        #endregion

        #region Construction and Finalization

        public CptCodesDetailsView()
        {
            InitializeComponent();
            EnableThemesOn(this);
            ConfigureControls();
            SetupListOfCptTextBoxes();
        }

        #endregion

        #region Private Methods

        private MaskedEditTextBox GetCptTextBoxFor(CptFields cptField)
        {
            switch (cptField)
            {
                case CptFields.Code1:
                    return mtbCptCode1;

                case CptFields.Code2:
                    return mtbCptCode2;

                case CptFields.Code3:
                    return mtbCptCode3;

                case CptFields.Code4:
                    return mtbCptCode4;

                case CptFields.Code5:
                    return mtbCptCode5;

                case CptFields.Code6:
                    return mtbCptCode6;

                case CptFields.Code7:
                    return mtbCptCode7;

                case CptFields.Code8:
                    return mtbCptCode8;

                case CptFields.Code9:
                    return mtbCptCode9;

                case CptFields.Code10:
                    return mtbCptCode10;

                default:
                    throw new ArgumentOutOfRangeException("CPT code Masked edit text box");
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode1);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode2);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode3);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode4);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode5);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode6);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode7);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode8);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode9);
            MaskedEditTextBoxBuilder.ConfigureCptCode(mtbCptCode10);
        }

        private void SetupListOfCptTextBoxes()
        {
            cptTextBoxes[0] = mtbCptCode1;
            cptTextBoxes[1] = mtbCptCode2;
            cptTextBoxes[2] = mtbCptCode3;
            cptTextBoxes[3] = mtbCptCode4;
            cptTextBoxes[4] = mtbCptCode5;
            cptTextBoxes[5] = mtbCptCode6;
            cptTextBoxes[6] = mtbCptCode7;
            cptTextBoxes[7] = mtbCptCode8;
            cptTextBoxes[8] = mtbCptCode9;
            cptTextBoxes[9] = mtbCptCode10;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mtbCptCode1_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code1, textBox.Text);
            }
        }

        private void mtbCptCode2_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code2, textBox.Text);
            }
        }

        private void mtbCptCode3_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code3, textBox.Text);
            }
        }

        private void mtbCptCode4_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code4, textBox.Text);
            }
        }

        private void mtbCptCode5_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code5, textBox.Text);
            }
        }

        private void mtbCptCode6_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code6, textBox.Text);
            }
        }

        private void mtbCptCode7_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code7, textBox.Text);
            }
        }

        private void mtbCptCode8_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code8, textBox.Text);
            }
        }

        private void mtbCptCode9_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code9, textBox.Text);
            }
        }

        private void mtbCptCode10_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as MaskedEditTextBox;

            if (textBox != null)
            {
                Presenter.ValidateCptCode(CptFields.Code10, textBox.Text);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            cptCodes.Clear();

            for (var i = 0; i < CptCodesDetailsPresenter.MAX_NUMBER_OF_CPTCODES; i++)
            {
                var cptTxtBox = cptTextBoxes[i];

                if (cptTxtBox != null && cptTxtBox.Text != string.Empty)
                {
                    cptCodes.Add(i + 1, cptTxtBox.Text);
                }
            }

            Presenter.UpdateCptCodes();
        }

        private void CptCodesDetailsView_Shown(object sender, EventArgs e)
        {
            mtbCptCode1.Focus();
        }

        #endregion

        #region Public Methods

        public void SetNormalColor(CptFields cptField)
        {
            var mtbTextBox = GetCptTextBoxFor(cptField);
            UIColors.SetNormalBgColor(mtbTextBox);
        }

        public void SetErrorColor(CptFields cptField)
        {
            var mtbTextBox = GetCptTextBoxFor(cptField);
            UIColors.SetErrorBgColor(mtbTextBox);
        }

        public void SetFocus(CptFields cptField)
        {
            var mtbTextBox = GetCptTextBoxFor(cptField);
            mtbTextBox.Focus();
        }

        public Dictionary<int, string> CptCodes
        {
            get
            {
                return cptCodes;
            }
        }

        public void ShowAsDialog()
        {
            ShowDialog();
        }

        public void CloseView()
        {
            Close();
        }

        public void ClearCptCodes()
        {
            foreach (var cptTxtBox in cptTextBoxes)
            {
                if (cptTxtBox != null) cptTxtBox.Text = string.Empty;
            }
        }

        public void SetCptCodes(string[] textCptCodes)
        {
            for (var i = 0; i < CptCodesDetailsPresenter.MAX_NUMBER_OF_CPTCODES; i++)
            {
                cptTextBoxes[i].Text = textCptCodes[i];
            }
        }
        #endregion

        #region Data Elements

        private readonly Dictionary<int, string> cptCodes = new Dictionary<int, string>(CptCodesDetailsPresenter.MAX_NUMBER_OF_CPTCODES);
        public CptCodesDetailsPresenter Presenter { private get; set; }
        private readonly MaskedEditTextBox[] cptTextBoxes = new MaskedEditTextBox[CptCodesDetailsPresenter.MAX_NUMBER_OF_CPTCODES];

        #endregion

        #region Constants

        #endregion

    }
}
