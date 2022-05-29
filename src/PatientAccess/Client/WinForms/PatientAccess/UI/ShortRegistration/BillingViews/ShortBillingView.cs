using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.BillingViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.TransferViews;

namespace PatientAccess.UI.ShortRegistration.BillingViews
{
    /// <summary>
    /// Summary description for ShortBillingView.
    /// </summary>
    public class ShortBillingView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers
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
            RuleEngine.GetInstance().EvaluateRule( typeof( OnShortBillingForm ), Model );
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
 

        private void ConditionCode1RequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboCond1 );
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

        #endregion

        #region Public methods
        public override void UpdateView()
        {
            try
            {
                // Disable painting of controls while they are loading
                BeginUpdateForControlsIn( grpConditions.Controls );
                BeginUpdateForControlsIn( grpOccurrences.Controls );

                firstTimeLoad = firstTimeLoad == string.Empty ? YES : NO;

                RemoveOCCHandler();
                RemoveConditionHandler();

                if ( firstTimeLoad == YES )
                {
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

                Model.Patient.SelectedAccount = Model;

                if ( firstTimeLoad == YES && IsActivatePreRegistrationActivity() )
                {
                    Model.OccurrenceSpans.Clear();
                }

                RegisterRulesEvents();
                RunRules();
            }
            finally
            {
                EndUpdateForControlsIn( grpConditions.Controls );
                EndUpdateForControlsIn( grpOccurrences.Controls );
            }
        }

        private bool IsActivatePreRegistrationActivity()
        {
            return ( Model.Activity != null &&
                     Model.Activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) &&
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

            SortOCCCodes();

            RuleEngine.GetInstance().EvaluateRule( typeof( OnShortBillingForm ), Model );
            RunRequiredRules();
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

            Refresh();

            

            RuleEngine.GetInstance().EvaluateRule( typeof( ConditionCode1Required ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode1DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode2DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode3DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode4DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode5DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode6DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode7DateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( OccurrenceCode8DateRequired ), Model );
 
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
            SortOccurrencesByCode sortOccurrencesByCode = new SortOccurrencesByCode();

            ( ( ArrayList )Model.OccurrenceCodes ).Sort( sortOccurrencesByCode );
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
                occCboMgr.ComboValueSet( occurrenceCodeComboBox, occurrenceCodeComboBox.SelectedItem );

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

        private static bool IsNotFutureDate( MaskedEditTextBox mtbDate, string errMsg )
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
                tempHolder[0] = (OccurrenceCode)(i_OCC1.Clone());
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
                    tempHolder[i - 1] = (OccurrenceCode)((OccurrenceCode)(cboOCC.SelectedItem)).Clone();
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

        private void EnableAllDatePickersClick()
        {
            foreach ( DateTimePickerPlus dtpPlus in i_DateTimePickers )
            {
                dtpPlus.SuppressClick = false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ShortBillingView ) );
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
            this.mtbOcc8Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc8Date.Location = new System.Drawing.Point( 396, 230 );
            this.mtbOcc8Date.Mask = "  /  /";
            this.mtbOcc8Date.MaxLength = 10;
            this.mtbOcc8Date.Name = "mtbOcc8Date";
            this.mtbOcc8Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc8Date.TabIndex = 13;
            this.mtbOcc8Date.ValidationExpression = resources.GetString( "mtbOcc8Date.ValidationExpression" );
            this.mtbOcc8Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc8Date_Validating );
            // 
            // mtbOcc7Date
            // 
            this.mtbOcc7Date.Enabled = false;
            this.mtbOcc7Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc7Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc7Date.Location = new System.Drawing.Point( 396, 204 );
            this.mtbOcc7Date.Mask = "  /  /";
            this.mtbOcc7Date.MaxLength = 10;
            this.mtbOcc7Date.Name = "mtbOcc7Date";
            this.mtbOcc7Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc7Date.TabIndex = 11;
            this.mtbOcc7Date.ValidationExpression = resources.GetString( "mtbOcc7Date.ValidationExpression" );
            this.mtbOcc7Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc7Date_Validating );
            // 
            // mtbOcc6Date
            // 
            this.mtbOcc6Date.Enabled = false;
            this.mtbOcc6Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc6Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc6Date.Location = new System.Drawing.Point( 396, 177 );
            this.mtbOcc6Date.Mask = "  /  /";
            this.mtbOcc6Date.MaxLength = 10;
            this.mtbOcc6Date.Name = "mtbOcc6Date";
            this.mtbOcc6Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc6Date.TabIndex = 9;
            this.mtbOcc6Date.ValidationExpression = resources.GetString( "mtbOcc6Date.ValidationExpression" );
            this.mtbOcc6Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc6Date_Validating );
            // 
            // mtbOcc5Date
            // 
            this.mtbOcc5Date.Enabled = false;
            this.mtbOcc5Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc5Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc5Date.Location = new System.Drawing.Point( 396, 150 );
            this.mtbOcc5Date.Mask = "  /  /";
            this.mtbOcc5Date.MaxLength = 10;
            this.mtbOcc5Date.Name = "mtbOcc5Date";
            this.mtbOcc5Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc5Date.TabIndex = 7;
            this.mtbOcc5Date.ValidationExpression = resources.GetString( "mtbOcc5Date.ValidationExpression" );
            this.mtbOcc5Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc5Date_Validating );
            // 
            // mtbOcc4Date
            // 
            this.mtbOcc4Date.Enabled = false;
            this.mtbOcc4Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc4Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc4Date.Location = new System.Drawing.Point( 396, 122 );
            this.mtbOcc4Date.Mask = "  /  /";
            this.mtbOcc4Date.MaxLength = 10;
            this.mtbOcc4Date.Name = "mtbOcc4Date";
            this.mtbOcc4Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc4Date.TabIndex = 5;
            this.mtbOcc4Date.ValidationExpression = resources.GetString( "mtbOcc4Date.ValidationExpression" );
            this.mtbOcc4Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc4Date_Validating );
            // 
            // mtbOcc3Date
            // 
            this.mtbOcc3Date.Enabled = false;
            this.mtbOcc3Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc3Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc3Date.Location = new System.Drawing.Point( 396, 94 );
            this.mtbOcc3Date.Mask = "  /  /";
            this.mtbOcc3Date.MaxLength = 10;
            this.mtbOcc3Date.Name = "mtbOcc3Date";
            this.mtbOcc3Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc3Date.TabIndex = 3;
            this.mtbOcc3Date.ValidationExpression = resources.GetString( "mtbOcc3Date.ValidationExpression" );
            this.mtbOcc3Date.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOcc3Date_Validating );
            // 
            // mtbOcc2Date
            // 
            this.mtbOcc2Date.Enabled = false;
            this.mtbOcc2Date.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOcc2Date.KeyPressExpression = "^\\d*$";
            this.mtbOcc2Date.Location = new System.Drawing.Point( 396, 67 );
            this.mtbOcc2Date.Mask = "  /  /";
            this.mtbOcc2Date.MaxLength = 10;
            this.mtbOcc2Date.Name = "mtbOcc2Date";
            this.mtbOcc2Date.Size = new System.Drawing.Size( 75, 20 );
            this.mtbOcc2Date.TabIndex = 1;
            this.mtbOcc2Date.ValidationExpression = resources.GetString( "mtbOcc2Date.ValidationExpression" );
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
            this.cboOccCode8.SelectedIndexChanged += new System.EventHandler( this.cboOccCode8_SelectedIndexChanged );
            this.cboOccCode8.Enter += new System.EventHandler( this.cboOccCode8_Enter );
            this.cboOccCode8.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode8_Validating );
            // 
            // cboOccCode7
            // 
            this.cboOccCode7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode7.Enabled = false;
            this.cboOccCode7.Location = new System.Drawing.Point( 115, 203 );
            this.cboOccCode7.Name = "cboOccCode7";
            this.cboOccCode7.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode7.TabIndex = 10;
            this.cboOccCode7.SelectedIndexChanged += new System.EventHandler( this.cboOccCode7_SelectedIndexChanged );
            this.cboOccCode7.Enter += new System.EventHandler( this.cboOccCode7_Enter );
            this.cboOccCode7.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode7_Validating );
            // 
            // cboOccCode6
            // 
            this.cboOccCode6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode6.Enabled = false;
            this.cboOccCode6.Location = new System.Drawing.Point( 116, 175 );
            this.cboOccCode6.Name = "cboOccCode6";
            this.cboOccCode6.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode6.TabIndex = 8;
            this.cboOccCode6.SelectedIndexChanged += new System.EventHandler( this.cboOccCode6_SelectedIndexChanged );
            this.cboOccCode6.Enter += new System.EventHandler( this.cboOccCode6_Enter );
            this.cboOccCode6.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode6_Validating );
            // 
            // cboOccCode5
            // 
            this.cboOccCode5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode5.Enabled = false;
            this.cboOccCode5.Location = new System.Drawing.Point( 116, 148 );
            this.cboOccCode5.Name = "cboOccCode5";
            this.cboOccCode5.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode5.TabIndex = 6;
            this.cboOccCode5.SelectedIndexChanged += new System.EventHandler( this.cboOccCode5_SelectedIndexChanged );
            this.cboOccCode5.Enter += new System.EventHandler( this.cboOccCode5_Enter );
            this.cboOccCode5.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode5_Validating );
            // 
            // cboOccCode4
            // 
            this.cboOccCode4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode4.Enabled = false;
            this.cboOccCode4.Location = new System.Drawing.Point( 116, 120 );
            this.cboOccCode4.Name = "cboOccCode4";
            this.cboOccCode4.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode4.TabIndex = 4;
            this.cboOccCode4.SelectedIndexChanged += new System.EventHandler( this.cboOccCode4_SelectedIndexChanged );
            this.cboOccCode4.Enter += new System.EventHandler( this.cboOccCode4_Enter );
            this.cboOccCode4.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode4_Validating );
            // 
            // cboOccCode3
            // 
            this.cboOccCode3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode3.Enabled = false;
            this.cboOccCode3.Location = new System.Drawing.Point( 116, 93 );
            this.cboOccCode3.Name = "cboOccCode3";
            this.cboOccCode3.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode3.TabIndex = 2;
            this.cboOccCode3.SelectedIndexChanged += new System.EventHandler( this.cboOccCode3_SelectedIndexChanged );
            this.cboOccCode3.Enter += new System.EventHandler( this.cboOccCode3_Enter );
            this.cboOccCode3.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode3_Validating );
            // 
            // cboOccCode2
            // 
            this.cboOccCode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccCode2.Enabled = false;
            this.cboOccCode2.Location = new System.Drawing.Point( 116, 66 );
            this.cboOccCode2.Name = "cboOccCode2";
            this.cboOccCode2.Size = new System.Drawing.Size( 222, 21 );
            this.cboOccCode2.TabIndex = 0;
            this.cboOccCode2.SelectedIndexChanged += new System.EventHandler( this.cboOccCode2_SelectedIndexChanged );
            this.cboOccCode2.Enter += new System.EventHandler( this.cboOccCode2_Enter );
            this.cboOccCode2.Validating += new System.ComponentModel.CancelEventHandler( this.cboOccCode2_Validating );
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
            this.dateTimePicker7.Size = new System.Drawing.Size( 200, 20 );
            this.dateTimePicker7.TabIndex = 0;
            // 
            // dateTimePicker6
            // 
            this.dateTimePicker6.Location = new System.Drawing.Point( 0, 0 );
            this.dateTimePicker6.Name = "dateTimePicker6";
            this.dateTimePicker6.Size = new System.Drawing.Size( 200, 20 );
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
            this.cboCond7.SelectedIndexChanged += new System.EventHandler( this.cboCond7_SelectedIndexChanged );
            this.cboCond7.Enter += new System.EventHandler( this.cboCond7_Enter );
            this.cboCond7.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond7_Validating );
            // 
            // cboCond6
            // 
            this.cboCond6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond6.Enabled = false;
            this.cboCond6.Location = new System.Drawing.Point( 105, 167 );
            this.cboCond6.Name = "cboCond6";
            this.cboCond6.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond6.TabIndex = 19;
            this.cboCond6.SelectedIndexChanged += new System.EventHandler( this.cboCond6_SelectedIndexChanged );
            this.cboCond6.Enter += new System.EventHandler( this.cboCond6_Enter );
            this.cboCond6.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond6_Validating );
            // 
            // cboCond5
            // 
            this.cboCond5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond5.Enabled = false;
            this.cboCond5.Location = new System.Drawing.Point( 105, 138 );
            this.cboCond5.Name = "cboCond5";
            this.cboCond5.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond5.TabIndex = 18;
            this.cboCond5.SelectedIndexChanged += new System.EventHandler( this.cboCond5_SelectedIndexChanged );
            this.cboCond5.Enter += new System.EventHandler( this.cboCond5_Enter );
            this.cboCond5.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond5_Validating );
            // 
            // cboCond4
            // 
            this.cboCond4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond4.Enabled = false;
            this.cboCond4.Location = new System.Drawing.Point( 105, 109 );
            this.cboCond4.Name = "cboCond4";
            this.cboCond4.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond4.TabIndex = 17;
            this.cboCond4.SelectedIndexChanged += new System.EventHandler( this.cboCond4_SelectedIndexChanged );
            this.cboCond4.Enter += new System.EventHandler( this.cboCond4_Enter );
            this.cboCond4.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond4_Validating );
            // 
            // cboCond3
            // 
            this.cboCond3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond3.Enabled = false;
            this.cboCond3.Location = new System.Drawing.Point( 105, 80 );
            this.cboCond3.Name = "cboCond3";
            this.cboCond3.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond3.TabIndex = 16;
            this.cboCond3.SelectedIndexChanged += new System.EventHandler( this.cboCond3_SelectedIndexChanged );
            this.cboCond3.Enter += new System.EventHandler( this.cboCond3_Enter );
            this.cboCond3.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond3_Validating );
            // 
            // cboCond2
            // 
            this.cboCond2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond2.Enabled = false;
            this.cboCond2.Location = new System.Drawing.Point( 105, 51 );
            this.cboCond2.Name = "cboCond2";
            this.cboCond2.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond2.TabIndex = 15;
            this.cboCond2.SelectedIndexChanged += new System.EventHandler( this.cboCond2_SelectedIndexChanged );
            this.cboCond2.Enter += new System.EventHandler( this.cboCond2_Enter );
            this.cboCond2.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond2_Validating );
            // 
            // cboCond1
            // 
            this.cboCond1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCond1.Enabled = false;
            this.cboCond1.Location = new System.Drawing.Point( 105, 22 );
            this.cboCond1.Name = "cboCond1";
            this.cboCond1.Size = new System.Drawing.Size( 217, 21 );
            this.cboCond1.TabIndex = 14;
            this.cboCond1.SelectedIndexChanged += new System.EventHandler( this.cboCond1_SelectedIndexChanged );
            this.cboCond1.Enter += new System.EventHandler( this.cboCond1_Enter );
            this.cboCond1.Validating += new System.ComponentModel.CancelEventHandler( this.cboCond1_Validating );
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
            // ShortBillingView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.grpConditions );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.grpOccurrences );
            this.Name = "ShortBillingView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Enter += new System.EventHandler( this.BillingView_Enter );
            this.Leave += new System.EventHandler( this.BillingView_Leave );
            this.Disposed += new System.EventHandler( this.BillingView_Disposed );
            this.grpOccurrences.ResumeLayout( false );
            this.grpOccurrences.PerformLayout();
            this.grpConditions.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties

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
        public ShortBillingView()
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

        private string firstTimeLoad = string.Empty;

        private bool blnLeaveRun;
        private EmergencyToInPatientTransferCodeManager emergencyToInpatientTransferCodeManager;

        #endregion

        #region Constants
        private const int
          MAX_OCCURRENCECODES = 8;
        private const string
            YES = "Y",
            NO = "N",
            OCC_CBO_NAME_PREFIX = "cboOccCode",
            COND_CBO_NAME_PREFIX = "cboCond";
        #endregion
    }
}
