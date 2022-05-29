using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FUSNotes
{
    public partial class AddFUSNoteView : TimeOutFormView
    {
        #region Events
        #endregion

        #region Event Handlers

        void AddFUSNoteView_Enter( object sender, EventArgs e )
        {
            this.mtbActivityCode.Focus();
        }

        private void AddFUSNote_Load( object sender, EventArgs e )
        {
            this.mtbActivityCode.Enabled = true;
            this.cmbDescription.Enabled = true;
            
            this.WriteableActivityCodesHashtable = fusNoteBroker.AllWriteableCodesHashtable;
            this.WriteableFusActivities = ( ArrayList )fusNoteBroker.AllWriteableActivityCodes();

            this.PopulateFusActivityDescriptions();
            this.cmbDescription.SelectedIndex = -1;

            this.PopulateMonth();
            this.cmbMonth.SelectedIndex = -1;            
        }

        private void AddFUSNote_Leave( object sender, EventArgs e )
        {
            this.Cursor = Cursors.Default;
        }

        private void mtbActivityCode_Click( object sender, EventArgs e )
        {
            CheckActivityCodeIsEmpty();
        }

        private void mtbActivityCode_TextChanged( object sender, EventArgs e )
        {
            CheckActivityCodeIsEmpty();

            if( this.mtbActivityCode.Text.Trim().Length == 5 )
            {
                this.AcceptButton = this.btnSelect;
            }
        }

        /// <summary>
        /// cmbDescription_SelectedIndexChanged - if Description is not blank when selected, enable remaining fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDescription_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.mtbActivityCode );

            // if Description combo box value is being set by clicking
            // on Select button, return without changing any values.
            if( isDescriptionSetBySelect )
            {
                isDescriptionSetBySelect = false;
                return;
            }
            else
            {
                FusActivity activity = this.cmbDescription.SelectedItem as FusActivity;

                // If selected item is CREMC activity code, display warning message and return
                if( activity.Code == EXTENSION_ACTIVITY_CODE  )
                {
                    MessageBox.Show( UIErrorMessages.FUS_ACTIVITY_NOT_WRITEABLE, TITLE_INVALID_ACTIVITY_CODE, MessageBoxButtons.OK, MessageBoxIcon.Error );
                    this.btnOK.Enabled = false;
                    this.cmbDescription.Focus();
                    return;
                }

                // Warn user before clearing previously entered values for fields
                // when user changes activity code or description field values
                bool blnContinue = this.VerifyActivityCodeOrDescriptionChangeFor( activity.Code );

                if ( !blnContinue )
                {
                    // Reset description selection back to the original value when user does not want to continue with the change
                    this.SetDescriptionSelectedItemWith( this.FusActivity.Code, true );
                    return;
                }
                else
                {
                    if ( activity.Code != String.Empty )
                    {
                        this.mtbActivityCode.Text = activity.Code.ToUpper();
                        SetDefaultViewFor( activity );
                        this.FusActivity = activity;
                        this.btnOK.Enabled = true;
                    }
                    else
                    {
                        // When 'Blank' is selected in the Description combo box, 
                        // clear out the activity code and reset the default view
                        this.isDescriptionSetBySelect = false;
                        this.mtbActivityCode.Text = string.Empty;
                        this.ResetView();
                    }
                }
            }
        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            bool blnContinue = this.VerifyActivityCodeDescriptionMismatch();

            if ( !blnContinue )
            {
                // If there is a mismatch between Activity code and Description,
                // return to the previous screen and clear Activity code field.
                this.mtbActivityCode.Text = String.Empty;
                this.mtbActivityCode.Focus();
                return;
            }
            else
            {
                this.ExtendedFUSNote.Date1 = this.VerifyDate( ref mtbDate1 );
                this.ExtendedFUSNote.Date2 = this.VerifyDate( ref mtbDate2 );
                this.ExtendedFUSNote.WorklistDate = this.VerifyDate( ref mtbWorklistDate );

                VerifyMonth();

                // Add the newly created Fus Note to the account.
                User currentUser = User.GetCurrent();

                this.ExtendedFUSNote.Persisted = false;
                this.ExtendedFUSNote.ManuallyEntered = true;
                this.ExtendedFUSNote.UserID = currentUser.PBAREmployeeID;

                
                ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                DateTime tempDateTime = timeBroker.TimeAt( currentUser.Facility.GMTOffset, currentUser.Facility.DSTOffset );
                
                DateTime previousNoteDateTimeWithIncrement = tempDateTime;

                if( this.Model_Account.FusNotes != null
                    && this.Model_Account.FusNotes.Count > 0 )
                {
                    ExtendedFUSNote previousFUSNote = ( this.Model_Account.FusNotes[0] as ExtendedFUSNote );

                    int increment = previousFUSNote.Remarks.Length / 200;
                    previousNoteDateTimeWithIncrement = previousFUSNote.CreatedOn.AddSeconds( increment + 1 );
                }

                if( tempDateTime > previousNoteDateTimeWithIncrement )
                {
                    this.ExtendedFUSNote.CreatedOn = tempDateTime;
                }
                else
                {
                    this.ExtendedFUSNote.CreatedOn = previousNoteDateTimeWithIncrement;
                }

                this.Model_Account.AddFusNote( this.ExtendedFUSNote );
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnSelect_Click( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.mtbActivityCode );
            string activityCode = mtbActivityCode.Text.Trim();

            if( activityCode.Length <= 5 )
            {
                if( activityCode == EXTENSION_ACTIVITY_CODE || activityCode.Length < 5 )
                {
                    UIColors.SetErrorBgColor( this.mtbActivityCode );
                    MessageBox.Show( UIErrorMessages.FUS_ACTIVITY_NOT_WRITEABLE, TITLE_INVALID_ACTIVITY_CODE, MessageBoxButtons.OK, MessageBoxIcon.Error );
                    this.btnOK.Enabled = false;
                    this.mtbActivityCode.Focus();
                    return;
                }
                else
                {
                    bool isWriteable = this.IsWriteableActivityCode( activityCode ) ;
                    this.btnOK.Enabled = isWriteable;

                    if( !isWriteable )
                    {
                        UIColors.SetErrorBgColor( this.mtbActivityCode );
                        MessageBox.Show( UIErrorMessages.FUS_ACTIVITY_NOT_WRITEABLE, TITLE_INVALID_ACTIVITY_CODE, MessageBoxButtons.OK, MessageBoxIcon.Error );
                        this.mtbActivityCode.Focus();
                        return;
                    }
                }
            }
            else
            {
                this.mtbActivityCode.SelectAll();
                this.mtbActivityCode.Focus();
                this.btnOK.Enabled = false;
            }

            // Warn user before clearing previously entered values for fields
            // when user changes activity code or description field values
            bool blnContinue = this.VerifyActivityCodeOrDescriptionChangeFor( activityCode );

            if ( !blnContinue )
            {
                // Reset Activity code value back to the original value when user does not want to continue with the change
                FusActivity activity = this.cmbDescription.SelectedItem as FusActivity;
                this.mtbActivityCode.Text = activity.Code;
                return;
            }
            else
            {
                if ( activityCode != String.Empty )
                {
                    this.SetDescriptionSelectedItemWith( activityCode, false );
                    this.SetDefaultSettingsFor( this.FusActivity.NoteType );
                }
                else
                {
                    this.btnOK.Enabled = false;
                }
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void mtbWorklistDate_Validating(object sender, EventArgs e)
        {
            if( dtpWorklistDate.Focused == false )
            {
                this.ExtendedFUSNote.WorklistDate = this.VerifyDate( ref mtbWorklistDate );
            }
        }

        private void mtbRemarks_Validating(object sender, EventArgs e)
        {
            this.ExtendedFUSNote.Remarks = mtbRemarks.Text.Trim();
        }

        
        private void mtbDollar1_Validating(object sender, EventArgs e)
        {
            if( mtbDollar1.Text.Trim() != String.Empty )
            {
                decimal dollar1 = 0;
                if( mtbDollar1.Text.Trim() != "." )
                {
                    dollar1 = Convert.ToDecimal( mtbDollar1.Text.Trim() );
                }
                this.ExtendedFUSNote.Dollar1 = dollar1;
            }
            CommonFormatting.FormatTextBoxCurrency( mtbDollar1, "#,###,##0.00" );   
        }

        private void mtbDollar2_Validating(object sender, EventArgs e)
        {
            if( mtbDollar2.Text.Trim() != String.Empty )
            {
                decimal dollar2 = 0;
                if( mtbDollar2.Text.Trim() != "." )
                {
                    dollar2 = Convert.ToDecimal( mtbDollar2.Text.Trim() );
                }
                this.ExtendedFUSNote.Dollar2 = dollar2;
            }
            CommonFormatting.FormatTextBoxCurrency( mtbDollar2, "#,###,##0.00" );   
        }

        private void mtbDate1_Validating(object sender, EventArgs e)
        {
            if( dtpDate1.Focused == false )
            {
                this.ExtendedFUSNote.Date1 = this.VerifyDate( ref mtbDate1 );
            }
        }

        private void mtbDate2_Validating(object sender, EventArgs e)
        {
            if( dtpDate2.Focused == false )
            {
                this.ExtendedFUSNote.Date2 = this.VerifyDate( ref mtbDate2 );
            }
        }

        private void dtpWorklistDate_CloseUp(object sender, EventArgs e)
        {
			if( dtpWorklistDate.Checked == true )
			{
				DateTime dt = dtpWorklistDate.Value;
				mtbWorklistDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                this.ExtendedFUSNote.WorklistDate = this.VerifyDate( ref mtbWorklistDate );
			}
        }

        private void dtpDate1_CloseUp(object sender, EventArgs e)
        {
			if( dtpDate1.Checked == true )
			{
				DateTime dt = dtpDate1.Value;
				mtbDate1.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                this.ExtendedFUSNote.Date1 = this.VerifyDate( ref mtbDate1 );
			}
			else
			{
				mtbDate1.Text = String.Empty;
			}
        }

        private void dtpDate2_CloseUp(object sender, EventArgs e)
        {
			if( dtpDate2.Checked == true )
			{
				DateTime dt = dtpDate2.Value;
				mtbDate2.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                this.ExtendedFUSNote.Date2 = this.VerifyDate( ref mtbDate2 );
			}
			else
			{
				mtbDate2.Text = String.Empty;
			}
        }

        private void cmbMonth_SelectedIndexChanged( object sender, EventArgs e )
        {
            Month month = this.cmbMonth.SelectedItem as Month;
            if( month != null )
            {
                this.ExtendedFUSNote.Month = month.Code;
            }
            else
            {
                this.ExtendedFUSNote.Month = string.Empty;
            }
        }

        #endregion

        #region Methods

        public void SetFocus()
        {
            // Set initial focus after loading view on ActivityCode field.
            this.Activate();
            SendKeys.Send( "{Tab}" );
        }

        public override void UpdateView()
        {            
        }

        #endregion

        #region Private Methods

        private void SetDescriptionSelectedItemWith( string activityCode, bool isReset )
        {
            FusActivity activity = fusNoteBroker.FusActivityWith( activityCode );
            this.isDescriptionSetBySelect = true;
            this.cmbDescription.SelectedIndex = cmbDescription.FindString( activity.ToString() );

            if( !isReset )
            {
                SetDefaultViewFor( activity );
            }
            this.FusActivity = activity;
            this.btnOK.Enabled = true;
        }

        private bool IsWriteableActivityCode( string activityCode )
        {
            bool isWriteable = false;

            if ( this.WriteableActivityCodesHashtable.Contains( activityCode ) )
            {
                isWriteable = true;
            }

            return isWriteable;
        }

        private bool VerifyActivityCodeOrDescriptionChangeFor( string activityCode )
        {
            bool blnContinue = true;

            if( this.FusActivity != null && this.FusActivity.Code != activityCode )
            {
                DialogResult result = MessageBox.Show( 
                    UIErrorMessages.FUS_ACTIVITY_CODE_CHANGE_MSG,
                    TITLE_FUS_ACTIVITY_CHANGE, MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning );

                if( result == DialogResult.No )
                {
                    blnContinue =  false;
                }
            }

            return blnContinue;
        }

        private bool VerifyActivityCodeDescriptionMismatch()
        {
            bool blnContinue = true;

            string activityCode = this.mtbActivityCode.Text.Trim();
            if( this.FusActivity != null && this.FusActivity.Code != activityCode )
            {
                DialogResult result = MessageBox.Show( 
                    String.Format( UIErrorMessages.FUS_ACTIVITY_MISMATCH_MSG, this.FusActivity ),
                    TITLE_FUS_ACTIVITY_MISMATCH, MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning );

                if( result == DialogResult.No )
                {
                    blnContinue =  false;
                }
            }

            return blnContinue;
        }

        /// <summary>
        /// Verify validity of Date2
        /// </summary>
        private DateTime VerifyDate( ref MaskedEditTextBox mtbDate )
        {
            DateTime date = DateTime.MinValue;
            UIColors.SetNormalBgColor( mtbDate );

            if( mtbDate.UnMaskedText.Length == 0
                || mtbDate.UnMaskedText == String.Empty )
            {
                return date;
            }

            if( mtbDate.Text.Length != 10 )
            {
                mtbDate.Focus();
                UIColors.SetErrorBgColor( mtbDate );
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return date;
            }

            string month = mtbDate.Text.Substring( 0, 2 );
            string day   = mtbDate.Text.Substring( 3, 2 );
            string year  = mtbDate.Text.Substring( 6, 4 );
  
            int verifyMonth = Convert.ToInt32( month );
            int verifyDay   = Convert.ToInt32( day );
            int verifyYear  = Convert.ToInt32( year );

            try
            {   // Check the date entered is valid
                DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );
                
                if( DateValidator.IsValidDate( theDate ) == false )
                {
                    mtbDate.Focus();
                    UIColors.SetErrorBgColor( mtbDate );
                    MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return date;
                }
                else if( theDate < earliestDate )
                {
                    mtbDate.Focus();
                    UIColors.SetErrorBgColor( mtbDate );
                    MessageBox.Show( UIErrorMessages.DATE_OUT_OF_RANGE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return date;
                }

                if( mtbDate.Name == "mtbWorklistDate" )
                {
                    DateTime maxWorklistDate = DateTime.Today.AddDays( this.FusActivity.MaxWorklistDays );
                    if( theDate > maxWorklistDate )
                    {
                        this.mtbWorklistDate.Focus();
                        MessageBox.Show( UIErrorMessages.FUS_MAX_WORKLIST_DATE_ERROR, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                            DateTime worklistDate = DateTime.Today.AddDays( this.FusActivity.DefaultWorklistDays );
                            this.mtbWorklistDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                                                worklistDate.Month,
                                                                                worklistDate.Day,
                                                                                worklistDate.Year );
                        return worklistDate;
                    }
                }
            }
            catch( ArgumentOutOfRangeException )
            {
                mtbDate.Focus();
                UIColors.SetErrorBgColor( mtbDate );
                MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return date;
            }

            date = Convert.ToDateTime( mtbDate.Text );

            return date;
        }

        private void VerifyMonth()
        {
            if( this.ExtendedFUSNote.Month == null )
            {
                if( this.cmbMonth.SelectedItem != null )
                {
                    this.ExtendedFUSNote.Month = ( this.cmbMonth.SelectedItem as Month ).Code;
                }
                else 
                { 
                    this.ExtendedFUSNote.Month = string.Empty;
                }
            }
        }

        private void PopulateFusActivityDescriptions()
        {
            if( cmbDescription.Items.Count > 0 )
            {
                return;
            }

            if( this.WriteableActivityCodesHashtable == null )
            {
                MessageBox.Show( "No writeable Activity Codes were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            cmbDescription.Items.Clear();

            cmbDescription.Items.Add( new FusActivity() );

            foreach ( FusActivity activity in this.WriteableFusActivities )
            {
                cmbDescription.Items.Add( activity );
            }
        }

        private void PopulateMonth()
        {
            if( cmbMonth.Items.Count > 0 )
            {
                return;
            }

            cmbMonth.Items.Clear();

            ArrayList monthsList = DateTimeUtilities.MonthsList();

            cmbMonth.Items.Add( new Month() ); // blank first Month entry

            foreach ( Month month in monthsList )
            {
                cmbMonth.Items.Add( month );
            }
        }

        private void SetDefaultViewFor( FusActivity activity )
        {
            FusActivity.FusActivityNoteType noteType = activity.NoteType;

            // Clear all existing values
            this.ResetView();

            // Worklist Date is always enabled for all note types
            this.mtbWorklistDate.Enabled = true;
            this.dtpWorklistDate.Enabled = true;
            DateTime worklistDate = DateTime.Today.AddDays( activity.DefaultWorklistDays );
            this.mtbWorklistDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                                                                    worklistDate.Month,
                                                                    worklistDate.Day,
                                                                    worklistDate.Year );

            // Set other fields' default settings based on NoteType
            this.SetDefaultSettingsFor( noteType );
            this.AcceptButton = this.btnOK;
        }

        private void SetDefaultSettingsFor( FusActivity.FusActivityNoteType noteType )
        {    
            // Set other fields' default settings based on NoteType
            this.mtbRemarks.Enabled = this.EnableRemarksFor( noteType );
            this.mtbDollar1.Enabled = this.EnableDollar1For( noteType );
            this.mtbDate1.Enabled = this.dtpDate1.Enabled = this.EnableDate1For( noteType );

            this.mtbDollar2.Enabled = ( noteType == FusActivity.FusActivityNoteType.Type04 ||
                                        noteType == FusActivity.FusActivityNoteType.Type06 );

            this.mtbDate2.Enabled = this.dtpDate2.Enabled = ( noteType == FusActivity.FusActivityNoteType.Type04 ||
                                                              noteType == FusActivity.FusActivityNoteType.Type05 );

            this.cmbMonth.Enabled = ( noteType == FusActivity.FusActivityNoteType.Type12 );
        }

        private bool EnableRemarksFor( FusActivity.FusActivityNoteType noteType )
        {
            bool blnEnable = false;
            if ( noteType == FusActivity.FusActivityNoteType.Type07 ||
                 noteType == FusActivity.FusActivityNoteType.Type10 )
            {
                blnEnable = true;
                this.mtbRemarks.Focus();
            }

            return blnEnable;
        }

        private bool EnableDollar1For( FusActivity.FusActivityNoteType noteType )
        {
            bool blnEnable = false;
            if ( noteType == FusActivity.FusActivityNoteType.Type02 ||
                 noteType == FusActivity.FusActivityNoteType.Type03 ||
                 noteType == FusActivity.FusActivityNoteType.Type04 ||
                 noteType == FusActivity.FusActivityNoteType.Type06 ||
                 noteType == FusActivity.FusActivityNoteType.Type10 ||
                 noteType == FusActivity.FusActivityNoteType.Type12 )
            {
                blnEnable = true;
                if ( noteType != FusActivity.FusActivityNoteType.Type10 )
                {
                    this.mtbDollar1.Focus();
                }
            }

            return blnEnable;
        }

        private bool EnableDate1For( FusActivity.FusActivityNoteType noteType )
        {
            bool blnEnable = false;
            if ( noteType == FusActivity.FusActivityNoteType.Type01 ||
                 noteType == FusActivity.FusActivityNoteType.Type03 ||
                 noteType == FusActivity.FusActivityNoteType.Type04 ||
                 noteType == FusActivity.FusActivityNoteType.Type05 )
            {
                blnEnable = true;
                this.mtbDate1.Focus();
            }

            return blnEnable;
        }

        private void ResetView()
        {
            this.mtbWorklistDate.Clear();
            this.mtbRemarks.Clear();
            this.mtbDollar1.Clear();
            this.mtbDollar2.Clear();
            this.mtbDate1.Clear();
            this.mtbDate2.Clear();
            this.cmbMonth.SelectedIndex = -1;
            this.btnOK.Enabled = false;
            this.ExtendedFUSNote = new ExtendedFUSNote();
        }

        private void CheckActivityCodeIsEmpty()
        {
            if( mtbActivityCode.UnMaskedText.Trim().Length == 0 )
            {
                UIColors.SetNormalBgColor( this.mtbActivityCode );
            }
        }

        #endregion

        #region Properties
        
        public Account Model_Account
        {
            private get
            {
                return this.Model as Account;
            }
            set
            {
                this.Model = value;
            }
        }

        #endregion

        #region Private Properties
        private Hashtable WriteableActivityCodesHashtable
        {
            get
            {
                return i_AllWriteableActivityCodesHashtable;
            }
            set
            {
                i_AllWriteableActivityCodesHashtable = value;
            }
        }

        private ArrayList WriteableFusActivities
        {
            get
            {
                return i_WriteableFusActivities;
            }
            set
            {
                i_WriteableFusActivities = value;
            }
        }

        private FusActivity FusActivity
        {
            get
            {
                return this.ExtendedFUSNote.FusActivity;
            }
            set
            {
                this.ExtendedFUSNote.FusActivity = value;
            }
        }

        private ExtendedFUSNote ExtendedFUSNote
        {
            get
            {
                return i_ExtendedFusNote as ExtendedFUSNote;
            }
            set
            {
                i_ExtendedFusNote = value;
            }
        }
        #endregion

        #region Construction and Finalization
        
        public AddFUSNoteView()
        {
            InitializeComponent();
        }
    
        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements

        private Hashtable           i_AllWriteableActivityCodesHashtable = new Hashtable();
        private ArrayList           i_WriteableFusActivities = new ArrayList();
        private ExtendedFUSNote     i_ExtendedFusNote = new ExtendedFUSNote();
        private bool                isDescriptionSetBySelect = false;
        private DateTime            earliestDate = new DateTime( 1800, 01, 01 );
        private FusNoteBrokerProxy  fusNoteBroker = new FusNoteBrokerProxy();

        #endregion

        #region Constants

        private const string    
            TITLE_INVALID_ACTIVITY_CODE     = "Invalid Activity Code",
            TITLE_FUS_ACTIVITY_CHANGE       = "Activity Code / Description Change",
            TITLE_FUS_ACTIVITY_MISMATCH     = "Activity Code - Description Mismatch";
        
        private const string    EXTENSION_ACTIVITY_CODE = "CREMC";

        #endregion        
    }
}
