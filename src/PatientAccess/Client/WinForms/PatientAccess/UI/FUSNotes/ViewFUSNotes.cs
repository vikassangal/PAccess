using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Properties;

namespace PatientAccess.UI.FUSNotes
{

    /// <summary>
    /// ViewFUSNotes - this view displays FUS notes associated with the current account.  Included in the display are
    /// notes that have been persisted to PBAR and notes that are pending (i.e. user-added notes or system-generated notes).
    /// The user may elect to view all notes or filter for a particular activity code.
    /// </summary>
    public partial class ViewFUSNotes : TimeOutFormView
    {

        #region Events

        #endregion

        #region Event Handlers

        /// <summary>
        /// ProcessCmdKey - Override this method to handle tabbing and the enter (when focused on the data grid vew).
        /// Tab should go to the next row; shift-Tab should go to the previous row. Also, tabbing on the last row should go to the OK button
        /// and shift-tabbing from the first row should go to the Refresh button.  Enter - while on a row - will expand contract the row and
        /// move to the next row.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey( ref Message message, Keys keyData )
        {
            if( this.dgFUSNotes.Focused )
            {
                const int WM_KEYDOWN = 0x100;
                const int WM_SYSKEYDOWN = 0x104;

                if( ( message.Msg == WM_KEYDOWN ) || ( message.Msg == WM_SYSKEYDOWN ) )
                {
                    if( keyData == Keys.Tab ) // move to next row
                    {
                        int row = 0;

                        if( this.dgFUSNotes.CurrentRow != null )
                        {
                            row = this.dgFUSNotes.CurrentRow.Index;
                            row++;

                            if( row != this.dgFUSNotes.Rows.Count )
                            {
                                this.dgFUSNotes.CurrentCell = this.dgFUSNotes.Rows[row].Cells[0];
                            }
                            else
                            {
                                this.btnOK.Focus();
                            }
                        }
                    }
                    else if( keyData == ( Keys.Tab | Keys.Shift ) ) // move to prior row
                    {
                        int row = 0;

                        if( this.dgFUSNotes.CurrentRow != null )
                        {
                            row = this.dgFUSNotes.CurrentRow.Index;

                            if( row < 0 )
                            {
                                this.btnRefresh.Focus();
                            }
                            else
                            {
                                this.dgFUSNotes.CurrentCell = this.dgFUSNotes.Rows[row].Cells[0];                                
                            }
                        }
                    }
                    else if( keyData == Keys.Right ) // right arrow expands display
                    {
                        int row = 0;

                        if( this.dgFUSNotes.CurrentRow != null )
                        {
                            row = this.dgFUSNotes.CurrentRow.Index;

                            if(  (this.dgFUSNotes.CurrentRow.Cells["ExpandContract"].Value as Image ).Tag.ToString() == PLUS )
                            {
                                ToggleExpandContract( row );
                            }
                        }
                    }
                    else if( keyData == Keys.Left ) // Left arrow contracts display
                    {
                        int row = 0;

                        if( this.dgFUSNotes.CurrentRow != null )
                        {
                            row = this.dgFUSNotes.CurrentRow.Index;

                            if( ( this.dgFUSNotes.CurrentRow.Cells["ExpandContract"].Value as Image ).Tag.ToString() == MINUS )
                            {
                                ToggleExpandContract( row );
                            }
                        }
                    }
                    else if( keyData == Keys.Enter ) 
                    {
                        int row = 0;

                        if( this.dgFUSNotes.CurrentRow != null )
                        {
                            row = this.dgFUSNotes.CurrentRow.Index;
                            ToggleExpandContract( row );
                        }
                    }
                }
            }
            else if( this.btnRefresh.Focused )
            {
                const int WM_KEYDOWN = 0x100;
                const int WM_SYSKEYDOWN = 0x104;

                if( ( message.Msg == WM_KEYDOWN ) || ( message.Msg == WM_SYSKEYDOWN ) )
                {
                    if( keyData == Keys.Tab )
                    {
                        if( this.dgFUSNotes.Rows.Count > 0 )
                        {                            
                            this.dgFUSNotes.CurrentCell = this.dgFUSNotes.Rows[0].Cells[0];
                        }
                    }
                }
            }

            return base.ProcessCmdKey( ref message, keyData );

        }


