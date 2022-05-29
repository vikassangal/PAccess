using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.TransferViews;

namespace PatientAccess.UI.BillingViews
{
    /// <summary>
    /// Summary description for BillingView.
    /// </summary>
    public class BillingView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void BillingView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    if ( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model ) );
                    }
                }
            }
        }

        private void BillingView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            UpdateModelOCCs();
            SortOCCCodes();
            RuleEngine.GetInstance().EvaluateRule( typeof( OnBillingForm ), Model );
            blnLeaveRun = false;
        }

        private void BillingView_Disposed( object sender, EventArgs e )
        {
            UnRegisterRulesEvents();
        }

        private void cboOccCode2_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode2 );
        }

        private void cboOccCode3_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode3 );
        }

        private void cboOccCode4_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode4 );
        }

        private void cboOccCode5_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode5 );
        }

        private void cboOccCode6_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode6 );
        }

        private void cboOccCode7_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode7 );
        }

        private void cboOccCode8_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleOCCIndexChanged( cboOccCode8 );
        }

        private void dtppOcc2Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode2, mtbOcc2Date, dtppOcc2Date );
        }

        private void dtppOcc3Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode3, mtbOcc3Date, dtppOcc3Date );
        }

        private void dtppOcc4Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode4, mtbOcc4Date, dtppOcc4Date );
        }

        private void dtppOcc5Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode5, mtbOcc5Date, dtppOcc5Date );
        }

        private void dtppOcc6Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode6, mtbOcc6Date, dtppOcc6Date );
        }

        private void dtppOcc7Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode7, mtbOcc7Date, dtppOcc7Date );
        }

        private void dtppOcc8Date_CloseUp( object sender, EventArgs e )
        {
            UpdateOCCDate( cboOccCode8, mtbOcc8Date, dtppOcc8Date );
        }

        private void mtbOcc2Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode2, mtbOcc2Date, dtppOcc2Date );
        }

        private void mtbOcc3Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode3, mtbOcc3Date, dtppOcc3Date );
        }

        private void mtbOcc4Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode4, mtbOcc4Date, dtppOcc4Date );
        }

        private void mtbOcc5Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode5, mtbOcc5Date, dtppOcc5Date );
        }

        private void mtbOcc6Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode6, mtbOcc6Date, dtppOcc6Date );
        }

        private void mtbOcc7Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode7, mtbOcc7Date, dtppOcc7Date );
        }

        private void mtbOcc8Date_Validating( object sender, CancelEventArgs e )
        {
            HandleOCCDateLeave( cboOccCode8, mtbOcc8Date, dtppOcc8Date );
        }

        private void cboOccCode2_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode2;
        }

        private void cboOccCode3_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode3;
        }

        private void cboOccCode4_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode4;
        }

        private void cboOccCode5_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode5;
        }

        private void cboOccCode6_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode6;
        }

        private void cboOccCode7_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode7;
        }

        private void cboOccCode8_Enter( object sender, EventArgs e )
        {
            occCboMgr.currentCombo = cboOccCode8;
        }

        private void cboCond1_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond1;
        }

        private void cboCond2_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond2;
        }

        private void cboCond3_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond3;
        }

        private void cboCond4_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond4;
        }

        private void cboCond5_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond5;
        }

        private void cboCond6_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond6;
        }

        private void cboCond7_Enter( object sender, EventArgs e )
        {
            condCboMgr.currentCombo = cboCond7;
        }

        private void cboCond1_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond1 );
        }

        private void cboCond2_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond2 );
        }

        private void cboCond3_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond3 );
        }

        private void cboCond4_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond4 );
        }

        private void cboCond5_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond5 );
        }

        private void cboCond6_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond6 );
        }

        private void cboCond7_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleConditionIndexChanged( cboCond7 );
        }

        private void OccurrenceCode1DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblOccCode1DateVal );
        }

        private void OccurrenceCode2DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc2Date );
        }

        private void OccurrenceCode3DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc3Date );
        }

        private void OccurrenceCode4DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc4Date );
        }

        private void OccurrenceCode5DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc5Date );
        }

        private void OccurrenceCode6DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc6Date );
        }

        private void OccurrenceCode7DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc7Date );
        }

        private void OccurrenceCode8DateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbOcc8Date );
        }

        private void ConditionCode1RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboCond1 );
        }

        private void Span1FromDateRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cboSpan1.Enabled )
            {
                dtppSpan1From.Enabled = true;
                mtbSpan1FromDate.Enabled = true;

                UIColors.SetRequiredBgColor( mtbSpan1FromDate );
            }
        }

        private void Span1ToDateRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cboSpan1.Enabled )
            {
                dtppSpan1To.Enabled = true;
                mtbSpan1ToDate.Enabled = true;

                UIColors.SetRequiredBgColor( mtbSpan1ToDate );
            }
        }

        private void Span2FromDateRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cboSpan2.Enabled )
            {
                dtppSpan2From.Enabled = true;
                mtbSpan2FromDate.Enabled = true;

                UIColors.SetRequiredBgColor( mtbSpan2FromDate );
            }
        }

        private void Span2ToDateRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cboSpan2.Enabled )
            {
                dtppSpan2To.Enabled = true;
                mtbSpan2ToDate.Enabled = true;

                UIColors.SetRequiredBgColor( mtbSpan2ToDate );
            }
        }

        private void dtppSpan2To_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2ToDate );
            DateTime dt = dtppSpan2To.Value;
            mtbSpan2ToDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan2ToDate.Focus();
        }

        private void dtppSpan1To_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1ToDate );
            DateTime dt = dtppSpan1To.Value;
            mtbSpan1ToDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan1ToDate.Focus();
        }

        private void dtppSpan1From_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1FromDate );
            DateTime dt = dtppSpan1From.Value;
            mtbSpan1FromDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan1FromDate.Focus();
        }

        private void dtppSpan2From_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2FromDate );
            DateTime dt = dtppSpan2From.Value;
            mtbSpan2FromDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan2FromDate.Focus();
        }

        private void cboSpan1_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateSpan1InModel();
            if ( ( ( SpanCode )cboSpan1.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                SetSpan1ControlsDisableBGColor( false );
                SetSpan1ControlsEnableStatus( true );

                RunRequiredRules();
            }
            else
            {
                SetSpan1ControlsEnableStatus( false );
                SetSpan1ControlsDisableBGColor( true );
            }
        }

        private void cboSpan2_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateSpan2InModel();
            if ( ( ( SpanCode )cboSpan2.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                SetSpan2ControlsDisableBGColor( false );
                SetSpan2ControlsEnableStatus( true );

                RunRequiredRules();
            }
            else
            {
                SetSpan2ControlsEnableStatus( false );
                SetSpan2ControlsDisableBGColor( true );
            }
        }

        private void mtbSpan1FromDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            mtb.Refresh();
        }

        private void mtbSpan2FromDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            mtb.Refresh();
        }

        private void mtbSpan1ToDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            mtb.Refresh();
        }

        private void mtbSpan2ToDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            mtb.Refresh();
        }

        private void mtbSpan1FromDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1FromDate );

            if ( !dtppSpan1From.Focused )
            {
                if ( mtbSpan1FromDate.UnMaskedText != String.Empty )
                {
                    if ( TransferService.IsTransferDateValid( mtbSpan1FromDate ) &&
                        IsNotFutureDate( mtbSpan1FromDate, UIErrorMessages.SPANCODE_FROMDATE_IN_FUTURE_ERRMSG ) )
                    {
                        EnableAllDatePickersClick();
                        UpdateSpan1InModel();
                        CheckForValidSpan1Range();
                        RunRequiredRules();
                    }
                    else
                    {
                        DisableOtherDatePickersClick( dtppSpan1From );
                    }

                    Refresh();
                }
                else
                {
                    UpdateSpan1InModel();
                    RunRequiredRules();
                }
            }
        }

        private void mtbSpan1ToDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1ToDate );

            if ( !dtppSpan1To.Focused )
            {
                if ( mtbSpan1ToDate.UnMaskedText != String.Empty )
                {
                    if ( TransferService.IsTransferDateValid( mtbSpan1ToDate ) )
                    {
                        EnableAllDatePickersClick();
                        UpdateSpan1InModel();
                        CheckForValidSpan1Range();
                        RunRequiredRules();
                    }
                    else
                    {
                        DisableOtherDatePickersClick( dtppSpan1To );
                    }

                    Refresh();
                }
                else
                {
                    UpdateSpan1InModel();
                    RunRequiredRules();
                }
            }
        }

        private void mtbFacility_Validating( object sender, CancelEventArgs e )
        {
            UpdateSpan1InModel();
        }

        private void mtbSpan2FromDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2FromDate );

            if ( !dtppSpan2From.Focused )
            {
                if ( mtbSpan2FromDate.UnMaskedText != String.Empty )
                {
                    if ( TransferService.IsTransferDateValid( mtbSpan2FromDate ) &&
                        IsNotFutureDate( mtbSpan2FromDate, UIErrorMessages.SPANCODE_FROMDATE_IN_FUTURE_ERRMSG ) )
                    {
                        EnableAllDatePickersClick();
                        UpdateSpan2InModel();
                        CheckForValidSpan2Range();
                        RunRequiredRules();
                    }
                    else
                    {
                        DisableOtherDatePickersClick( dtppSpan2From );
                    }
                    Refresh();
                }
                else
                {
                    UpdateSpan2InModel();
                    RunRequiredRules();
                }
            }
        }

        private void mtbSpan2ToDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2ToDate );

            if ( !dtppSpan2To.Focused )
            {
                if ( mtbSpan2ToDate.UnMaskedText != String.Empty )
                {
                    if ( TransferService.IsTransferDateValid( mtbSpan2ToDate ) )
                    {
                        EnableAllDatePickersClick();
                        UpdateSpan2InModel();
                        CheckForValidSpan2Range();
                        RunRequiredRules();
                    }
                    else
                    {
                        DisableOtherDatePickersClick( dtppSpan2To );
                    }
                    Refresh();
                }
                else
                {
                    UpdateSpan2InModel();
                    RunRequiredRules();
                }
            }
        }

        //---------------------Evaluate ComboBoxes -----------------------------------------------------

        private void cboOccCode2_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode2 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_2 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_2Change ), Model );
            }
        }

        private void cboOccCode3_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode3 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_3 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_3Change ), Model );
            }
        }

        private void cboOccCode4_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode4 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccurrenceCode_4 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_4Change ), Model );
            }
        }

        private void cboOccCode5_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode5 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_5 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_5Change ), Model );
            }
        }

        private void cboOccCode6_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode6 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_6 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_6Change ), Model );
            }
        }

        private void cboOccCode7_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode7 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_7 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_7Change ), Model );
            }
        }

        private void cboOccCode8_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboOccCode8 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_8 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidOccuranceCode_8Change ), Model );
            }
        }

        private void cboCond1_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond1 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_1 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_1Change ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( ConditionCode1Required ), Model );
            }
        }

        private void cboCond2_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond2 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_2 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_2Change ), Model );
            }
        }

        private void cboCond3_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond3 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_3 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_3Change ), Model );
            }
        }

        private void cboCond4_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond4 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_4 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_4Change ), Model );
            }
        }

        private void cboCond5_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond5 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_5 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_5Change ), Model );
            }
        }

        private void cboCond6_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond6 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_6 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_6Change ), Model );
            }
        }

        private void cboCond7_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboCond7 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_7 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidConditionCode_7Change ), Model );
            }
        }

        private void cboSpan1_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboSpan1 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidSpanCode_1 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidSpanCode_1Change ), Model );
            }
        }

        private void cboSpan2_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cboSpan1 );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidSpanCode_1 ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidSpanCode_1Change ), Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidOccurrenceCode_2ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode2 );
        }

        private void InvalidOccurrenceCode_3ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode3 );
        }

        private void InvalidOccurrenceCode_4ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode4 );
        }

        private void InvalidOccurrenceCode_5ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode5 );
        }

        private void InvalidOccurrenceCode_6ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode6 );
        }

        private void InvalidOccurrenceCode_7ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode7 );
        }

        private void InvalidOccurrenceCode_8ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboOccCode8 );
        }

        private void InvalidConditionCode_1ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond1 );
        }

        private void InvalidConditionCode_2ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond2 );
        }

        private void InvalidConditionCode_3ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond3 );
        }

        private void InvalidConditionCode_4ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond4 );
        }

        private void InvalidConditionCode_5ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond5 );
        }

        private void InvalidConditionCode_6ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond6 );
        }

        private void InvalidConditionCode_7ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboCond7 );
        }

        private void InvalidSpanCode_1ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboSpan1 );
        }

        private void InvalidSpanCode_2ChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cboSpan2 );
        }

        //----------------------------------------------------------------------

        private void InvalidOccurrenceCode_2EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode2 );
        }

        private void InvalidOccurrenceCode_3EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode3 );
        }

        private void InvalidOccurrenceCode_4EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode4 );
        }

        private void InvalidOccurrenceCode_5EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode5 );
        }

        private void InvalidOccurrenceCode_6EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode6 );
        }

        private void InvalidOccurrenceCode_7EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode7 );
        }

        private void InvalidOccurrenceCode_8EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboOccCode8 );
        }

        private void InvalidConditionCode_1EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond1 );
        }

        private void InvalidConditionCode_2EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond2 );
        }

        private void InvalidConditionCode_3EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond3 );
        }

        private void InvalidConditionCode_4EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond4 );
        }

        private void InvalidConditionCode_5EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond5 );
        }

        private void InvalidConditionCode_6EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond6 );
        }

        private void InvalidConditionCode_7EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboCond7 );
        }

        private void InvalidSpanCode_1EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboSpan1 );
        }

        private void InvalidSpanCode_2EventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cboSpan2 );
        }

        #endregion

        #region Public methods
        public override void UpdateView()
        {
            try
            {
                // Disable painting of controls while they are loading
                BeginUpdateForControlsIn( grpConditions.Controls );
                BeginUpdateForControlsIn( grpOccSpans.Controls );
                BeginUpdateForControlsIn( grpOccurrences.Controls );

                firstTimeLoad = firstTimeLoad == string.Empty ? YES : NO;

                RemoveOCCHandler();
                RemoveConditionHandler();

                if ( firstTimeLoad == YES )
                {
                    Model.RemoveOccurrenceCode50IfNotApplicable();                    	

                    LoadOCCCboControls();
                    LoadOCCDatesControls();
                    LoadOCCDatePickerControls();
                    LoadConditionCboControls();

                    LoadDateTimePickers();
                }
                else
                {
                    LoadOCCCboControls();
                }

                EmergencyToInPatientTransferCodeManager.UpdateConditionCodes();
                GetFilteredConditionCodesList();

                SortOCCCodes();
                SortConditionCodes();

                PopulateOCC1();
                PopulateOCC2To8();
                PopulateConditionCodes();

                AddOCCHandler();
                AddConditionHandler();

                SystemGeneratedSpan1 = null;
                SystemGeneratedSpan2 = null;

                SetSpan1ControlsDisableBGColor( true );
                SetSpan2ControlsDisableBGColor( true );

                SetSpan1ControlsEnableStatus( false );
                SetSpan2ControlsEnableStatus( false );

                Model.Patient.SelectedAccount = Model;

                if ( firstTimeLoad == YES && IsActivatePreRegistrationActivity() )
                {
                    Model.OccurrenceSpans.Clear();
                }

                Model.Patient.ClearPriorSystemGeneratedOccurrenceSpans();

                UpdateSystemGeneratedSpansFromModel();

                PopulateSpanLists();

                if ( Model != null )
                {
                    if ( !Model.Activity.GetType().Equals( typeof( EditAccountActivity ) ) &&
                        !Model.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) &&
                        Model.KindOfVisit != null &&
                        ( ( Model.KindOfVisit.Code == VisitType.PREREG_PATIENT && !Model.Activity.IsActivatePreRegisterActivity() ) ||
                           Model.KindOfVisit.Code == VisitType.INPATIENT ) )
                    {
                        SpanCode spanCode70 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
                        SpanCode spanCode71 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.PRIOR_STAY_DATES );

                        Model.Patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );

                        if ( Model.OccurrenceSpans != null &&
                            Model.OccurrenceSpans.Count > 0 )
                        {
                            UpdateSystemGeneratedSpansFromModel();
                            ShowSpans();
                        }
                    }
                    else
                    {
                        if ( Model.OccurrenceSpans != null &&
                            Model.OccurrenceSpans.Count > 0 )
                        {
                            ShowSpans();
                        }
                    }
                }

                RegisterRulesEvents();
                RunRules();
            }
            finally
            {
                EndUpdateForControlsIn( grpConditions.Controls );
                EndUpdateForControlsIn( grpOccSpans.Controls );
                EndUpdateForControlsIn( grpOccurrences.Controls );
            }
        }

        private void UpdateSystemGeneratedSpansFromModel()
        {
            if ( Model.OccurrenceSpans != null &&
                 Model.OccurrenceSpans.Count > 0 )
            {
                SystemGeneratedSpan1 = ( OccurrenceSpan )Model.OccurrenceSpans[0];
                if ( Model.OccurrenceSpans.Count > 1 )
                {
                    SystemGeneratedSpan2 = ( OccurrenceSpan )Model.OccurrenceSpans[1];
                }
            }
        }

        private bool IsActivatePreRegistrationActivity()
        {
            return ( Model.Activity != null &&
                     Model.Activity.GetType().Equals( typeof( RegistrationActivity ) ) &&
                     Model.Activity.AssociatedActivityType != null &&
                     Model.Activity.AssociatedActivityType.Equals( typeof( ActivatePreRegistrationActivity ) ) );
        }

        public override void UpdateModel()
        {
        }
        #endregion

        #region Public Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private methods

        private static void BeginUpdateForControlsIn( ControlCollection aControlsCollection )
        {
            foreach ( Control c in aControlsCollection )
            {
                ComboBox cb = c as ComboBox;
                if ( cb != null )
                {
                    cb.BeginUpdate();
                }
            }
        }

        private static void EndUpdateForControlsIn( ControlCollection aControlsCollection )
        {
            foreach ( Control c in aControlsCollection )
            {
                ComboBox cb = c as ComboBox;
                if ( cb != null )
                {
                    cb.EndUpdate();
                }
            }
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode1DateRequired ), Model, new EventHandler( OccurrenceCode1DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode2DateRequired ), Model, new EventHandler( OccurrenceCode2DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode3DateRequired ), Model, new EventHandler( OccurrenceCode3DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode4DateRequired ), Model, new EventHandler( OccurrenceCode4DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode5DateRequired ), Model, new EventHandler( OccurrenceCode5DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode6DateRequired ), Model, new EventHandler( OccurrenceCode6DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode7DateRequired ), Model, new EventHandler( OccurrenceCode7DateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( OccurrenceCode8DateRequired ), Model, new EventHandler( OccurrenceCode8DateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( ConditionCode1Required ), Model, new EventHandler( ConditionCode1RequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( Span1FromDateRequired ), Model, new EventHandler( Span1FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span1ToDateRequired ), Model, new EventHandler( Span1ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( Span2FromDateRequired ), Model, new EventHandler( Span2FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span2ToDateRequired ), Model, new EventHandler( Span2ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_2 ), Model, new EventHandler( InvalidOccurrenceCode_2EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_2Change ), Model, new EventHandler( InvalidOccurrenceCode_2ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_3 ), Model, new EventHandler( InvalidOccurrenceCode_3EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_3Change ), Model, new EventHandler( InvalidOccurrenceCode_3ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccurrenceCode_4 ), Model, new EventHandler( InvalidOccurrenceCode_4EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_4Change ), Model, new EventHandler( InvalidOccurrenceCode_4ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_5 ), Model, new EventHandler( InvalidOccurrenceCode_5EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_5Change ), Model, new EventHandler( InvalidOccurrenceCode_5ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_6 ), Model, new EventHandler( InvalidOccurrenceCode_6EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_6Change ), Model, new EventHandler( InvalidOccurrenceCode_6ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_7 ), Model, new EventHandler( InvalidOccurrenceCode_7EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_7Change ), Model, new EventHandler( InvalidOccurrenceCode_7ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_8 ), Model, new EventHandler( InvalidOccurrenceCode_8EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidOccuranceCode_8Change ), Model, new EventHandler( InvalidOccurrenceCode_8ChangeEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_1 ), Model, new EventHandler( InvalidConditionCode_1EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_1Change ), Model, new EventHandler( InvalidConditionCode_1ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_2 ), Model, new EventHandler( InvalidConditionCode_2EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_2Change ), Model, new EventHandler( InvalidConditionCode_2ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_3 ), Model, new EventHandler( InvalidConditionCode_3EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_3Change ), Model, new EventHandler( InvalidConditionCode_3ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_4 ), Model, new EventHandler( InvalidConditionCode_4EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_4Change ), Model, new EventHandler( InvalidConditionCode_4ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_5 ), Model, new EventHandler( InvalidConditionCode_5EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_5Change ), Model, new EventHandler( InvalidConditionCode_5ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_6 ), Model, new EventHandler( InvalidConditionCode_6EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_6Change ), Model, new EventHandler( InvalidConditionCode_6ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_7 ), Model, new EventHandler( InvalidConditionCode_7EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidConditionCode_7Change ), Model, new EventHandler( InvalidConditionCode_7ChangeEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidSpanCode_1 ), Model, new EventHandler( InvalidSpanCode_1EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidSpanCode_1Change ), Model, new EventHandler( InvalidSpanCode_1ChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidSpanCode_2 ), Model, new EventHandler( InvalidSpanCode_2EventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidSpanCode_2Change ), Model, new EventHandler( InvalidSpanCode_2ChangeEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS    
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode1DateRequired ), Model, OccurrenceCode1DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode2DateRequired ), Model, OccurrenceCode2DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode3DateRequired ), Model, OccurrenceCode3DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode4DateRequired ), Model, OccurrenceCode4DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode5DateRequired ), Model, OccurrenceCode5DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode6DateRequired ), Model, OccurrenceCode6DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode7DateRequired ), Model, OccurrenceCode7DateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( OccurrenceCode8DateRequired ), Model, ( OccurrenceCode8DateRequiredEventHandler ) );

            RuleEngine.GetInstance().UnregisterEvent( typeof( ConditionCode1Required ), Model, ( ConditionCode1RequiredEventHandler ) );

            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1FromDateRequired ), Model, ( Span1FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1ToDateRequired ), Model, ( Span1ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2FromDateRequired ), Model, ( Span2FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2ToDateRequired ), Model, ( Span2ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_2 ), Model, InvalidOccurrenceCode_2EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_2Change ), Model, InvalidOccurrenceCode_2ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_3 ), Model, InvalidOccurrenceCode_3EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_3Change ), Model, InvalidOccurrenceCode_3ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccurrenceCode_4 ), Model, InvalidOccurrenceCode_4EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_4Change ), Model, InvalidOccurrenceCode_4ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_5 ), Model, InvalidOccurrenceCode_5EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_5Change ), Model, InvalidOccurrenceCode_5ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_6 ), Model, InvalidOccurrenceCode_6EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_6Change ), Model, InvalidOccurrenceCode_6ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_7 ), Model, InvalidOccurrenceCode_7EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_7Change ), Model, InvalidOccurrenceCode_7ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_8 ), Model, InvalidOccurrenceCode_8EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidOccuranceCode_8Change ), Model, InvalidOccurrenceCode_8ChangeEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_1 ), Model, InvalidConditionCode_1EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_1Change ), Model, InvalidConditionCode_1ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_2 ), Model, InvalidConditionCode_2EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_2Change ), Model, InvalidConditionCode_2ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_3 ), Model, InvalidConditionCode_3EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_3Change ), Model, InvalidConditionCode_3ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_4 ), Model, InvalidConditionCode_4EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_4Change ), Model, InvalidConditionCode_4ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_5 ), Model, InvalidConditionCode_5EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_5Change ), Model, InvalidConditionCode_5ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_6 ), Model, InvalidConditionCode_6EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_6Change ), Model, InvalidConditionCode_6ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_7 ), Model, InvalidConditionCode_7EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidConditionCode_7Change ), Model, InvalidConditionCode_7ChangeEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidSpanCode_1 ), Model, InvalidSpanCode_1EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidSpanCode_1Change ), Model, InvalidSpanCode_1ChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidSpanCode_2 ), Model, InvalidSpanCode_2EventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidSpanCode_2Change ), Model, InvalidSpanCode_2ChangeEventHandler );
        }

        private void RunRules()
        {
            //reset all fields that might have error, preferred, or required backgrounds                        
            for ( int i = 2; i <= 8; i++ )
            {
                string cboName = OCC_CBO_NAME_PREFIX + i;

                MaskedEditTextBox mtbDate = ( MaskedEditTextBox )occDateControls[cboName];
                if ( mtbDate.Enabled )
                {
                    UIColors.SetNormalBgColor( mtbDate );
                }
            }
         
            UIColors.SetNormalBgColor( cboOccCode2 );
            UIColors.SetNormalBgColor( cboOccCode3 );
            UIColors.SetNormalBgColor( cboOccCode4 );
            UIColors.SetNormalBgColor( cboOccCode5 );
            UIColors.SetNormalBgColor( cboOccCode6 );
            UIColors.SetNormalBgColor( cboOccCode7 );
            UIColors.SetNormalBgColor( cboOccCode8 );

            UIColors.SetNormalBgColor(mtbOcc2Date);
            UIColors.SetNormalBgColor(mtbOcc3Date);
            UIColors.SetNormalBgColor(mtbOcc4Date);
            UIColors.SetNormalBgColor(mtbOcc5Date);
            UIColors.SetNormalBgColor(mtbOcc6Date);
            UIColors.SetNormalBgColor(mtbOcc7Date);
            UIColors.SetNormalBgColor(mtbOcc8Date);
         
            UIColors.SetNormalBgColor( cboCond1 );
            UIColors.SetNormalBgColor( cboCond2 );
            UIColors.SetNormalBgColor( cboCond3 );
            UIColors.SetNormalBgColor( cboCond4 );
            UIColors.SetNormalBgColor( cboCond5 );
            UIColors.SetNormalBgColor( cboCond6 );
            UIColors.SetNormalBgColor( cboCond7 );
            UIColors.SetNormalBgColor( cboSpan1 );
            UIColors.SetNormalBgColor( cboSpan2 );

            SortOCCCodes();
            RuleEngine.GetInstance().EvaluateRule( typeof( OnBillingForm ), Model );
        }

        private void RunRequiredRules()
        {
            //reset all fields that might have error, preferred, or required backgrounds                        
            for ( int i = 2; i <= 8; i++ )
            {
                string cboName = OCC_CBO_NAME_PREFIX + i;

                MaskedEditTextBox mtbDate = ( MaskedEditTextBox )occDateControls[cboName];
                if ( mtbDate.Enabled )
                {
                    UIColors.SetNormalBgColor( mtbDate );
                }
            }

            UIColors.SetNormalBgColor( cboOccCode2 );
            UIColors.SetNormalBgColor( cboOccCode3 );
            UIColors.SetNormalBgColor( cboOccCode4 );
            UIColors.SetNormalBgColor( cboOccCode5 );
            UIColors.SetNormalBgColor( cboOccCode6 );
            UIColors.SetNormalBgColor( cboOccCode7 );
            UIColors.SetNormalBgColor( cboOccCode8 );
            UIColors.SetNormalBgColor( cboCond1 );
            UIColors.SetNormalBgColor( cboSpan1 );
            UIColors.SetNormalBgColor( cboSpan2 );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode1DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode2DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode3DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode4DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode5DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode6DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode7DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode8DateRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( ConditionCode1Required ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( Span1FromDateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span1ToDateRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( Span2FromDateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span2ToDateRequired ), Model );
        }

        private void LoadOCCCboControls()
        {
            occCboMgr.ResetComboBoxManager();

            cboOccCode2.Enabled = false;
            cboOccCode3.Enabled = false;
            cboOccCode4.Enabled = false;
            cboOccCode5.Enabled = false;
            cboOccCode6.Enabled = false;
            cboOccCode7.Enabled = false;
            cboOccCode8.Enabled = false;

            occCboMgr.AddComboBox( cboOccCode2 );
            occCboMgr.AddComboBox( cboOccCode3 );
            occCboMgr.AddComboBox( cboOccCode4 );
            occCboMgr.AddComboBox( cboOccCode5 );
            occCboMgr.AddComboBox( cboOccCode6 );
            occCboMgr.AddComboBox( cboOccCode7 );
            occCboMgr.AddComboBox( cboOccCode8 );

            occCboMgr.DataSource = GetOCCItemData();
        }

        private void LoadOCCDatesControls()
        {
            occDateControls.Clear();

            mtbOcc2Date.UnMaskedText = string.Empty;
            mtbOcc3Date.UnMaskedText = string.Empty;
            mtbOcc4Date.UnMaskedText = string.Empty;
            mtbOcc5Date.UnMaskedText = string.Empty;
            mtbOcc6Date.UnMaskedText = string.Empty;
            mtbOcc7Date.UnMaskedText = string.Empty;
            mtbOcc8Date.UnMaskedText = string.Empty;

            mtbOcc2Date.Enabled = false;
            mtbOcc3Date.Enabled = false;
            mtbOcc4Date.Enabled = false;
            mtbOcc5Date.Enabled = false;
            mtbOcc6Date.Enabled = false;
            mtbOcc7Date.Enabled = false;
            mtbOcc8Date.Enabled = false;

            mtbOcc2Date.BackColor = SystemColors.Control;
            mtbOcc3Date.BackColor = SystemColors.Control;
            mtbOcc4Date.BackColor = SystemColors.Control;
            mtbOcc5Date.BackColor = SystemColors.Control;
            mtbOcc6Date.BackColor = SystemColors.Control;
            mtbOcc7Date.BackColor = SystemColors.Control;
            mtbOcc8Date.BackColor = SystemColors.Control;

            occDateControls.Add( cboOccCode2.Name, mtbOcc2Date );
            occDateControls.Add( cboOccCode3.Name, mtbOcc3Date );
            occDateControls.Add( cboOccCode4.Name, mtbOcc4Date );
            occDateControls.Add( cboOccCode5.Name, mtbOcc5Date );
            occDateControls.Add( cboOccCode6.Name, mtbOcc6Date );
            occDateControls.Add( cboOccCode7.Name, mtbOcc7Date );
            occDateControls.Add( cboOccCode8.Name, mtbOcc8Date );
        }

        private void LoadOCCDatePickerControls()
        {
            occDatePickerControls.Clear();

            dtppOcc2Date.Enabled = false;
            dtppOcc3Date.Enabled = false;
            dtppOcc4Date.Enabled = false;
            dtppOcc5Date.Enabled = false;
            dtppOcc6Date.Enabled = false;
            dtppOcc7Date.Enabled = false;
            dtppOcc8Date.Enabled = false;

            occDatePickerControls.Add( cboOccCode2.Name, dtppOcc2Date );
            occDatePickerControls.Add( cboOccCode3.Name, dtppOcc3Date );
            occDatePickerControls.Add( cboOccCode4.Name, dtppOcc4Date );
            occDatePickerControls.Add( cboOccCode5.Name, dtppOcc5Date );
            occDatePickerControls.Add( cboOccCode6.Name, dtppOcc6Date );
            occDatePickerControls.Add( cboOccCode7.Name, dtppOcc7Date );
            occDatePickerControls.Add( cboOccCode8.Name, dtppOcc8Date );
        }

        private void LoadConditionCboControls()
        {
            condCboMgr.ResetComboBoxManager();

            cboCond1.Enabled = false;
            cboCond2.Enabled = false;
            cboCond3.Enabled = false;
            cboCond4.Enabled = false;
            cboCond5.Enabled = false;
            cboCond6.Enabled = false;
            cboCond7.Enabled = false;

            condCboMgr.AddComboBox( cboCond1 );
            condCboMgr.AddComboBox( cboCond2 );
            condCboMgr.AddComboBox( cboCond3 );
            condCboMgr.AddComboBox( cboCond4 );
            condCboMgr.AddComboBox( cboCond5 );
            condCboMgr.AddComboBox( cboCond6 );
            condCboMgr.AddComboBox( cboCond7 );

            GetSelectableConditionCodes();
        }

        private void LoadDateTimePickers()
        {
            i_DateTimePickers.Clear();

            i_DateTimePickers.Add( dtppOcc2Date );
            i_DateTimePickers.Add( dtppOcc3Date );
            i_DateTimePickers.Add( dtppOcc4Date );
            i_DateTimePickers.Add( dtppOcc5Date );
            i_DateTimePickers.Add( dtppOcc6Date );
            i_DateTimePickers.Add( dtppOcc7Date );
            i_DateTimePickers.Add( dtppOcc8Date );

            i_DateTimePickers.Add( dtppSpan1From );
            i_DateTimePickers.Add( dtppSpan1To );
            i_DateTimePickers.Add( dtppSpan2From );
            i_DateTimePickers.Add( dtppSpan2To );
        }

        private ArrayList GetOCCItemData()
        {
            OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
            ArrayList occurrenceCodesList = ( ArrayList )brokerProxy.AllSelectableOccurrenceCodes( User.GetCurrent().Facility.Oid );

            if ( !Model.Patient.Sex.Code.Equals( Gender.FEMALE_CODE ) )
            {
                foreach ( OccurrenceCode occ in occurrenceCodesList )
                {
                    if ( occ.Code.Equals( OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION ) )
                    {
                        occurrenceCodesList.Remove( occ );
                        break;
                    }
                }
            }
            if (!Model.IsValidForDuplicateOccurenceCode50())
            {
                foreach (OccurrenceCode occ in occurrenceCodesList)
                {
                    if (occ.Code.Equals(OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB))
                    {
                        occurrenceCodesList.Remove(occ);
                        break;
                    }
                }
            }
            occCboMgr.OCCManualCodes = occurrenceCodesList;
            return occurrenceCodesList;
        }

        private void GetFilteredConditionCodesList()
        {
            var conditionCodesList = SelectableConditionCodes.Cast<ConditionCode>();

            var filteredConditionCodesList =
                EmergencyToInPatientTransferCodeManager.FilterConditionCodesMasterList( conditionCodesList );

            ArrayList filteredConditionCodes = new ArrayList( filteredConditionCodesList.ToArray() );

            condCboMgr.ConditionManualCodes = filteredConditionCodes;
            condCboMgr.DataSource = filteredConditionCodes;
        }

        private void GetSelectableConditionCodes()
        {
            IConditionCodeBroker broker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            SelectableConditionCodes = broker.AllSelectableConditionCodes( User.GetCurrent().Facility.Oid );
        }

        /// <summary>
        /// Populates the span lists.
        /// </summary>
        private void PopulateSpanLists()
        {
            RemoveSpanHandler();

            InitializeSpanListControls();

            if ( Model.OccurrenceSpans != null )
            {
                if ( Model.OccurrenceSpans.Count > 0 && Model.OccurrenceSpans[0] != null )
                    SetSpanCodeSelection( Model.OccurrenceSpans[0] as OccurrenceSpan,
                                               cboSpan1,
                                               mtbSpan1FromDate,
                                               dtppSpan1From,
                                               mtbSpan1ToDate,
                                               dtppSpan1To,
                                               mtbFacility );

                if ( Model.OccurrenceSpans.Count > 1 && Model.OccurrenceSpans[1] != null )
                    SetSpanCodeSelection( Model.OccurrenceSpans[1] as OccurrenceSpan,
                                               cboSpan2,
                                               mtbSpan2FromDate,
                                               dtppSpan2From,
                                               mtbSpan2ToDate,
                                               dtppSpan2To,
                                               null );
            }//if

            // No point in enabling the combo if there is only a blank item in it
            cboSpan1.Enabled = cboSpan1.Items.Count > 1;
            cboSpan2.Enabled = cboSpan2.Items.Count > 1;

            AddSpanHandler();
        }


        /// <summary>
        /// Sets the span code selection.
        /// </summary>
        /// <param name="occurrenceSpan">The occurrence span.</param>
        /// <param name="spanCodeComboBox">The span code combo box.</param>
        /// <param name="fromDateMaskedEditTextBox">From date masked edit text box.</param>
        /// <param name="fromDateDateTimePicker">From date date time picker.</param>
        /// <param name="toDateMaskedEditTextBox">To date masked edit text box.</param>
        /// <param name="toDateDateTimePicker">To date date time picker.</param>
        /// <param name="facilityMaskedEditTextBox">The facility masked edit text box.</param>
        private void SetSpanCodeSelection( OccurrenceSpan occurrenceSpan,
                                           PatientAccessComboBox spanCodeComboBox,
                                           MaskedEditTextBox fromDateMaskedEditTextBox,
                                           DateTimePicker fromDateDateTimePicker,
                                           MaskedEditTextBox toDateMaskedEditTextBox,
                                           DateTimePicker toDateDateTimePicker,
                                           MaskedEditTextBox facilityMaskedEditTextBox )
        {
            SpanCode spanCode = occurrenceSpan.SpanCode;

            if ( Model.CanAccept( spanCode ) )
            {
                spanCodeComboBox.SelectedItem = spanCode;
            }
            else if ( !( spanCode.IsNoncoveredLevelOfCare || spanCode.IsPriorStayDates || spanCode.IsQualifyingStayDate ) )
            {
                spanCodeComboBox.Items.Add( spanCode );
                spanCodeComboBox.SelectedItem = spanCode;
            }

            if ( !String.IsNullOrEmpty( ( ( SpanCode )spanCodeComboBox.SelectedItem ).Code ) )
            {
                fromDateMaskedEditTextBox.UnMaskedText = occurrenceSpan.FromDate != DateTime.MinValue
                                                             ? CommonFormatting.MaskedDateFormat(
                                                                   occurrenceSpan.FromDate )
                                                             : String.Empty;

                UIColors.SetNormalBgColor( fromDateMaskedEditTextBox );
                fromDateMaskedEditTextBox.Enabled = true;
                fromDateDateTimePicker.Enabled = true;

                toDateMaskedEditTextBox.UnMaskedText = occurrenceSpan.ToDate != DateTime.MinValue
                                                           ? CommonFormatting.MaskedDateFormat( occurrenceSpan.ToDate )
                                                           : String.Empty;

                UIColors.SetNormalBgColor( toDateMaskedEditTextBox );
                toDateMaskedEditTextBox.Enabled = true;
                toDateDateTimePicker.Enabled = true;

                if ( facilityMaskedEditTextBox != null )
                {
                    UIColors.SetNormalBgColor( facilityMaskedEditTextBox );
                    facilityMaskedEditTextBox.UnMaskedText = occurrenceSpan.Facility;
                    facilityMaskedEditTextBox.Enabled = true;
                }
            }
            else
            {
                fromDateDateTimePicker.Enabled = false;
                fromDateMaskedEditTextBox.Enabled = false;
                toDateDateTimePicker.Enabled = false;
                toDateMaskedEditTextBox.Enabled = false;
                if ( facilityMaskedEditTextBox != null )
                    facilityMaskedEditTextBox.Enabled = false;
            }
        }

        /// <summary>
        /// Initializes the span list controls.
        /// </summary>
        private void InitializeSpanListControls()
        {
            ISpanCodeBroker spanCodeBroker = new SpanCodeBrokerProxy();
            ArrayList allSpanCodes = ( ArrayList )spanCodeBroker.AllSpans( User.GetCurrent().Facility.Oid );
            SpanCode blankSpan = new SpanCode( BLANK_OPTION_OID, PersistentModel.NEW_VERSION, String.Empty, String.Empty );

            cboSpan1.Items.Clear();
            mtbSpan1FromDate.UnMaskedText = String.Empty;
            mtbSpan1ToDate.UnMaskedText = String.Empty;
            mtbFacility.UnMaskedText = String.Empty;

            cboSpan2.Items.Clear();
            mtbSpan2FromDate.UnMaskedText = String.Empty;
            mtbSpan2ToDate.UnMaskedText = String.Empty;

            cboSpan1.Items.Add( blankSpan );
            cboSpan2.Items.Add( blankSpan );

            foreach ( SpanCode spanCode in allSpanCodes )
            {
                if ( Model.CanAccept( spanCode ) )
                {
                    cboSpan1.Items.Add( spanCode );
                    cboSpan2.Items.Add( spanCode );
                }
            }

            cboSpan1.SelectedItem = blankSpan;
            cboSpan2.SelectedItem = blankSpan;
        }

        private void PopulateOCC1()
        {
            bool hasOccurrenceCode1 = false;

            foreach ( OccurrenceCode oc in SortedOccurrenceCodes )
            {
                if ( oc.IsAccidentCrimeOccurrenceCode() )
                {
                    lblOccCode1Val.Text = oc.ToString();

                    if ( oc.OccurrenceDate != DateTime.MinValue )
                    {
                        lblOccCode1DateVal.Text = CommonFormatting.LongDateFormat( oc.OccurrenceDate );
                        UIColors.SetNormalBgColor( lblOccCode1DateVal );
                    }
                    else
                    {
                        lblOccCode1DateVal.Text = String.Empty;
                        UIColors.SetRequiredBgColor( lblOccCode1DateVal );
                    }

                    i_OCC1 = ( OccurrenceCode )oc.Clone();

                    hasOccurrenceCode1 = true;
                    break;
                }
            }

            if ( !hasOccurrenceCode1 )
            {
                i_OCC1 = null;
                lblOccCode1Val.Text = String.Empty;
                lblOccCode1DateVal.Text = String.Empty;
                UIColors.SetNormalBgColor( lblOccCode1DateVal );
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                OccurrenceCode firstOccurrenceCode = SortedOccurrenceCodes[0] as OccurrenceCode;
                if ( hasOccurrenceCode1 || firstOccurrenceCode != null && firstOccurrenceCode.Code == string.Empty )
                {
                    SortedOccurrenceCodes.RemoveAt( 0 );
                }
            }
        }

        private void PopulateOCC2To8()
        {
            ResetSelectedIndexForOccurenceCodeComboboxesExceptForFirstTwo();

            ( ( MaskedEditTextBox )occDateControls[cboOccCode2.Name] ).UnMaskedText = string.Empty;
            ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).UnMaskedText = string.Empty;

            ( ( MaskedEditTextBox )occDateControls[cboOccCode2.Name] ).BackColor = SystemColors.Control;
            ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).BackColor = SystemColors.Control;

            cboOccCode2.Enabled = true;
            i_LastEnabledOCCName = cboOccCode2.Name;

            if ( SortedOccurrenceCodes.Count == 0 )
            {
                // do nothing
            }
            else if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode2, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode3.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode3.Items.Count > 0 )
                {
                    cboOccCode3.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode3.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode3.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode3.Enabled = false;

                if ( cboOccCode3.Items.Count > 0 )
                {
                    cboOccCode3.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode3.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode3.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode3, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode4.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode4.Items.Count > 0 )
                {
                    cboOccCode4.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode4.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode4.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode4.Enabled = false;

                if ( cboOccCode4.Items.Count > 0 )
                {
                    cboOccCode4.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode4.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode4.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode4, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode5.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode5.Items.Count > 0 )
                {
                    cboOccCode5.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode5.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode5.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode5.Enabled = false;

                if ( cboOccCode5.Items.Count > 0 )
                {
                    cboOccCode5.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode5.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode5.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode5, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode6.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode6.Items.Count > 0 )
                {
                    cboOccCode6.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode6.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode6.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode6.Enabled = false;

                if ( cboOccCode6.Items.Count > 0 )
                {
                    cboOccCode6.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode6.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode6.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode6, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode7.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode7.Items.Count > 0 )
                {
                    cboOccCode7.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode7.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode7.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode7.Enabled = false;

                if ( cboOccCode7.Items.Count > 0 )
                {
                    cboOccCode7.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode7.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode7.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode7, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
                cboOccCode8.Enabled = true;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).UnMaskedText = string.Empty;

                if ( cboOccCode8.Items.Count > 0 )
                {
                    cboOccCode8.SelectedIndex = 0;
                }

                i_LastEnabledOCCName = cboOccCode8.Name;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode8.Name] ).Enabled = false;
            }
            else
            {
                cboOccCode8.Enabled = false;

                if ( cboOccCode8.Items.Count > 0 )
                {
                    cboOccCode8.SelectedIndex = 0;
                }

                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).BackColor = SystemColors.Control;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).Enabled = false;
                ( ( DateTimePicker )occDatePickerControls[cboOccCode8.Name] ).Enabled = false;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                FillOCCAndDate( cboOccCode8, ( OccurrenceCode )( ( OccurrenceCode )SortedOccurrenceCodes[0] ).Clone() );
            }
            else
            {
                ( ( MaskedEditTextBox )occDateControls[cboOccCode8.Name] ).UnMaskedText = string.Empty;
            }
        }

        private void PopulateConditionCodes()
        {
            cboCond1.Enabled = true;
            cboCond1.SelectedIndex = 0;
            cboCond2.SelectedIndex = 0;
            cboCond3.SelectedIndex = 0;
            cboCond4.SelectedIndex = 0;
            cboCond5.SelectedIndex = 0;
            cboCond6.SelectedIndex = 0;
            cboCond7.SelectedIndex = 0;

            i_LastEnabledCondName = cboCond1.Name;

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond1, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond2.Enabled = true;
                cboCond2.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond2.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond2, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond3.Enabled = true;
                cboCond3.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond3.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond3, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond4.Enabled = true;
                cboCond4.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond4.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond4, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond5.Enabled = true;
                cboCond5.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond5.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond5, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond6.Enabled = true;
                cboCond6.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond6.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond6, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
                cboCond7.Enabled = true;
                cboCond7.SelectedIndex = 0;
                i_LastEnabledCondName = cboCond7.Name;
            }

            if ( SortedConditionCodes.Count > 0 )
            {
                condCboMgr.ComboValueInitialSet( cboCond7, ( ( ConditionCode )SortedConditionCodes[0] ).Clone() );
                SortedConditionCodes.RemoveAt( 0 );
            }
        }

        private void SortOCCCodes()
        {

            var occurrenceCodeComparerByCodeAndDate = new OccurrenceCodeComparerByCodeAndDate();

            ( ( ArrayList )Model.OccurrenceCodes ).Sort( occurrenceCodeComparerByCodeAndDate );
            SortedOccurrenceCodes = ( ArrayList )( ( ArrayList )Model.OccurrenceCodes ).Clone();
        }

        private void SortConditionCodes()
        {
            SortConditionsByCode sortConditions = new SortConditionsByCode();

            ( ( ArrayList )Model.ConditionCodes ).Sort( sortConditions );
            SortedConditionCodes = ( ArrayList )( ( ArrayList )Model.ConditionCodes ).Clone();
        }

        private void FillOCCAndDate( PatientAccessComboBox cbo, OccurrenceCode occ )
        {
            occCboMgr.ComboValueInitialSet( cbo, occ );

            if ( occ.OccurrenceDate != DateTime.MinValue )
            {
                //since some non-manual code is disabled.
                ( ( MaskedEditTextBox )occDateControls[cbo.Name] ).UnMaskedText =
                    CommonFormatting.MaskedDateFormat( occ.OccurrenceDate );

                ( ( MaskedEditTextBox )occDateControls[cbo.Name] ).Enabled = cbo.Enabled;
                ( ( MaskedEditTextBox )occDateControls[cbo.Name] ).BackColor = cbo.BackColor;
                ( ( DateTimePicker )occDatePickerControls[cbo.Name] ).Enabled = cbo.Enabled;
            }
            else
            {
                ( ( MaskedEditTextBox )occDateControls[cbo.Name] ).UnMaskedText = string.Empty;
                ( ( MaskedEditTextBox )occDateControls[cbo.Name] ).Enabled = true;
                ( ( DateTimePicker )occDatePickerControls[cbo.Name] ).Enabled = true;
            }

            if ( SortedOccurrenceCodes.Count > 0 )
            {
                SortedOccurrenceCodes.RemoveAt( 0 );
            }
        }

        private void UpdateOCCDate( PatientAccessComboBox occurrenceCodeComboBox, MaskedEditTextBox maskedEditTextBox, DateTimePicker dateTimePicker )
        {
            maskedEditTextBox.UnMaskedText = CommonFormatting.MaskedDateFormat( dateTimePicker.Value );
            ( ( OccurrenceCode )occurrenceCodeComboBox.SelectedItem ).OccurrenceDate = dateTimePicker.Value;

            RunRequiredRules();
            maskedEditTextBox.Focus();
        }

        private void EnableDisableNextOCC( PatientAccessComboBox cboOccCode )
        {
            int nextOccurrenceCodeNum = Convert.ToInt16( cboOccCode.Name.Substring( 10, 1 ) ) + 1;
            PatientAccessComboBox nextOccurrenceCode = occCboMgr.GetCombox( OCC_CBO_NAME_PREFIX + nextOccurrenceCodeNum );

            if ( cboOccCode.SelectedIndex > 0 )
            {
                if ( cboOccCode.Name == i_LastEnabledOCCName )
                {
                    nextOccurrenceCode.Enabled = true;
                    i_LastEnabledOCCName = nextOccurrenceCode.Name;
                }
            }
            else if ( nextOccurrenceCode.Name == i_LastEnabledOCCName &&
                     !( nextOccurrenceCode.Name == OCC_CBO_NAME_PREFIX + "8" &&
                        nextOccurrenceCode.SelectedIndex > 0 ) )
            {
                nextOccurrenceCode.Enabled = false;

                ( ( MaskedEditTextBox )occDateControls[nextOccurrenceCode.Name] ).Enabled = false;
                ( ( MaskedEditTextBox )occDateControls[nextOccurrenceCode.Name] ).BackColor = SystemColors.Control;
                ( ( DateTimePicker )occDatePickerControls[nextOccurrenceCode.Name] ).Enabled = false;
                i_LastEnabledOCCName = cboOccCode.Name;
            }
        }

        private void EnableDisableNextCondition( PatientAccessComboBox cboCondCode )
        {
            int nextCondNum = Convert.ToInt16( cboCondCode.Name.Substring( 7, 1 ) ) + 1;
            PatientAccessComboBox nextCond = condCboMgr.GetCombox( COND_CBO_NAME_PREFIX + nextCondNum );

            if ( cboCondCode.SelectedIndex > 0 )
            {
                if ( cboCondCode.Name == i_LastEnabledCondName )
                {
                    nextCond.Enabled = true;
                    i_LastEnabledCondName = nextCond.Name;
                }
            }
            else if ( nextCond.Name == i_LastEnabledCondName &&
                !( nextCond.Name == COND_CBO_NAME_PREFIX + "7" &&
                nextCond.SelectedIndex > 0 ) )
            {
                nextCond.Enabled = false;

                i_LastEnabledCondName = cboCondCode.Name;
            }
        }

        private void HandleOCCIndexChanged( PatientAccessComboBox occurrenceCodeComboBox )
        {
            if ( occurrenceCodeComboBox.Name == occCboMgr.currentCombo.Name )
            {
                ( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] ).UnMaskedText = string.Empty;

                if ( occurrenceCodeComboBox.SelectedIndex > 0 )
                {
                    ( ( OccurrenceCode )occurrenceCodeComboBox.SelectedItem ).OccurrenceDate = DateTime.MinValue;
                    ( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] ).Enabled = true;
                    UIColors.SetNormalBgColor( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] );
                    ( ( DateTimePicker )occDatePickerControls[occurrenceCodeComboBox.Name] ).Enabled = true;
                }
                else
                {
                    ( ( OccurrenceCode )occurrenceCodeComboBox.SelectedItem ).OccurrenceDate = DateTime.MinValue;
                    ( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] ).UnMaskedText = string.Empty;
                    ( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] ).Enabled = false;
                    ( ( MaskedEditTextBox )occDateControls[occurrenceCodeComboBox.Name] ).BackColor = SystemColors.Control;
                    ( ( DateTimePicker )occDatePickerControls[occurrenceCodeComboBox.Name] ).Enabled = false;
                }

                RemoveOCCHandler();
                occCboMgr.ComboValueSet(occurrenceCodeComboBox, occurrenceCodeComboBox.SelectedItem);
                UpdateModelOCCs();
                SortOCCCodes();
                PopulateOCC1();
                PopulateOCC2To8();
                AddOCCHandler();

                Refresh();

                if ( occurrenceCodeComboBox.Name != OCC_CBO_NAME_PREFIX + "8" )
                {
                    EnableDisableNextOCC( occurrenceCodeComboBox );
                }
                PurgeOCCBlankFromBottom();

                RunRequiredRules();
            }
        }

     
        private void RemoveOCCHandler()
        {
            cboOccCode2.SelectedIndexChanged -= cboOccCode2_SelectedIndexChanged;
            cboOccCode3.SelectedIndexChanged -= cboOccCode3_SelectedIndexChanged;
            cboOccCode4.SelectedIndexChanged -= cboOccCode4_SelectedIndexChanged;
            cboOccCode5.SelectedIndexChanged -= cboOccCode5_SelectedIndexChanged;
            cboOccCode6.SelectedIndexChanged -= cboOccCode6_SelectedIndexChanged;
            cboOccCode7.SelectedIndexChanged -= cboOccCode7_SelectedIndexChanged;
            cboOccCode8.SelectedIndexChanged -= cboOccCode8_SelectedIndexChanged;
        }

        private void AddOCCHandler()
        {
            cboOccCode2.SelectedIndexChanged += cboOccCode2_SelectedIndexChanged;
            cboOccCode3.SelectedIndexChanged += cboOccCode3_SelectedIndexChanged;
            cboOccCode4.SelectedIndexChanged += cboOccCode4_SelectedIndexChanged;
            cboOccCode5.SelectedIndexChanged += cboOccCode5_SelectedIndexChanged;
            cboOccCode6.SelectedIndexChanged += cboOccCode6_SelectedIndexChanged;
            cboOccCode7.SelectedIndexChanged += cboOccCode7_SelectedIndexChanged;
            cboOccCode8.SelectedIndexChanged += cboOccCode8_SelectedIndexChanged;
        }

        private void PurgeOCCBlankFromBottom()
        {
            int lastOccurrenceCodeNum = Convert.ToInt16( i_LastEnabledOCCName.Substring( 10, 1 ) );
            for ( int i = lastOccurrenceCodeNum; i > 2; i-- )
            {
                PatientAccessComboBox thisOCC = occCboMgr.GetCombox( OCC_CBO_NAME_PREFIX + i );
                PatientAccessComboBox prevOCC = occCboMgr.GetCombox( OCC_CBO_NAME_PREFIX + ( i - 1 ) );
                if ( thisOCC.SelectedIndex <= 0 && prevOCC.SelectedIndex <= 0 )
                {
                    thisOCC.Enabled = false;
                    i_LastEnabledOCCName = prevOCC.Name;
                    prevOCC.Focus();
                }
                else
                {
                    break;
                }
            }
        }

        private void HandleConditionIndexChanged( PatientAccessComboBox cboCond )
        {
            if ( cboCond.Name == condCboMgr.currentCombo.Name )
            {
                RemoveConditionHandler();
                condCboMgr.ComboValueSet( cboCond, cboCond.SelectedItem );
                AddConditionHandler();
                if ( cboCond.Name != COND_CBO_NAME_PREFIX + "7" )
                {
                    EnableDisableNextCondition( cboCond );
                }
                PurgeConditionBlankFromBottom();

                UpdateModelConditionCodes();

                RunRequiredRules();
            }
        }

        private void RemoveConditionHandler()
        {
            cboCond1.SelectedIndexChanged -= cboCond1_SelectedIndexChanged;
            cboCond2.SelectedIndexChanged -= cboCond2_SelectedIndexChanged;
            cboCond3.SelectedIndexChanged -= cboCond3_SelectedIndexChanged;
            cboCond4.SelectedIndexChanged -= cboCond4_SelectedIndexChanged;
            cboCond5.SelectedIndexChanged -= cboCond5_SelectedIndexChanged;
            cboCond6.SelectedIndexChanged -= cboCond6_SelectedIndexChanged;
            cboCond7.SelectedIndexChanged -= cboCond7_SelectedIndexChanged;
        }

        private void AddConditionHandler()
        {
            cboCond1.SelectedIndexChanged += cboCond1_SelectedIndexChanged;
            cboCond2.SelectedIndexChanged += cboCond2_SelectedIndexChanged;
            cboCond3.SelectedIndexChanged += cboCond3_SelectedIndexChanged;
            cboCond4.SelectedIndexChanged += cboCond4_SelectedIndexChanged;
            cboCond5.SelectedIndexChanged += cboCond5_SelectedIndexChanged;
            cboCond6.SelectedIndexChanged += cboCond6_SelectedIndexChanged;
            cboCond7.SelectedIndexChanged += cboCond7_SelectedIndexChanged;
        }

        private void PurgeConditionBlankFromBottom()
        {
            int lastCondNum = Convert.ToInt16( i_LastEnabledCondName.Substring( 7, 1 ) );
            for ( int i = lastCondNum; i > 1; i-- )
            {
                PatientAccessComboBox thisCond = condCboMgr.GetCombox( COND_CBO_NAME_PREFIX + i );
                PatientAccessComboBox prevCond = condCboMgr.GetCombox( COND_CBO_NAME_PREFIX + ( i - 1 ) );
                if ( thisCond.SelectedIndex <= 0 && prevCond.SelectedIndex <= 0 )
                {
                    thisCond.Enabled = false;
                    i_LastEnabledCondName = prevCond.Name;
                    prevCond.Focus();
                }
                else
                {
                    break;
                }
            }
        }

        private void HandleOCCDateLeave( PatientAccessComboBox occurrenceCodeComboBox, MaskedEditTextBox mtbDate, DateTimePickerPlus dateTimePickerPlus )
        {
            try
            {
                if ( !dateTimePickerPlus.Focused )
                {
                    if ( mtbDate.UnMaskedText != String.Empty )
                    {
                        if ( TransferService.IsTransferDateValid( mtbDate ) &&  // used as OCC date
                            IsNotFutureDate( mtbDate, UIErrorMessages.OCCURRENCECODE_DATE_IN_FUTURE_MSG ) &&
                            !IsBadMenstrualDate( occurrenceCodeComboBox, mtbDate ) )
                        {
                            EnableAllDatePickersClick();
                            ( ( OccurrenceCode )occurrenceCodeComboBox.SelectedItem ).OccurrenceDate = Convert.ToDateTime( mtbDate.Text );
                            UpdateModelOCCs();
                            RunRequiredRules();
                        }
                        Refresh();
                    }
                    else
                    {
                        ( ( OccurrenceCode )occurrenceCodeComboBox.SelectedItem ).OccurrenceDate = DateTime.MinValue;
                        UpdateModelOCCs();
                        RunRequiredRules();
                    }
                }
            }
            catch ( Exception )
            { }
        }

        private bool IsNotFutureDate( MaskedEditTextBox mtbDate, string errMsg )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            DateTime todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                                     User.GetCurrent().Facility.DSTOffset );

            DateTime comparedDate = Convert.ToDateTime( mtbDate.Text );

            if ( comparedDate > todaysDate )
            {
                UIColors.SetErrorBgColor( mtbDate );
                MessageBox.Show( errMsg, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );

                mtbDate.Focus();

                return false;
            }

            return true;
        }

        private bool IsBadMenstrualDate( PatientAccessComboBox cboOCC, MaskedEditTextBox mtbDate )
        {
            try
            {
                if ( ( ( OccurrenceCode )cboOCC.SelectedItem ).Code == "10" &&
                    Model.AdmitDate != DateTime.MinValue )
                {
                    DateTime menstrualDate = Convert.ToDateTime( mtbDate.Text );
                    if ( menstrualDate.AddYears( 1 ) < Model.AdmitDate ||
                        menstrualDate > Model.AdmitDate )
                    {
                        UIColors.SetErrorBgColor( mtbDate );

                        string errMsg = "Either the date of the last menstrual period\n("
                            + mtbDate.Text + ") or the admit date\n("
                            + CommonFormatting.LongDateFormat( Model.AdmitDate ) + ") must be modified, "
                            + UIErrorMessages.OCCURRENCECODE_BAD_MENSTRUAL_DATE_MSG;

                        MessageBox.Show( errMsg, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );

                        mtbDate.Focus();

                        return true;
                    }
                }
            }
            catch ( Exception )
            {
                return false;
            }

            return false;
        }

        private void UpdateModelOCCs()
        {
            var occurrenceCodeComparerByCodeAndDate = new OccurrenceCodeComparerByCodeAndDate();
            ((ArrayList)Model.OccurrenceCodes).Sort(occurrenceCodeComparerByCodeAndDate);

            var tempHolder = new Hashtable();
            for (var i = 0; i < Model.OccurrenceCodes.Count; i++)
            {
                tempHolder.Add(i, Model.OccurrenceCodes[i]);
            }
            Model.OccurrenceCodes.Clear();
            if (i_OCC1 != null)
            {
                tempHolder[0] = (OccurrenceCode) (i_OCC1.Clone());
            }
            else
            {
                tempHolder[0] = new OccurrenceCode();
            }

            int lastOCCNum = Convert.ToInt16(i_LastEnabledOCCName.Substring(10, 1));

            for (var i = 2; i <= lastOCCNum; i++)
            {
                PatientAccessComboBox cboOCC = occCboMgr.GetCombox(OCC_CBO_NAME_PREFIX + i);
                if (cboOCC.SelectedIndex > 0)
                {
                    tempHolder[i - 1] = (OccurrenceCode) ((OccurrenceCode) (cboOCC.SelectedItem)).Clone();
                }
                else
                {
                    tempHolder[i - 1] = null;
                }
            }
            foreach (
                var OC in
                    tempHolder.Values.Cast<OccurrenceCode>()
                        .Where(OC => OC != null && OC.IsAccidentCrimeOccurrenceCode()))
            {
                Model.AddOccurrenceCode(OC);
            }
            if (Model.OccurrenceCodes.Count == 0)
            {
                Model.AddOccurrenceCode(new OccurrenceCode());
            }
            foreach (var OC in tempHolder.Values.Cast<OccurrenceCode>().Where(OC => OC != null &&
                                                                                    !string.IsNullOrEmpty(OC.Code.Trim()) &&
                                                                                    !OC.IsAccidentCrimeOccurrenceCode())
                )
            {
                Model.AddOccurrenceCode(OC);
            }
            ((ArrayList)Model.OccurrenceCodes).Sort(occurrenceCodeComparerByCodeAndDate);
        }

        private void UpdateModelConditionCodes()
        {
            Model.ConditionCodes.Clear();

            int lastCondNum = Convert.ToInt16( i_LastEnabledCondName.Substring( 7, 1 ) );
            for ( int i = 1; i <= lastCondNum; i++ )
            {
                PatientAccessComboBox cboCond = condCboMgr.GetCombox( COND_CBO_NAME_PREFIX + i );
                if ( cboCond.SelectedIndex > 0 )
                {
                    Model.AddConditionCode( ( ConditionCode )( ( ConditionCode )( cboCond.SelectedItem ) ).Clone() );
                }
            }
        }

        private void ShowSpans()
        {
            if ( SystemGeneratedSpan1 != null )
            {
                FillSpan1ComboBoxWithSystemGeneratedSpanCode1();
            }

            if ( SystemGeneratedSpan2 != null )
            {
                FillSpan2ComboBoxWithSystemGeneratedSpanCode2();
            }
        }

        private void FillSpan1ComboBoxWithSystemGeneratedSpanCode1()
        {
            mtbSpan1FromDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SystemGeneratedSpan1.FromDate );
            mtbSpan1ToDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SystemGeneratedSpan1.ToDate );
            cboSpan1.SelectedItem = SystemGeneratedSpan1.SpanCode;
            mtbFacility.UnMaskedText = SystemGeneratedSpan1.Facility;
        }

        private void FillSpan2ComboBoxWithSystemGeneratedSpanCode2()
        {
            mtbSpan2FromDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SystemGeneratedSpan2.FromDate );
            mtbSpan2ToDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SystemGeneratedSpan2.ToDate );
            cboSpan2.SelectedItem = SystemGeneratedSpan2.SpanCode;
            RunRequiredRules();
        }

        private void SetSpan1ControlsDisableBGColor( bool disable )
        {
            if ( !disable )
            {
                UIColors.SetNormalBgColor( mtbSpan1FromDate );
                UIColors.SetNormalBgColor( mtbSpan1ToDate );
            }
            else
            {
                mtbSpan1FromDate.UnMaskedText = String.Empty;
                mtbSpan1ToDate.UnMaskedText = String.Empty;
                mtbFacility.UnMaskedText = String.Empty;

                mtbSpan1FromDate.BackColor = SystemColors.Control;
                mtbSpan1ToDate.BackColor = SystemColors.Control;
                Refresh();
            }
        }

        private void SetSpan2ControlsDisableBGColor( bool disable )
        {
            if ( !disable )
            {
                UIColors.SetNormalBgColor( mtbSpan2FromDate );
                UIColors.SetNormalBgColor( mtbSpan2ToDate );
            }
            else
            {
                mtbSpan2FromDate.UnMaskedText = string.Empty;
                mtbSpan2ToDate.UnMaskedText = string.Empty;

                mtbSpan2FromDate.BackColor = SystemColors.Control;
                mtbSpan2ToDate.BackColor = SystemColors.Control;
                Refresh();
            }
        }

        private void SetSpan1ControlsEnableStatus( bool status )
        {
            dtppSpan1From.Enabled = status;
            dtppSpan1To.Enabled = status;
            mtbSpan1FromDate.Enabled = status;
            mtbSpan1ToDate.Enabled = status;

            mtbFacility.Enabled = status;
        }

        private void SetSpan2ControlsEnableStatus( bool status )
        {
            dtppSpan2From.Enabled = status;
            dtppSpan2To.Enabled = status;
            mtbSpan2FromDate.Enabled = status;
            mtbSpan2ToDate.Enabled = status;
        }

        private void UpdateSpan1InModel()
        {
            OccurrenceSpan os1 = new OccurrenceSpan();

            if ( Model.OccurrenceSpans.Count > 0 )
            {
                if ( Model.OccurrenceSpans[0] != null )
                {
                    os1.IsSystemGenerated =
                        ( ( OccurrenceSpan )Model.OccurrenceSpans[0] ).IsSystemGenerated;
                }
                Model.OccurrenceSpans.RemoveAt( 0 );
            }

            if ( ( ( SpanCode )cboSpan1.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                os1.SpanCode = ( SpanCode )cboSpan1.SelectedItem;
                os1.FromDate = mtbSpan1FromDate.UnMaskedText != string.Empty ?
                    Convert.ToDateTime( mtbSpan1FromDate.Text ) :
                    DateTime.MinValue;
                os1.ToDate = mtbSpan1ToDate.UnMaskedText != string.Empty ?
                    Convert.ToDateTime( mtbSpan1ToDate.Text ) :
                    DateTime.MinValue;
                os1.Facility = mtbFacility.UnMaskedText;

                Model.OccurrenceSpans.Insert( 0, os1 );
            }
            else
            {
                Model.OccurrenceSpans.Insert( 0, null );
            }
        }

        private void UpdateSpan2InModel()
        {
            OccurrenceSpan os2 = new OccurrenceSpan();

            if ( Model.OccurrenceSpans.Count > 1 )
            {
                if ( Model.OccurrenceSpans[1] != null )
                {
                    os2.IsSystemGenerated =
                        ( ( OccurrenceSpan )Model.OccurrenceSpans[1] ).IsSystemGenerated;
                }
                Model.OccurrenceSpans.RemoveAt( 1 );
            }

            if ( ( ( SpanCode )cboSpan2.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                os2.SpanCode = ( SpanCode )cboSpan2.SelectedItem;
                os2.FromDate = mtbSpan2FromDate.UnMaskedText != string.Empty ?
                    Convert.ToDateTime( mtbSpan2FromDate.Text ) :
                    DateTime.MinValue;
                os2.ToDate = mtbSpan2ToDate.UnMaskedText != string.Empty ?
                    Convert.ToDateTime( mtbSpan2ToDate.Text ) :
                    DateTime.MinValue;

                Model.OccurrenceSpans.Add( os2 );
                // Make sure the 'to' date is not earlier than the 'from' date
            }
            else
            {
                Model.OccurrenceSpans.Add( null );
            }
        }

        private void CheckForValidSpan1Range()
        {
            if ( Model.OccurrenceSpans[0] != null )
            {
                OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[0];
                DateTime spanFromDate = sp.FromDate;
                DateTime spanToDate = sp.ToDate;

                if ( spanFromDate == DateTime.MinValue || spanToDate == DateTime.MinValue )
                {
                    return;
                }

                if ( spanFromDate > spanToDate )
                {
                    mtbSpan1ToDate.Focus();
                    UIColors.SetErrorBgColor( mtbSpan1ToDate );
                    MessageBox.Show( UIErrorMessages.TRANSFER_RANGE_INVALID_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void CheckForValidSpan2Range()
        {
            if ( Model.OccurrenceSpans[1] != null )
            {
                OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[1];
                DateTime spanFromDate = sp.FromDate;
                DateTime spanToDate = sp.ToDate;

                if ( spanFromDate == DateTime.MinValue || spanToDate == DateTime.MinValue )
                {
                    return;
                }

                if ( spanFromDate > spanToDate )
                {
                    mtbSpan2ToDate.Focus();
                    UIColors.SetErrorBgColor( mtbSpan2ToDate );
                    MessageBox.Show( UIErrorMessages.TRANSFER_RANGE_INVALID_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void RemoveSpanHandler()
        {
            cboSpan1.SelectedIndexChanged -= cboSpan1_SelectedIndexChanged;
            cboSpan2.SelectedIndexChanged -= cboSpan2_SelectedIndexChanged;
        }

        private void AddSpanHandler()
        {
            cboSpan1.SelectedIndexChanged += cboSpan1_SelectedIndexChanged;
            cboSpan2.SelectedIndexChanged += cboSpan2_SelectedIndexChanged;
        }

        private void EnableAllDatePickersClick()
        {
            foreach ( DateTimePickerPlus dtpPlus in i_DateTimePickers )
            {
                dtpPlus.SuppressClick = false;
            }
        }

        private void DisableOtherDatePickersClick( DateTimePickerPlus dtpp )
        {
            foreach ( DateTimePickerPlus dtpPlus in i_DateTimePickers )
            {
                if ( dtpPlus.Name != dtpp.Name )
                {
                    dtpPlus.SuppressClick = true;
                }
            }
        }

        private void ResetSelectedIndexForOccurenceCodeComboboxesExceptForFirstTwo()
        {
            for (int i = 2; i <= MAX_OCCURRENCECODES; i++)
            {
                var occCombo = occCboMgr.GetCombox(OCC_CBO_NAME_PREFIX + i);

                if (occCombo.Items.Count > 0)
                {
                    occCombo.SelectedIndex = 0;
                }
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpOccurrences = new System.Windows.Forms.GroupBox();
            this.lblOccCode1DateVal = new System.Windows.Forms.Label();
            this.mtbOcc8Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc7Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc6Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc5Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc4Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc3Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOcc2Date = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDate8 = new System.Windows.Forms.Label();
            this.lblDate7 = new System.Windows.Forms.Label();
            this.lblDate6 = new System.Windows.Forms.Label();
            this.lblDate5 = new System.Windows.Forms.Label();
            this.lblDate4 = new System.Windows.Forms.Label();
            this.lblDate3 = new System.Windows.Forms.Label();
            this.lblDate2 = new System.Windows.Forms.Label();
            this.lblOccCode1Date = new System.Windows.Forms.Label();
            this.cboOccCode8 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode7 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode6 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode5 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode4 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode3 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboOccCode2 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblOccCode1Val = new System.Windows.Forms.Label();
            this.lblOccCode8 = new System.Windows.Forms.Label();
            this.lblOccCode7 = new System.Windows.Forms.Label();
            this.lblOccCode6 = new System.Windows.Forms.Label();
            this.lblOccCode5 = new System.Windows.Forms.Label();
            this.lblOccCode4 = new System.Windows.Forms.Label();
            this.lblOccCode3 = new System.Windows.Forms.Label();
            this.lblOccCode1 = new System.Windows.Forms.Label();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblOccCode2 = new System.Windows.Forms.Label();
            this.dtppOcc2Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc3Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc4Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc5Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc6Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc7Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppOcc8Date = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dateTimePicker7 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker6 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.grpOccSpans = new System.Windows.Forms.GroupBox();
            this.mtbSpan2ToDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan2ToDate = new System.Windows.Forms.Label();
            this.mtbSpan2FromDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan2FromDate = new System.Windows.Forms.Label();
            this.mtbFacility = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFac = new System.Windows.Forms.Label();
            this.mtbSpan1ToDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan1ToDate = new System.Windows.Forms.Label();
            this.mtbSpan1FromDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan1FromDate = new System.Windows.Forms.Label();
            this.cboSpan2 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboSpan1 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblSpan2 = new System.Windows.Forms.Label();
            this.lblSpan1 = new System.Windows.Forms.Label();
            this.dtppSpan1From = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppSpan2From = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppSpan1To = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.dtppSpan2To = new PatientAccess.UI.CommonControls.DateTimePickerPlus();
            this.grpConditions = new System.Windows.Forms.GroupBox();
            this.cboCond7 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond6 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond5 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond4 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond3 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond2 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboCond1 = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblCond7 = new System.Windows.Forms.Label();
            this.lblCond6 = new System.Windows.Forms.Label();
            this.lblCond5 = new System.Windows.Forms.Label();
            this.lblCond4 = new System.Windows.Forms.Label();
            this.lblCond3 = new System.Windows.Forms.Label();
            this.lblCond2 = new System.Windows.Forms.Label();
            this.lblCond1 = new System.Windows.Forms.Label();
            this.grpOccurrences.SuspendLayout();
            this.grpOccSpans.SuspendLayout();
            this.grpConditions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOccurrences
            // 
            this.grpOccurrences.Controls.Add( this.lblOccCode1DateVal );
            this.grpOccurrences.Controls.Add( this.mtbOcc8Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc7Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc6Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc5Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc4Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc3Date );
            this.grpOccurrences.Controls.Add( this.mtbOcc2Date );
            this.grpOccurrences.Controls.Add( this.lblDate8 );
            this.grpOccurrences.Controls.Add( this.lblDate7 );
            this.grpOccurrences.Controls.Add( this.lblDate6 );
            this.grpOccurrences.Controls.Add( this.lblDate5 );
            this.grpOccurrences.Controls.Add( this.lblDate4 );
            this.grpOccurrences.Controls.Add( this.lblDate3 );
            this.grpOccurrences.Controls.Add( this.lblDate2 );
            this.grpOccurrences.Controls.Add( this.lblOccCode1Date );
            this.grpOccurrences.Controls.Add( this.cboOccCode8 );
            this.grpOccurrences.Controls.Add( this.cboOccCode7 );
            this.grpOccurrences.Controls.Add( this.cboOccCode6 );
            this.grpOccurrences.Controls.Add( this.cboOccCode5 );
            this.grpOccurrences.Controls.Add( this.cboOccCode4 );
            this.grpOccurrences.Controls.Add( this.cboOccCode3 );
            this.grpOccurrences.Controls.Add( this.cboOccCode2 );
            this.grpOccurrences.Controls.Add( this.lblOccCode1Val );
            this.grpOccurrences.Controls.Add( this.lblOccCode8 );
            this.grpOccurrences.Controls.Add( this.lblOccCode7 );
            this.grpOccurrences.Controls.Add( this.lblOccCode6 );
            this.grpOccurrences.Controls.Add( this.lblOccCode5 );
            this.grpOccurrences.Controls.Add( this.lblOccCode4 );
            this.grpOccurrences.Controls.Add( this.lblOccCode3 );
            this.grpOccurrences.Controls.Add( this.lblOccCode1 );
            this.grpOccurrences.Controls.Add( this.lineLabel1 );
            this.grpOccurrences.Controls.Add( this.lblOccCode2 );
            this.grpOccurrences.Controls.Add( this.dtppOcc2Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc3Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc4Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc5Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc6Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc7Date );
            this.grpOccurrences.Controls.Add( this.dtppOcc8Date );
            this.grpOccurrences.Location = new System.Drawing.Point( 10, 14 );
            this.grpOccurrences.Name = "grpOccurrences";
            this.grpOccurrences.Size = new System.Drawing.Size( 505, 266 );
            this.grpOccurrences.TabIndex = 0;
            this.grpOccurrences.TabStop = false;
            this.grpOccurrences.Text = "Occurrences";
            // 
            // lblOccCode1DateVal
            // 
            this.lblOccCode1DateVal.Location = new System.Drawing.Point( 395, 31 );
            this.lblOccCode1DateVal.Name = "lblOccCode1DateVal";
            this.lblOccCode1DateVal.Size = new System.Drawing.Size( 89, 18 );
            this.lblOccCode1DateVal.TabIndex = 61;
            // 
            // mtbOcc8Date
            // 
            this.mtbOcc8Date.Enabled = false;
            this.mtbOcc8Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc8Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc8Date.Location = new System.Drawing.Point( 396, 230 );
            this.mtbOcc8Date.Mask = "  /  /";
            this.mtbOcc8Date.MaxLength = 10;
            this.mtbOcc8Date.Name = "mtbOcc8Date";
            this.mtbOcc8Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc8Date.TabIndex = 13;
            this.mtbOcc8Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc8Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc8Date_Validating );
            // 
            // mtbOcc7Date
            // 
            this.mtbOcc7Date.Enabled = false;
            this.mtbOcc7Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc7Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc7Date.Location = new System.Drawing.Point( 396, 204 );
            this.mtbOcc7Date.Mask = "  /  /";
            this.mtbOcc7Date.MaxLength = 10;
            this.mtbOcc7Date.Name = "mtbOcc7Date";
            this.mtbOcc7Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc7Date.TabIndex = 11;
            this.mtbOcc7Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc7Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc7Date_Validating );
            // 
            // mtbOcc6Date
            // 
            this.mtbOcc6Date.Enabled = false;
            this.mtbOcc6Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc6Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc6Date.Location = new System.Drawing.Point( 396, 177 );
            this.mtbOcc6Date.Mask = "  /  /";
            this.mtbOcc6Date.MaxLength = 10;
            this.mtbOcc6Date.Name = "mtbOcc6Date";
            this.mtbOcc6Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc6Date.TabIndex = 9;
            this.mtbOcc6Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc6Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc6Date_Validating );
            // 
            // mtbOcc5Date
            // 
            this.mtbOcc5Date.Enabled = false;
            this.mtbOcc5Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc5Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc5Date.Location = new System.Drawing.Point( 396, 150 );
            this.mtbOcc5Date.Mask = "  /  /";
            this.mtbOcc5Date.MaxLength = 10;
            this.mtbOcc5Date.Name = "mtbOcc5Date";
            this.mtbOcc5Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc5Date.TabIndex = 7;
            this.mtbOcc5Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc5Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc5Date_Validating );
            // 
            // mtbOcc4Date
            // 
            this.mtbOcc4Date.Enabled = false;
            this.mtbOcc4Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc4Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc4Date.Location = new System.Drawing.Point( 396, 122 );
            this.mtbOcc4Date.Mask = "  /  /";
            this.mtbOcc4Date.MaxLength = 10;
            this.mtbOcc4Date.Name = "mtbOcc4Date";
            this.mtbOcc4Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc4Date.TabIndex = 5;
            this.mtbOcc4Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc4Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc4Date_Validating );
            // 
            // mtbOcc3Date
            // 
            this.mtbOcc3Date.Enabled = false;
            this.mtbOcc3Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc3Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc3Date.Location = new System.Drawing.Point( 396, 94 );
            this.mtbOcc3Date.Mask = "  /  /";
            this.mtbOcc3Date.MaxLength = 10;
            this.mtbOcc3Date.Name = "mtbOcc3Date";
            this.mtbOcc3Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc3Date.TabIndex = 3;
            this.mtbOcc3Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc3Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc3Date_Validating );
            // 
            // mtbOcc2Date
            // 
            this.mtbOcc2Date.Enabled = false;
            this.mtbOcc2Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc2Date.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbOcc2Date.Location = new System.Drawing.Point( 396, 67 );
            this.mtbOcc2Date.Mask = "  /  /";
            this.mtbOcc2Date.MaxLength = 10;
            this.mtbOcc2Date.Name = "mtbOcc2Date";
            this.mtbOcc2Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc2Date.TabIndex = 1;
            this.mtbOcc2Date.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbOcc2Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc2Date_Validating );
            // 
            // lblDate8
            // 
            this.lblDate8.Location = new System.Drawing.Point( 357, 233 );
            this.lblDate8.Name = "lblDate8";
            this.lblDate8.Size = new System.Drawing.Size( 33, 16 );
            this.lblDate8.TabIndex = 24;
            this.lblDate8.Text = "Date:";
            // 
            // lblDate7
            // 
            this.lblDate7.Location = new System.Drawing.Point( 357, 208 );
            this.lblDate7.Name = "lblDate7";
            this.lblDate7.Size = new System.Drawing.Size( 33, 12 );
            this.lblDate7.TabIndex = 23;
            this.lblDate7.Text = "Date:";
            // 
            // lblDate6
            // 
            this.lblDate6.Location = new System.Drawing.Point( 357, 180 );
            this.lblDate6.Name = "lblDate6";
            this.lblDate6.Size = new System.Drawing.Size( 33, 16 );
            this.lblDate6.TabIndex = 22;
            this.lblDate6.Text = "Date:";
            // 
            // lblDate5
            // 
            this.lblDate5.Location = new System.Drawing.Point( 357, 152 );
            this.lblDate5.Name = "lblDate5";
            this.lblDate5.Size = new System.Drawing.Size( 33, 12 );
            this.lblDate5.TabIndex = 21;
            this.lblDate5.Text = "Date:";
            // 
            // lblDate4
            // 
            this.lblDate4.Location = new System.Drawing.Point( 357, 125 );
            this.lblDate4.Name = "lblDate4";
            this.lblDate4.Size = new System.Drawing.Size( 33, 16 );
            this.lblDate4.TabIndex = 20;
            this.lblDate4.Text = "Date:";
            // 
            // lblDate3
            // 
            this.lblDate3.Location = new System.Drawing.Point( 356, 97 );
            this.lblDate3.Name = "lblDate3";
            this.lblDate3.Size = new System.Drawing.Size( 33, 14 );
            this.lblDate3.TabIndex = 19;
            this.lblDate3.Text = "Date:";
            // 
            // lblDate2
            // 
            this.lblDate2.Location = new System.Drawing.Point( 357, 70 );
            this.lblDate2.Name = "lblDate2";
            this.lblDate2.Size = new System.Drawing.Size( 33, 25 );
            this.lblDate2.TabIndex = 18;
            this.lblDate2.Text = "Date:";
            // 
            // lblOccCode1Date
            // 
            this.lblOccCode1Date.Location = new System.Drawing.Point( 356, 31 );
            this.lblOccCode1Date.Name = "lblOccCode1Date";
            this.lblOccCode1Date.Size = new System.Drawing.Size( 34, 14 );
            this.lblOccCode1Date.TabIndex = 17;
            this.lblOccCode1Date.Text = "Date:";
            // 
            // cboOccCode8
            // 
            this.cboOccCode8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode8.Enabled = false;
            this.cboOccCode8.Location = new System.Drawing.Point( 115, 229 );
            this.cboOccCode8.Name = "cboOccCode8";
            this.cboOccCode8.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode8.TabIndex = 12;
            this.cboOccCode8.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode8_Validating );
            this.cboOccCode8.SelectedIndexChanged += new System.EventHandler( this.cboOccCode8_SelectedIndexChanged );
            this.cboOccCode8.Enter += new System.EventHandler( this.cboOccCode8_Enter );
            // 
            // cboOccCode7
            // 
            this.cboOccCode7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode7.Enabled = false;
            this.cboOccCode7.Location = new System.Drawing.Point( 115, 203 );
            this.cboOccCode7.Name = "cboOccCode7";
            this.cboOccCode7.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode7.TabIndex = 10;
            this.cboOccCode7.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode7_Validating );
            this.cboOccCode7.SelectedIndexChanged += new System.EventHandler( this.cboOccCode7_SelectedIndexChanged );
            this.cboOccCode7.Enter += new System.EventHandler( this.cboOccCode7_Enter );
            // 
            // cboOccCode6
            // 
            this.cboOccCode6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode6.Enabled = false;
            this.cboOccCode6.Location = new System.Drawing.Point( 116, 175 );
            this.cboOccCode6.Name = "cboOccCode6";
            this.cboOccCode6.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode6.TabIndex = 8;
            this.cboOccCode6.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode6_Validating );
            this.cboOccCode6.SelectedIndexChanged += new System.EventHandler( this.cboOccCode6_SelectedIndexChanged );
            this.cboOccCode6.Enter += new System.EventHandler( this.cboOccCode6_Enter );
            // 
            // cboOccCode5
            // 
            this.cboOccCode5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode5.Enabled = false;
            this.cboOccCode5.Location = new System.Drawing.Point( 116, 148 );
            this.cboOccCode5.Name = "cboOccCode5";
            this.cboOccCode5.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode5.TabIndex = 6;
            this.cboOccCode5.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode5_Validating );
            this.cboOccCode5.SelectedIndexChanged += new System.EventHandler( this.cboOccCode5_SelectedIndexChanged );
            this.cboOccCode5.Enter += new System.EventHandler( this.cboOccCode5_Enter );
            // 
            // cboOccCode4
            // 
            this.cboOccCode4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode4.Enabled = false;
            this.cboOccCode4.Location = new System.Drawing.Point( 116, 120 );
            this.cboOccCode4.Name = "cboOccCode4";
            this.cboOccCode4.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode4.TabIndex = 4;
            this.cboOccCode4.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode4_Validating );
            this.cboOccCode4.SelectedIndexChanged += new System.EventHandler( this.cboOccCode4_SelectedIndexChanged );
            this.cboOccCode4.Enter += new System.EventHandler( this.cboOccCode4_Enter );
            // 
            // cboOccCode3
            // 
            this.cboOccCode3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode3.Enabled = false;
            this.cboOccCode3.Location = new System.Drawing.Point( 116, 93 );
            this.cboOccCode3.Name = "cboOccCode3";
            this.cboOccCode3.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode3.TabIndex = 2;
            this.cboOccCode3.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode3_Validating );
            this.cboOccCode3.SelectedIndexChanged += new System.EventHandler( this.cboOccCode3_SelectedIndexChanged );
            this.cboOccCode3.Enter += new System.EventHandler( this.cboOccCode3_Enter );
            // 
            // cboOccCode2
            // 
            this.cboOccCode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode2.Enabled = false;
            this.cboOccCode2.Location = new System.Drawing.Point( 116, 66 );
            this.cboOccCode2.Name = "cboOccCode2";
            this.cboOccCode2.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode2.TabIndex = 0;
            this.cboOccCode2.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode2_Validating );
            this.cboOccCode2.SelectedIndexChanged += new System.EventHandler( this.cboOccCode2_SelectedIndexChanged );
            this.cboOccCode2.Enter += new System.EventHandler( this.cboOccCode2_Enter );
            // 
            // lblOccCode1Val
            // 
            this.lblOccCode1Val.Location = new System.Drawing.Point( 119, 29 );
            this.lblOccCode1Val.Name = "lblOccCode1Val";
            this.lblOccCode1Val.Size = new System.Drawing.Size( 222, 19 );
            this.lblOccCode1Val.TabIndex = 9;
            // 
            // lblOccCode8
            // 
            this.lblOccCode8.Location = new System.Drawing.Point( 8, 232 );
            this.lblOccCode8.Name = "lblOccCode8";
            this.lblOccCode8.Size = new System.Drawing.Size( 105, 17 );
            this.lblOccCode8.TabIndex = 8;
            this.lblOccCode8.Text = "Occurrence code 8:";
            // 
            // lblOccCode7
            // 
            this.lblOccCode7.Location = new System.Drawing.Point( 8, 207 );
            this.lblOccCode7.Name = "lblOccCode7";
            this.lblOccCode7.Size = new System.Drawing.Size( 105, 17 );
            this.lblOccCode7.TabIndex = 7;
            this.lblOccCode7.Text = "Occurrence code 7:";
            // 
            // lblOccCode6
            // 
            this.lblOccCode6.Location = new System.Drawing.Point( 8, 182 );
            this.lblOccCode6.Name = "lblOccCode6";
            this.lblOccCode6.Size = new System.Drawing.Size( 103, 16 );
            this.lblOccCode6.TabIndex = 6;
            this.lblOccCode6.Text = "Occurrence code 6:";
            // 
            // lblOccCode5
            // 
            this.lblOccCode5.Location = new System.Drawing.Point( 8, 155 );
            this.lblOccCode5.Name = "lblOccCode5";
            this.lblOccCode5.Size = new System.Drawing.Size( 105, 18 );
            this.lblOccCode5.TabIndex = 5;
            this.lblOccCode5.Text = "Occurrence code 5:";
            // 
            // lblOccCode4
            // 
            this.lblOccCode4.Location = new System.Drawing.Point( 8, 127 );
            this.lblOccCode4.Name = "lblOccCode4";
            this.lblOccCode4.Size = new System.Drawing.Size( 105, 16 );
            this.lblOccCode4.TabIndex = 4;
            this.lblOccCode4.Text = "Occurrence code 4:";
            // 
            // lblOccCode3
            // 
            this.lblOccCode3.Location = new System.Drawing.Point( 8, 99 );
            this.lblOccCode3.Name = "lblOccCode3";
            this.lblOccCode3.Size = new System.Drawing.Size( 105, 16 );
            this.lblOccCode3.TabIndex = 3;
            this.lblOccCode3.Text = "Occurrence code 3:";
            // 
            // lblOccCode1
            // 
            this.lblOccCode1.Location = new System.Drawing.Point( 9, 31 );
            this.lblOccCode1.Name = "lblOccCode1";
            this.lblOccCode1.Size = new System.Drawing.Size( 105, 23 );
            this.lblOccCode1.TabIndex = 0;
            this.lblOccCode1.Text = "Occurrence code 1:";
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point( 8, 43 );
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size( 482, 18 );
            this.lineLabel1.TabIndex = 2;
            this.lineLabel1.TabStop = false;
            // 
            // lblOccCode2
            // 
            this.lblOccCode2.Location = new System.Drawing.Point( 8, 71 );
            this.lblOccCode2.Name = "lblOccCode2";
            this.lblOccCode2.Size = new System.Drawing.Size( 105, 20 );
            this.lblOccCode2.TabIndex = 1;
            this.lblOccCode2.Text = "Occurrence code 2:";
            // 
            // dtppOcc2Date
            // 
            this.dtppOcc2Date.Location = new System.Drawing.Point( 470, 67 );
            this.dtppOcc2Date.Name = "dtppOcc2Date";
            this.dtppOcc2Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc2Date.TabIndex = 5;
            this.dtppOcc2Date.CloseUp += new System.EventHandler( this.dtppOcc2Date_CloseUp );
            // 
            // dtppOcc3Date
            // 
            this.dtppOcc3Date.Location = new System.Drawing.Point( 470, 94 );
            this.dtppOcc3Date.Name = "dtppOcc3Date";
            this.dtppOcc3Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc3Date.TabIndex = 61;
            this.dtppOcc3Date.CloseUp += new System.EventHandler( this.dtppOcc3Date_CloseUp );
            // 
            // dtppOcc4Date
            // 
            this.dtppOcc4Date.Location = new System.Drawing.Point( 470, 122 );
            this.dtppOcc4Date.Name = "dtppOcc4Date";
            this.dtppOcc4Date.Size = new System.Drawing.Size( 22, 20 );
            this.dtppOcc4Date.TabIndex = 61;
            this.dtppOcc4Date.CloseUp += new System.EventHandler( this.dtppOcc4Date_CloseUp );
            // 
            // dtppOcc5Date
            // 
            this.dtppOcc5Date.Location = new System.Drawing.Point( 470, 150 );
            this.dtppOcc5Date.Name = "dtppOcc5Date";
            this.dtppOcc5Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc5Date.TabIndex = 61;
            this.dtppOcc5Date.CloseUp += new System.EventHandler( this.dtppOcc5Date_CloseUp );
            // 
            // dtppOcc6Date
            // 
            this.dtppOcc6Date.Location = new System.Drawing.Point( 470, 177 );
            this.dtppOcc6Date.Name = "dtppOcc6Date";
            this.dtppOcc6Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc6Date.TabIndex = 61;
            this.dtppOcc6Date.CloseUp += new System.EventHandler( this.dtppOcc6Date_CloseUp );
            // 
            // dtppOcc7Date
            // 
            this.dtppOcc7Date.Location = new System.Drawing.Point( 470, 204 );
            this.dtppOcc7Date.Name = "dtppOcc7Date";
            this.dtppOcc7Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc7Date.TabIndex = 61;
            this.dtppOcc7Date.CloseUp += new System.EventHandler( this.dtppOcc7Date_CloseUp );
            // 
            // dtppOcc8Date
            // 
            this.dtppOcc8Date.Location = new System.Drawing.Point( 470, 230 );
            this.dtppOcc8Date.Name = "dtppOcc8Date";
            this.dtppOcc8Date.Size = new System.Drawing.Size( 21, 20 );
            this.dtppOcc8Date.TabIndex = 61;
            this.dtppOcc8Date.CloseUp += new System.EventHandler( this.dtppOcc8Date_CloseUp );
            // 
            // dateTimePicker7
            // 
            this.dateTimePicker7.Location = new System.Drawing.Point( 0, 0 );
            this.dateTimePicker7.Name = "dateTimePicker7";
            this.dateTimePicker7.TabIndex = 0;
            // 
            // dateTimePicker6
            // 
            this.dateTimePicker6.Location = new System.Drawing.Point( 0, 0 );
            this.dateTimePicker6.Name = "dateTimePicker6";
            this.dateTimePicker6.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 713, 127 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 1, 0 );
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // grpOccSpans
            // 
            this.grpOccSpans.Controls.Add( this.mtbSpan2ToDate );
            this.grpOccSpans.Controls.Add( this.lblSpan2ToDate );
            this.grpOccSpans.Controls.Add( this.mtbSpan2FromDate );
            this.grpOccSpans.Controls.Add( this.lblSpan2FromDate );
            this.grpOccSpans.Controls.Add( this.mtbFacility );
            this.grpOccSpans.Controls.Add( this.lblFac );
            this.grpOccSpans.Controls.Add( this.mtbSpan1ToDate );
            this.grpOccSpans.Controls.Add( this.lblSpan1ToDate );
            this.grpOccSpans.Controls.Add( this.mtbSpan1FromDate );
            this.grpOccSpans.Controls.Add( this.lblSpan1FromDate );
            this.grpOccSpans.Controls.Add( this.cboSpan2 );
            this.grpOccSpans.Controls.Add( this.cboSpan1 );
            this.grpOccSpans.Controls.Add( this.lblSpan2 );
            this.grpOccSpans.Controls.Add( this.lblSpan1 );
            this.grpOccSpans.Controls.Add( this.dtppSpan1From );
            this.grpOccSpans.Controls.Add( this.dtppSpan2From );
            this.grpOccSpans.Controls.Add( this.dtppSpan1To );
            this.grpOccSpans.Controls.Add( this.dtppSpan2To );
            this.grpOccSpans.Location = new System.Drawing.Point( 10, 290 );
            this.grpOccSpans.Name = "grpOccSpans";
            this.grpOccSpans.Size = new System.Drawing.Size( 770, 80 );
            this.grpOccSpans.TabIndex = 3;
            this.grpOccSpans.TabStop = false;
            this.grpOccSpans.Text = "Occurrence spans";
            // 
            // mtbSpan2ToDate
            // 
            this.mtbSpan2ToDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSpan2ToDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbSpan2ToDate.Location = new System.Drawing.Point( 530, 50 );
            this.mtbSpan2ToDate.Mask = "  /  /";
            this.mtbSpan2ToDate.MaxLength = 10;
            this.mtbSpan2ToDate.Name = "mtbSpan2ToDate";
            this.mtbSpan2ToDate.Size = new System.Drawing.Size( 73, 20 );
            this.mtbSpan2ToDate.TabIndex = 27;
            this.mtbSpan2ToDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbSpan2ToDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan2ToDate_Validating );
            this.mtbSpan2ToDate.Enter += new System.EventHandler( this.mtbSpan2ToDate_Enter );
            // 
            // lblSpan2ToDate
            // 
            this.lblSpan2ToDate.Location = new System.Drawing.Point( 480, 54 );
            this.lblSpan2ToDate.Name = "lblSpan2ToDate";
            this.lblSpan2ToDate.Size = new System.Drawing.Size( 46, 19 );
            this.lblSpan2ToDate.TabIndex = 62;
            this.lblSpan2ToDate.Text = "To date:";
            // 
            // mtbSpan2FromDate
            // 
            this.mtbSpan2FromDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSpan2FromDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbSpan2FromDate.Location = new System.Drawing.Point( 366, 50 );
            this.mtbSpan2FromDate.Mask = "  /  /";
            this.mtbSpan2FromDate.MaxLength = 10;
            this.mtbSpan2FromDate.Name = "mtbSpan2FromDate";
            this.mtbSpan2FromDate.Size = new System.Drawing.Size( 75, 20 );
            this.mtbSpan2FromDate.TabIndex = 26;
            this.mtbSpan2FromDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbSpan2FromDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan2FromDate_Validating );
            this.mtbSpan2FromDate.Enter += new System.EventHandler( this.mtbSpan2FromDate_Enter );
            // 
            // lblSpan2FromDate
            // 
            this.lblSpan2FromDate.Location = new System.Drawing.Point( 310, 53 );
            this.lblSpan2FromDate.Name = "lblSpan2FromDate";
            this.lblSpan2FromDate.Size = new System.Drawing.Size( 60, 20 );
            this.lblSpan2FromDate.TabIndex = 59;
            this.lblSpan2FromDate.Text = "From date:";
            // 
            // mtbFacility
            // 
            this.mtbFacility.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFacility.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFacility.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.mtbFacility.Location = new System.Drawing.Point( 685, 24 );
            this.mtbFacility.Mask = "";
            this.mtbFacility.MaxLength = 13;
            this.mtbFacility.Name = "mtbFacility";
            this.mtbFacility.Size = new System.Drawing.Size( 69, 20 );
            this.mtbFacility.TabIndex = 24;
            this.mtbFacility.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbFacility.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFacility_Validating );
            // 
            // lblFac
            // 
            this.lblFac.Location = new System.Drawing.Point( 644, 28 );
            this.lblFac.Name = "lblFac";
            this.lblFac.Size = new System.Drawing.Size( 52, 17 );
            this.lblFac.TabIndex = 57;
            this.lblFac.Text = "Facility:";
            // 
            // mtbSpan1ToDate
            // 
            this.mtbSpan1ToDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSpan1ToDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbSpan1ToDate.Location = new System.Drawing.Point( 530, 24 );
            this.mtbSpan1ToDate.Mask = "  /  /";
            this.mtbSpan1ToDate.MaxLength = 10;
            this.mtbSpan1ToDate.Name = "mtbSpan1ToDate";
            this.mtbSpan1ToDate.Size = new System.Drawing.Size( 73, 20 );
            this.mtbSpan1ToDate.TabIndex = 23;
            this.mtbSpan1ToDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbSpan1ToDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan1ToDate_Validating );
            this.mtbSpan1ToDate.Enter += new System.EventHandler( this.mtbSpan1ToDate_Enter );
            // 
            // lblSpan1ToDate
            // 
            this.lblSpan1ToDate.Location = new System.Drawing.Point( 480, 28 );
            this.lblSpan1ToDate.Name = "lblSpan1ToDate";
            this.lblSpan1ToDate.Size = new System.Drawing.Size( 48, 16 );
            this.lblSpan1ToDate.TabIndex = 49;
            this.lblSpan1ToDate.Text = "To date:";
            // 
            // mtbSpan1FromDate
            // 
            this.mtbSpan1FromDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSpan1FromDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbSpan1FromDate.Location = new System.Drawing.Point( 367, 23 );
            this.mtbSpan1FromDate.Mask = "  /  /";
            this.mtbSpan1FromDate.MaxLength = 10;
            this.mtbSpan1FromDate.Name = "mtbSpan1FromDate";
            this.mtbSpan1FromDate.Size = new System.Drawing.Size( 74, 20 );
            this.mtbSpan1FromDate.TabIndex = 22;
            this.mtbSpan1FromDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbSpan1FromDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan1FromDate_Validating );
            this.mtbSpan1FromDate.Enter += new System.EventHandler( this.mtbSpan1FromDate_Enter );
            // 
            // lblSpan1FromDate
            // 
            this.lblSpan1FromDate.Location = new System.Drawing.Point( 310, 27 );
            this.lblSpan1FromDate.Name = "lblSpan1FromDate";
            this.lblSpan1FromDate.Size = new System.Drawing.Size( 59, 19 );
            this.lblSpan1FromDate.TabIndex = 45;
            this.lblSpan1FromDate.Text = "From date:";
            // 
            // cboSpan2
            // 
            this.cboSpan2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpan2.Location = new System.Drawing.Point( 83, 50 );
            this.cboSpan2.Name = "cboSpan2";
            this.cboSpan2.Size = new System.Drawing.Size( 206, 21 );
            this.cboSpan2.TabIndex = 25;
            this.cboSpan2.Validating += new System.ComponentModel.CancelEventHandler( this.cboSpan2_Validating );
            this.cboSpan2.SelectedIndexChanged += new System.EventHandler( this.cboSpan2_SelectedIndexChanged );
            // 
            // cboSpan1
            // 
            this.cboSpan1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpan1.Location = new System.Drawing.Point( 83, 23 );
            this.cboSpan1.Name = "cboSpan1";
            this.cboSpan1.Size = new System.Drawing.Size( 207, 21 );
            this.cboSpan1.TabIndex = 21;
            this.cboSpan1.Validating += new System.ComponentModel.CancelEventHandler( this.cboSpan1_Validating );
            this.cboSpan1.SelectedIndexChanged += new System.EventHandler( this.cboSpan1_SelectedIndexChanged );
            // 
            // lblSpan2
            // 
            this.lblSpan2.Location = new System.Drawing.Point( 7, 54 );
            this.lblSpan2.Name = "lblSpan2";
            this.lblSpan2.Size = new System.Drawing.Size( 74, 17 );
            this.lblSpan2.TabIndex = 37;
            this.lblSpan2.Text = "Span code 2:";
            // 
            // lblSpan1
            // 
            this.lblSpan1.Location = new System.Drawing.Point( 8, 27 );
            this.lblSpan1.Name = "lblSpan1";
            this.lblSpan1.Size = new System.Drawing.Size( 71, 16 );
            this.lblSpan1.TabIndex = 35;
            this.lblSpan1.Text = "Span code 1:";
            // 
            // dtppSpan1From
            // 
            this.dtppSpan1From.Location = new System.Drawing.Point( 440, 23 );
            this.dtppSpan1From.Name = "dtppSpan1From";
            this.dtppSpan1From.Size = new System.Drawing.Size( 21, 20 );
            this.dtppSpan1From.TabIndex = 4;
            this.dtppSpan1From.CloseUp += new System.EventHandler( this.dtppSpan1From_CloseUp );
            // 
            // dtppSpan2From
            // 
            this.dtppSpan2From.Location = new System.Drawing.Point( 440, 50 );
            this.dtppSpan2From.Name = "dtppSpan2From";
            this.dtppSpan2From.Size = new System.Drawing.Size( 22, 20 );
            this.dtppSpan2From.TabIndex = 5;
            this.dtppSpan2From.CloseUp += new System.EventHandler( this.dtppSpan2From_CloseUp );
            // 
            // dtppSpan1To
            // 
            this.dtppSpan1To.Location = new System.Drawing.Point( 603, 24 );
            this.dtppSpan1To.Name = "dtppSpan1To";
            this.dtppSpan1To.Size = new System.Drawing.Size( 22, 20 );
            this.dtppSpan1To.TabIndex = 6;
            this.dtppSpan1To.CloseUp += new System.EventHandler( this.dtppSpan1To_CloseUp );
            // 
            // dtppSpan2To
            // 
            this.dtppSpan2To.Location = new System.Drawing.Point( 602, 50 );
            this.dtppSpan2To.Name = "dtppSpan2To";
            this.dtppSpan2To.Size = new System.Drawing.Size( 22, 20 );
            this.dtppSpan2To.TabIndex = 6;
            this.dtppSpan2To.CloseUp += new System.EventHandler( this.dtppSpan2To_CloseUp );
            // 
            // grpConditions
            // 
            this.grpConditions.Controls.Add( this.cboCond7 );
            this.grpConditions.Controls.Add( this.cboCond6 );
            this.grpConditions.Controls.Add( this.cboCond5 );
            this.grpConditions.Controls.Add( this.cboCond4 );
            this.grpConditions.Controls.Add( this.cboCond3 );
            this.grpConditions.Controls.Add( this.cboCond2 );
            this.grpConditions.Controls.Add( this.cboCond1 );
            this.grpConditions.Controls.Add( this.lblCond7 );
            this.grpConditions.Controls.Add( this.lblCond6 );
            this.grpConditions.Controls.Add( this.lblCond5 );
            this.grpConditions.Controls.Add( this.lblCond4 );
            this.grpConditions.Controls.Add( this.lblCond3 );
            this.grpConditions.Controls.Add( this.lblCond2 );
            this.grpConditions.Controls.Add( this.lblCond1 );
            this.grpConditions.Location = new System.Drawing.Point( 538, 15 );
            this.grpConditions.Name = "grpConditions";
            this.grpConditions.Size = new System.Drawing.Size( 337, 230 );
            this.grpConditions.TabIndex = 2;
            this.grpConditions.TabStop = false;
            this.grpConditions.Text = "Conditions";
            // 
            // cboCond7
            // 
            this.cboCond7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond7.Enabled = false;
            this.cboCond7.Location = new System.Drawing.Point( 105, 195 );
            this.cboCond7.Name = "cboCond7";
            this.cboCond7.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond7.TabIndex = 20;
            this.cboCond7.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond7_Validating );
            this.cboCond7.SelectedIndexChanged += new System.EventHandler( this.cboCond7_SelectedIndexChanged );
            this.cboCond7.Enter += new System.EventHandler( this.cboCond7_Enter );
            // 
            // cboCond6
            // 
            this.cboCond6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond6.Enabled = false;
            this.cboCond6.Location = new System.Drawing.Point( 105, 167 );
            this.cboCond6.Name = "cboCond6";
            this.cboCond6.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond6.TabIndex = 19;
            this.cboCond6.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond6_Validating );
            this.cboCond6.SelectedIndexChanged += new System.EventHandler( this.cboCond6_SelectedIndexChanged );
            this.cboCond6.Enter += new System.EventHandler( this.cboCond6_Enter );
            // 
            // cboCond5
            // 
            this.cboCond5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond5.Enabled = false;
            this.cboCond5.Location = new System.Drawing.Point( 105, 138 );
            this.cboCond5.Name = "cboCond5";
            this.cboCond5.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond5.TabIndex = 18;
            this.cboCond5.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond5_Validating );
            this.cboCond5.SelectedIndexChanged += new System.EventHandler( this.cboCond5_SelectedIndexChanged );
            this.cboCond5.Enter += new System.EventHandler( this.cboCond5_Enter );
            // 
            // cboCond4
            // 
            this.cboCond4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond4.Enabled = false;
            this.cboCond4.Location = new System.Drawing.Point( 105, 109 );
            this.cboCond4.Name = "cboCond4";
            this.cboCond4.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond4.TabIndex = 17;
            this.cboCond4.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond4_Validating );
            this.cboCond4.SelectedIndexChanged += new System.EventHandler( this.cboCond4_SelectedIndexChanged );
            this.cboCond4.Enter += new System.EventHandler( this.cboCond4_Enter );
            // 
            // cboCond3
            // 
            this.cboCond3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond3.Enabled = false;
            this.cboCond3.Location = new System.Drawing.Point( 105, 80 );
            this.cboCond3.Name = "cboCond3";
            this.cboCond3.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond3.TabIndex = 16;
            this.cboCond3.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond3_Validating );
            this.cboCond3.SelectedIndexChanged += new System.EventHandler( this.cboCond3_SelectedIndexChanged );
            this.cboCond3.Enter += new System.EventHandler( this.cboCond3_Enter );
            // 
            // cboCond2
            // 
            this.cboCond2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond2.Enabled = false;
            this.cboCond2.Location = new System.Drawing.Point( 105, 51 );
            this.cboCond2.Name = "cboCond2";
            this.cboCond2.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond2.TabIndex = 15;
            this.cboCond2.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond2_Validating );
            this.cboCond2.SelectedIndexChanged += new System.EventHandler( this.cboCond2_SelectedIndexChanged );
            this.cboCond2.Enter += new System.EventHandler( this.cboCond2_Enter );
            // 
            // cboCond1
            // 
            this.cboCond1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond1.Enabled = false;
            this.cboCond1.Location = new System.Drawing.Point( 105, 22 );
            this.cboCond1.Name = "cboCond1";
            this.cboCond1.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond1.TabIndex = 14;
            this.cboCond1.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond1_Validating );
            this.cboCond1.SelectedIndexChanged += new System.EventHandler( this.cboCond1_SelectedIndexChanged );
            this.cboCond1.Enter += new System.EventHandler( this.cboCond1_Enter );
            // 
            // lblCond7
            // 
            this.lblCond7.Location = new System.Drawing.Point( 8, 199 );
            this.lblCond7.Name = "lblCond7";
            this.lblCond7.Size = new System.Drawing.Size( 92, 23 );
            this.lblCond7.TabIndex = 6;
            this.lblCond7.Text = "Condition code 7:";
            // 
            // lblCond6
            // 
            this.lblCond6.Location = new System.Drawing.Point( 8, 172 );
            this.lblCond6.Name = "lblCond6";
            this.lblCond6.Size = new System.Drawing.Size( 92, 18 );
            this.lblCond6.TabIndex = 5;
            this.lblCond6.Text = "Condition code 6:";
            // 
            // lblCond5
            // 
            this.lblCond5.Location = new System.Drawing.Point( 8, 143 );
            this.lblCond5.Name = "lblCond5";
            this.lblCond5.Size = new System.Drawing.Size( 92, 16 );
            this.lblCond5.TabIndex = 4;
            this.lblCond5.Text = "Condition code 5:";
            // 
            // lblCond4
            // 
            this.lblCond4.Location = new System.Drawing.Point( 8, 115 );
            this.lblCond4.Name = "lblCond4";
            this.lblCond4.Size = new System.Drawing.Size( 92, 18 );
            this.lblCond4.TabIndex = 3;
            this.lblCond4.Text = "Condition code 4:";
            // 
            // lblCond3
            // 
            this.lblCond3.Location = new System.Drawing.Point( 8, 86 );
            this.lblCond3.Name = "lblCond3";
            this.lblCond3.Size = new System.Drawing.Size( 92, 23 );
            this.lblCond3.TabIndex = 2;
            this.lblCond3.Text = "Condition code 3:";
            // 
            // lblCond2
            // 
            this.lblCond2.Location = new System.Drawing.Point( 8, 56 );
            this.lblCond2.Name = "lblCond2";
            this.lblCond2.Size = new System.Drawing.Size( 92, 25 );
            this.lblCond2.TabIndex = 1;
            this.lblCond2.Text = "Condition code 2:";
            // 
            // lblCond1
            // 
            this.lblCond1.Location = new System.Drawing.Point( 8, 26 );
            this.lblCond1.Name = "lblCond1";
            this.lblCond1.Size = new System.Drawing.Size( 92, 24 );
            this.lblCond1.TabIndex = 0;
            this.lblCond1.Text = "Condition code 1:";
            // 
            // BillingView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.grpConditions );
            this.Controls.Add( this.grpOccSpans );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.grpOccurrences );
            this.Name = "BillingView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Disposed += new System.EventHandler( this.BillingView_Disposed );
            this.Enter += new System.EventHandler( this.BillingView_Enter );
            this.Leave += new System.EventHandler( this.BillingView_Leave );
            this.grpOccurrences.ResumeLayout( false );
            this.grpOccSpans.ResumeLayout( false );
            this.grpConditions.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties

        private OccurrenceSpan SystemGeneratedSpan1 { get; set; }

        private OccurrenceSpan SystemGeneratedSpan2 { get; set; }

        private IList SelectableConditionCodes { get; set; }

        private ArrayList SortedOccurrenceCodes
        {
            get
            {
                return i_SortedOccurrenceCodes;
            }
            set
            {
                i_SortedOccurrenceCodes = value;
            }
        }

        private ArrayList SortedConditionCodes
        {
            get
            {
                return i_SortedConditionCodes;
            }
            set
            {
                i_SortedConditionCodes = value;
            }
        }

        private EmergencyToInPatientTransferCodeManager EmergencyToInPatientTransferCodeManager
        {
            get
            {
                if ( emergencyToInpatientTransferCodeManager == null )
                {
                    var accountBroker =BrokerFactory.BrokerOfType<IAccountBroker>();
                    var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
                    emergencyToInpatientTransferCodeManager = new EmergencyToInPatientTransferCodeManager(
                        DateTime.Parse( ConfigurationManager.AppSettings[ApplicationConfigurationKeys.ER_TO_IP_CONDITION_CODE_START_DATE] ),
                        Model, 
                        accountBroker,
                        conditionCodeBroker);
                }

                return emergencyToInpatientTransferCodeManager;
            }
        }

        #endregion

        #region Construction and Finalization
        public BillingView()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            UnRegisterRulesEvents();

            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        private Label label1;
        private GroupBox grpOccurrences;
        private Label lblOccCode1;
        private Label lblOccCode2;
        private LineLabel lineLabel1;
        private Label lblOccCode3;
        private Label lblOccCode4;
        private Label lblOccCode5;
        private Label lblOccCode6;
        private Label lblOccCode7;
        private Label lblOccCode8;
        private Label lblOccCode1Val;
        private GroupBox grpOccSpans;
        private Label lblSpan1;
        private Label lblSpan2;
        private PatientAccessComboBox cboSpan1;
        private PatientAccessComboBox cboSpan2;
        private Label lblSpan1FromDate;
        private MaskedEditTextBox mtbSpan1FromDate;
        private Label lblSpan1ToDate;
        private MaskedEditTextBox mtbSpan1ToDate;
        private Label lblFac;
        private MaskedEditTextBox mtbFacility;
        private Label lblSpan2FromDate;
        private MaskedEditTextBox mtbSpan2FromDate;
        private Label lblSpan2ToDate;
        private MaskedEditTextBox mtbSpan2ToDate;
        private Label lblDate8;
        private Label lblDate7;
        private Label lblDate6;
        private Label lblDate5;
        private Label lblDate4;
        private Label lblDate3;
        private Label lblDate2;
        private DateTimePicker dateTimePicker6;
        private DateTimePicker dateTimePicker7;
        private MaskedEditTextBox mtbOcc8Date;
        private MaskedEditTextBox mtbOcc7Date;
        private MaskedEditTextBox mtbOcc6Date;
        private MaskedEditTextBox mtbOcc5Date;
        private MaskedEditTextBox mtbOcc4Date;
        private MaskedEditTextBox mtbOcc3Date;
        private MaskedEditTextBox mtbOcc2Date;
        private Label lblOccCode1Date;
        private PatientAccessComboBox cboOccCode8;
        private PatientAccessComboBox cboOccCode7;
        private PatientAccessComboBox cboOccCode6;
        private PatientAccessComboBox cboOccCode5;
        private PatientAccessComboBox cboOccCode4;
        private PatientAccessComboBox cboOccCode3;
        private PatientAccessComboBox cboOccCode2;
        private Label lblOccCode1DateVal;
        private GroupBox grpConditions;
        private Label lblCond1;
        private Label lblCond2;
        private Label lblCond3;
        private Label lblCond4;
        private Label lblCond5;
        private Label lblCond6;
        private Label lblCond7;
        private PatientAccessComboBox cboCond1;
        private PatientAccessComboBox cboCond2;
        private PatientAccessComboBox cboCond3;
        private PatientAccessComboBox cboCond4;
        private PatientAccessComboBox cboCond5;
        private PatientAccessComboBox cboCond6;
        private PatientAccessComboBox cboCond7;

        private DateTimePickerPlus dtppSpan1From;
        private DateTimePickerPlus dtppSpan2From;
        private DateTimePickerPlus dtppSpan1To;
        private DateTimePickerPlus dtppSpan2To;
        private DateTimePickerPlus dtppOcc2Date;
        private DateTimePickerPlus dtppOcc3Date;
        private DateTimePickerPlus dtppOcc4Date;
        private DateTimePickerPlus dtppOcc5Date;
        private DateTimePickerPlus dtppOcc6Date;
        private DateTimePickerPlus dtppOcc7Date;
        private DateTimePickerPlus dtppOcc8Date;


        #region Data Elements
        private Container components = null;

        private Hashtable occDateControls = new Hashtable();
        private Hashtable occDatePickerControls = new Hashtable();
        private ComboBoxManager occCboMgr = new ComboBoxManager();
        private ComboBoxManager condCboMgr = new ComboBoxManager();

        private ArrayList i_SortedOccurrenceCodes = new ArrayList();
        private ArrayList i_SortedConditionCodes = new ArrayList();

        private ArrayList i_DateTimePickers = new ArrayList();
        private OccurrenceCode i_OCC1;
        private string i_LastEnabledOCCName = string.Empty;
        private string i_LastEnabledCondName = string.Empty;

        private ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
        private string firstTimeLoad = string.Empty;

        private bool blnLeaveRun;
        private EmergencyToInPatientTransferCodeManager emergencyToInpatientTransferCodeManager;

        #endregion

        #region Constants

        private const int
            MAX_OCCURRENCECODES = 8;
        private const long
            BLANK_OPTION_OID = -1L;

        private const string
            YES = "Y",
            NO = "N",
            OCC_CBO_NAME_PREFIX = "cboOccCode",
            COND_CBO_NAME_PREFIX = "cboCond";
        #endregion
    }
}
