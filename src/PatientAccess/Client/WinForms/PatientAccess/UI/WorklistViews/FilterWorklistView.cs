using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for FilterWorklistView.
    /// </summary>
    public class FilterWorklistView : ControlView
    {
        #region Events
        public delegate void Print();
        public event EventHandler FirstFilterLetterSelected;
        public event EventHandler LastFilterLetterSelected;
        public event EventHandler PeriodFilterSelected;
        public event EventHandler AdmitFromFilterSelected;
        public event EventHandler AdmitToFilterSelected;
        public event EventHandler ResetButtonClick;
        public event EventHandler ShowWorklistRequest;
        public event Print PrintReport;
        #endregion

        #region Event Handlers
        private void btnShow_Click( object sender, EventArgs e )
        {
            if( VerifyDate( maskAdmitTo ) && VerifyDate( maskAdmitFrom ) && ShowWorklistRequest != null )
            {
                UpdateModel();                
                ShowWorklistRequest( this, null );
            }
        }

        private void btnReset_Click( object sender, EventArgs e )
        {
            SetNormalBgColor(maskAdmitFrom);
            SetNormalBgColor(maskAdmitTo);
            cmboFirstLetter.SelectedIndex = 0;
            cmboLastLetter.SelectedIndex  = 0;
            cmboAdmitPeriod.SelectedIndex = 0;
            maskAdmitFrom.UnMaskedText    = String.Empty;
            maskAdmitTo.UnMaskedText      = String.Empty;
            btnShow.Enabled               = true;
            btnPrint.Enabled              = false;
            
            if( ResetButtonClick != null )
            {
                ResetButtonClick( null, null );
            }
        }

        private void btnPrint_Click( object sender, EventArgs e )
        {
            if( PrintReport != null )
            {
                PrintReport();
            }
        }

        private void cmboFirstLetter_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( FirstFilterLetterSelected != null && cmboFirstLetter.SelectedIndex != -1 )
            {
                string firstLetter = ( string )cmboFirstLetter.SelectedItem;

                if ( LastFilterLetterSelected != null && cmboLastLetter.SelectedIndex != -1 )
                {
                    string lastLetter = ( string ) cmboLastLetter.SelectedItem;

                    // if FirstLetter is greater than LastLetter, set LastLetter to 'Z'
                    if ( String.Compare( firstLetter, lastLetter ) > 0 )
                    {
                        lastLetter = LETTER_Z;
                    }
                    SetLastLetterIndex( lastLetter );
                }

                beginningWithLetter = firstLetter;
                FirstFilterLetterSelected( this, new LooseArgs( cmboFirstLetter.SelectedItem ) );
            }
        }

        private void cmboLastLetter_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( LastFilterLetterSelected != null && cmboLastLetter.SelectedIndex != -1  )
            {
                string lastLetter = ( string )cmboLastLetter.SelectedItem;

                if ( FirstFilterLetterSelected != null && cmboFirstLetter.SelectedIndex != -1 )
                {
                    string firstLetter = ( string )cmboFirstLetter.SelectedItem;

                    // if LastLetter is lesser than FirstLetter, set FirstLetter to 'A'
                    if ( String.Compare( firstLetter, lastLetter ) > 0 )
                    {
                        firstLetter = LETTER_A;
                    }
                    SetFirstLetterIndex( firstLetter );
                }

                endingWithLetter = lastLetter;
                LastFilterLetterSelected( this, new LooseArgs( cmboLastLetter.SelectedItem ) );
            }
        }

        private void cmboAdmitPeriod_SelectedIndexChanged( object sender, EventArgs e )
        {
            // Check that the DateTime field holding today's date is still valid
            DateTime temp = DateTime.Now;

            if( temp.Day != todaysDate.Day || temp.Month != todaysDate.Month || temp.Year != todaysDate.Year )
            {
                ITimeBroker timeBroker  = ProxyFactory.GetTimeBroker();
                todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset, 
                                                User.GetCurrent().Facility.DSTOffset );
            }
            SetNormalBgColor(maskAdmitFrom);
            SetNormalBgColor(maskAdmitTo);

            string maskedTextFrom   = string.Empty;
            string maskedTextTo     = string.Empty;

            if( maskAdmitFrom.UnMaskedText.Trim().Length == 8 )
            {
                maskedTextFrom =    maskAdmitFrom.UnMaskedText.Substring(0,2) + "/" + 
                                    maskAdmitFrom.UnMaskedText.Substring(2,2) + "/" +
                                    maskAdmitFrom.UnMaskedText.Substring(4,4);
            }

            if( maskAdmitTo.UnMaskedText.Trim().Length == 8 )
            {
                maskedTextTo =      maskAdmitTo.UnMaskedText.Substring(0,2) + "/" + 
                                    maskAdmitTo.UnMaskedText.Substring(2,2) + "/" +
                                    maskAdmitTo.UnMaskedText.Substring(4,4);
            }

            WorklistSelectionRange range = (WorklistSelectionRange)PeriodComboBox.SelectedItem;

            if( range != null)
            {
                switch( range.Oid )
                {
                    case WorklistSelectionRange.ALL:
                        maskAdmitTo.UnMaskedText   = String.Empty;
                        maskAdmitFrom.UnMaskedText = String.Empty;
                        btnShow.Enabled            = true;
                        dateFrom                   = DateTime.MinValue;
                        dateTo                     = DateTime.MinValue;
                        break;

                    case WorklistSelectionRange.TODAY:
                    {
                        string date = String.Format( "{0:D2}{1:D2}{2:D4}",
                            todaysDate.Month, todaysDate.Day, todaysDate.Year );
                        maskAdmitFrom.UnMaskedText = date;
                        maskAdmitTo.UnMaskedText   = date;
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.YESTERDAY:
                    {
                        TimeSpan ts = new TimeSpan( -1, 0, 0, 0 );
                        DateTime yesterday = todaysDate + ts;
                        string date = String.Format( "{0:D2}{1:D2}{2:D4}",
                            yesterday.Month, yesterday.Day, yesterday.Year );

                        maskAdmitFrom.UnMaskedText = date;
                        maskAdmitTo.UnMaskedText   = date;
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.TOMORROW:
                    {
                        TimeSpan ts = new TimeSpan( 1, 0, 0, 0 );
                        DateTime tomorrow = todaysDate + ts;
                        string date = String.Format( "{0:D2}{1:D2}{2:D4}",
                            tomorrow.Month, tomorrow.Day, tomorrow.Year );
        
                        maskAdmitFrom.UnMaskedText = date;
                        maskAdmitTo.UnMaskedText   = date;
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.NEXT_3_DAYS:
                    {
                        maskAdmitFrom.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            todaysDate.Month, todaysDate.Day, todaysDate.Year );

                        TimeSpan ts = new TimeSpan( 3, 0, 0, 0 );
                        DateTime endRange = todaysDate + ts;
                        maskAdmitTo.UnMaskedText  = String.Format( "{0:D2}{1:D2}{2:D4}",
                            endRange.Month, endRange.Day, endRange.Year );
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.NEXT_10_DAYS:
                    {
                        maskAdmitFrom.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            todaysDate.Month, todaysDate.Day, todaysDate.Year );

                        TimeSpan ts = new TimeSpan( 10, 0, 0, 0 );
                        DateTime endRange = todaysDate + ts;
                        maskAdmitTo.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            endRange.Month, endRange.Day, endRange.Year );
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.LAST_3_DAYS:
                    {
                        TimeSpan ts = new TimeSpan( -3, 0, 0, 0 );
                        DateTime yesterday = todaysDate + ts;
                        maskAdmitFrom.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            yesterday.Month, yesterday.Day, yesterday.Year );

                        maskAdmitTo.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            todaysDate.Month, todaysDate.Day, todaysDate.Year );
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.LAST_10_DAYS:
                    {
                        TimeSpan ts = new TimeSpan( -10, 0, 0, 0 );
                        DateTime tenDaysAgo = todaysDate + ts;
                        maskAdmitFrom.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            tenDaysAgo.Month, tenDaysAgo.Day, tenDaysAgo.Year );

                        maskAdmitTo.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}",
                            todaysDate.Month, todaysDate.Day, todaysDate.Year );
                        btnShow.Enabled = true;
                    }
                        break;

                    case WorklistSelectionRange.DATE_RANGE:
                    {
                        // OTD# 37302 fix - When Period selection is 'Date Range' always default
                        // the 'From' and 'To' dates to blank for all types of Worklists

                        maskAdmitTo.UnMaskedText   = String.Empty;
                        maskAdmitFrom.UnMaskedText = String.Empty;
                        btnShow.Enabled            = true;
                        dateFrom                   = DateTime.MinValue;
                        dateTo                     = DateTime.MinValue;
                        
                        btnShow.Enabled = ( maskAdmitTo.Text.Length == FULL_DATE_FIELD &&
                             maskAdmitFrom.Text.Length == FULL_DATE_FIELD );
                    }
                        break;
                }

                if ( PeriodFilterSelected != null )
                {
                    PeriodFilterSelected( this, new LooseArgs( range) );
                }
            }           
        }

        // Start of "From" date event handlers
        private void dateFromTimePicker_CloseUp( object sender, EventArgs e )
        {
            SetNormalBgColor(maskAdmitFrom);
            
            int index = cmboAdmitPeriod.FindString( "Date range" );
            if( index != -1 )
            {
                cmboAdmitPeriod.SelectedIndex = index;
            }

            DateTime dt        = dateFromTimePicker.Value;
            maskAdmitFrom.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            dateFrom = dt;
            VerifyDate( maskAdmitFrom );
        }

        private void maskAdmitFrom_Validating(object sender, CancelEventArgs e)
        {
            if( !blnLeaveRun && !dateFromTimePicker.Focused && !dateToTimePicker.Focused && VerifyDate(maskAdmitFrom) )
            {
                SetNormalBgColor( maskAdmitTo );
            }         
        }

        private void maskAdmitFrom_TextChanged( object sender, EventArgs e )
        {
            SetNormalBgColor(maskAdmitFrom);
            btnShow.Enabled = (maskAdmitTo.Text.Length == FULL_DATE_FIELD && maskAdmitFrom.Text.Length == FULL_DATE_FIELD);

            if( maskAdmitFrom.UnMaskedText.Length == 1 )
            {   // Set the Period ComboBox to display "Date range" selection automatically
                dateFrom = DateTime.MinValue;
                int index = cmboAdmitPeriod.FindString( "Date range" );
                if( index != -1 )
                {
                    cmboAdmitPeriod.SelectedIndex = index;
                }
                return;
            }
            
            if( maskAdmitFrom.Text.Substring( 0, 1 ).Equals( " " ) || maskAdmitFrom.Text.Length != FULL_DATE_FIELD )
            {   // Reset key click -- OR -- the manual entry of an incomplete date value
                dateFrom = DateTime.MinValue;
                return;
            }

            // Date field is fully populated.  Enforce date requirements.
            if( ValidateDate( maskAdmitFrom ) )
            {
                int fromMonth = Convert.ToInt32( maskAdmitFrom.Text.Substring( 0, 2 ) );
                int fromDay = Convert.ToInt32( maskAdmitFrom.Text.Substring( 3, 2 ) );
                int fromYear = Convert.ToInt32( maskAdmitFrom.Text.Substring( 6, 4 ) );

                dateFrom = new DateTime( fromYear, fromMonth, fromDay );

                if( AdmitFromFilterSelected != null )
                {   // Date looks good so fire event for the WorklistsView
                    if( dateTo != DateTime.MinValue && dateFrom != DateTime.MinValue )
                    {
                        btnShow.Enabled = true;
                    }
                    AdmitFromFilterSelected( this, new LooseArgs( maskAdmitFrom.Text ) );
                }
            }
        }

        private bool ValidateDate(Control field)
        {
            try
            {
                int fromToMonth = Convert.ToInt32( field.Text.Substring( 0, 2 ) );
                int fromToDay = Convert.ToInt32( field.Text.Substring( 3, 2 ) );
                int fromToYear = Convert.ToInt32( field.Text.Substring( 6, 4 ) );

                DateTime dateFromTo = new DateTime( fromToYear, fromToMonth, fromToDay );

                if( DateValidator.IsValidDate( dateFromTo ) == false )
                {
                    field.Focus();
                    SetErrBgColor( field );
                    MessageBox.Show( DATE_DOES_NOT_EXIST_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return false;
                }                
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                field.Focus();
                SetErrBgColor( field );
                MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }
            return true;
        }
        // End of "From" date event handlers

        // Start of "To" date event handlers
        private void dateToTimePicker_CloseUp( object sender, EventArgs e )
        {
            SetNormalBgColor(maskAdmitTo);
            
            int index = cmboAdmitPeriod.FindString( "Date range" );
            if( index != -1 )
            {
                cmboAdmitPeriod.SelectedIndex = index;
            }
            DateTime dt = dateToTimePicker.Value;
            maskAdmitTo.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            dateTo = dt;
            VerifyDate( maskAdmitTo );
        }

        private void maskAdmitTo_Validating(object sender, CancelEventArgs e)
        {
            if( !blnLeaveRun && !dateFromTimePicker.Focused && !dateToTimePicker.Focused && VerifyDate( maskAdmitTo ) )
            {
                SetNormalBgColor( maskAdmitTo );
            }
        }

        private bool VerifyDate(Control field)
        {
            SetNormalBgColor( maskAdmitFrom );
            SetNormalBgColor( maskAdmitTo );
            
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                            User.GetCurrent().Facility.DSTOffset );

            if( field.Text == string.Empty || field.Text == maskAdmitTo.Mask )
            {
                return true;
            }

            if( cmboAdmitPeriod.SelectedIndex != 0 && 
                field.Text != maskAdmitTo.Mask &&
                field.Text.Length != FULL_DATE_FIELD )
            {
                SetErrBgColor( field );
                MessageBox.Show( DATE_ERROR_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                field.Focus();                
                return false;
            }

            if (ValidateDate(field) == false)
            {
                return false;
            }

            if( dateTo != DateTime.MinValue && dateTo < dateFrom )
            {
                SetErrBgColor(field);
                MessageBox.Show( TO_DATE_IS_EARLIER_MSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                field.Focus();              
                return false;
            }

            if( ScreenIndexes.PREREGISTRATION == ( ScreenIndexes )WorklistType )
            {   // For PreRegistration worklist, the "From"/"To" date must be today's date or later
                if( (field.Name == maskAdmitFrom.Name && dateFrom.Date  < todaysDate.Date  ) || 
                    (field.Name == maskAdmitTo.Name && dateTo.Date  < todaysDate.Date ) 
                    )
                    
                {
                    SetErrBgColor(field);
                    MessageBox.Show( TODAY_DATE_OR_LATER_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    field.Focus();
                    return false;
                }
            }
            else if( ( ScreenIndexes.POSTREGISTRATION == (ScreenIndexes)WorklistType ||
                       ScreenIndexes.PREMSE == (ScreenIndexes)WorklistType ||
                       ScreenIndexes.NOSHOW == (ScreenIndexes)WorklistType ) )
            {   // For PostRegistration, ED, or No-Show worklist, the "From"/"To" date must be today's date or earlier
                if( field.Name == maskAdmitFrom.Name && dateFrom > todaysDate 
                    || field.Name == maskAdmitTo.Name && dateTo > todaysDate )
                {
                    SetErrBgColor(field);
                    MessageBox.Show( TODAY_DATE_OR_EARLIER_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    field.Focus();
                    return false;
                }
            }
            return true;
        }

        private void maskAdmitTo_TextChanged( object sender, EventArgs e )
        {
            SetNormalBgColor(maskAdmitTo);

            btnShow.Enabled = (maskAdmitTo.Text.Length == FULL_DATE_FIELD && maskAdmitFrom.Text.Length == FULL_DATE_FIELD);

            if( maskAdmitTo.UnMaskedText.Length == 1 )
            {   // Set the Period ComboBox to display "Date range" selection automatically
                dateTo = DateTime.MinValue;
                int index = cmboAdmitPeriod.FindString( "Date range" );
                if( index != -1 )
                {
                    cmboAdmitPeriod.SelectedIndex = index;
                }
                return;
            }
            
            if( maskAdmitTo.Text.Substring( 0, 1 ).Equals( " " ) || maskAdmitTo.Text.Length != FULL_DATE_FIELD )
            {   // Reset key click -- OR -- the manual entry of an incomplete date value
                dateTo = DateTime.MinValue;
                return;
            }

            // Date field is fully populated.  Enforce date requirements.
            if( ValidateDate( maskAdmitTo ) )
            {
                int toMonth = Convert.ToInt32( maskAdmitTo.Text.Substring( 0, 2 ) );
                int toDay = Convert.ToInt32( maskAdmitTo.Text.Substring( 3, 2 ) );
                int toYear = Convert.ToInt32( maskAdmitTo.Text.Substring( 6, 4 ) );

                dateTo = new DateTime( toYear, toMonth, toDay );

                if( AdmitToFilterSelected != null )
                {   // Date looks good so fire event for the WorklistsView
                    if( dateTo != DateTime.MinValue && dateFrom != DateTime.MinValue )
                    {
                        btnShow.Enabled = true;
                    }
                    AdmitToFilterSelected( this, new LooseArgs( maskAdmitTo.Text ) );
                }
            }
        }
        // End of "To" date event handlers

        public void WorkListView_Leave( )
        {
            blnLeaveRun = true;
            if( VerifyDate( maskAdmitTo ) )
            {
                VerifyDate( maskAdmitFrom );
            }
            blnLeaveRun = false;
        }

        private void FilterWorklistView_Load( object sender, EventArgs e )
        {

            if( !IsInDesignMode )
            {
                EnableThemesOn( this );

                ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                                User.GetCurrent().Facility.DSTOffset );
            }
        }
        #endregion

        #region Methods
        public void SetPrintButtonState( bool state )
        {
            btnPrint.Enabled = state;
        }
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            int index = -1;

            DateTime temp = DateTime.Now;

            if( temp.Day != todaysDate.Day || temp.Month != todaysDate.Month || temp.Year != todaysDate.Year )
            {
                ITimeBroker timeBroker  = ProxyFactory.GetTimeBroker();
                todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset, 
                                                User.GetCurrent().Facility.DSTOffset );
            }
            dateFrom = todaysDate;
            dateTo   = todaysDate;

            beginningWithLetter = Model_WorklistSetting.BeginningWithLetter;
            endingWithLetter    = Model_WorklistSetting.EndingWithLetter;

            SetFirstLetterIndex( Model_WorklistSetting.BeginningWithLetter );
            SetLastLetterIndex( Model_WorklistSetting.EndingWithLetter );

            if( Model_WorklistSetting.WorklistSelectionRange != null &&
                Model_WorklistSetting.WorklistSelectionRange.Description != String.Empty )
            {
                if( Model_WorklistSetting.WorklistSelectionRange.Oid == 9 )     
                {
                    // OTD# 37302 fix - When Period selection is 'Date Range' always default
                    // the 'From' and 'To' dates to blank for all types of Worklists

                    index = cmboAdmitPeriod.FindString( Model_WorklistSetting.WorklistSelectionRange.Description );
                    if( index != -1 )
                    {
                        cmboAdmitPeriod.SelectedIndex = index;
                    }
                    dateFrom                    = DateTime.MinValue;
                    maskAdmitFrom.UnMaskedText  = String.Empty;
                    dateTo                      = DateTime.MinValue;
                    maskAdmitTo.UnMaskedText    = String.Empty;
                }
                else
                {
                    cmboAdmitPeriod.SelectedIndex = -1;

                    cmboAdmitPeriod.SelectedItem = Model_WorklistSetting.WorklistSelectionRange;
                    index = cmboAdmitPeriod.SelectedIndex;

                    if( index != -1 )
                    {
                        cmboAdmitPeriod.SelectedIndex = index;
                    }

                }
            }
            else
            {
                index = cmboAdmitPeriod.FindString( "All" );
                if( index != -1 )
                {
                    cmboAdmitPeriod.SelectedIndex = index;
                }
            }
        }
        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            // WorklistSettings.SortedColumn gets set in the parent view's ListView.ColumnClick handler
            Model_WorklistSetting.BeginningWithLetter = beginningWithLetter;
            Model_WorklistSetting.EndingWithLetter    = endingWithLetter;
            Model_WorklistSetting.FromDate            = dateFrom;
            Model_WorklistSetting.ToDate              = dateTo;
            WorklistSelectionRange range = (WorklistSelectionRange)PeriodComboBox.SelectedItem;
            if( range != null )
            {
                Model_WorklistSetting.WorklistSelectionRange = range;
            }
        }
        #endregion

        #region Properties

        public int Items
        {
            get
            {
                return i_Items;
            }
            set
            {
                i_Items = value;
            }
        }

        public WorklistSettings Model_WorklistSetting
        {
            get
            {
                return i_worklistSetting;
            }
            set
            {
                i_worklistSetting = value;
            }
        }

        public ComboBox PeriodComboBox
        {   // Need to expose the period ComboBox so the parent view can populate
            // it with the string set appropriate to the worklist view.
            get
            {
                return cmboAdmitPeriod;
            }
        }

        public int WorklistType
        {   
            // You need this to determine what type of worklist this FilterView is sitting in  
            // so that you can enforce the correct DateTime inputs for the "From" and "To" date
            // TextChanged event handlers.  They differ depending on the type of worklist view.
            get
            {
                return i_screenIndex;
            }
            set
            {
                i_screenIndex = value;
            }
        }
        #endregion

        #region Private Methods

        private void SetFirstLetterIndex( string firstLetter )
        {
            int index = cmboFirstLetter.FindString( firstLetter );
            if ( index != -1 )
            {
                cmboFirstLetter.SelectedIndex = index;
            }
        }

        private void SetLastLetterIndex( string lastLetter )
        {
            int index = cmboLastLetter.FindString( lastLetter );
            if ( index != -1 )
            {
                cmboLastLetter.SelectedIndex = index;
            }
        }

        private void SetNormalBgColor(Control field)
        {
            UIColors.SetNormalBgColor( field );
            Refresh();
        }

        private void SetErrBgColor(Control field)
        {
            UIColors.SetErrorBgColor( field );
            Refresh();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpName = new System.Windows.Forms.GroupBox();
            this.cmboLastLetter = new System.Windows.Forms.ComboBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.cmboFirstLetter = new System.Windows.Forms.ComboBox();
            this.lblFrom = new System.Windows.Forms.Label();
            this.grpAdmitDate = new System.Windows.Forms.GroupBox();
            this.dateToTimePicker = new System.Windows.Forms.DateTimePicker();
            this.maskAdmitTo = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAdmitTo = new System.Windows.Forms.Label();
            this.dateFromTimePicker = new System.Windows.Forms.DateTimePicker();
            this.maskAdmitFrom = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAdmitFrom = new System.Windows.Forms.Label();
            this.cmboAdmitPeriod = new System.Windows.Forms.ComboBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.btnShow = new LoggingButton();
            this.btnReset = new LoggingButton();
            this.btnPrint = new LoggingButton();
            this.grpName.SuspendLayout();
            this.grpAdmitDate.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpName
            // 
            this.grpName.Controls.Add(this.cmboLastLetter);
            this.grpName.Controls.Add(this.lblTo);
            this.grpName.Controls.Add(this.cmboFirstLetter);
            this.grpName.Controls.Add(this.lblFrom);
            this.grpName.Location = new System.Drawing.Point(8, 8);
            this.grpName.Name = "grpName";
            this.grpName.Size = new System.Drawing.Size(192, 57);
            this.grpName.TabIndex = 1;
            this.grpName.TabStop = false;
            this.grpName.Text = "Show for last name";
            // 
            // cmboLastLetter
            // 
            this.cmboLastLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLastLetter.Items.AddRange(new object[] {
                                                                "A",
                                                                "B",
                                                                "C",
                                                                "D",
                                                                "E",
                                                                "F",
                                                                "G",
                                                                "H",
                                                                "I",
                                                                "J",
                                                                "K",
                                                                "L",
                                                                "M",
                                                                "N",
                                                                "O",
                                                                "P",
                                                                "Q",
                                                                "R",
                                                                "S",
                                                                "T",
                                                                "U",
                                                                "V",
                                                                "W",
                                                                "X",
                                                                "Y",
                                                                "Z"});
            this.cmboLastLetter.Location = new System.Drawing.Point(134, 23);
            this.cmboLastLetter.Name = "cmboLastLetter";
            this.cmboLastLetter.Size = new System.Drawing.Size(50, 21);
            this.cmboLastLetter.Sorted = true;
            this.cmboLastLetter.TabIndex = 2;
            this.cmboLastLetter.SelectedIndexChanged += new System.EventHandler(this.cmboLastLetter_SelectedIndexChanged);
            // 
            // lblTo
            // 
            this.lblTo.Location = new System.Drawing.Point(106, 26);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(32, 23);
            this.lblTo.TabIndex = 0;
            this.lblTo.Text = "To:";
            // 
            // cmboFirstLetter
            // 
            this.cmboFirstLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboFirstLetter.Items.AddRange(new object[] {
                                                                 "A",
                                                                 "B",
                                                                 "C",
                                                                 "D",
                                                                 "E",
                                                                 "F",
                                                                 "G",
                                                                 "H",
                                                                 "I",
                                                                 "J",
                                                                 "K",
                                                                 "L",
                                                                 "M",
                                                                 "N",
                                                                 "O",
                                                                 "P",
                                                                 "Q",
                                                                 "R",
                                                                 "S",
                                                                 "T",
                                                                 "U",
                                                                 "V",
                                                                 "W",
                                                                 "X",
                                                                 "Y",
                                                                 "Z"});
            this.cmboFirstLetter.Location = new System.Drawing.Point(45, 23);
            this.cmboFirstLetter.Name = "cmboFirstLetter";
            this.cmboFirstLetter.Size = new System.Drawing.Size(50, 21);
            this.cmboFirstLetter.Sorted = true;
            this.cmboFirstLetter.TabIndex = 1;
            this.cmboFirstLetter.SelectedIndexChanged += new System.EventHandler(this.cmboFirstLetter_SelectedIndexChanged);
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(8, 26);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(34, 23);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "From:";
            // 
            // grpAdmitDate
            // 
            this.grpAdmitDate.Controls.Add(this.dateToTimePicker);
            this.grpAdmitDate.Controls.Add(this.maskAdmitTo);
            this.grpAdmitDate.Controls.Add(this.lblAdmitTo);
            this.grpAdmitDate.Controls.Add(this.dateFromTimePicker);
            this.grpAdmitDate.Controls.Add(this.maskAdmitFrom);
            this.grpAdmitDate.Controls.Add(this.lblAdmitFrom);
            this.grpAdmitDate.Controls.Add(this.cmboAdmitPeriod);
            this.grpAdmitDate.Controls.Add(this.lblPeriod);
            this.grpAdmitDate.Location = new System.Drawing.Point(214, 8);
            this.grpAdmitDate.Name = "grpAdmitDate";
            this.grpAdmitDate.Size = new System.Drawing.Size(449, 57);
            this.grpAdmitDate.TabIndex = 3;
            this.grpAdmitDate.TabStop = false;
            this.grpAdmitDate.Text = "Show for admit date";
            // 
            // dateToTimePicker
            // 
            this.dateToTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateToTimePicker.Checked = false;
            this.dateToTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateToTimePicker.Location = new System.Drawing.Point(420, 23);
            this.dateToTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateToTimePicker.Name = "dateToTimePicker";
            this.dateToTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateToTimePicker.TabIndex = 7;
            this.dateToTimePicker.TabStop = false;
            this.dateToTimePicker.CloseUp += new System.EventHandler(this.dateToTimePicker_CloseUp);
            // 
            // maskAdmitTo
            // 
            this.maskAdmitTo.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskAdmitTo.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.maskAdmitTo.Location = new System.Drawing.Point(350, 23);
            this.maskAdmitTo.Mask = "  /  /";
            this.maskAdmitTo.MaxLength = 10;
            this.maskAdmitTo.Name = "maskAdmitTo";
            this.maskAdmitTo.Size = new System.Drawing.Size(70, 20);
            this.maskAdmitTo.TabIndex = 3;
            this.maskAdmitTo.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.maskAdmitTo.Validating += new System.ComponentModel.CancelEventHandler(this.maskAdmitTo_Validating);
            this.maskAdmitTo.TextChanged += new System.EventHandler(this.maskAdmitTo_TextChanged);
            // 
            // lblAdmitTo
            // 
            this.lblAdmitTo.Location = new System.Drawing.Point(323, 26);
            this.lblAdmitTo.Name = "lblAdmitTo";
            this.lblAdmitTo.Size = new System.Drawing.Size(24, 23);
            this.lblAdmitTo.TabIndex = 5;
            this.lblAdmitTo.Text = "To:";
            // 
            // dateFromTimePicker
            // 
            this.dateFromTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateFromTimePicker.Checked = false;
            this.dateFromTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateFromTimePicker.Location = new System.Drawing.Point(290, 23);
            this.dateFromTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateFromTimePicker.Name = "dateFromTimePicker";
            this.dateFromTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateFromTimePicker.TabIndex = 4;
            this.dateFromTimePicker.TabStop = false;
            this.dateFromTimePicker.CloseUp += new System.EventHandler(this.dateFromTimePicker_CloseUp);
            // 
            // maskAdmitFrom
            // 
            this.maskAdmitFrom.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskAdmitFrom.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.maskAdmitFrom.Location = new System.Drawing.Point(220, 23);
            this.maskAdmitFrom.Mask = "  /  /";
            this.maskAdmitFrom.MaxLength = 10;
            this.maskAdmitFrom.Name = "maskAdmitFrom";
            this.maskAdmitFrom.Size = new System.Drawing.Size(70, 20);
            this.maskAdmitFrom.TabIndex = 2;
            this.maskAdmitFrom.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.maskAdmitFrom.Validating += new System.ComponentModel.CancelEventHandler(this.maskAdmitFrom_Validating);
            this.maskAdmitFrom.TextChanged += new System.EventHandler(this.maskAdmitFrom_TextChanged);
            // 
            // lblAdmitFrom
            // 
            this.lblAdmitFrom.Location = new System.Drawing.Point(183, 26);
            this.lblAdmitFrom.Name = "lblAdmitFrom";
            this.lblAdmitFrom.Size = new System.Drawing.Size(48, 23);
            this.lblAdmitFrom.TabIndex = 2;
            this.lblAdmitFrom.Text = "From:";
            // 
            // cmboAdmitPeriod
            // 
            this.cmboAdmitPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboAdmitPeriod.Location = new System.Drawing.Point(51, 23);
            this.cmboAdmitPeriod.Name = "cmboAdmitPeriod";
            this.cmboAdmitPeriod.Size = new System.Drawing.Size(121, 21);
            this.cmboAdmitPeriod.TabIndex = 1;
            this.cmboAdmitPeriod.SelectedIndexChanged += new System.EventHandler(this.cmboAdmitPeriod_SelectedIndexChanged);
            // 
            // lblPeriod
            // 
            this.lblPeriod.Location = new System.Drawing.Point(8, 26);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(48, 23);
            this.lblPeriod.TabIndex = 0;
            this.lblPeriod.Text = "Period:";
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(677, 27);
            this.btnShow.Name = "btnShow";
            this.btnShow.TabIndex = 4;
            this.btnShow.Text = "Sh&ow";
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(757, 27);
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(918, 27);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "Pri&nt Report";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // FilterWorklistView
            // 
            this.AcceptButton = this.btnShow;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.grpAdmitDate);
            this.Controls.Add(this.grpName);
            this.Name = "FilterWorklistView";
            this.Size = new System.Drawing.Size(1010, 75);
            this.Load += new System.EventHandler(this.FilterWorklistView_Load);
            this.grpName.ResumeLayout(false);
            this.grpAdmitDate.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public FilterWorklistView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container             components = null;

        private LoggingButton                 btnShow;
        private LoggingButton                 btnReset;
        private LoggingButton                 btnPrint;

        private ComboBox               cmboLastLetter;
        private ComboBox               cmboFirstLetter;
        private ComboBox               cmboAdmitPeriod;

        private DateTimePicker         dateToTimePicker;
        private DateTimePicker         dateFromTimePicker;

        private GroupBox               grpName;
        private GroupBox               grpAdmitDate;

        private Label                  lblTo;
        private Label                  lblFrom;
        private Label                  lblAdmitFrom;
        private Label                  lblPeriod;
        private Label                  lblAdmitTo;
        
        private MaskedEditTextBox    maskAdmitTo;
        private MaskedEditTextBox    maskAdmitFrom;

        private DateTime                                    dateFrom;
        private DateTime                                    dateTo;
        private DateTime                                    todaysDate;
        private WorklistSettings                            i_worklistSetting;

        private int                                         i_Items;
        private int                                         i_screenIndex;
        private string                                      beginningWithLetter;
        private string                                      endingWithLetter;
        private bool                                        blnLeaveRun;

        private enum ScreenIndexes 
        { 
            NONE,
            PREREGISTRATION, 
            POSTREGISTRATION, 
            INSURANCE, 
            LIABILITY, 
            PREMSE, 
            NOSHOW,
            ONLINEPREREGISTRATION
        };
        
        #endregion

        #region Constants
        private const int    FULL_DATE_FIELD  = 10;
        private const string DATE_DOES_NOT_EXIST_MSG = "The date does not exist on the calendar.";
        private const string DATE_ERROR_MSG = "The date must contain 8 numeric digits, including a 4-digit year.";
        private const string TODAY_DATE_OR_EARLIER_MSG =  "For this worklist, the date must be today's date or earlier.";
        private const string TODAY_DATE_OR_LATER_MSG =  "For this worklist, the date must be today's date or later.";        
        private const string TO_DATE_IS_EARLIER_MSG = "The \"To\" date is earlier than the \"From\" date.  Modify the dates and try again.";

        private const string LETTER_A = "A";
        private const string LETTER_Z = "Z";

        #endregion

    }
}