        /// <summary>
        /// pbExpandContract_Click - picture box that shows a '+' or '-'; causes the display of all notes
        /// to expand (showing system text, remarks, and any extension fields ) or contract (showing only description)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void pbExpandContract_Click( object sender, EventArgs eventArgs )
        {
            Bitmap minusImage = Resources.minus;
            minusImage.Tag = MINUS;

            Bitmap plusImage = Resources.plus;
            plusImage.Tag = PLUS;
           
            foreach( DataGridViewRow aRow in this.dgFUSNotes.Rows )
            {
                if( this.pbExpandContract.Image.Tag as string == PLUS )
                {
                    aRow.Cells["ExpandContract"].Value = plusImage;
                }
                else
                {
                    aRow.Cells["ExpandContract"].Value = minusImage;
                }

                this.ToggleExpandContract( aRow.Index );
            }

            if( this.pbExpandContract.Image.Tag as string == PLUS )
            {
                this.pbExpandContract.Image = minusImage;
                this.lblExpandContract.Text = CONTRACT_ALL;
            }
            else
            {
                this.pbExpandContract.Image = plusImage;
                this.lblExpandContract.Text = EXPAND_ALL;
            }

        }


        /// <summary>
        /// cmbActivityCodes_SelectedIndexChanged - the Description was selected, if it is not blank, enable the Show button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void cmbActivityCodes_SelectedIndexChanged( object sender, EventArgs eventArgs )
        {
            if( this.cmbActivityCodes.SelectedIndex > 0 )
            {

                FusActivity selectedActivity = this.cmbActivityCodes.SelectedItem as FusActivity;

                if( selectedActivity != null )
                {
                    this.mtbActivityCode.Text = selectedActivity.Code;
                }

                this.btnShow.Enabled = true;
            }
            else if( !string.IsNullOrEmpty( this.cmbActivityCodes.Text.Trim() )
                    && this.cmbActivityCodes.Text.Trim().Length == 5 )
            {
                FusActivity anActivity = this.ActivityCodesHash[this.cmbActivityCodes.Text.Trim().ToUpper()] as FusActivity;

                if( anActivity != null )
                {
                    this.cmbActivityCodes.SelectedItem = anActivity;
                }
            }
            else
            {
                this.mtbActivityCode.Text = string.Empty;
                this.btnShow.Enabled = false;
            }
        }


