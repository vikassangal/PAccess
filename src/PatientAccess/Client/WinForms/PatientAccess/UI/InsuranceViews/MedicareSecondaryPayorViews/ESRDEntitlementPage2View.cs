using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.MSP2Presenters;
using PatientAccess.UI.InsuranceViews.MSP2Views;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for ESRDEntitlementPage2View.
    /// </summary>
    public class ESRDEntitlementPage2View : ControlView, IESRDEntitlementPage2
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers

        private void mskKidneyTransplantDate_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText.Length == 0)
            {
                UIColors.SetRequiredBgColor(mtb);
                FormCanTransition();
            }
            else
            {
                UIColors.SetNormalBgColor(mtb);
            }
            if (mtb.UnMaskedText.Length == 8)
            {
                FormCanTransition();
            }
        }

        private void mskKidneyTransplantDate_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor(mtb);
            Refresh();
        }

        private void mskKidneyTransplantDate_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            transplantDateFieldError = false;

            if (mtb.UnMaskedText == String.Empty)
            {
                UIColors.SetRequiredBgColor(mtb);
                Refresh();
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).TransplantDate = DateTime.MinValue;
            }
            else if (mtb.Text.Length != 10)
            {
                mtb.Focus();
                UIColors.SetErrorBgColor(mtb);
                transplantDateFieldError = true;
                MessageBox.Show(UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
            }
            else
            {
                try
                {
                    DateTime transplantDate = new DateTime(Convert.ToInt32(mtb.Text.Substring(6, 4)),
                                                           Convert.ToInt32(mtb.Text.Substring(0, 2)),
                                                           Convert.ToInt32(mtb.Text.Substring(3, 2)));

                    if (DateValidator.IsValidDate(transplantDate) == false)
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor(mtb);
                        transplantDateFieldError = true;
                        MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        (Model_MedicareSecondaryPayor.MedicareEntitlement as
                         ESRDEntitlement).TransplantDate = transplantDate;
                        UIColors.SetNormalBgColor(mtb);
                        Refresh();
                        if ((bool) Tag == true)
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                    }
                }
                catch
                {
                    // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    mtb.Focus();
                    UIColors.SetErrorBgColor(mtb);
                    transplantDateFieldError = true;
                    MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                }
            }
            FormCanTransition();
        }

        private void mskDialysisDate_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText.Length == 0)
            {
                UIColors.SetRequiredBgColor(mtb);
                FormCanTransition();
            }
            else
            {
                UIColors.SetNormalBgColor(mtb);
            }
            if (mtb.UnMaskedText.Length == 8)
            {
                FormCanTransition();
            }
        }

        private void mskDialysisDate_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor(mtb);
            Refresh();
        }

        private void mskDialysisDate_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            dialysisDateFieldError = false;

            if (mtb.UnMaskedText == String.Empty)
            {
                UIColors.SetRequiredBgColor(mtb);
                Refresh();
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).DialysisDate = DateTime.MinValue;
            }
            else if (mtb.Text.Length != 10)
            {
                mtb.Focus();
                UIColors.SetErrorBgColor(mtb);
                dialysisDateFieldError = true;
                MessageBox.Show(UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
            }
            else
            {
                try
                {
                    DateTime dialysisDate = new DateTime(Convert.ToInt32(mtb.Text.Substring(6, 4)),
                                                         Convert.ToInt32(mtb.Text.Substring(0, 2)),
                                                         Convert.ToInt32(mtb.Text.Substring(3, 2)));

                    if (DateValidator.IsValidDate(dialysisDate) == false)
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor(mtb);
                        dialysisDateFieldError = true;
                        MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        (Model_MedicareSecondaryPayor.MedicareEntitlement as
                         ESRDEntitlement).DialysisDate = dialysisDate;
                        UIColors.SetNormalBgColor(mtb);
                        Refresh();
                        if ((bool) Tag == true)
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                    }
                }
                catch
                {
                    // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    mtb.Focus();
                    UIColors.SetErrorBgColor(mtb);
                    dialysisDateFieldError = true;
                    MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                }
            }
            FormCanTransition();
        }

        private void mskTrainingDate_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText.Length == 0)
            {
                UIColors.SetRequiredBgColor(mtb);
                FormCanTransition();
            }
            else
            {
                UIColors.SetNormalBgColor(mtb);
            }
            if (mtb.UnMaskedText.Length == 8)
            {
                FormCanTransition();
            }
        }

        private void mskTrainingDate_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor(mtb);
            Refresh();
        }

        private void mskTrainingDate_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            trainingDateFieldError = false;

            if (mtb.UnMaskedText == String.Empty)
            {
                // This field is not required for the question
                UIColors.SetRequiredBgColor(mtb);
                Refresh();
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).DialysisTrainingStartDate = DateTime.MinValue;
            }
            else if (mtb.Text.Length != 10)
            {
                mtb.Focus();
                UIColors.SetErrorBgColor(mtb);
                trainingDateFieldError = true;
                MessageBox.Show(UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
            }
            else
            {
                try
                {
                    DateTime trainingDate = new DateTime(Convert.ToInt32(mtb.Text.Substring(6, 4)),
                                                         Convert.ToInt32(mtb.Text.Substring(0, 2)),
                                                         Convert.ToInt32(mtb.Text.Substring(3, 2)));

                    if (DateValidator.IsValidDate(trainingDate) == false)
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor(mtb);
                        trainingDateFieldError = true;
                        MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        (Model_MedicareSecondaryPayor.MedicareEntitlement as
                         ESRDEntitlement).DialysisTrainingStartDate = trainingDate;
                        UIColors.SetNormalBgColor(mtb);
                        Refresh();
                        if ((bool) Tag == true)
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                    }
                }
                catch
                {
                    // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    mtb.Focus();
                    UIColors.SetErrorBgColor(mtb);
                    trainingDateFieldError = true;
                    MessageBox.Show(UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                }
            }
            FormCanTransition();
        }

        private void btnMoreInfo_Click(object sender, EventArgs e)
        {
            int X = this.Location.X + this.Size.Width/2;
            int Y = this.Location.Y + this.Size.Height/4;
            ESRDInfoDialog d = new ESRDInfoDialog();
            try
            {
                d.Location = new Point(X, Y);
                d.ShowDialog(this);
            }
            finally
            {
                d.Dispose();
            }
        }

        private void rbQuestion2Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question2Response = true;
                SetTransplantDateControlState(true);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).KidneyTransplant.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question2Response = true;
                SetTransplantDateControlState(false);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).KidneyTransplant.SetNo();

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).TransplantDate = DateTime.MinValue;

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion3Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question3Response = true;
                SetDialysisDateControlState(true);
                ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).DialysisTreatment.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion3No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question3Response = true;
                SetDialysisDateControlState(false);
                ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).DialysisTreatment.SetNo();
                UpdateDialysisCenterName(String.Empty);
                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion4Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question4Response = true;
                SetQuestion5State(true);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).WithinCoordinationPeriod.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion4No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question4Response = true;
                SetQuestion5State(false);
                SetQuestion6State(false);
                SetQuestion7State(false);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).WithinCoordinationPeriod.SetNo();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion5Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question5Response = true;
                SetQuestion6State(true);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).ESRDandAgeOrDisability.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion5No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question5Response = true;
                SetQuestion6State(false);
                SetQuestion7State(false);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).ESRDandAgeOrDisability.SetNo();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion6Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question6Response = true;
                SetQuestion7State(false);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnESRD.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion6No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question6Response = true;
                SetQuestion7State(true);

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnESRD.SetNo();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion7Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question7Response = true;

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnAgeOrDisability.SetYes();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion7No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Checked)
            {
                question7Response = true;

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnAgeOrDisability.SetNo();

                FormCanTransition();

                if ((bool) Tag == true)
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            SendMessage(parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero);
            SendMessage(parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero);
            ESRDEntitlementPagePresenter = new ESRDEntitlementPagePresenter(this, this.Model_Account);
            ESRDEntitlementPagePresenter.EnablePanels();
            if ((bool) Tag == true && FormChanged)
            {
                // User went back and made a change
                ResetView();
            }
            else if (formActivating)
            {
                ResetView();

                if ((Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement) == null)
                {
                    return;
                }
                else if (
                    Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals(
                        typeof (ESRDEntitlement)))
                {
                    // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls
                    YesNoFlag flag = new YesNoFlag();
                    flag =
                        (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                            KidneyTransplant;
                    if (flag.Code.Equals("Y"))
                    {
                        // Transplant date is required to set 'Yes' checkbox
                        DateTime date =
                            (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                TransplantDate;

                        if (date != DateTime.MinValue)
                        {
                            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).TransplantDate =
                                date;
                            mskKidneyTransplantDate.Text = String.Format("{0:D2}{1:D2}{2:D4}", date.Month, date.Day,
                                                                         date.Year);
                            UIColors.SetNormalBgColor(mskKidneyTransplantDate);
                        }
                        rbQuestion2Yes.Checked = true;
                    }
                    else if (flag.Code.Equals("N"))
                    {
                        rbQuestion2No.Checked = true;
                         
                    }
                    flag =
                        (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                            DialysisTreatment;
                    if (flag.Code.Equals("Y"))
                    {
                        // Dialysis date is required to set 'Yes' checkbox
                        DateTime dialDate =
                            (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                DialysisDate;

                        if (dialDate != DateTime.MinValue)
                        {
                            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).DialysisDate =
                                dialDate;
                            mskDialysisDate.Text = String.Format("{0:D2}{1:D2}{2:D4}", dialDate.Month, dialDate.Day,
                                                                 dialDate.Year);
                            UIColors.SetNormalBgColor(mskDialysisDate);
                        }

                        DateTime trainDate =
                            (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                DialysisTrainingStartDate;

                        if (trainDate != DateTime.MinValue)
                        {
                            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                DialysisTrainingStartDate = trainDate;
                            mskTrainingDate.Text = String.Format("{0:D2}{1:D2}{2:D4}", trainDate.Month,
                                                                 trainDate.Day, trainDate.Year);
                        }
                        EsrdEntitlement =
                            (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement);
                        ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                        rbQuestion3Yes.Checked = true;
                    }
                    else if (flag.Code.Equals("N"))
                    {
                        rbQuestion3No.Checked = true;
                        ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                        
                    }
                    flag =
                        (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                            WithinCoordinationPeriod;
                    if (flag.Code.Equals("Y"))
                    {
                        rbQuestion4Yes.Checked = true;
                        flag =
                            (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                ESRDandAgeOrDisability;
                        if (flag.Code.Equals("Y"))
                        {
                            rbQuestion5Yes.Checked = true;
                            flag =
                                (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                    BasedOnESRD;
                            if (flag.Code.Equals("Y"))
                            {
                                rbQuestion6Yes.Checked = true;
                            }
                            else if (flag.Code.Equals("N"))
                            {
                                rbQuestion6No.Checked = true;
                                flag =
                                    (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).
                                        BasedOnAgeOrDisability;
                                if (flag.Code.Equals("Y"))
                                {
                                    rbQuestion7Yes.Checked = true;
                                }
                                else if (flag.Code.Equals("N"))
                                {
                                    rbQuestion7No.Checked = true;
                                }
                            }
                        }
                        else if (flag.Code.Equals("N"))
                        {
                            rbQuestion5No.Checked = true;
                        }
                    }
                    else if (flag.Code.Equals("N"))
                    {
                        rbQuestion4No.Checked = true;
                    }
                }
            }
            FormCanTransition();
        }

        public void PopulateDialysisCenterNames(IEnumerable<string> dialysisCenterNames)
        {
            cmbDialysisCenter.Items.Clear();
            foreach (var dialysisCenter in dialysisCenterNames)
            {
                cmbDialysisCenter.Items.Add(dialysisCenter);
            }
        }

        public void SetDialysisCenterName(string dialysisCenterName)
        {

            // If the value is not in the list, add it as a valid choice. This will
            // prevent data loss in the event that the value stored with the account
            // is removed from the lookup table
            if (!cmbDialysisCenter.Items.Contains(dialysisCenterName))
            {
                cmbDialysisCenter.Items.Add(dialysisCenterName);
            }
            cmbDialysisCenter.SelectedItem = dialysisCenterName;
        }

        public void SetDialysisCenterNameRequired()
        {
            UIColors.SetRequiredBgColor(this.cmbDialysisCenter);
        }

        public void SetDialysisCenterNameNormal()
        {
            UIColors.SetNormalBgColor(this.cmbDialysisCenter);
        }

        public void ClearDialysisCenterNames()
        {
            if (cmbDialysisCenter.Items.Count > 0)
            {
                cmbDialysisCenter.SelectedIndex = -1;
            }
        }

        public void EnableDialysisCenterNames()
        {
            cmbDialysisCenter.Enabled = true;
            lblDialysisCentername.Enabled = true;
        }

        public void DisableDialysisCenterNames()
        {
            cmbDialysisCenter.Enabled = false;
            lblDialysisCentername.Enabled = false;
        }
        public void ResetSelections()
        {
        }
        public void ResetDialysisCenterSelection()
        {
        }

        public bool GHP()
        {
            if (ESRDEntitlementPage1View.GHPSelectd)
            {
                return true;
            }
            return false;
        }
        public void EnablePanels(bool ghp)
        {

            panel1.Enabled = ghp;
            panel2.Enabled = !ghp;
            panel3.Enabled = ghp;
        }

        #endregion

        #region Properties

        [Browsable(false)]
        private MedicareSecondaryPayor Model_MedicareSecondaryPayor
        {
            get { return (MedicareSecondaryPayor) this.Model; }
        }

        [Browsable(false)]
        public Account Model_Account
        {
            private get { return (Account) this.i_account; }
            set { i_account = value; }
        }

        [Browsable(false)]
        public bool FormChanged
        {
            get { return formWasChanged; }
            set { formWasChanged = value; }
        }

        [Browsable(false)]
        public int Response
        {
            get { return response; }
        }

        private IESRDEntitlementPagePresenter ESRDEntitlementPagePresenter
        {
            get { return esrdEntitlementPagePresenter; }

            set { esrdEntitlementPagePresenter = value; }
        }

        public ESRDEntitlement EsrdEntitlement
        {
            get { return Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement; }

            set { Model_MedicareSecondaryPayor.MedicareEntitlement = value; }
        }
        public bool ReceivedMaintenanceDialysisTreatment
        {
            get { return rbQuestion3Yes.Checked; }

            set { rbQuestion3Yes.Checked = value; }
        }
        public string SelectedDialysisCenter
        {
            get
            {
                return selectedDialysisCenter;
            }

            set
            {
                selectedDialysisCenter = value;
            }
        }
        public bool DialysisCenterNamesEnabled
        {
            get
            {
                return this.cmbDialysisCenter.Enabled;
            }

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            bool result = false;

            if (question2Response && question3Response && question4Response &&
                question5Response && question6Response && question7Response)
            {
                if (rbQuestion2Yes.Checked && mskKidneyTransplantDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && mskDialysisDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && cmbDialysisCenter.Enabled && String.IsNullOrEmpty(cmbDialysisCenter.SelectedItem as string))
                {
                    result = false;
                }
                else if (rbQuestion7Yes.Checked)
                {
                    // GHP is primary
                    response = MSPEventCode.YesStimulus();
                    result = true;
                }
                else
                {
                    // Medicare is primary
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
            }
            else if (question2Response && question3Response && question4Response &&
                     question5Response && question6Response)
            {
                if (rbQuestion2Yes.Checked && mskKidneyTransplantDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && mskDialysisDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && cmbDialysisCenter.Enabled && String.IsNullOrEmpty(cmbDialysisCenter.SelectedItem as string))
                {
                    result = false;
                }
                else if (rbQuestion6Yes.Checked)
                {
                    // GHP is primary
                    response = MSPEventCode.YesStimulus();
                    result = true;
                }
            }
            else if (question2Response && question3Response &&
                     question4Response && question5Response)
            {
                if (rbQuestion2Yes.Checked && mskKidneyTransplantDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && mskDialysisDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion5No.Checked)
                {
                    // GHP is primary
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
            }
            else if (question2Response && question3Response && question4Response)
            {
                if (rbQuestion2Yes.Checked && mskKidneyTransplantDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && mskDialysisDate.UnMaskedText.Length != 8)
                {
                    result = false;
                }
                else if (rbQuestion3Yes.Checked && cmbDialysisCenter.Enabled && String.IsNullOrEmpty(cmbDialysisCenter.SelectedItem as string))
                {
                    result = false;
                }
                else if (rbQuestion4No.Checked)
                {
                    // Medicare is primary
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
            }
            else if(question3Response && !panel1.Enabled && !panel3.Enabled && !panel5.Enabled && !panel6.Enabled )
            {
                if (DialysisDateHasValidValue() && DialysisCenterHasValidValue() || (rbQuestion3No.Checked))
                {
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
            }
            else if ((!question3Response && !panel2.Enabled)  && 
                     ( (rbQuestion2Yes.Checked && mskKidneyTransplantDate.UnMaskedText.Length == 8) ||(rbQuestion2No.Checked)) && 
                     question4Response &&
                     ((!question5Response && !panel4.Enabled) || (question5Response && panel4.Enabled)) &&
                     (( !question6Response && !panel5.Enabled) ||(question6Response && panel5.Enabled)) && 
                      (( !question7Response && !panel6.Enabled) || (question7Response && panel6.Enabled)))
            {
                
                    response = MSPEventCode.NoStimulus();
                    result = true;
                
            }
            if (transplantDateFieldError || dialysisDateFieldError || trainingDateFieldError)
            {
                response = -1;
                result = false;
            }
            if (result)
            {
                SendMessage(parentForm.Handle, CONTINUE_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero);
                SendMessage(parentForm.Handle, CONTINUE_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                SendMessage(parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero);
            }
        }
        private bool DialysisDateHasValidValue()
        {
            if( rbQuestion3Yes.Checked && mskDialysisDate.UnMaskedText.Length == 8)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool DialysisCenterHasValidValue()
        {
            if ( rbQuestion3Yes.Checked && 
                 cmbDialysisCenter.Enabled && 
                 !String.IsNullOrEmpty(cmbDialysisCenter.SelectedItem as string))
            {
                return true;
            }
            else if(rbQuestion3Yes.Checked && 
                  !cmbDialysisCenter.Enabled )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void ResetView()
        {
            rbQuestion2No.Checked = false;
            rbQuestion2Yes.Checked = false;
            rbQuestion3Yes.Checked = false;
            rbQuestion3No.Checked = false;
            rbQuestion4No.Checked = false;
            rbQuestion4Yes.Checked = false;
            rbQuestion5Yes.Checked = false;
            rbQuestion5No.Checked = false;
            rbQuestion6Yes.Checked = false;
            rbQuestion6No.Checked = false;
            rbQuestion7Yes.Checked = false;
            rbQuestion7No.Checked = false;
            mskDialysisDate.UnMaskedText = String.Empty;
            UIColors.SetNormalBgColor(mskDialysisDate);
            UIColors.SetNormalBgColor(cmbDialysisCenter);
            mskKidneyTransplantDate.UnMaskedText = String.Empty;
            UIColors.SetNormalBgColor(mskKidneyTransplantDate);
            mskTrainingDate.UnMaskedText = String.Empty;
            question2Response = false;
            question3Response = false;
            question4Response = false;
            question5Response = false;
            question6Response = false;
            question7Response = false;
            transplantDateFieldError = false;
            dialysisDateFieldError = false;
            trainingDateFieldError = false;
            formActivating = false;
        }

        private void SetTransplantDateControlState(bool state)
        {
            lblStaticKidney.Enabled = state;
            mskKidneyTransplantDate.Enabled = state;
            transplantDateFieldError = false;

            if (state)
            {
                if (mskKidneyTransplantDate.UnMaskedText.Length == 0)
                {
                    UIColors.SetRequiredBgColor(mskKidneyTransplantDate);
                }
            }
            else
            {
                mskKidneyTransplantDate.Text = String.Empty;
                mskKidneyTransplantDate.UnMaskedText = String.Empty;
                UIColors.SetNormalBgColor(mskKidneyTransplantDate);
                Refresh();
            }
        }

        /// <summary>
        /// Enable or disable the controls for question 5
        /// </summary>
        private void SetQuestion5State(bool state)
        {
            if (state == false)
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).ESRDandAgeOrDisability.SetBlank();
            }
            panel4.Enabled = state;
            rbQuestion5Yes.Checked = false;
            rbQuestion5No.Checked = false;
            question5Response = false;
        }

        /// <summary>
        /// Enable or disable the controls for question 6
        /// </summary>
        private void SetQuestion6State(bool state)
        {
            if (state == false)
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnESRD.SetBlank();
            }
            panel5.Enabled = state;
            rbQuestion6Yes.Checked = false;
            rbQuestion6No.Checked = false;
            question6Response = false;
        }

        /// <summary>
        /// Enable or disable the controls for question 7
        /// </summary>
        private void SetQuestion7State(bool state)
        {
            if (state == false)
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                 ESRDEntitlement).BasedOnAgeOrDisability.SetBlank();
            }
            panel6.Enabled = state;
            rbQuestion7Yes.Checked = false;
            rbQuestion7No.Checked = false;
            question7Response = false;
        }

        private void SetDialysisDateControlState(bool state)
        {
            lblStaticDialysis.Enabled = state;
            mskDialysisDate.Enabled = state;
            label1.Enabled = state;
            mskTrainingDate.Enabled = state;
            dialysisDateFieldError = false;
            trainingDateFieldError = false;

            if (state)
            {
                if (mskDialysisDate.UnMaskedText.Length == 0)
                {
                    UIColors.SetRequiredBgColor(mskDialysisDate);
                }
            }
            else
            {
                mskDialysisDate.Text = String.Empty;
                mskDialysisDate.UnMaskedText = String.Empty;
                UIColors.SetNormalBgColor(mskDialysisDate);
                mskTrainingDate.Text = String.Empty;
                mskTrainingDate.UnMaskedText = String.Empty;
                Refresh();
            }
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ESRDEntitlementPage2View));
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblQuestion7a = new System.Windows.Forms.Label();
            this.lblQuestion7 = new System.Windows.Forms.Label();
            this.rbQuestion7Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion7No = new System.Windows.Forms.RadioButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblQuestion6a = new System.Windows.Forms.Label();
            this.lblQuestion6 = new System.Windows.Forms.Label();
            this.rbQuestion6Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion6No = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblQuestion5a = new System.Windows.Forms.Label();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.rbQuestion5Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion5No = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnMoreInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQuestion4 = new System.Windows.Forms.Label();
            this.rbQuestion4No = new System.Windows.Forms.RadioButton();
            this.rbQuestion4Yes = new System.Windows.Forms.RadioButton();
            this.lblStaticKidney = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.rbQuestion3Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion3No = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.rbQuestion2No = new System.Windows.Forms.RadioButton();
            this.rbQuestion2Yes = new System.Windows.Forms.RadioButton();
            this.mskKidneyTransplantDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDialysis = new System.Windows.Forms.Label();
            this.mskDialysisDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mskTrainingDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbDialysisCenter = new System.Windows.Forms.ComboBox();
            this.lblDialysisCentername = new System.Windows.Forms.Label();
            this.txtTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblQuestion7a);
            this.panel6.Controls.Add(this.lblQuestion7);
            this.panel6.Controls.Add(this.rbQuestion7Yes);
            this.panel6.Controls.Add(this.rbQuestion7No);
            this.panel6.Enabled = false;
            this.panel6.Location = new System.Drawing.Point(16, 382);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(600, 32);
            this.panel6.TabIndex = 9;
            this.panel6.TabStop = true;
            // 
            // lblQuestion7a
            // 
            this.lblQuestion7a.Location = new System.Drawing.Point(13, 14);
            this.lblQuestion7a.Name = "lblQuestion7a";
            this.lblQuestion7a.Size = new System.Drawing.Size(264, 23);
            this.lblQuestion7a.TabIndex = 0;
            this.lblQuestion7a.Text = "primarily based on age or disability entitlement)?";
            // 
            // lblQuestion7
            // 
            this.lblQuestion7.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion7.Name = "lblQuestion7";
            this.lblQuestion7.Size = new System.Drawing.Size(384, 32);
            this.lblQuestion7.TabIndex = 0;
            this.lblQuestion7.Text = "7. Does the working aged or disability MSP provision apply (i.e., is the GHP";
            // 
            // rbQuestion7Yes
            // 
            this.rbQuestion7Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion7Yes.Name = "rbQuestion7Yes";
            this.rbQuestion7Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion7Yes.TabIndex = 1;
            this.rbQuestion7Yes.TabStop = true;
            this.rbQuestion7Yes.Text = "Yes";
            this.rbQuestion7Yes.CheckedChanged += new System.EventHandler(this.rbQuestion7Yes_CheckedChanged);
            // 
            // rbQuestion7No
            // 
            this.rbQuestion7No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion7No.Name = "rbQuestion7No";
            this.rbQuestion7No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion7No.TabIndex = 2;
            this.rbQuestion7No.TabStop = true;
            this.rbQuestion7No.Text = "No";
            this.rbQuestion7No.CheckedChanged += new System.EventHandler(this.rbQuestion7No_CheckedChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblQuestion6a);
            this.panel5.Controls.Add(this.lblQuestion6);
            this.panel5.Controls.Add(this.rbQuestion6Yes);
            this.panel5.Controls.Add(this.rbQuestion6No);
            this.panel5.Enabled = false;
            this.panel5.Location = new System.Drawing.Point(16, 335);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(600, 32);
            this.panel5.TabIndex = 8;
            this.panel5.TabStop = true;
            // 
            // lblQuestion6a
            // 
            this.lblQuestion6a.Location = new System.Drawing.Point(13, 14);
            this.lblQuestion6a.Name = "lblQuestion6a";
            this.lblQuestion6a.Size = new System.Drawing.Size(163, 23);
            this.lblQuestion6a.TabIndex = 0;
            this.lblQuestion6a.Text = "entitlement) based on ESRD?";
            // 
            // lblQuestion6
            // 
            this.lblQuestion6.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion6.Name = "lblQuestion6";
            this.lblQuestion6.Size = new System.Drawing.Size(432, 32);
            this.lblQuestion6.TabIndex = 0;
            this.lblQuestion6.Text = "6. Was your initial entitlement to Medicare (including simultaneous or dual";
            // 
            // rbQuestion6Yes
            // 
            this.rbQuestion6Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion6Yes.Name = "rbQuestion6Yes";
            this.rbQuestion6Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion6Yes.TabIndex = 1;
            this.rbQuestion6Yes.TabStop = true;
            this.rbQuestion6Yes.Text = "Yes";
            this.rbQuestion6Yes.CheckedChanged += new System.EventHandler(this.rbQuestion6Yes_CheckedChanged);
            // 
            // rbQuestion6No
            // 
            this.rbQuestion6No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion6No.Name = "rbQuestion6No";
            this.rbQuestion6No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion6No.TabIndex = 2;
            this.rbQuestion6No.TabStop = true;
            this.rbQuestion6No.Text = "No";
            this.rbQuestion6No.CheckedChanged += new System.EventHandler(this.rbQuestion6No_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblQuestion5a);
            this.panel4.Controls.Add(this.lblQuestion5);
            this.panel4.Controls.Add(this.rbQuestion5Yes);
            this.panel4.Controls.Add(this.rbQuestion5No);
            this.panel4.Enabled = false;
            this.panel4.Location = new System.Drawing.Point(16, 288);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(600, 32);
            this.panel4.TabIndex = 7;
            this.panel4.TabStop = true;
            // 
            // lblQuestion5a
            // 
            this.lblQuestion5a.Location = new System.Drawing.Point(11, 14);
            this.lblQuestion5a.Name = "lblQuestion5a";
            this.lblQuestion5a.Size = new System.Drawing.Size(100, 23);
            this.lblQuestion5a.TabIndex = 0;
            this.lblQuestion5a.Text = "and disability?";
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(392, 32);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5. Are you entitled to Medicare on the basis of either ESRD and age or ESRD";
            // 
            // rbQuestion5Yes
            // 
            this.rbQuestion5Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion5Yes.Name = "rbQuestion5Yes";
            this.rbQuestion5Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5Yes.TabIndex = 1;
            this.rbQuestion5Yes.TabStop = true;
            this.rbQuestion5Yes.Text = "Yes";
            this.rbQuestion5Yes.CheckedChanged += new System.EventHandler(this.rbQuestion5Yes_CheckedChanged);
            // 
            // rbQuestion5No
            // 
            this.rbQuestion5No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion5No.Name = "rbQuestion5No";
            this.rbQuestion5No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5No.TabIndex = 2;
            this.rbQuestion5No.TabStop = true;
            this.rbQuestion5No.Text = "No";
            this.rbQuestion5No.CheckedChanged += new System.EventHandler(this.rbQuestion5No_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnMoreInfo);
            this.panel3.Controls.Add(this.lblQuestion4);
            this.panel3.Controls.Add(this.rbQuestion4No);
            this.panel3.Controls.Add(this.rbQuestion4Yes);
            this.panel3.Location = new System.Drawing.Point(16, 249);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(600, 24);
            this.panel3.TabIndex = 6;
            this.panel3.TabStop = true;
            // 
            // btnMoreInfo
            // 
            this.btnMoreInfo.Location = new System.Drawing.Point(280, 0);
            this.btnMoreInfo.Message = null;
            this.btnMoreInfo.Name = "btnMoreInfo";
            this.btnMoreInfo.Size = new System.Drawing.Size(75, 23);
            this.btnMoreInfo.TabIndex = 1;
            this.btnMoreInfo.Text = "More In&fo";
            this.btnMoreInfo.Click += new System.EventHandler(this.btnMoreInfo_Click);
            // 
            // lblQuestion4
            // 
            this.lblQuestion4.Location = new System.Drawing.Point(0, 8);
            this.lblQuestion4.Name = "lblQuestion4";
            this.lblQuestion4.Size = new System.Drawing.Size(288, 23);
            this.lblQuestion4.TabIndex = 0;
            this.lblQuestion4.Text = "4. Are you within the 30-month coordination period?";
            // 
            // rbQuestion4No
            // 
            this.rbQuestion4No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion4No.Name = "rbQuestion4No";
            this.rbQuestion4No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion4No.TabIndex = 3;
            this.rbQuestion4No.TabStop = true;
            this.rbQuestion4No.Text = "No";
            this.rbQuestion4No.CheckedChanged += new System.EventHandler(this.rbQuestion4No_CheckedChanged);
            // 
            // rbQuestion4Yes
            // 
            this.rbQuestion4Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion4Yes.Name = "rbQuestion4Yes";
            this.rbQuestion4Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion4Yes.TabIndex = 2;
            this.rbQuestion4Yes.TabStop = true;
            this.rbQuestion4Yes.Text = "Yes";
            this.rbQuestion4Yes.CheckedChanged += new System.EventHandler(this.rbQuestion4Yes_CheckedChanged);
            // 
            // lblStaticKidney
            // 
            this.lblStaticKidney.Enabled = false;
            this.lblStaticKidney.Location = new System.Drawing.Point(27, 94);
            this.lblStaticKidney.Name = "lblStaticKidney";
            this.lblStaticKidney.Size = new System.Drawing.Size(98, 23);
            this.lblStaticKidney.TabIndex = 0;
            this.lblStaticKidney.Text = "Date of transplant:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblQuestion3);
            this.panel2.Controls.Add(this.rbQuestion3Yes);
            this.panel2.Controls.Add(this.rbQuestion3No);
            this.panel2.Location = new System.Drawing.Point(16, 129);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(675, 114);
            this.panel2.TabIndex = 3;
            this.panel2.TabStop = true;
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(288, 23);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3. Have you received maintenance dialysis treatments?";
            // 
            // rbQuestion3Yes
            // 
            this.rbQuestion3Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion3Yes.Name = "rbQuestion3Yes";
            this.rbQuestion3Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion3Yes.TabIndex = 1;
            this.rbQuestion3Yes.TabStop = true;
            this.rbQuestion3Yes.Text = "Yes";
            this.rbQuestion3Yes.CheckedChanged += new System.EventHandler(this.rbQuestion3Yes_CheckedChanged);
            // 
            // rbQuestion3No
            // 
            this.rbQuestion3No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion3No.Name = "rbQuestion3No";
            this.rbQuestion3No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion3No.TabIndex = 2;
            this.rbQuestion3No.TabStop = true;
            this.rbQuestion3No.Text = "No";
            this.rbQuestion3No.CheckedChanged += new System.EventHandler(this.rbQuestion3No_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblQuestion2);
            this.panel1.Controls.Add(this.rbQuestion2No);
            this.panel1.Controls.Add(this.rbQuestion2Yes);
            this.panel1.Location = new System.Drawing.Point(16, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(248, 23);
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2. Have you received a kidney transplant?";
            // 
            // rbQuestion2No
            // 
            this.rbQuestion2No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion2No.Name = "rbQuestion2No";
            this.rbQuestion2No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion2No.TabIndex = 2;
            this.rbQuestion2No.TabStop = true;
            this.rbQuestion2No.Text = "No";
            this.rbQuestion2No.CheckedChanged += new System.EventHandler(this.rbQuestion2No_CheckedChanged);
            // 
            // rbQuestion2Yes
            // 
            this.rbQuestion2Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion2Yes.Name = "rbQuestion2Yes";
            this.rbQuestion2Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion2Yes.TabIndex = 1;
            this.rbQuestion2Yes.TabStop = true;
            this.rbQuestion2Yes.Text = "Yes";
            this.rbQuestion2Yes.CheckedChanged += new System.EventHandler(this.rbQuestion2Yes_CheckedChanged);
            // 
            // mskKidneyTransplantDate
            // 
            this.mskKidneyTransplantDate.BackColor = System.Drawing.Color.White;
            this.mskKidneyTransplantDate.Enabled = false;
            this.mskKidneyTransplantDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskKidneyTransplantDate.KeyPressExpression = "^\\d*$";
            this.mskKidneyTransplantDate.Location = new System.Drawing.Point(123, 91);
            this.mskKidneyTransplantDate.Mask = "  /  /";
            this.mskKidneyTransplantDate.MaxLength = 10;
            this.mskKidneyTransplantDate.Name = "mskKidneyTransplantDate";
            this.mskKidneyTransplantDate.Size = new System.Drawing.Size(70, 20);
            this.mskKidneyTransplantDate.TabIndex = 2;
            this.mskKidneyTransplantDate.ValidationExpression = resources.GetString("mskKidneyTransplantDate.ValidationExpression");
            this.mskKidneyTransplantDate.TextChanged += new System.EventHandler(this.mskKidneyTransplantDate_TextChanged);
            this.mskKidneyTransplantDate.Enter += new System.EventHandler(this.mskKidneyTransplantDate_Enter);
            this.mskKidneyTransplantDate.Leave += new System.EventHandler(this.mskKidneyTransplantDate_Leave);
            // 
            // lblStaticDialysis
            // 
            this.lblStaticDialysis.Enabled = false;
            this.lblStaticDialysis.Location = new System.Drawing.Point(26, 154);
            this.lblStaticDialysis.Name = "lblStaticDialysis";
            this.lblStaticDialysis.Size = new System.Drawing.Size(120, 23);
            this.lblStaticDialysis.TabIndex = 0;
            this.lblStaticDialysis.Text = "Date dialysis began:";
            // 
            // mskDialysisDate
            // 
            this.mskDialysisDate.BackColor = System.Drawing.Color.White;
            this.mskDialysisDate.Enabled = false;
            this.mskDialysisDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskDialysisDate.KeyPressExpression = "^\\d*$";
            this.mskDialysisDate.Location = new System.Drawing.Point(407, 154);
            this.mskDialysisDate.Mask = "  /  /";
            this.mskDialysisDate.MaxLength = 10;
            this.mskDialysisDate.Name = "mskDialysisDate";
            this.mskDialysisDate.Size = new System.Drawing.Size(70, 20);
            this.mskDialysisDate.TabIndex = 4;
            this.mskDialysisDate.ValidationExpression = resources.GetString("mskDialysisDate.ValidationExpression");
            this.mskDialysisDate.TextChanged += new System.EventHandler(this.mskDialysisDate_TextChanged);
            this.mskDialysisDate.Enter += new System.EventHandler(this.mskDialysisDate_Enter);
            this.mskDialysisDate.Leave += new System.EventHandler(this.mskDialysisDate_Leave);
            // 
            // label1
            // 
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(26, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(390, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "If you participated in a self-dialysis training program, provide date training st" +
                "arted:";
            // 
            // mskTrainingDate
            // 
            this.mskTrainingDate.BackColor = System.Drawing.Color.White;
            this.mskTrainingDate.Enabled = false;
            this.mskTrainingDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskTrainingDate.KeyPressExpression = "^\\d*$";
            this.mskTrainingDate.Location = new System.Drawing.Point(407, 186);
            this.mskTrainingDate.Mask = "  /  /";
            this.mskTrainingDate.MaxLength = 10;
            this.mskTrainingDate.Name = "mskTrainingDate";
            this.mskTrainingDate.Size = new System.Drawing.Size(70, 20);
            this.mskTrainingDate.TabIndex = 5;
            this.mskTrainingDate.ValidationExpression = resources.GetString("mskTrainingDate.ValidationExpression");
            this.mskTrainingDate.TextChanged += new System.EventHandler(this.mskTrainingDate_TextChanged);
            this.mskTrainingDate.Enter += new System.EventHandler(this.mskTrainingDate_Enter);
            this.mskTrainingDate.Leave += new System.EventHandler(this.mskTrainingDate_Leave);
            // 
            // cmbDialysisCenter
            // 
            this.cmbDialysisCenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDialysisCenter.Enabled = false;
            this.cmbDialysisCenter.Location = new System.Drawing.Point(406, 213);
            this.cmbDialysisCenter.MaxLength = 41;
            this.cmbDialysisCenter.Name = "cmbDialysisCenter";
            this.cmbDialysisCenter.Size = new System.Drawing.Size(275, 21);
            this.cmbDialysisCenter.TabIndex = 6;
            this.cmbDialysisCenter.SelectedIndexChanged += new System.EventHandler(this.cmbDialysisCenter_SelectedIndexChanged);
            this.cmbDialysisCenter.Validating += new System.ComponentModel.CancelEventHandler(this.cmbDialysisCenter__Validating);
            // 
            // lblDialysisCentername
            // 
            this.lblDialysisCentername.Enabled = false;
            this.lblDialysisCentername.Location = new System.Drawing.Point(26, 213);
            this.lblDialysisCentername.Name = "lblDialysisCentername";
            this.lblDialysisCentername.Size = new System.Drawing.Size(210, 22);
            this.lblDialysisCentername.TabIndex = 10;
            this.lblDialysisCentername.Text = "Name of Dialysis Center:";
            // 
            // txtTitle
            // 
            this.txtTitle.BackColor = System.Drawing.Color.White;
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(16, 16);
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(216, 23);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Text = "Medicare Entitlement - ESRD";
            // 
            // ESRDEntitlementPage2View
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cmbDialysisCenter);
            this.Controls.Add(this.lblDialysisCentername);
            this.Controls.Add(this.mskTrainingDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mskDialysisDate);
            this.Controls.Add(this.mskKidneyTransplantDate);
            this.Controls.Add(this.lblStaticDialysis);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.lblStaticKidney);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtTitle);
            this.Name = "ESRDEntitlementPage2View";
            this.Size = new System.Drawing.Size(735, 520);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Construction and Finalization

        public ESRDEntitlementPage2View(MSPDialog form)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn(this);
            parentForm = form;
            formActivating = true; // Used in setting radio button states
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

        #endregion

        #region Data Elements

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton btnMoreInfo;

        private Label lblQuestion2;
        private Label lblQuestion3;
        private Label lblQuestion4;
        private Label lblQuestion5;
        private Label lblQuestion5a;
        private Label lblQuestion6;
        private Label lblQuestion6a;
        private Label lblQuestion7;
        private Label lblQuestion7a;
        private Label lblStaticKidney;
        private Label label1;
        private Label lblStaticDialysis;

        private Panel panel6;
        private Panel panel5;
        private Panel panel4;
        private Panel panel3;
        private Panel panel2;
        private Panel panel1;

        private RadioButton rbQuestion2No;
        private RadioButton rbQuestion2Yes;
        private RadioButton rbQuestion3Yes;
        private RadioButton rbQuestion3No;
        private RadioButton rbQuestion4No;
        private RadioButton rbQuestion4Yes;
        private RadioButton rbQuestion5Yes;
        private RadioButton rbQuestion5No;
        private RadioButton rbQuestion6Yes;
        private RadioButton rbQuestion6No;
        private RadioButton rbQuestion7Yes;
        private RadioButton rbQuestion7No;

        private MaskedEditTextBox mskDialysisDate;
        private MaskedEditTextBox mskKidneyTransplantDate;
        private MaskedEditTextBox mskTrainingDate;

        private NonSelectableTextBox txtTitle;

        private Account i_account;
        private MSPDialog parentForm;
        private int response;
        private static bool formWasChanged;
        public static bool formActivating;
        private bool question2Response;
        private bool question3Response;
        private bool question4Response;
        private bool question5Response;
        private bool question6Response;
        private bool question7Response;
        private bool transplantDateFieldError;
        private bool dialysisDateFieldError;
        private bool trainingDateFieldError;
        private IESRDEntitlementPagePresenter esrdEntitlementPagePresenter;
        private string selectedDialysisCenter = string.Empty;
        //private ESRDEntitlement entitlement;

        #endregion

        #region Constants

        private const Int32 WM_USER = 0x400;
        private const Int32 CONTINUE_BUTTON_DISABLED = WM_USER + 1;
        private const Int32 CONTINUE_BUTTON_ENABLED = WM_USER + 2;
        private const Int32 CONTINUE_BUTTON_FOCUS = WM_USER + 3;
        private const Int32 NEXT_BUTTON_DISABLED = WM_USER + 4;
        private const Int32 NEXT_BUTTON_ENABLED = WM_USER + 5;
        private ComboBox cmbDialysisCenter;
        private Label lblDialysisCentername;
        private const Int32 NEXT_BUTTON_FOCUS = WM_USER + 6;

        #endregion

        private void cmbDialysisCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDialysisCenterName(cmbDialysisCenter.SelectedItem as string);
        }

        private void cmbDialysisCenter__Validating(object sender, CancelEventArgs e)
        {
           UpdateDialysisCenterName(cmbDialysisCenter.SelectedItem as string);
        }

        private void UpdateDialysisCenterName(string selectedDialysisCenter)
        {
            UIColors.SetNormalBgColor(this.cmbDialysisCenter);
            if (selectedDialysisCenter != null)
            {
                esrdEntitlementPagePresenter.UpdateDialysisCenterName(selectedDialysisCenter);
                if ((Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement) != null)
                {
                    esrdEntitlementPagePresenter.SaveDialysisCenterName();
                }
            }
            esrdEntitlementPagePresenter.SetDialysisCenterNameColor();
            FormCanTransition();
        }
    }


}
