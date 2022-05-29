using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// DupeBedAssignmentWarning - this class represents a 'popup' with a warning that a potential duplicate bed assignment
    /// exists in the system.  It is used to display 'exact' matches and potential matches, and the formatting is slightly different
    /// for the two (which is designated by the constructor parms).
    /// </summary>
    
    public partial class DupeBedAssignmentWarning : TimeOutFormView
    {
        #region Events
        #endregion

        #region Event Handlers

        /// <summary>
        /// Yes button event handler - for potential dupes, continue with the bed assignment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYes_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Yes;
            this.Dispose();
        }

        /// <summary>
        /// No button event handler - for potential dupes, do not continue with the bed assignment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNo_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.No;
            this.Dispose();
        }

        /// <summary>
        /// OK button event handler - to close the window for exact matches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click( object sender, EventArgs e )
        {
            this.Dispose();
        }

        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public DupeBedAssignmentWarning()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DupeBedAssignmentWarning - constructor 
        /// </summary>
        /// <param name="isPotential">Indicates if the warning is for potential matches</param>
        /// <param name="accounts">Accounts to display</param>
        public DupeBedAssignmentWarning(DuplicateLocationResult result)
        {
            InitializeComponent();

            base.EnableThemesOn( this );

            if (result.dupeStatus == DuplicateBeds.PotentialDupes)
            {
                this.Text                   = POTENTIAL_MATCHES_HEADER;
                this.lblAccountsText.Text   = POTENTIAL_MATCHES_ACCOUNT_TEXT;
                this.lblText.Text           = POTENTIAL_MATCHES_TEXT;

                this.lblContinue.Visible    = true;

                this.btnYes.Visible         = true;
                this.btnNo.Visible          = true;
                this.btnOK.Visible          = false;
            }
            if (result.dupeStatus == DuplicateBeds.AllowDupes)
            {
                this.Text = MATCHES_HEADER;
                this.lblAccountsText.Text = MATCHES_ACCOUNT_TEXT;
                this.lblText.Text = DUPLICATE_MATCHES_TEXT;

                this.lblContinue.Visible = true;

                this.btnYes.Visible = true;
                this.btnNo.Visible = true;
                this.btnOK.Visible = false;
            }
            else if (result.dupeStatus == DuplicateBeds.MatchedDupes)
            {
                this.Text                   = MATCHES_HEADER;
                this.lblAccountsText.Text   = MATCHES_ACCOUNT_TEXT;
                this.lblText.Text           = MATCHES_TEXT;

                this.lblContinue.Visible    = false;

                this.btnYes.Visible         = false;
                this.btnNo.Visible          = false;
                this.btnOK.Visible          = true;
            }

            // create a data table to bind to the data grid view

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add( "Account Number", typeof( string ) );
            List<long> accounts = result.accounts;
            foreach( long acct in accounts )
            {
                dataTable.Rows.Add( new object[] { acct.ToString() } );
            }

            paDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            paDataGridView1.DataSource = dataTable;            
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string MATCHES_HEADER = "Warning - Duplicate Bed Assignment";
        private const string MATCHES_TEXT = "The patient has previously been assigned a bed in the system.  A duplicate bed assignment " +
            "is not allowed.  Please check the conflicting account(s) and make the necessary adjustments.";
        private const string MATCHES_ACCOUNT_TEXT = "Account Number(s) of the conflicting bed assignment(s):";
        private const string POTENTIAL_MATCHES_HEADER = "Warning - Potential Duplicate Bed Assignment";
        private const string POTENTIAL_MATCHES_TEXT = "A patient with a matching Name and Date of Birth is already " +
            "assigned a bed in the system.  Please check the matching account(s) to ensure you are not making a duplicate bed assignment.";
        private const string POTENTIAL_MATCHES_ACCOUNT_TEXT = "Account Number(s) of the matching bed assignment(s):";
        private const string DUPLICATE_MATCHES_TEXT = "The patient has previously been assigned a bed in the system. " +
                                            " Please check the conflicting account(s) and make the necessary adjustments.";

        #endregion
    }
}