        /// <summary>
        /// The user keyed into the Activity code text box... if it is 5 chars, enable the Show button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void mtbActivityCode_TextChanged( object sender, EventArgs eventArgs )
        {
            if( this.mtbActivityCode.Text.Trim().Length == 0 )
            {
                this.mtbActivityCode.SelectAll();
            }

            if( this.mtbActivityCode.Text.Trim().Length == 5 )
            {
                if( this.mtbActivityCode.Text.Trim() == EXTENSION_ACTIVITY_CODE )
                {
                    UIColors.SetErrorBgColor( this.mtbActivityCode );
                    MessageBox.Show( UIErrorMessages.FUS_CREMC_NOT_RECOGNIZED, "Invalid Activity Code", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }
                else
                {
                    bool isInvalid = true;

                    
                   FusActivity anActivity = this.ActivityCodesHash[this.mtbActivityCode.Text.Trim()] as FusActivity;

                    if( anActivity != null )
                    {
                        isInvalid = false;
                        this.cmbActivityCodes.SelectedItem = anActivity;
                    }

                    if( isInvalid )
                    {
                        MessageBox.Show( UIErrorMessages.FUS_ACTIVITY_NOT_RECOGNIZED, "Invalid Activity Code", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        return;
                    }
                    else
                    {
                        this.btnShow.Enabled = true;
                        this.btnShow.Focus();
                    }
                }
            }
            else
            {
                this.btnShow.Enabled = false;
            }
        }


        /// <summary>
        /// Auto-select all in preparation for entering the masked text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void mtbActivityCode_Click( object sender, EventArgs eventArgs )
        {
            UIColors.SetNormalBgColor( this.mtbActivityCode );

            if( this.mtbActivityCode.Text.Trim().Length == 0 )
            {
                this.mtbActivityCode.SelectAll();
            }
        }


        /// <summary>
        /// auto-select all in preparation for  entering the masked text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void mtbActivityCode_GotFocus( object sender, EventArgs eventArgs )
        {
            UIColors.SetNormalBgColor( this.mtbActivityCode );

            if( this.mtbActivityCode.Text.Trim().Length == 0 )
            {
                this.mtbActivityCode.SelectAll();
            }
        }


        /// <summary>
        /// reset the cursor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ViewFUSNotes_Leave( object sender, EventArgs eventArgs )
        {
            this.Cursor = Cursors.Default;
        }


        /// <summary>
        /// Initialize the screen, drop downs, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void ViewFUSNotes_Load( object sender, EventArgs eventArgs )
        {

            this.mtbActivityCode.Enabled = false;
            this.cmbActivityCodes.Enabled = false;

            Bitmap plusImage = Resources.plus;
            plusImage.Tag = PLUS;

            this.pbExpandContract.Image = plusImage;

            // get activity codes hashtable and load dropdown
            this.cmbActivityCodes.Items.Clear();
            this.cmbActivityCodes.Items.Add( new FusActivity() );

            foreach( FusActivity activity  in this.ActivityCodesArray )
            {
                cmbActivityCodes.Items.Add( activity );
            }

        }


        /// <summary>
        /// enable/disable and clear values as appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void rbSearchByActivity_CheckedChanged( object sender, EventArgs eventArgs )
        {

            this.btnShow.Enabled = false;

            this.mtbActivityCode.Clear();            
            this.cmbActivityCodes.SelectedIndex = 0;

            this.mtbActivityCode.Enabled = true;
            this.cmbActivityCodes.Enabled = false;

            this.mtbActivityCode.Focus();            

        }


        /// <summary>
        /// enable/disable and clear values as appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        private void rbSearchDescription_CheckedChanged( object sender, EventArgs eventArgs )
        {

            this.btnShow.Enabled = false;            

            this.mtbActivityCode.Clear();
            this.cmbActivityCodes.SelectedIndex = 0;

            this.mtbActivityCode.Enabled = false;
            this.cmbActivityCodes.Enabled = true;

        }


        /// <summary>
        /// enable/disable and clear values as appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>       
        private void rbAll_CheckedChanged( object sender, EventArgs eventArgs )
        {   
         
            this.mtbActivityCode.Clear();
            this.cmbActivityCodes.SelectedIndex = 0;

            this.mtbActivityCode.Enabled = false;
            this.cmbActivityCodes.Enabled = false;

            this.btnShow.Enabled = true;

        }


        /// <summary>
        /// launch the 'Add a fus note' dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void btnAdd_Click( object sender, EventArgs eventArgs )
        {

            AddFUSNoteView addFusNoteView = new AddFUSNoteView();
            addFusNoteView.Model_Account = this.Model_Account;
            addFusNoteView.UpdateView();

            addFusNoteView.SetFocus();
            DialogResult dialogResult = addFusNoteView.ShowDialog();


            if( dialogResult == DialogResult.Cancel )
            {
                return;
            }

            addFusNoteView.Hide();

            // update the display once the adds are done.

            if( this.dgFUSNotes.Rows.Count > 0 
                || ( this.dgFUSNotes.Rows.Count == 0 && this.Model_Account.FusNotes.Count > 0 ) )
            {
                //LooseArgs args = new LooseArgs( "btnAdd" );
                this.btnShow_Click( sender, eventArgs );
            }

        }


        /// <summary>
        /// Take actions before the show call
        /// </summary>
        private void BeforeShow()
        {
            this.Cursor = Cursors.WaitCursor;

            this.pnlNoResults.Visible = false;
            this.pnlNoResults.SendToBack();

            this.SetPlusButtonImage();

            this.dgFUSNotes.Rows.Clear();
        }


        /// <summary>
        /// Update the view after the show.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void AfterShow( object sender, RunWorkerCompletedEventArgs eventArgs )
        {

            if( eventArgs.Error != null )
            {
                this.Cursor = Cursors.Default;

                MessageBox.Show( 
                    UIErrorMessages.FUS_UNAVAILABLE, 
                    "FUS System unavailable", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error );
            }
            else
            {

                try
                {

                    // Only replace the notes if the DoShow call returned results
                    if( null != eventArgs.Result )
                    {
                        this.Model_Account.PersistedFusNotes = eventArgs.Result as ArrayList;
                    }

                    if( null != this.Model_Account.AllFusNotes &&
                        this.Model_Account.AllFusNotes.Count > 0 )
                    {

                        foreach( ExtendedFUSNote note in this.Model_Account.AllFusNotes )
                        {
                            this.AddFusNoteToGrid( note );
                        }

                        if( dgFUSNotes.Rows.Count == 0 )
                        {
                            this.DisplayNoResultsPanel();
                        }

                    }
                    else
                    {
                        this.DisplayNoResultsPanel();
                    }

                }
                finally
                {

                    this.Cursor = Cursors.Default;
                    this.btnRefresh.Enabled = true;

                }
            }

        }


        /// <summary>
        /// Displays the no results panel.
        /// </summary>
        private void DisplayNoResultsPanel()
        {

            this.lblNoResults.Text = NO_MATCHING_RESULTS;
            pnlNoResults.Visible = true;
            pnlNoResults.BringToFront();

        }


        /// <summary>
        /// Adds the FUS note to the grid.
        /// </summary>
        /// <param name="note">The note.</param>
        private void AddFusNoteToGrid( ExtendedFUSNote note )
        {

            if( this.ShouldThisNoteBeProcessed( note ) )
            {
                    Bitmap aBitmap = Resources.plus;
                    aBitmap.Tag = PLUS;

                string entryTimeText = this.GetEntryTimeTextFor( note );

                    this.dgFUSNotes.Rows.Add(
                        entryTimeText,                          // this.PostedDate
                        aBitmap,                                // this.ExpandContract
                        note.FusActivity.Code,                  // this.ActivityCode
                        note.FusActivity.Description,           // this.ActivityDescription
                        note.FusActivity.Description,           // this.Description
                        note.WorklistDate.ToShortDateString(),  // this.WorklistDate
                        note.UserID,                            // this.UserID
                        note.Remarks,                           // this.Remarks
                        note.Text,                              // this.NoteText
                        note.FusActivity.Code,                  // this.ParentActivityCode
                        note.FusActivity.NoteType.ToString(),   // this.NoteType
                        note.Dollar1.ToString( "C" ),           // this.Amount1
                        note.Dollar2.ToString( "C" ),           // this.Amount2
                        note.Date1.ToShortDateString(),         // this.Date1
                        note.Date2.ToShortDateString(),         // this.Date2
                        note.Month,                             // this.Month
                        note.FusActivity.Code + entryTimeText,  //this.SortableActivityCode
                        "1",                                    //this.SortSequence
                        note.FusActivity.Description );         //this.ParentActivityDescription

                }
            }

        private string GetEntryTimeTextFor( ExtendedFUSNote note ) 
        {
            DateTime entryTime = DateTime.Parse( note.CreatedOn.ToString() );
            string entryTimeText = entryTime.ToString( "MM/dd/yyyy HH:mm" );

            // Tag newly-added notes with an asterisk
            if (!note.Persisted)
            {
                entryTimeText = " * " + entryTimeText;
            }
            else
                {
                entryTimeText = "   " + entryTimeText;
                }
            return entryTimeText;
            }

        /// <summary>
        /// Should this note be processed?
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns>true if the note is an extension note or is not filtered</returns>
        private bool ShouldThisNoteBeProcessed( FusNote note )
        {

            bool shouldProcess = true;

            if( !this.rbAll.Checked )
            {
                if( note.FusActivity.Code != this.mtbActivityCode.Text.Trim() && !note.IsExtensionNote )
                {
                    shouldProcess = false;
                }
            }

            return shouldProcess;

        }

        private void DoShow( object sender, DoWorkEventArgs eventArgs )
        {

            Account theModel = eventArgs.Argument as Account;
            
            // only retrieve from PBAR if note have not previously been retrieved. Note, btnRefresh_Click handler will always
            // retrieve from PBAR
            if( theModel.PersistedFusNotes.Count == 0 )
            {

                IFUSNoteBroker iFUSNoteBroker = BrokerFactory.BrokerOfType<IFUSNoteBroker>();

                eventArgs.Result = iFUSNoteBroker.GetMergedFUSNotesFor( theModel );
            }

        }


        /// <summary>
        /// show FUS notes associated with this account; notes are shown in the contracted
        /// state (showing the description associated  with the activity code only).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void btnShow_Click( object sender, EventArgs e )
        {

            BackgroundWorker backgroundWorker = new BackgroundWorker();

            this.BeforeShow();
            backgroundWorker.DoWork += this.DoShow;
            backgroundWorker.RunWorkerCompleted += this.AfterShow;
            backgroundWorker.RunWorkerAsync( this.Model_Account );

        }


        /// <summary>
        /// if the cell is the ExpandContract icon, toggle the display; if it is the
        /// description cell, launch the <see cref="ViewDescription"/> dialog to see the
        /// full note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        void dgFUSNotes_CellDoubleClick( object sender, DataGridViewCellEventArgs eventArgs )
        {

            if( eventArgs.RowIndex >= 0 )
            {
                if( this.dgFUSNotes.Columns[eventArgs.ColumnIndex].Name == "ExpandContract" )
                {            
                    this.ToggleExpandContract( eventArgs.RowIndex );
                }
                else
                {
                    DataGridViewRow aRow = dgFUSNotes.Rows[eventArgs.RowIndex];
                    ViewDescription viewDescription = null;

                    if( ( aRow.Cells["ExpandContract"].Value as Bitmap ).Tag as string == PLUS )
                    {
                        viewDescription = new ViewDescription( this.FormatDescription( aRow ) );
                    }
                    else
                    {
                        viewDescription = new ViewDescription( this.dgFUSNotes.CurrentRow.Cells["Description"].Value.ToString() );
                    }

                    viewDescription.Text = 
                        this.dgFUSNotes.CurrentRow.Cells["ActivityCode"].Value +
                        " - " + this.dgFUSNotes.CurrentRow.Cells["PostedDate"].Value.ToString().Trim();
                    viewDescription.SetBackgroundToWhite();
                    viewDescription.ShowDialog( this );


                }
            }

        }
        

        /// <summary>
        /// expand or contract, based on the current state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void dgFUSNotes_CellClick( object sender, DataGridViewCellEventArgs eventArgs )
        {
            if( this.dgFUSNotes.Columns[eventArgs.ColumnIndex].Name == "ExpandContract" )
            {
                this.ToggleExpandContract( eventArgs.RowIndex );
            }
        }


        /// <summary>
        /// toggle the display to/from expanded/contracted
        /// </summary>
        /// <param name="rowIndex"></param>
        private void ToggleExpandContract( int rowIndex )
        {
            if( rowIndex >= 0 )
            {
                DataGridViewRow aRow = dgFUSNotes.Rows[ rowIndex ];

                if( ( aRow.Cells["ExpandContract"].Value as Bitmap ).Tag as string == PLUS )
                {
                    Bitmap minusBitmap = Resources.minus;
                    minusBitmap.Tag = MINUS;

                    aRow.Cells["ExpandContract"].Value = minusBitmap;
                    aRow.Cells["Description"].Value = this.FormatDescription( aRow );
                }
                else
                {
                    Bitmap plusBitmap = Resources.plus;
                    plusBitmap.Tag = PLUS;

                    aRow.Cells["ExpandContract"].Value = plusBitmap;
                    aRow.Cells["Description"].Value = aRow.Cells["ActivityDescription"].Value;
                }
            }
        }


        /// <summary>
        /// format the display for the expanded note
        /// </summary>
        /// <param name="aRow"></param>
        /// <returns></returns>        
        private string FormatDescription( DataGridViewRow aRow )
        {

            StringBuilder aStringBuilder = new StringBuilder();
            string remarks  = aRow.Cells["Remarks"].Value as string;

            if( aRow.Cells["ActivityCode"].Value.ToString() != EXTENSION_ACTIVITY_CODE )
            {
                if( aRow.Cells["ActivityDescription"].Value.ToString() != string.Empty )
                {
                    string strToAppend = aRow.Cells["ActivityDescription"].Value as string;

                    if( !string.IsNullOrEmpty( strToAppend.TrimEnd() ) )
                    {
                        aStringBuilder.Append( strToAppend.TrimEnd() );
                    }
                }

                // if the note includes additional, system-generated text, display it on a separate line

                if( !string.IsNullOrEmpty( aRow.Cells["NoteText"].Value.ToString().TrimEnd() ) )
                {
                    aStringBuilder.Append( Environment.NewLine );
                    aStringBuilder.Append( "     " + aRow.Cells["NoteText"].Value.ToString().TrimEnd() );
                }

                // add a blank line if remarks will follow...

                if( !string.IsNullOrEmpty( remarks.TrimEnd() ) )
                {
                    aStringBuilder.Append( Environment.NewLine );
                    aStringBuilder.Append( Environment.NewLine );
                }
            }                        

            // format out the 3 lines of the FUS remarks.

            string remarks1 = string.Empty;
            string remarks2 = string.Empty;
            string remarks3 = string.Empty;

            if( aRow.Cells["ActivityCode"].Value.ToString() == EXTENSION_ACTIVITY_CODE )
            {
                aStringBuilder.Append( remarks );
            }
            else
            {
                if( !string.IsNullOrEmpty( remarks.TrimEnd() ) )
                {
                    if( remarks.Length >= 44 )
                    {
                        remarks1 = remarks1.PadRight( 34, ' ' ) + remarks.Substring( 0, 44 );
                    }
                    else
                    {
                        remarks1 = remarks1.PadRight( 34, ' ' ) + remarks;
                    }

                    if( remarks.Length >= 122 )
                    {
                        remarks2 = remarks.Substring( 44, 78 );
                    }
                    else
                    {
                        if( remarks.TrimEnd().Length > 44 )
                        {
                            remarks2 = remarks.Substring( 44 );
                        }
                    }

                    if( remarks.Length > 122 )
                    {
                        remarks3 = remarks.Substring( 122 );
                    }

                    if( remarks1.Trim() != string.Empty )
                    {
                        aStringBuilder.Append( remarks1 );
                    }

                    if( remarks2.Trim() != string.Empty )
                    {
                        aStringBuilder.Append( Environment.NewLine );
                        aStringBuilder.Append( remarks2 );
                    }

                    if( remarks3.Trim() != string.Empty )
                    {
                        aStringBuilder.Append( Environment.NewLine );
                        aStringBuilder.Append( remarks3 );
                    }
                }
            }

            string extendedNoteLines = this.FormatExtendedNoteLines( aRow );

            if( !string.IsNullOrEmpty( extendedNoteLines.TrimEnd() ) )
            {
                aStringBuilder.Append( extendedNoteLines.TrimEnd() );
            }

            return aStringBuilder.ToString();

        }


        /// <summary>
        /// determine the format of additional lines if the note type has extended values
        /// (date1, date2, amount1, amount2, or month)
        /// </summary>
        /// <param name="aRow"></param>
        /// <returns></returns>        
        private string FormatExtendedNoteLines( DataGridViewRow aRow )
        {

            // format the line to 80 spaces
            string extendedNoteLines = string.Empty;

            string noteType = aRow.Cells["NoteType"].Value.ToString();

            switch (noteType)
            {
                case "Type00":
                {
                    break;
                }
                case "Type01":
                {
                    string date1 = aRow.Cells["Date1"].Value.ToString();

                    extendedNoteLines = date1 + extendedNoteLines.PadRight( LINE_LENGTH - date1.Length, CHAR_SPACE );
                    break;
                }
                case "Type02":
                {
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();

                    extendedNoteLines = extendedNoteLines.PadRight( LINE_LENGTH - dollar1.Length, CHAR_SPACE ) + dollar1;
                    break;
                }
                case "Type03":
                {
                    string date1 = aRow.Cells["Date1"].Value.ToString();
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();

                    extendedNoteLines = date1 + extendedNoteLines.PadRight( LINE_LENGTH - date1.Length - dollar1.Length, CHAR_SPACE )
                        + dollar1;
                    break;
                }
                case "Type04":
                {
                    string extendedNoteLine1 = string.Empty;
                    string extendedNoteLine2 = string.Empty;

                    string date1 = aRow.Cells["Date1"].Value.ToString();
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();

                    string date2 = aRow.Cells["Date2"].Value.ToString();
                    string dollar2 = aRow.Cells["Amount2"].Value.ToString();

                    extendedNoteLine1 = date1 + extendedNoteLine1.PadRight( LINE_LENGTH - date1.Length - dollar1.Length, CHAR_SPACE )
                        + dollar1;                    
                    extendedNoteLine2 = date2 + extendedNoteLine2.PadRight( LINE_LENGTH - date2.Length - dollar2.Length, CHAR_SPACE )
                        + dollar2;

                    extendedNoteLines = string.Concat( extendedNoteLine1, Environment.NewLine, extendedNoteLine2 );

                    break;
                }
                case "Type05":
                {
                    string date1 = aRow.Cells["Date1"].Value.ToString();
                    string date2 = aRow.Cells["Date2"].Value.ToString();

                    extendedNoteLines = date1 + extendedNoteLines.PadRight( LINE_LENGTH - date1.Length - date2.Length, CHAR_SPACE )
                        + date2;

                    break;
                }
                case "Type06":
                {
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();
                    string dollar2 = aRow.Cells["Amount2"].Value.ToString();

                    extendedNoteLines = dollar1 + extendedNoteLines.PadRight( LINE_LENGTH - dollar1.Length - dollar2.Length, CHAR_SPACE )
                        + dollar2;

                    break;
                }
                case "Type07":
                {
                    break;
                }
                case "Type10":
                {
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();

                    extendedNoteLines = extendedNoteLines.PadRight( LINE_LENGTH - dollar1.Length, CHAR_SPACE ) + dollar1;

                    break;
                }
                case "Type12":
                {
                    string dollar1 = aRow.Cells["Amount1"].Value.ToString();
                    string month = aRow.Cells["Month"].Value.ToString();

                    extendedNoteLines = month + extendedNoteLines.PadRight( LINE_LENGTH - month.Length - dollar1.Length, CHAR_SPACE ) + dollar1;
                    break;
                }
            }

            if( !string.IsNullOrEmpty( extendedNoteLines.TrimEnd() ) )
            {
                extendedNoteLines = string.Concat( Environment.NewLine, Environment.NewLine, extendedNoteLines.TrimEnd() );
            }

            return extendedNoteLines;

        }


        /// <summary>
        /// clear the displayed notes and reset the criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void btnReset_Click( object sender, EventArgs e )
        {

            this.pnlNoResults.Visible = false;
            this.pnlNoResults.SendToBack();

            this.dgFUSNotes.Rows.Clear();

            this.rbAll.Select();

            this.mtbActivityCode.Enabled = false;
            this.cmbActivityCodes.Enabled = false;

            this.SetPlusButtonImage();

        }


        /// <summary>
        /// clear any previous notes from the persisted collection on the account
        /// and refresh via a new call to the CIE web service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void btnRefresh_Click( object sender, EventArgs e )
        {
            // OTD36995 fix - Disable the button to prevent the user from clicking on it more than once
            this.btnRefresh.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Clear the display
                this.dgFUSNotes.Rows.Clear();
                this.dgFUSNotes.Refresh();

                // Clear the data
                this.Model_Account.ClearPersistedFusNotes();

                // Delegate to the show handler
                this.btnShow_Click( sender, e );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }
        

        /// <summary>
        /// set the main expand/contract back to initial state (plus)
        /// </summary>        
        private void SetPlusButtonImage()
        {

            Bitmap plusImage = Resources.plus;
            plusImage.Tag = PLUS;

            this.pbExpandContract.Image = plusImage;
            this.lblExpandContract.Text = EXPAND_ALL;

        }


        /// <summary>
        /// close this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// initialize values based on the contents of Model_Account
        /// </summary>
        public override void UpdateView()
        {

            if( this.Model_Account != null )
            {

                // This check is here to handle a lack of data integrity in the FUS system
                if( this.Model_Account.AccountNumber != 0 )
                {
                    this.lblAccountValue.Text = this.Model_Account.AccountNumber.ToString();
                }

                if( this.Model_Account.Patient != null )
                {
                    if( this.Model_Account.Patient.FormattedName.Trim().Length > 1 )
                    {
                        this.lblPatientValue.Text = this.Model_Account.Patient.FormattedName;
                    }
                }

            }
        }


        /// <summary>
        /// catch the compare event to do custom sorting on multiple columns
        /// If the primary sort (the column clicked) results in a match, then the secondary sort will
        /// order by the parent activity code or description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>        
        void dgFUSNotes_SortCompare( object sender, DataGridViewSortCompareEventArgs eventArgs )
        {

            if( eventArgs.Column.Name == "ActivityCode" )
            {
                eventArgs.SortResult = String.Compare(
                    dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["ActivityCode"].Value.ToString(),
                    dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["ActivityCode"].Value.ToString() );

                this.SecondarySort( eventArgs );                    
            }
            else if( eventArgs.Column.Name == "PostedDate" )
            {
                eventArgs.SortResult = String.Compare(
                    dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["PostedDate"].Value.ToString(),
                    dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["PostedDate"].Value.ToString() );

                if( eventArgs.SortResult == 0 )
                {
                    this.SecondarySort( eventArgs );
                }
            }
            else if( eventArgs.Column.Name == "UserID" )
            {
                eventArgs.SortResult = String.Compare(
                    dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["UserID"].Value.ToString(),
                    dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["UserID"].Value.ToString() );

                if( eventArgs.SortResult == 0 )
                {
                    this.SecondarySort( eventArgs );
                }
            }
            else if( eventArgs.Column.Name == "WorklistDate" )
            {
                eventArgs.SortResult = String.Compare(
                    dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["WorklistDate"].Value.ToString(),
                    dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["WorklistDate"].Value.ToString() );

                if( eventArgs.SortResult == 0 )
                {
                    this.SecondarySort( eventArgs );
                }
            }
            else if( eventArgs.Column.Name == "Description" )
            {
                eventArgs.SortResult = String.Compare(
                    dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["ParentActivityDescription"].Value.ToString(),
                    dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["ParentActivityDescription"].Value.ToString() );

                if( eventArgs.SortResult == 0 )
                {
                    this.SecondarySort( eventArgs );
                }
            }

            eventArgs.Handled = true;
        }


        /// <summary>
        /// the secondary sort (by code/description and sequence) is invoked when the primary sort is a match.
        /// This code is now a bit of an overkill as the CREMC notes are no longer separate notes; it remains for posterity.
        /// </summary>
        /// <param name="eventArgs"></param>
        private void SecondarySort( DataGridViewSortCompareEventArgs eventArgs )
        {

            eventArgs.SortResult = String.Compare(
            dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["SortableActivityCode"].Value.ToString(),
            dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["SortableActivityCode"].Value.ToString() );

            if( eventArgs.SortResult == 0 )
            {

                string row1ValueText = dgFUSNotes.Rows[eventArgs.RowIndex1].Cells["SortSequence"].Value.ToString();
                string row2ValueText = dgFUSNotes.Rows[eventArgs.RowIndex2].Cells["SortSequence"].Value.ToString();

                int row1Value = Convert.ToInt16( row1ValueText );
                int row2Value = Convert.ToInt16( row2ValueText );

                // force the CREMC notes to always display ascending under the parent activity note
                if( this.dgFUSNotes.SortOrder == SortOrder.Descending )
                {
                    row1Value = 5000 - row1Value;
                    row2Value = 5000 - row2Value;
                }

                row1ValueText = row1Value.ToString();
                row2ValueText = row2Value.ToString();

                eventArgs.SortResult = String.Compare( row1ValueText, row2ValueText );

            }

        }

        #endregion

        #region Properties

        /// <summary>
        /// local <see cref="Hashtable"/> of all activities (for code verification)
        /// </summary>        
        private Hashtable ActivityCodesHash
        {
            get
            {
                if( this.i_ActivityCodesHash.Count == 0 )
                {
                    FusNoteBrokerProxy fusNoteBrokerProxy = new FusNoteBrokerProxy();

                    this.i_ActivityCodesHash = fusNoteBrokerProxy.AllActivityCodesHashtable();
                }

                return i_ActivityCodesHash;
            }
        }


        /// <summary>
        /// local < see cref="ArrayList"/> of all activity codes (for drop down)
        /// </summary>       
        private ArrayList ActivityCodesArray
        {
            get
            {
                if( this.i_ActivityCodesArray.Count == 0 )
                {
                    FusNoteBrokerProxy fusNoteBrokerProxy = new FusNoteBrokerProxy();

                    this.i_ActivityCodesArray = fusNoteBrokerProxy.AllActivityCodes() as ArrayList;
                }

                return i_ActivityCodesArray;
            }
        }


        /// <summary>
        /// model for this view
        /// </summary>       
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

        #region Private Methods

        /// <summary>
        /// Construct this view!
        /// </summary>        
        public ViewFUSNotes()
        {
            InitializeComponent();
        }

        #endregion

        #region Component Designer Generated Code

        #endregion

        #region Data Elements

        private Hashtable                           i_ActivityCodesHash         = new Hashtable();
        private ArrayList                           i_ActivityCodesArray        = new ArrayList();

        #endregion

        #region Constants

        private const string                        PLUS = "PLUS",
                                                    MINUS = "MINUS";

        private const string                        EXPAND_ALL = "Expand All",
                                                    CONTRACT_ALL = "Collapse All";

        private const string                        NO_MATCHING_RESULTS = "There are no matching FUS Notes associated with this account.";

        private const string                        EXTENSION_ACTIVITY_CODE = "CREMC";

        private const char                          CHAR_SPACE = ' ';

        private const int                           LINE_LENGTH = 78;

        #endregion        

    }
}
