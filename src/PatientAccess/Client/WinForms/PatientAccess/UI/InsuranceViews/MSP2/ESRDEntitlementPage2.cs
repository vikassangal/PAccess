using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.MSP2Views;
using PatientAccess.UI.ShortRegistration;
using PatientAccess.UI.InsuranceViews.MSP2Presenters;


namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// ESRDEntitlementPage2 - the second page of Entitlement by ESRD; captures information related to 
    /// kidney transplants
    /// </summary>
   
    [Serializable]
    public class ESRDEntitlementPage2 : WizardPage, IESRDEntitlementPage2
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// btnInsEditInsurance_Click - the user has elected to abandon the MSP wizard and edit the insurance info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsEditInsurance_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.INSURANCE );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.INSURANCE );
            }
        }

        /// <summary>
        /// mtbQ2DateOfTransplant_Leave - check the format of the date entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ2DateOfTransplant_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            DisableNextAndContinue();
            dateChecks( mtb );
        }

        /// <summary>
        /// mtbQ3Training_Leave - check the format of the date entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ3Training_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            DisableNextAndContinue();
            dateChecks( mtb );
        }

        /// <summary>
        /// mtbQ3DateDialysisBegan_Leave - check the format of the date entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ3DateDialysisBegan_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            DisableNextAndContinue();
            dateChecks( mtb );
        }

        /// <summary>
        /// dateChecks - check the date passed in
        /// </summary>
        /// <param name="mtb"></param>
        private void dateChecks( MaskedEditTextBox mtb )
        {
            UIColors.SetNormalBgColor( mtb );
            Refresh();

            if(  mtb.UnMaskedText != string.Empty
                && mtb.Text.Length != 0
                && mtb.Text.Length != 10 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );

                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return;
            }
            else
            {
                if(  mtb.UnMaskedText != string.Empty
                    && mtb.Text.Length != 0 )
                {
                
                    try
                    {   // Check the date entered is not in the future
                        DateTime benefitsDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                            Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                            Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                        if( !DateValidator.IsValidDate( benefitsDate ) )                         
                        {
                            mtb.Focus();
                            UIColors.SetErrorBgColor( mtb );
                            MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1 );
                            return;
                        }
                    }
                    catch
                    {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                        // an invalid year, month, or day.  Simply set field to error color.
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        return;
                    }
                }
            }

            CanPageNavigate();
        }
        private bool IsValidDate(MaskedEditTextBox mtb)
        {
             
            if (mtb.UnMaskedText != string.Empty
                && mtb.Text.Length != 0
                && mtb.Text.Length != 10)
            {
                return false;
            }
            else
            {
                if (mtb.UnMaskedText != string.Empty
                    && mtb.Text.Length != 0)
                {

                    try
                    {   // Check the date entered is not in the future
                        DateTime benefitsDate = new DateTime(Convert.ToInt32(mtb.Text.Substring(6, 4)),
                            Convert.ToInt32(mtb.Text.Substring(0, 2)),
                            Convert.ToInt32(mtb.Text.Substring(3, 2)));

                        if (!DateValidator.IsValidDate(benefitsDate))
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// btnQ4MoreInfo_Click - display the custom 'messagebox' with more info for the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ4MoreInfo_Click(object sender, EventArgs e)
        {
            CloseMessageBox box = new CloseMessageBox( "30-month Coordination Period",
                "The 30-month coordination period that starts on the first day of the month that an individual is eligible "
                + "for Medicare (even if not yet enrolled in Medicare) because of kidney failure (usually the fourth " 
                + "month of dialysis).\r\n\r\n"
                + "If the individual is participating in a self-dialysis training program or has a kidney transplant " 
                + "during the 3-month waiting period, the 30-month coordination period starts with the first day of " 
                + "the month of dialysis or the kidney transplant." );

            box.Show();
        }
        
        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup2_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                pnlQuestion2b.Enabled                  = true;
                pnlInsurance.Enabled                   = true;
            }      
            else
            {
                mtbQ2DateOfTransplant.UnMaskedText     = string.Empty;
                pnlQuestion2b.Enabled                  = false;
            }

            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup3_RadioChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "rbYes" && ((RadioButton)sender).Checked)
            {
                pnlQuestion3b.Enabled = true;
                pnlQuestion3c.Enabled = true;
                ESRDEntitlementPagePresenter.EnableDiasableDialysisCenterNames();
                ESRDEntitlementPagePresenter.SetDialysisCenterNameOnView();
            }

            else if (checkBoxYesNoGroup3.rbNo.Checked)
            {
                mtbQ3DateDialysisBegan.UnMaskedText = string.Empty;
                mtbQ3Training.UnMaskedText = string.Empty;
                pnlQuestion3b.Enabled = false;
                pnlQuestion3c.Enabled = false;
                ESRDEntitlementPagePresenter.EnableDiasableDialysisCenterNames();
                ESRDEntitlementPagePresenter.UpdateDialysisCenterName(String.Empty);
                ESRDEntitlementPagePresenter.SetDialysisCenterNameOnView();
            }

            else
            {
                mtbQ3DateDialysisBegan.UnMaskedText = string.Empty;
                mtbQ3Training.UnMaskedText = string.Empty;
                pnlQuestion3b.Enabled = false;
                pnlQuestion3c.Enabled = false;
                ESRDEntitlementPagePresenter.EnableDiasableDialysisCenterNames();
                ESRDEntitlementPagePresenter.UpdateDialysisCenterName(String.Empty);
            }
            
            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup4_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                pnlQuestion5.Enabled                   = true;
            }      
            else
            {                
                checkBoxYesNoGroup5.rbYes.Checked      = false;
                checkBoxYesNoGroup5.rbNo.Checked       = false;
                pnlQuestion5.Enabled                   = false;
            }

            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup5_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                pnlQuestion6.Enabled                  = true;
            }      
            else
            {                
                checkBoxYesNoGroup6.rbYes.Checked      = false;
                checkBoxYesNoGroup6.rbNo.Checked       = false;
                pnlQuestion6.Enabled                  = false;
            }

            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup6_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbNo"
                && ((RadioButton)sender).Checked )
            {
                pnlQuestion7.Enabled                  = true;
            }      
            else
            {                
                checkBoxYesNoGroup7.rbYes.Checked      = false;
                checkBoxYesNoGroup7.rbNo.Checked       = false;
                pnlQuestion7.Enabled                   = false;
            }

            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup7_RadioChanged(object sender, EventArgs e)
        {            
            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
         
        }

        /// <summary>
        /// ESRDEntitlementPage2_EnabledChanged - update view if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ESRDEntitlementPage2_EnabledChanged(object sender, EventArgs e)
        {
            if( Enabled )
            {
                UpdateView();
            }
        }

        /// <summary>
        /// ESRDEntitlementPage2_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ESRDEntitlementPage2_Load(object sender, EventArgs e)
        {
            ShowLink                           = false;

            MyWizardMessages.Message1          = "Entitlement by ESRD";            
            MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            MyWizardMessages.TextSize1         = 8.25;
            MyWizardMessages.FontStyle1        = FontStyle.Bold;

            MyWizardMessages.Message2          = string.Empty;

            MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            MyWizardMessages.TextSize2         = 8.25;

            MyWizardMessages.ShowMessages();

            pnlQuestion2b.Enabled              = false;
            pnlQuestion3b.Enabled              = false;
            pnlQuestion3c.Enabled              = false;
            pnlQuestion5.Enabled               = false;
            pnlQuestion6.Enabled               = false;
            pnlQuestion7.Enabled               = false;

            pnlInsurance.Enabled               = false;
        }

        /// <summary>
        /// Cancel - is the delegate for the Cancel button click event
        /// </summary>
        private void Cancel()
        {
            MyWizardContainer.Cancel();
            
            if( MSPCancelled != null )
            {
                MSPCancelled(this, null);
            }
        }

        #endregion

        #region Methods
        public void DisablePage()
        {
            Enabled = false;
        }
        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        public override void ResetPage()
        {
            base.ResetPage ();
            ResetSelections();
            checkBoxYesNoGroup3.rbNo.Checked   = false;
            checkBoxYesNoGroup3.rbYes.Checked  = false;
            mtbQ2DateOfTransplant.UnMaskedText     = string.Empty;
            mtbQ3DateDialysisBegan.UnMaskedText    = string.Empty;
            mtbQ3Training.UnMaskedText             = string.Empty;
            ClearDialysisCenterNames();
            pnlQuestion2b.Enabled              = false;
            pnlQuestion3b.Enabled              = false;
            pnlQuestion3c.Enabled              = false;
            pnlQuestion5.Enabled               = false;
            pnlQuestion6.Enabled               = false;
            pnlQuestion7.Enabled               = false;

            pnlInsurance.Enabled               = false;

            UIColors.SetNormalBgColor(mtbQ2DateOfTransplant);
            UIColors.SetNormalBgColor(mtbQ3DateDialysisBegan);
            UIColors.SetNormalBgColor(mtbQ3Training);
            UIColors.SetNormalBgColor(cmbDialysisCenter);
            
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        private bool CheckForSummary()
        {
            bool rc = CanNavigate;

            if( rc )
            {
                HasSummary = true;
                MyWizardButtons.UpdateNavigation("&Continue to Summary", "SummaryPage");
                MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
            }

            HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {           
            bool canNav = false;

            UIColors.SetNormalBgColor( mtbQ2DateOfTransplant );
            UIColors.SetNormalBgColor( mtbQ3DateDialysisBegan );
            UIColors.SetNormalBgColor(cmbDialysisCenter);         

            MyWizardButtons.UpdateNavigation( "&Next >", string.Empty );
            MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if( 
                runRules()
                && 
                (( pnlQuestion2.Enabled && ( checkBoxYesNoGroup2.rbNo.Checked ||
                checkBoxYesNoGroup2.rbYes.Checked )) || !pnlQuestion2.Enabled )
                &&
                (( pnlQuestion3.Enabled && (checkBoxYesNoGroup3.rbNo.Checked ||
                checkBoxYesNoGroup3.rbYes.Checked )) || ! pnlQuestion3.Enabled )
                &&
                ((pnlQuestion4.Enabled && (checkBoxYesNoGroup4.rbNo.Checked ||
                 checkBoxYesNoGroup4.rbYes.Checked)) || ! pnlQuestion4.Enabled )

                && ( ( pnlQuestion5.Enabled && (checkBoxYesNoGroup5.rbNo.Checked 
                ||  checkBoxYesNoGroup5.rbYes.Checked)) || !pnlQuestion5.Enabled)

                && ( (pnlQuestion6.Enabled && (checkBoxYesNoGroup6.rbYes.Checked 
                ||  checkBoxYesNoGroup6.rbNo.Checked)) || !pnlQuestion6.Enabled)

                && ((pnlQuestion7.Enabled && (checkBoxYesNoGroup7.rbNo.Checked
                || checkBoxYesNoGroup7.rbYes.Checked ) ) || !pnlQuestion7.Enabled)
                )
                
            {
                canNav = true;                       
            }        

            CanNavigate = canNav;

            CheckForSummary();
      
            return canNav;
        }

        private void ResetSumamryPage()
        {
            var sumamryPage = this.MyWizardContainer.GetWizardPage("SummaryPage")
                 as SummaryPage;
            if (sumamryPage == null) return;
            sumamryPage.DisablePage();
            this.MyWizardLinks.SetPanel();
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {
            ESRDEntitlementPagePresenter = new ESRDEntitlementPagePresenter(this, Model_Account);
            base.UpdateView();
            ESRDEntitlementPagePresenter.EnablePanels();

            if( !blnLoaded )
            {
                blnLoaded       = true;  
              
                if( Model_Account == null
                    || Model_Account.MedicareSecondaryPayor == null
                    || (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement) == null )
                {
                    return;
                }
                
                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( ESRDEntitlement ) ) )
                {
                    EsrdEntitlement = Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as ESRDEntitlement;

                    if (EsrdEntitlement.KidneyTransplant.Code == YesNoFlag.CODE_YES)
                    {
                        DateTime date = EsrdEntitlement.TransplantDate;
                    
                        if( date != DateTime.MinValue )
                        {
                            EsrdEntitlement.TransplantDate = date;
                            mtbQ2DateOfTransplant.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                        }
                        
                        checkBoxYesNoGroup2.rbYes.Checked              = true;
                    }
                    else if (EsrdEntitlement.KidneyTransplant.Code == YesNoFlag.CODE_NO)
                    {
                        checkBoxYesNoGroup2.rbNo.Checked               = true;
                    }

                    if (EsrdEntitlement.DialysisTreatment.Code == YesNoFlag.CODE_YES)
                    {
                        DateTime dialDate = EsrdEntitlement.DialysisDate;

                        if( dialDate != DateTime.MinValue )
                        {
                            EsrdEntitlement.DialysisDate = dialDate;
                            mtbQ3DateDialysisBegan.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dialDate.Month, dialDate.Day, dialDate.Year );                            
                        }

                        DateTime trainDate = EsrdEntitlement.DialysisTrainingStartDate;

                        if( trainDate != DateTime.MinValue )
                        {
                            EsrdEntitlement.DialysisTrainingStartDate = trainDate;
                            mtbQ3Training.Text = String.Format( "{0:D2}{1:D2}{2:D4}", trainDate.Month, trainDate.Day, trainDate.Year );
                        }
                        
                        checkBoxYesNoGroup3.rbYes.Checked              = true;
                        ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                    }
                    else if (EsrdEntitlement.DialysisTreatment.Code == YesNoFlag.CODE_NO)
                    {
                        checkBoxYesNoGroup3.rbNo.Checked               = true;
                        ESRDEntitlementPagePresenter.HandleDialysisCenterNames();
                    }

                    if (EsrdEntitlement.WithinCoordinationPeriod.Code == YesNoFlag.CODE_YES)
                    {
                        checkBoxYesNoGroup4.rbYes.Checked              = true;

                        if (EsrdEntitlement.BasedOnAgeOrDisability.Code == YesNoFlag.CODE_YES)
                        {
                             checkBoxYesNoGroup5.rbYes.Checked         = true;

                             if (EsrdEntitlement.BasedOnESRD.Code == YesNoFlag.CODE_YES)
                            {
                                checkBoxYesNoGroup6.rbYes.Checked      = true;
                            }
                             else if (EsrdEntitlement.BasedOnESRD.Code == YesNoFlag.CODE_NO)
                            {
                                checkBoxYesNoGroup6.rbNo.Checked       = true;

                                if (EsrdEntitlement.ProvisionAppliesFlag.Code == YesNoFlag.CODE_YES)
                                {
                                    checkBoxYesNoGroup7.rbYes.Checked  = true;
                                }
                                else if (EsrdEntitlement.ProvisionAppliesFlag.Code == YesNoFlag.CODE_NO)
                                {
                                    checkBoxYesNoGroup7.rbNo.Checked   = true;
                                }
                            }
                        }
                        else if (EsrdEntitlement.BasedOnAgeOrDisability.Code == YesNoFlag.CODE_NO)
                        {
                            checkBoxYesNoGroup5.rbNo.Checked           = true;
                        }
                    }
                    else if (EsrdEntitlement.WithinCoordinationPeriod.Code == YesNoFlag.CODE_NO)
                    {
                        checkBoxYesNoGroup4.rbNo.Checked               = true;
                    }
                    
                }
            }

            if (EsrdEntitlementPage1 != null)
            {
                displayInsurance(true);
            }

            CanPageNavigate();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();

             entitlement = null;

            if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                entitlement = new ESRDEntitlement();
            }
            else
            {
                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType()
                    != typeof(ESRDEntitlement) )
                {
                    entitlement = new ESRDEntitlement();
                }
                else
                    entitlement = Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement;
            }

            if( checkBoxYesNoGroup2.rbYes.Checked )
            {
                entitlement.KidneyTransplant.Code = YesNoFlag.CODE_YES;
                if (IsValidDate(mtbQ2DateOfTransplant))
                {
                    entitlement.TransplantDate = DateTime.Parse(mtbQ2DateOfTransplant.Text);
                }
            }
            else if( checkBoxYesNoGroup2.rbNo.Checked )
            {
                entitlement.KidneyTransplant.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.KidneyTransplant.Code = YesNoFlag.CODE_BLANK;
            }

            if( checkBoxYesNoGroup3.rbYes.Checked )
            {
                entitlement.DialysisTreatment.Code = YesNoFlag.CODE_YES;

                if (mtbQ3DateDialysisBegan.UnMaskedText.Trim() != string.Empty && IsValidDate(mtbQ3DateDialysisBegan))
                {
                    entitlement.DialysisDate = DateTime.Parse( mtbQ3DateDialysisBegan.Text );
                }

                if (mtbQ3Training.UnMaskedText.Trim() != string.Empty && IsValidDate(mtbQ3Training))
                {
                    entitlement.DialysisTrainingStartDate = DateTime.Parse(mtbQ3Training.Text);
                }
                else
                {
                    entitlement.DialysisTrainingStartDate = DateTime.MinValue;
                }
                if (cmbDialysisCenter.SelectedItem != null)
                {
                    esrdEntitlementPagePresenter.UpdateDialysisCenterName(
                        cmbDialysisCenter.SelectedItem as string);
                    esrdEntitlementPagePresenter.SaveDialysisCenterName();
                }
            }
            else if( checkBoxYesNoGroup3.rbNo.Checked )
            {
                entitlement.DialysisTreatment.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.DialysisTreatment.Code = YesNoFlag.CODE_BLANK;
            }


            if( checkBoxYesNoGroup4.rbYes.Checked )
            {
                entitlement.WithinCoordinationPeriod.Code = YesNoFlag.CODE_YES;

                if( checkBoxYesNoGroup5.rbYes.Checked )
                {
                    entitlement.BasedOnAgeOrDisability.Code = YesNoFlag.CODE_YES;

                    if( checkBoxYesNoGroup6.rbYes.Checked )
                    {
                        entitlement.BasedOnESRD.Code = YesNoFlag.CODE_YES;
                    }
                    else
                    {
                        if( checkBoxYesNoGroup6.rbNo.Checked )
                        {                        
                            entitlement.BasedOnESRD.Code = YesNoFlag.CODE_NO;

                            if( checkBoxYesNoGroup7.rbYes.Checked )
                            {
                                entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_YES;
                            }
                            else
                            {
                                if( checkBoxYesNoGroup7.rbNo.Checked )
                                {
                                    entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_NO;
                                }
                                else
                                {
                                    entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_BLANK;
                                }
                            }
                        }
                        else
                        {
                            entitlement.BasedOnESRD.Code = YesNoFlag.CODE_BLANK;
                            entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_BLANK;
                        }
                    }
                }
                else if( checkBoxYesNoGroup5.rbNo.Checked )
                {
                    entitlement.BasedOnAgeOrDisability.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    entitlement.BasedOnAgeOrDisability.Code = YesNoFlag.CODE_BLANK;
                }
            }
            else
            {
                if( checkBoxYesNoGroup4.rbNo.Checked )
                {
                    entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_BLANK;
                    entitlement.BasedOnESRD.Code = YesNoFlag.CODE_BLANK;
                    entitlement.BasedOnAgeOrDisability.Code = YesNoFlag.CODE_BLANK;

                    entitlement.WithinCoordinationPeriod.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    entitlement.ProvisionAppliesFlag.Code = YesNoFlag.CODE_BLANK;
                    entitlement.BasedOnESRD.Code = YesNoFlag.CODE_BLANK;
                    entitlement.BasedOnAgeOrDisability.Code = YesNoFlag.CODE_BLANK;

                    entitlement.WithinCoordinationPeriod.Code = YesNoFlag.CODE_BLANK;
                }
            }   

            Model_Account.MedicareSecondaryPayor.MedicareEntitlement = entitlement;
        }
        public bool GHP()
        {
            if(EsrdEntitlementPage1.CheckBoxYesNoGroup1.rbYes.Checked)
            {
                return true;
            }
            return false;
        }
        public void EnablePanels(bool ghp)
        {
            pnlQuestion2.Enabled = ghp;
            pnlQuestion3.Enabled = !ghp;
            pnlQuestion4.Enabled = ghp;
        }

        public void ResetSelections()
        {
            mtbQ2DateOfTransplant.UnMaskedText = string.Empty;
            checkBoxYesNoGroup2.rbYes.Checked = false;
            checkBoxYesNoGroup2.rbNo.Checked = false;
            checkBoxYesNoGroup4.rbYes.Checked = false;
            checkBoxYesNoGroup4.rbNo.Checked = false;
            checkBoxYesNoGroup5.rbYes.Checked = false;
            checkBoxYesNoGroup5.rbNo.Checked = false;
            checkBoxYesNoGroup6.rbNo.Checked = false;
            checkBoxYesNoGroup6.rbYes.Checked = false;
            checkBoxYesNoGroup7.rbNo.Checked = false;
            checkBoxYesNoGroup7.rbYes.Checked = false;
        }

        public void ResetDialysisCenterSelection()
        {
            mtbQ3DateDialysisBegan.UnMaskedText = string.Empty;
            mtbQ3Training.UnMaskedText = string.Empty;
            checkBoxYesNoGroup3.rbYes.Checked = false;
            checkBoxYesNoGroup3.rbNo.Checked = false;
            cmbDialysisCenter.SelectedIndex = -1;
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( Cancel ) );
            MyWizardButtons.AddNavigation( "< &Back", "ESRDEntitlementPage1" );
            MyWizardButtons.AddNavigation( "&Next >", string.Empty );            
            MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            MyWizardButtons.SetPanel();
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
            UIColors.SetRequiredBgColor(cmbDialysisCenter);
        }
        public void SetDialysisCenterNameNormal()
        {
            UIColors.SetNormalBgColor(cmbDialysisCenter);
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
        #endregion

        #region Properties

        private bool IsShortRegAccount
        {
            get
            {
                return Model_Account.IsShortRegistered ||
                       AccountView.IsShortRegAccount;
            }
        }
        private IESRDEntitlementPagePresenter ESRDEntitlementPagePresenter
        {
            get
            {
                return esrdEntitlementPagePresenter;
            }

            set
            {
                esrdEntitlementPagePresenter = value;
            }
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
        public ESRDEntitlement EsrdEntitlement
        {
            get
            {
                return entitlement;
            }

            set
            {
                entitlement = value;
            }
        }

        private ESRDEntitlementPage1 EsrdEntitlementPage1
        {
            get
            {
                return MyWizardContainer.GetWizardPage("ESRDEntitlementPage1")
              as ESRDEntitlementPage1;
            }
        }
        public bool ReceivedMaintenanceDialysisTreatment
        {
            get
            {
                return this.checkBoxYesNoGroup3.rbYes.Checked;
            }
            set
            {
                this.checkBoxYesNoGroup3.rbYes.Checked = value;
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
        /// displayInsurance - display the patient's primary insurance info
        /// </summary>
        /// <param name="display"></param>
        private void displayInsurance( bool display )
        {
            pnlInsurance.Enabled = true;
            if( display )
            {           
                Coverage primaryCoverage = null;
                if (((MSP2Dialog)ParentForm) != null)
                {
                    primaryCoverage = ((MSP2Dialog) ParentForm).GetPrimaryCoverage();
                }
                if( primaryCoverage != null )
                {
                    if( primaryCoverage.Insured != null )
                    {
                        lblPrimaryInsuredText.Text     = primaryCoverage.Insured.FormattedName;
                    }
                    
                    if( primaryCoverage.InsurancePlan != null )
                    {
                        lblPrimaryPayorText.Text       = primaryCoverage.InsurancePlan.Payor.Name;
                    }                    
                }
                Coverage secondaryCoverage = null;
                if (((MSP2Dialog)ParentForm) != null)
                {
                    secondaryCoverage = ((MSP2Dialog) ParentForm).GetSecondaryCoverage();
                }
                if( secondaryCoverage != null )
                {
                    if( secondaryCoverage.Insured != null )
                    {
                        lblSecondaryInsuredText.Text     = secondaryCoverage.Insured.FormattedName;
                    }
                    
                    if( secondaryCoverage.InsurancePlan != null )
                    {
                        lblSecondaryPayorText.Text       = secondaryCoverage.InsurancePlan.Payor.Name;
                    }                    
                }
            }
            else
            {
                lblPrimaryInsuredText.Text             = string.Empty;
                lblPrimaryPayorText.Text               = string.Empty;

                lblSecondaryInsuredText.Text           = string.Empty;
                lblSecondaryPayorText.Text             = string.Empty;
            }                
        }

        /// <summary>
        /// runRules - evaluate any requirements (fields entered, questions answered, etc) for this page;
        /// determines if the page can navigate
        /// </summary>
        /// <returns></returns>
        private bool runRules()
        {
            bool blnRC = true;

            if( mtbQ2DateOfTransplant.Enabled
                && mtbQ2DateOfTransplant.UnMaskedText.Trim().Length != 8 )
            {
                UIColors.SetRequiredBgColor( mtbQ2DateOfTransplant );
                blnRC = false;
            }

            if( mtbQ3DateDialysisBegan.Enabled
                && mtbQ3DateDialysisBegan.UnMaskedText.Trim().Length != 8 )
            {
                UIColors.SetRequiredBgColor( mtbQ3DateDialysisBegan );
                blnRC = false;
            }
            if (cmbDialysisCenter.Enabled && (cmbDialysisCenter == null  || cmbDialysisCenter.SelectedIndex < 1))
            {
                UIColors.SetRequiredBgColor(cmbDialysisCenter);
                blnRC = false;
            }
            return blnRC;

        }
        private void DisableNextAndContinue()
        {
            MyWizardButtons.UpdateNavigation("&Next >", string.Empty);
            MyWizardButtons.UpdateNavigation("&Continue to Summary", string.Empty);
        }
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ESRDEntitlementPage2));
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlQuestion2 = new System.Windows.Forms.Panel();
            this.pnlDividerQ1 = new System.Windows.Forms.Panel();
            this.pnlQuestion2b = new System.Windows.Forms.Panel();
            this.lblQ2DateOfTransplant = new System.Windows.Forms.Label();
            this.mtbQ2DateOfTransplant = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.checkBoxYesNoGroup2 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.pnlQuestion3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbDialysisCenter = new System.Windows.Forms.ComboBox();
            this.lblDialysisCentername = new System.Windows.Forms.Label();
            this.pnlQuestion3c = new System.Windows.Forms.Panel();
            this.lblQ3Training = new System.Windows.Forms.Label();
            this.mtbQ3Training = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.pnlDividerQ3 = new System.Windows.Forms.Panel();
            this.pnlQuestion3b = new System.Windows.Forms.Panel();
            this.lblQ3DateDialysisBegan = new System.Windows.Forms.Label();
            this.mtbQ3DateDialysisBegan = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.checkBoxYesNoGroup3 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.pnlQuestion5 = new System.Windows.Forms.Panel();
            this.pnlDividerQ5 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup5 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.pnlQuestion6 = new System.Windows.Forms.Panel();
            this.lblQuestion6b = new System.Windows.Forms.Label();
            this.pnlDividerQ6 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup6 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion6 = new System.Windows.Forms.Label();
            this.pnlQuestion7 = new System.Windows.Forms.Panel();
            this.lblQuestion7b = new System.Windows.Forms.Label();
            this.checkBoxYesNoGroup7 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion7 = new System.Windows.Forms.Label();
            this.pnlDividerQ4 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup4 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion4 = new System.Windows.Forms.Label();
            this.pnlQuestion4 = new System.Windows.Forms.Panel();
            this.btnQ4MoreInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pnlInsurance = new System.Windows.Forms.Panel();
            this.btnInsEditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblSecondaryInsuredText = new System.Windows.Forms.Label();
            this.lblSecondaryPayorText = new System.Windows.Forms.Label();
            this.lblPrimaryInsuredText = new System.Windows.Forms.Label();
            this.lblPrimaryPayorText = new System.Windows.Forms.Label();
            this.lblSecondaryInsured = new System.Windows.Forms.Label();
            this.lblSecondaryPayor = new System.Windows.Forms.Label();
            this.lblPrimaryInsured = new System.Windows.Forms.Label();
            this.lblPrimaryPayor = new System.Windows.Forms.Label();
            this.pnlInsuranceDivider = new System.Windows.Forms.Panel();
            this.pnlWizardPageBody.SuspendLayout();
            this.pnlQuestion2.SuspendLayout();
            this.pnlQuestion2b.SuspendLayout();
            this.pnlQuestion3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlQuestion3c.SuspendLayout();
            this.pnlQuestion3b.SuspendLayout();
            this.pnlQuestion5.SuspendLayout();
            this.pnlQuestion6.SuspendLayout();
            this.pnlQuestion7.SuspendLayout();
            this.pnlQuestion4.SuspendLayout();
            this.pnlInsurance.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.pnlInsurance);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion7);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion6);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion5);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion4);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion3);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion2);
            this.pnlWizardPageBody.Controls.Add(this.pnlDivider1);
            this.pnlWizardPageBody.Size = new System.Drawing.Size(788, 559);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlDivider1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion2, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion3, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion4, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion5, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion6, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion7, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlInsurance, 0);
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point(7, 55);
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size(668, 2);
            this.pnlDivider1.TabIndex = 8;
            // 
            // pnlQuestion2
            // 
            this.pnlQuestion2.Controls.Add(this.pnlDividerQ1);
            this.pnlQuestion2.Controls.Add(this.pnlQuestion2b);
            this.pnlQuestion2.Controls.Add(this.checkBoxYesNoGroup2);
            this.pnlQuestion2.Controls.Add(this.lblQuestion2);
            this.pnlQuestion2.Location = new System.Drawing.Point(8, 63);
            this.pnlQuestion2.Name = "pnlQuestion2";
            this.pnlQuestion2.Size = new System.Drawing.Size(764, 64);
            this.pnlQuestion2.TabIndex = 9;
            this.pnlQuestion2.TabStop = true;
            // 
            // pnlDividerQ1
            // 
            this.pnlDividerQ1.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ1.Location = new System.Drawing.Point(5, 57);
            this.pnlDividerQ1.Name = "pnlDividerQ1";
            this.pnlDividerQ1.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ1.TabIndex = 5;
            // 
            // pnlQuestion2b
            // 
            this.pnlQuestion2b.Controls.Add(this.lblQ2DateOfTransplant);
            this.pnlQuestion2b.Controls.Add(this.mtbQ2DateOfTransplant);
            this.pnlQuestion2b.Location = new System.Drawing.Point(11, 22);
            this.pnlQuestion2b.Name = "pnlQuestion2b";
            this.pnlQuestion2b.Size = new System.Drawing.Size(287, 28);
            this.pnlQuestion2b.TabIndex = 4;
            // 
            // lblQ2DateOfTransplant
            // 
            this.lblQ2DateOfTransplant.Location = new System.Drawing.Point(13, 7);
            this.lblQ2DateOfTransplant.Name = "lblQ2DateOfTransplant";
            this.lblQ2DateOfTransplant.Size = new System.Drawing.Size(98, 14);
            this.lblQ2DateOfTransplant.TabIndex = 2;
            this.lblQ2DateOfTransplant.Text = "Date of transplant:";
            // 
            // mtbQ2DateOfTransplant
            // 
            this.mtbQ2DateOfTransplant.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2DateOfTransplant.KeyPressExpression = "^\\d*$";
            this.mtbQ2DateOfTransplant.Location = new System.Drawing.Point(197, 5);
            this.mtbQ2DateOfTransplant.Mask = "  /  /";
            this.mtbQ2DateOfTransplant.MaxLength = 10;
            this.mtbQ2DateOfTransplant.Name = "mtbQ2DateOfTransplant";
            this.mtbQ2DateOfTransplant.Size = new System.Drawing.Size(70, 20);
            this.mtbQ2DateOfTransplant.TabIndex = 1;
            this.mtbQ2DateOfTransplant.ValidationExpression = resources.GetString("mtbQ2DateOfTransplant.ValidationExpression");
            this.mtbQ2DateOfTransplant.Leave += new System.EventHandler(this.mtbQ2DateOfTransplant_Leave);
            // 
            // checkBoxYesNoGroup2
            // 
            this.checkBoxYesNoGroup2.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup2.Name = "checkBoxYesNoGroup2";
            this.checkBoxYesNoGroup2.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup2.TabIndex = 2;
            this.checkBoxYesNoGroup2.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup2_RadioChanged);
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(432, 15);
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2.  Have you received a kidney transplant?";
            // 
            // pnlQuestion3
            // 
            this.pnlQuestion3.Controls.Add(this.panel1);
            this.pnlQuestion3.Controls.Add(this.pnlQuestion3c);
            this.pnlQuestion3.Controls.Add(this.pnlDividerQ3);
            this.pnlQuestion3.Controls.Add(this.pnlQuestion3b);
            this.pnlQuestion3.Controls.Add(this.checkBoxYesNoGroup3);
            this.pnlQuestion3.Controls.Add(this.lblQuestion3);
            this.pnlQuestion3.Location = new System.Drawing.Point(8, 128);
            this.pnlQuestion3.Name = "pnlQuestion3";
            this.pnlQuestion3.Size = new System.Drawing.Size(764, 112);
            this.pnlQuestion3.TabIndex = 10;
            this.pnlQuestion3.TabStop = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbDialysisCenter);
            this.panel1.Controls.Add(this.lblDialysisCentername);
            this.panel1.Location = new System.Drawing.Point(12, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 31);
            this.panel1.TabIndex = 8;
            // 
            // cmbDialysisCenter
            // 
            this.cmbDialysisCenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDialysisCenter.Enabled = false;
            this.cmbDialysisCenter.Location = new System.Drawing.Point(406, 5);
            this.cmbDialysisCenter.MaxLength = 40;
            this.cmbDialysisCenter.Name = "cmbDialysisCenter";
            this.cmbDialysisCenter.Size = new System.Drawing.Size(275, 21);
            this.cmbDialysisCenter.TabIndex = 5;
            this.cmbDialysisCenter.SelectedIndexChanged += new System.EventHandler(this.cmbDialysisCenter_SelectedIndexChanged);
            this.cmbDialysisCenter.Validating += new System.ComponentModel.CancelEventHandler(this.cmbDialysisCenter__Validating);
            // 
            // lblDialysisCentername
            // 
            this.lblDialysisCentername.Enabled = false;
            this.lblDialysisCentername.Location = new System.Drawing.Point(6, 7);
            this.lblDialysisCentername.Name = "lblDialysisCentername";
            this.lblDialysisCentername.Size = new System.Drawing.Size(230, 22);
            this.lblDialysisCentername.TabIndex = 2;
            this.lblDialysisCentername.Text = "Name of Dialysis Center:";
            // 
            // pnlQuestion3c
            // 
            this.pnlQuestion3c.Controls.Add(this.lblQ3Training);
            this.pnlQuestion3c.Controls.Add(this.mtbQ3Training);
            this.pnlQuestion3c.Location = new System.Drawing.Point(12, 50);
            this.pnlQuestion3c.Name = "pnlQuestion3c";
            this.pnlQuestion3c.Size = new System.Drawing.Size(764, 31);
            this.pnlQuestion3c.TabIndex = 7;
            // 
            // lblQ3Training
            // 
            this.lblQ3Training.Location = new System.Drawing.Point(6, 7);
            this.lblQ3Training.Name = "lblQ3Training";
            this.lblQ3Training.Size = new System.Drawing.Size(402, 22);
            this.lblQ3Training.TabIndex = 2;
            this.lblQ3Training.Text = "If you participated in a self-dialysis training program, provide the date trainin" +
                "g started:";
            // 
            // mtbQ3Training
            // 
            this.mtbQ3Training.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ3Training.KeyPressExpression = "^\\d*$";
            this.mtbQ3Training.Location = new System.Drawing.Point(408, 4);
            this.mtbQ3Training.Mask = "  /  /";
            this.mtbQ3Training.MaxLength = 10;
            this.mtbQ3Training.Name = "mtbQ3Training";
            this.mtbQ3Training.Size = new System.Drawing.Size(70, 20);
            this.mtbQ3Training.TabIndex = 1;
            this.mtbQ3Training.ValidationExpression = resources.GetString("mtbQ3Training.ValidationExpression");
            this.mtbQ3Training.Leave += new System.EventHandler(this.mtbQ3Training_Leave);
            // 
            // pnlDividerQ3
            // 
            this.pnlDividerQ3.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ3.Location = new System.Drawing.Point(5, 108);
            this.pnlDividerQ3.Name = "pnlDividerQ3";
            this.pnlDividerQ3.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ3.TabIndex = 5;
            // 
            // pnlQuestion3b
            // 
            this.pnlQuestion3b.Controls.Add(this.lblQ3DateDialysisBegan);
            this.pnlQuestion3b.Controls.Add(this.mtbQ3DateDialysisBegan);
            this.pnlQuestion3b.Location = new System.Drawing.Point(11, 26);
            this.pnlQuestion3b.Name = "pnlQuestion3b";
            this.pnlQuestion3b.Size = new System.Drawing.Size(764, 24);
            this.pnlQuestion3b.TabIndex = 4;
            // 
            // lblQ3DateDialysisBegan
            // 
            this.lblQ3DateDialysisBegan.Location = new System.Drawing.Point(6, 7);
            this.lblQ3DateDialysisBegan.Name = "lblQ3DateDialysisBegan";
            this.lblQ3DateDialysisBegan.Size = new System.Drawing.Size(106, 14);
            this.lblQ3DateDialysisBegan.TabIndex = 2;
            this.lblQ3DateDialysisBegan.Text = "Date dialysis began:";
            // 
            // mtbQ3DateDialysisBegan
            // 
            this.mtbQ3DateDialysisBegan.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ3DateDialysisBegan.KeyPressExpression = "^\\d*$";
            this.mtbQ3DateDialysisBegan.Location = new System.Drawing.Point(408, 4);
            this.mtbQ3DateDialysisBegan.Mask = "  /  /";
            this.mtbQ3DateDialysisBegan.MaxLength = 10;
            this.mtbQ3DateDialysisBegan.Name = "mtbQ3DateDialysisBegan";
            this.mtbQ3DateDialysisBegan.Size = new System.Drawing.Size(70, 20);
            this.mtbQ3DateDialysisBegan.TabIndex = 1;
            this.mtbQ3DateDialysisBegan.ValidationExpression = resources.GetString("mtbQ3DateDialysisBegan.ValidationExpression");
            this.mtbQ3DateDialysisBegan.Leave += new System.EventHandler(this.mtbQ3DateDialysisBegan_Leave);
            // 
            // checkBoxYesNoGroup3
            // 
            this.checkBoxYesNoGroup3.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup3.Name = "checkBoxYesNoGroup3";
            this.checkBoxYesNoGroup3.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup3.TabIndex = 2;
            this.checkBoxYesNoGroup3.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup3_RadioChanged);
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(432, 15);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3.  Have you received maintenance dialysis treatments?";
            // 
            // pnlQuestion5
            // 
            this.pnlQuestion5.Controls.Add(this.pnlDividerQ5);
            this.pnlQuestion5.Controls.Add(this.checkBoxYesNoGroup5);
            this.pnlQuestion5.Controls.Add(this.lblQuestion5);
            this.pnlQuestion5.Location = new System.Drawing.Point(8, 290);
            this.pnlQuestion5.Name = "pnlQuestion5";
            this.pnlQuestion5.Size = new System.Drawing.Size(764, 39);
            this.pnlQuestion5.TabIndex = 12;
            this.pnlQuestion5.TabStop = true;
            // 
            // pnlDividerQ5
            // 
            this.pnlDividerQ5.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ5.Location = new System.Drawing.Point(5, 35);
            this.pnlDividerQ5.Name = "pnlDividerQ5";
            this.pnlDividerQ5.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ5.TabIndex = 5;
            // 
            // checkBoxYesNoGroup5
            // 
            this.checkBoxYesNoGroup5.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup5.Name = "checkBoxYesNoGroup5";
            this.checkBoxYesNoGroup5.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup5.TabIndex = 2;
            this.checkBoxYesNoGroup5.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup5_RadioChanged);
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(483, 15);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5.  Are you entitled to Medicare on the basis of either ESRD and age or ESRD and " +
                "disability?";
            // 
            // pnlQuestion6
            // 
            this.pnlQuestion6.Controls.Add(this.lblQuestion6b);
            this.pnlQuestion6.Controls.Add(this.pnlDividerQ6);
            this.pnlQuestion6.Controls.Add(this.checkBoxYesNoGroup6);
            this.pnlQuestion6.Controls.Add(this.lblQuestion6);
            this.pnlQuestion6.Location = new System.Drawing.Point(8, 335);
            this.pnlQuestion6.Name = "pnlQuestion6";
            this.pnlQuestion6.Size = new System.Drawing.Size(764, 49);
            this.pnlQuestion6.TabIndex = 13;
            this.pnlQuestion6.TabStop = true;
            // 
            // lblQuestion6b
            // 
            this.lblQuestion6b.Location = new System.Drawing.Point(28, 23);
            this.lblQuestion6b.Name = "lblQuestion6b";
            this.lblQuestion6b.Size = new System.Drawing.Size(446, 15);
            this.lblQuestion6b.TabIndex = 6;
            this.lblQuestion6b.Text = "based on ESRD?";
            // 
            // pnlDividerQ6
            // 
            this.pnlDividerQ6.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ6.Location = new System.Drawing.Point(5, 45);
            this.pnlDividerQ6.Name = "pnlDividerQ6";
            this.pnlDividerQ6.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ6.TabIndex = 5;
            // 
            // checkBoxYesNoGroup6
            // 
            this.checkBoxYesNoGroup6.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup6.Name = "checkBoxYesNoGroup6";
            this.checkBoxYesNoGroup6.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup6.TabIndex = 2;
            this.checkBoxYesNoGroup6.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup6_RadioChanged);
            // 
            // lblQuestion6
            // 
            this.lblQuestion6.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion6.Name = "lblQuestion6";
            this.lblQuestion6.Size = new System.Drawing.Size(446, 15);
            this.lblQuestion6.TabIndex = 0;
            this.lblQuestion6.Text = "6.  Was your initial entitlement to Medicare (including simultaneous or dual enti" +
                "tlement) ";
            // 
            // pnlQuestion7
            // 
            this.pnlQuestion7.Controls.Add(this.lblQuestion7b);
            this.pnlQuestion7.Controls.Add(this.checkBoxYesNoGroup7);
            this.pnlQuestion7.Controls.Add(this.lblQuestion7);
            this.pnlQuestion7.Location = new System.Drawing.Point(8, 390);
            this.pnlQuestion7.Name = "pnlQuestion7";
            this.pnlQuestion7.Size = new System.Drawing.Size(764, 49);
            this.pnlQuestion7.TabIndex = 14;
            this.pnlQuestion7.TabStop = true;
            // 
            // lblQuestion7b
            // 
            this.lblQuestion7b.Location = new System.Drawing.Point(25, 24);
            this.lblQuestion7b.Name = "lblQuestion7b";
            this.lblQuestion7b.Size = new System.Drawing.Size(432, 15);
            this.lblQuestion7b.TabIndex = 6;
            this.lblQuestion7b.Text = "(i.e., is the GHP already primary based on age or disability entitlement)?";
            // 
            // checkBoxYesNoGroup7
            // 
            this.checkBoxYesNoGroup7.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup7.Name = "checkBoxYesNoGroup7";
            this.checkBoxYesNoGroup7.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup7.TabIndex = 2;
            this.checkBoxYesNoGroup7.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup7_RadioChanged);
            // 
            // lblQuestion7
            // 
            this.lblQuestion7.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion7.Name = "lblQuestion7";
            this.lblQuestion7.Size = new System.Drawing.Size(432, 15);
            this.lblQuestion7.TabIndex = 0;
            this.lblQuestion7.Text = "7.  Does the working aged or disability MSP provision apply";
            // 
            // pnlDividerQ4
            // 
            this.pnlDividerQ4.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ4.Location = new System.Drawing.Point(5, 35);
            this.pnlDividerQ4.Name = "pnlDividerQ4";
            this.pnlDividerQ4.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ4.TabIndex = 5;
            // 
            // checkBoxYesNoGroup4
            // 
            this.checkBoxYesNoGroup4.BackColor = System.Drawing.Color.White;
            this.checkBoxYesNoGroup4.Location = new System.Drawing.Point(532, 0);
            this.checkBoxYesNoGroup4.Name = "checkBoxYesNoGroup4";
            this.checkBoxYesNoGroup4.Size = new System.Drawing.Size(764, 35);
            this.checkBoxYesNoGroup4.TabIndex = 3;
            this.checkBoxYesNoGroup4.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup4_RadioChanged);
            // 
            // lblQuestion4
            // 
            this.lblQuestion4.BackColor = System.Drawing.Color.White;
            this.lblQuestion4.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion4.Name = "lblQuestion4";
            this.lblQuestion4.Size = new System.Drawing.Size(269, 15);
            this.lblQuestion4.TabIndex = 0;
            this.lblQuestion4.Text = "4.  Are you within the 30-month coordination period?";
            // 
            // pnlQuestion4
            // 
            this.pnlQuestion4.BackColor = System.Drawing.Color.White;
            this.pnlQuestion4.Controls.Add(this.btnQ4MoreInfo);
            this.pnlQuestion4.Controls.Add(this.pnlDividerQ4);
            this.pnlQuestion4.Controls.Add(this.checkBoxYesNoGroup4);
            this.pnlQuestion4.Controls.Add(this.lblQuestion4);
            this.pnlQuestion4.Location = new System.Drawing.Point(8, 245);
            this.pnlQuestion4.Name = "pnlQuestion4";
            this.pnlQuestion4.Size = new System.Drawing.Size(666, 39);
            this.pnlQuestion4.TabIndex = 11;
            this.pnlQuestion4.TabStop = true;
            // 
            // btnQ4MoreInfo
            // 
            this.btnQ4MoreInfo.Location = new System.Drawing.Point(279, 4);
            this.btnQ4MoreInfo.Message = null;
            this.btnQ4MoreInfo.Name = "btnQ4MoreInfo";
            this.btnQ4MoreInfo.Size = new System.Drawing.Size(75, 23);
            this.btnQ4MoreInfo.TabIndex = 2;
            this.btnQ4MoreInfo.Text = "More info";
            this.btnQ4MoreInfo.Click += new System.EventHandler(this.btnQ4MoreInfo_Click);
            // 
            // pnlInsurance
            // 
            this.pnlInsurance.Controls.Add(this.btnInsEditInsurance);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsured);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayor);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsured);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayor);
            this.pnlInsurance.Controls.Add(this.pnlInsuranceDivider);
            this.pnlInsurance.Location = new System.Drawing.Point(8, 440);
            this.pnlInsurance.Name = "pnlInsurance";
            this.pnlInsurance.Size = new System.Drawing.Size(764, 116);
            this.pnlInsurance.TabIndex = 15;
            // 
            // btnInsEditInsurance
            // 
            this.btnInsEditInsurance.Location = new System.Drawing.Point(465, 25);
            this.btnInsEditInsurance.Message = null;
            this.btnInsEditInsurance.Name = "btnInsEditInsurance";
            this.btnInsEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnInsEditInsurance.TabIndex = 13;
            this.btnInsEditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnInsEditInsurance.Click += new System.EventHandler(this.btnInsEditInsurance_Click);
            // 
            // lblSecondaryInsuredText
            // 
            this.lblSecondaryInsuredText.Location = new System.Drawing.Point(104, 92);
            this.lblSecondaryInsuredText.Name = "lblSecondaryInsuredText";
            this.lblSecondaryInsuredText.Size = new System.Drawing.Size(344, 13);
            this.lblSecondaryInsuredText.TabIndex = 12;
            // 
            // lblSecondaryPayorText
            // 
            this.lblSecondaryPayorText.Location = new System.Drawing.Point(104, 73);
            this.lblSecondaryPayorText.Name = "lblSecondaryPayorText";
            this.lblSecondaryPayorText.Size = new System.Drawing.Size(344, 13);
            this.lblSecondaryPayorText.TabIndex = 11;
            // 
            // lblPrimaryInsuredText
            // 
            this.lblPrimaryInsuredText.Location = new System.Drawing.Point(104, 44);
            this.lblPrimaryInsuredText.Name = "lblPrimaryInsuredText";
            this.lblPrimaryInsuredText.Size = new System.Drawing.Size(344, 13);
            this.lblPrimaryInsuredText.TabIndex = 10;
            // 
            // lblPrimaryPayorText
            // 
            this.lblPrimaryPayorText.Location = new System.Drawing.Point(104, 26);
            this.lblPrimaryPayorText.Name = "lblPrimaryPayorText";
            this.lblPrimaryPayorText.Size = new System.Drawing.Size(344, 13);
            this.lblPrimaryPayorText.TabIndex = 9;
            // 
            // lblSecondaryInsured
            // 
            this.lblSecondaryInsured.Location = new System.Drawing.Point(56, 92);
            this.lblSecondaryInsured.Name = "lblSecondaryInsured";
            this.lblSecondaryInsured.Size = new System.Drawing.Size(45, 13);
            this.lblSecondaryInsured.TabIndex = 8;
            this.lblSecondaryInsured.Text = "Insured:";
            // 
            // lblSecondaryPayor
            // 
            this.lblSecondaryPayor.Location = new System.Drawing.Point(10, 73);
            this.lblSecondaryPayor.Name = "lblSecondaryPayor";
            this.lblSecondaryPayor.Size = new System.Drawing.Size(95, 13);
            this.lblSecondaryPayor.TabIndex = 7;
            this.lblSecondaryPayor.Text = "Secondary Payor:";
            // 
            // lblPrimaryInsured
            // 
            this.lblPrimaryInsured.Location = new System.Drawing.Point(56, 44);
            this.lblPrimaryInsured.Name = "lblPrimaryInsured";
            this.lblPrimaryInsured.Size = new System.Drawing.Size(45, 13);
            this.lblPrimaryInsured.TabIndex = 6;
            this.lblPrimaryInsured.Text = "Insured:";
            // 
            // lblPrimaryPayor
            // 
            this.lblPrimaryPayor.Location = new System.Drawing.Point(26, 26);
            this.lblPrimaryPayor.Name = "lblPrimaryPayor";
            this.lblPrimaryPayor.Size = new System.Drawing.Size(81, 13);
            this.lblPrimaryPayor.TabIndex = 5;
            this.lblPrimaryPayor.Text = "Primary Payor:";
            // 
            // pnlInsuranceDivider
            // 
            this.pnlInsuranceDivider.BackColor = System.Drawing.Color.Black;
            this.pnlInsuranceDivider.Location = new System.Drawing.Point(5, 11);
            this.pnlInsuranceDivider.Name = "pnlInsuranceDivider";
            this.pnlInsuranceDivider.Size = new System.Drawing.Size(656, 2);
            this.pnlInsuranceDivider.TabIndex = 4;
            // 
            // ESRDEntitlementPage2
            // 
            this.Name = "ESRDEntitlementPage2";
            this.Size = new System.Drawing.Size(815, 650);
            this.Load += new System.EventHandler(this.ESRDEntitlementPage2_Load);
            this.EnabledChanged += new System.EventHandler(this.ESRDEntitlementPage2_EnabledChanged);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.pnlQuestion2.ResumeLayout(false);
            this.pnlQuestion2b.ResumeLayout(false);
            this.pnlQuestion2b.PerformLayout();
            this.pnlQuestion3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pnlQuestion3c.ResumeLayout(false);
            this.pnlQuestion3c.PerformLayout();
            this.pnlQuestion3b.ResumeLayout(false);
            this.pnlQuestion3b.PerformLayout();
            this.pnlQuestion5.ResumeLayout(false);
            this.pnlQuestion6.ResumeLayout(false);
            this.pnlQuestion7.ResumeLayout(false);
            this.pnlQuestion4.ResumeLayout(false);
            this.pnlInsurance.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        private ESRDEntitlementPage2()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public ESRDEntitlementPage2( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public ESRDEntitlementPage2( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public ESRDEntitlementPage2( string pageName, WizardContainer wizardContainer, Account anAccount )
            : base( pageName, wizardContainer, anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements
        
        private IContainer                            components = null;

        private Panel                                  pnlDivider1;
        private Panel                                  pnlDividerQ1;
        private Panel                                  pnlQuestion2;                
        private Panel                                  pnlQuestion2b; 
        private Panel                                  pnlQuestion3;
        private Panel                                  pnlQuestion3b;
        private Panel                                  pnlQuestion3c;
        private Panel                                  pnlDividerQ3;
        private Panel                                  pnlQuestion4;
        private Panel                                  pnlDividerQ4;
        private Panel                                  pnlQuestion5;
        private Panel                                  pnlDividerQ5;
        private Panel                                  pnlQuestion6;
        private Panel                                  pnlDividerQ6;
        private Panel                                  pnlQuestion7;     
        private Panel                                  pnlInsurance;
        private Panel                                  pnlInsuranceDivider;
        
        private CheckBoxYesNoGroup     checkBoxYesNoGroup2;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup3;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup4;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup5;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup6;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup7;

        private Label                                  lblQuestion2;
        private Label                                  lblQ2DateOfTransplant;
        private Label                                  lblQuestion3;       
        private Label                                  lblQ3DateDialysisBegan;
        private Label                                  lblQ3Training;
        private Label                                  lblQuestion4;
        private Label                                  lblQuestion5;
        private Label                                  lblQuestion6;
        private Label                                  lblQuestion6b;               
        private Label                                  lblQuestion7;
        private Label                                  lblQuestion7b;       
        private Label                                  lblSecondaryInsuredText;
        private Label                                  lblSecondaryPayorText;
        private Label                                  lblPrimaryInsuredText;
        private Label                                  lblPrimaryPayorText;
        private Label                                  lblSecondaryInsured;
        private Label                                  lblSecondaryPayor;
        private Label                                  lblPrimaryInsured;
        private Label                                  lblPrimaryPayor;
        
        private MaskedEditTextBox                    mtbQ2DateOfTransplant;
        private MaskedEditTextBox                    mtbQ3DateDialysisBegan;
        private MaskedEditTextBox                    mtbQ3Training;
        
        private LoggingButton               btnQ4MoreInfo;             
        private LoggingButton               btnInsEditInsurance;
        private Panel panel1;
        private Label lblDialysisCentername;
        private ComboBox cmbDialysisCenter;
        private string selectedDialysisCenter = string.Empty;

        private bool                                                        blnLoaded = false;
        private IESRDEntitlementPagePresenter esrdEntitlementPagePresenter;
        private ESRDEntitlement entitlement;

        #endregion

        private void cmbDialysisCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(cmbDialysisCenter);
            string selectedDialysisCenter = cmbDialysisCenter.SelectedItem as string;
            if (selectedDialysisCenter != null)
            {
                esrdEntitlementPagePresenter.UpdateDialysisCenterName(selectedDialysisCenter);
            }
            else
            {
                esrdEntitlementPagePresenter.UpdateDialysisCenterName(string.Empty);
            }
            
            CanPageNavigate();
        }

        private void cmbDialysisCenter__Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(cmbDialysisCenter);
            string selectedDialysisCenter = cmbDialysisCenter.SelectedItem as string;
            if (selectedDialysisCenter != null)
            {
                esrdEntitlementPagePresenter.UpdateDialysisCenterName(selectedDialysisCenter);
            }
            CanPageNavigate();
            if (!CanNavigate)
            {
                ResetSumamryPage();
            }
        }

        #region Constants
        #endregion

    }
}

