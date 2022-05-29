using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Services.DocumentManagement;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Rules;
using Account = PatientAccess.Domain.Account;
using Microsoft.Win32;
using System.Text;
using System.Diagnostics;

namespace PatientAccess.UI.DocumentImagingViews
{
    /// <summary>
    /// Summary description for ListOfDocumentsView.
    /// </summary>
    //TODO: Create XML summary comment for ListOfDocumentsView
    [Serializable]
    public class ListOfDocumentsView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region btnScan_Click Handlers

        private void ListOfDocumentsView_Enter(object sender, EventArgs e)
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if (accountView.IsMedicareAdvisedForPatient())
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show(UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (warningResult == DialogResult.Yes)
                {
                    if (EnableInsuranceTab != null)
                    {
                        EnableInsuranceTab(this, new LooseArgs(Model));
                    }
                }
            }
        }

        /// <summary>
        /// chkAllDocuments_CheckedChanged = the user has checked or unchecked the 'Select all' checkbox;
        /// select or de-selected the documents in the listview accordingly.  We turn off the list item control
        /// change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAllDocuments_CheckedChanged(object sender, EventArgs e)
        {
            this.lvDocumentList.SelectedIndexChanged -= new EventHandler(this.lvDocumentList_SelectedIndexChanged);
            this.i_Docs.Clear();

            if (chkAllDocuments.Checked)
            {
                foreach (ListViewItem lvi in this.lvDocumentList.Items)
                {
                    lvi.Selected = true;
                    this.i_Docs.Add(lvi.Tag);
                }
                //this.lvDocumentList.Focus();
                this.btnViewDocument.Enabled = true;
            }
            else
            {
                foreach (ListViewItem lvi in this.lvDocumentList.Items)
                {
                    lvi.Selected = false;
                }
                this.btnViewDocument.Enabled = false;
            }

            this.lvDocumentList.SelectedIndexChanged += new EventHandler(this.lvDocumentList_SelectedIndexChanged);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            this.OpenViewDocumentsForm("SCAN");
            this.LoadScannedDocuments();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.LoadScannedDocuments();
            Cursor.Current = Cursors.Default;
        }

        private void btnViewDocument_Click(object sender, EventArgs e)
        {
            if (this.lvDocumentList.SelectedItems.Count > 20)
            {
                // error! more than 20 documents selected for viewing

                MessageBox.Show(UIErrorMessages.DOC_IMG_TOO_MANY_DOCS, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                this.OpenViewDocumentsForm("VIEW");
            }
        }

        /// <summary>
        /// lvDocumentList_SelectedIndexChanged - the user clicked into the list view - deal with the associated
        /// screen behavior.  We disable the Select all box event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvDocumentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvDocumentList.SelectedItems.Count > 0)
            {
                this.btnViewDocument.Enabled = this.EnableViewDocumentsButton();

                this.i_Docs.Clear();

                foreach (ListViewItem lvi in this.lvDocumentList.SelectedItems)
                {
                    this.i_Docs.Add(lvi.Tag);
                }

                if (this.lvDocumentList.SelectedItems.Count != this.lvDocumentList.Items.Count)
                {
                    this.chkAllDocuments.CheckedChanged -= new EventHandler(this.chkAllDocuments_CheckedChanged);
                    this.chkAllDocuments.Checked = false;
                    this.chkAllDocuments.CheckedChanged += new EventHandler(this.chkAllDocuments_CheckedChanged);
                }
            }
        }

        private void lvDocumentList_DoubleClick(object sender, EventArgs e)
        {
            if (this.Model.AccountNumber > 0 &&
                this.lvDocumentList.SelectedItems.Count > 0)
            {
                this.OpenViewDocumentsForm("VIEW");
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            Cursor.Current = Cursors.WaitCursor;

            this.panelNoDocuments.Hide();

            if (this.Model != null)
            {
                // OTD# 37444 fix - Do not load Scanned documents if it is a new account.
                // Some Production facilities like DHF, LAK & FVR have test accounts with account number = 0,
                // that have scanned documents associated with them in VIWeb. We do not want to load these.
                if (this.Model.AccountNumber > 1)
                {
                    this.LoadScannedDocuments();
                }

                this.IsPatientNameValid();

                if (!this.IsAccountNumberValid())
                {
                    this.panelNoDocuments.Hide();
                    this.lvDocumentList.Show();
                }
            }

            Cursor.Current = Cursors.Default;

            if (this.btnScan.Enabled)
            {
                this.btnScan.Focus();
            }
            else
            {
                this.btnRefresh.Focus();
            }
        }

        /// <summary>
        /// UpdateModel method
        /// </summary>
        public override void UpdateModel()
        {

        }
        #endregion

        #region Properties
        public new Account Model
        {
            private get
            {
                return (Account)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private Methods
        private bool EnableViewDocumentsButton()
        {
            if (this.Model.AccountNumber > 0 &&
                this.lvDocumentList.SelectedItems.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OpenViewDocumentsForm(string docAction)
        {
            var vIwebHtml5Handler=new VIwebHTML5Handler();
            // If Facility enabled
            VIWEBFeatureManager = new VIWEBFeatureManager();
            var isFacilityEnabled = VIWEBFeatureManager.IsHTML5VIWebEnabledForFacility(this.Model as Account);
            if (isFacilityEnabled)
            {
                if (vIwebHtml5Handler.IsDynamsoftInstalled())
                {
                    if (!vIwebHtml5Handler.IsChromeInstalled() 
                        && !vIwebHtml5Handler.IsEdgeInstalled() 
                        && !vIwebHtml5Handler.IsFirefoxInstalled())
                    {
                        MessageBox.Show(
                            UIErrorMessages.REQUIRED_MODERN_BROWSER_MSG,
                            UIErrorMessages.REQUIRED_MODERN_BROWSER_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        LoadLegacyVIweb(docAction);
                    }
                    else
                    {
                        vIwebHtml5Handler.Model = this.Model;
                        if (docAction.ToUpper() == "SCAN")
                        {
                            vIwebHtml5Handler.DoScanDocument();
                        }
                        else
                        {
                            vIwebHtml5Handler.DoViewDocument(this.i_Docs);
                        }
                    }
                }
                    // If scan software Dynamsoft is not installed into client machine
                else
                {
                    MessageBox.Show(
                            UIErrorMessages.REQUIRED_DRIVER_TO_SCAN_MSG,
                            UIErrorMessages.REQUIRED_DRIVER_TO_SCAN_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    LoadLegacyVIweb(docAction);
                }
            }
            // If facility is not enabled for new VIweb
            else
            {
                LoadLegacyVIweb(docAction);
            }
        }

        public void LoadLegacyVIweb(string docAction)
        {
            FormWebBrowserView formWebBrowserView = new FormWebBrowserView();
            formWebBrowserView.Model = this.Model;
            formWebBrowserView.UpdateView();

            if (docAction.ToUpper() == "SCAN")
            {
                formWebBrowserView.ScanDocument();
            }
            else
            {
                formWebBrowserView.ViewDocument(i_Docs);
            }

            formWebBrowserView.ShowDialog(this);
            formWebBrowserView.Dispose();
        }

        private void LoadScannedDocuments()
        {
            try
            {

                IVIWebServiceBroker broker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
                DocumentListResponse response = broker.GetDocumentList(this.Model.AccountNumber, this.Model.Facility.Code);

                this.lvDocumentList.Items.Clear();

                // load the non-cash documents

                DateTime docDate;
                foreach (NonCashDocument doc in response.account.nonCashDocuments)
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = doc.documentId;

                    item.SubItems.Add(doc.documentType);

                    docDate = doc.documentDate;
                    item.SubItems.Add(docDate.ToUniversalTime().ToString("MM/dd/yyyy"));
                    //documentDate.ToShortDateString().ToString( "MM/dd/yyyy" ) );
                    lvDocumentList.Items.Add(item);
                }

                // load the cash documents of type 'P'

                foreach (CashDocument doc in response.account.cashDocuments)
                {
                    if (CASH_DOCUMENT_TYPES.Contains(doc.documentType.ToUpper()))
                    {
                        ListViewItem item = new ListViewItem();
                        item.Tag = doc.documentId;

                        item.SubItems.Add(doc.documentType);
                        //item.SubItems.Add( doc.documentDate.ToString( "MM/dd/yyyy" ) );
                        docDate = doc.documentDate;
                        item.SubItems.Add(docDate.ToUniversalTime().ToString("MM/dd/yyyy"));

                        //item.SubItems.Add( doc.documentDate.ToShortDateString().ToString( "MM/dd/yyyy" ) );
                        lvDocumentList.Items.Add(item);
                    }
                }

                if (this.lvDocumentList.Items.Count > 0)
                {
                    // sort the list

                    Sorter listSorter = new Sorter();
                    this.lvDocumentList.Sorting = SortOrder.Descending;
                    this.lvDocumentList.ListViewItemSorter = listSorter;
                    this.lvDocumentList.Sort();

                    this.chkAllDocuments.Enabled = true;
                    this.lvDocumentList.Items[0].Selected = true;
                }
                else
                {
                    this.chkAllDocuments.Enabled = false;
                }

                if (!response.documentsWereFound)
                {
                    this.lvDocumentList.Hide();
                    this.lblNoDocuments.Text = UIErrorMessages.DOC_IMG_NO_DOCUMENTS_FOUND;
                    this.btnViewDocument.Enabled = false;
                    this.panelNoDocuments.Show();
                }
                else
                {
                    this.panelNoDocuments.Hide();
                    this.lvDocumentList.Show();
                    this.btnViewDocument.Enabled = true;
                    this.btnScan.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string abc = ex.Message;
                this.lvDocumentList.Hide();
                this.lblNoDocuments.Text = UIErrorMessages.DOC_IMG_NO_RESPONSE_MSG;
                this.btnViewDocument.Enabled = false;
                //this.btnScan.Enabled = false;
                this.panelNoDocuments.Show();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            // update the prev doc icons/menu options

            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions(this.Model);
        }

        private bool IsAccountNumberValid()
        {
            if (this.Model.AccountNumber < 1)
            {
                this.DisableButtons();
                this.lblInstructionalMsg.Text = UIErrorMessages.DOC_IMG_NO_ACCOUNT_NBR_MSG;
                return false;
            }
            return true;
        }

        private bool IsPatientNameValid()
        {
            if (this.Model.Patient.LastName == String.Empty ||
                this.Model.Patient.LastName == null ||
                this.Model.Patient.FirstName == String.Empty ||
                this.Model.Patient.FirstName == null)
            {
                this.btnScan.Enabled = false;
                return false;
            }
            return true;
        }

        private void DisableButtons()
        {
            this.btnScan.Enabled = false;
            this.btnRefresh.Enabled = false;
            this.btnViewDocument.Enabled = false;
            this.chkAllDocuments.Enabled = false;
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelDocuments = new System.Windows.Forms.Panel();
            this.lblControlClick = new System.Windows.Forms.Label();
            this.chkAllDocuments = new System.Windows.Forms.CheckBox();
            this.lvDocumentList = new System.Windows.Forms.ListView();
            this.chheader = new System.Windows.Forms.ColumnHeader();
            this.chType = new System.Windows.Forms.ColumnHeader();
            this.chDate = new System.Windows.Forms.ColumnHeader();
            this.panelNoDocuments = new System.Windows.Forms.Panel();
            this.lblNoDocuments = new System.Windows.Forms.Label();
            this.btnViewDocument = new LoggingButton();
            this.btnRefresh = new LoggingButton();
            this.btnScan = new LoggingButton();
            this.lblScannedDocuments = new System.Windows.Forms.Label();
            this.lblInstructionalMsg = new System.Windows.Forms.Label();
            this.panelDocuments.SuspendLayout();
            this.panelNoDocuments.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDocuments
            // 
            this.panelDocuments.BackColor = System.Drawing.Color.White;
            this.panelDocuments.Controls.Add(this.lblControlClick);
            this.panelDocuments.Controls.Add(this.chkAllDocuments);
            this.panelDocuments.Controls.Add(this.lvDocumentList);
            this.panelDocuments.Controls.Add(this.panelNoDocuments);
            this.panelDocuments.Controls.Add(this.btnViewDocument);
            this.panelDocuments.Controls.Add(this.btnRefresh);
            this.panelDocuments.Controls.Add(this.btnScan);
            this.panelDocuments.Controls.Add(this.lblScannedDocuments);
            this.panelDocuments.Controls.Add(this.lblInstructionalMsg);
            this.panelDocuments.Location = new System.Drawing.Point(2, 2);
            this.panelDocuments.Name = "panelDocuments";
            this.panelDocuments.Size = new System.Drawing.Size(820, 394);
            this.panelDocuments.TabIndex = 0;
            // 
            // lblControlClick
            // 
            this.lblControlClick.Location = new System.Drawing.Point(8, 94);
            this.lblControlClick.Name = "lblControlClick";
            this.lblControlClick.Size = new System.Drawing.Size(220, 23);
            this.lblControlClick.TabIndex = 9;
            this.lblControlClick.Text = "Ctrl + click to select multiple documents";
            // 
            // chkAllDocuments
            // 
            this.chkAllDocuments.Enabled = false;
            this.chkAllDocuments.Location = new System.Drawing.Point(11, 112);
            this.chkAllDocuments.Name = "chkAllDocuments";
            this.chkAllDocuments.Size = new System.Drawing.Size(104, 23);
            this.chkAllDocuments.TabIndex = 4;
            this.chkAllDocuments.Text = "Select all";
            this.chkAllDocuments.CheckedChanged += new System.EventHandler(this.chkAllDocuments_CheckedChanged);
            // 
            // lvDocumentList
            // 
            this.lvDocumentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.chheader,
																							 this.chType,
																							 this.chDate});
            this.lvDocumentList.FullRowSelect = true;
            this.lvDocumentList.GridLines = true;
            this.lvDocumentList.HideSelection = false;
            this.lvDocumentList.Location = new System.Drawing.Point(10, 140);
            this.lvDocumentList.Name = "lvDocumentList";
            this.lvDocumentList.Size = new System.Drawing.Size(184, 197);
            this.lvDocumentList.TabIndex = 5;
            this.lvDocumentList.View = System.Windows.Forms.View.Details;
            this.lvDocumentList.DoubleClick += new System.EventHandler(this.lvDocumentList_DoubleClick);
            this.lvDocumentList.SelectedIndexChanged += new System.EventHandler(this.lvDocumentList_SelectedIndexChanged);
            // 
            // chheader
            // 
            this.chheader.Text = "";
            this.chheader.Width = 0;
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 90;
            // 
            // chDate
            // 
            this.chDate.Text = "Date";
            this.chDate.Width = 90;
            // 
            // panelNoDocuments
            // 
            this.panelNoDocuments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNoDocuments.Controls.Add(this.lblNoDocuments);
            this.panelNoDocuments.Location = new System.Drawing.Point(10, 140);
            this.panelNoDocuments.Name = "panelNoDocuments";
            this.panelNoDocuments.Size = new System.Drawing.Size(184, 198);
            this.panelNoDocuments.TabIndex = 7;
            // 
            // lblNoDocuments
            // 
            this.lblNoDocuments.Location = new System.Drawing.Point(6, 7);
            this.lblNoDocuments.Name = "lblNoDocuments";
            this.lblNoDocuments.Size = new System.Drawing.Size(171, 184);
            this.lblNoDocuments.TabIndex = 6;
            // 
            // btnViewDocument
            // 
            this.btnViewDocument.BackColor = System.Drawing.SystemColors.Control;
            this.btnViewDocument.Location = new System.Drawing.Point(92, 344);
            this.btnViewDocument.Name = "btnViewDocument";
            this.btnViewDocument.Size = new System.Drawing.Size(102, 25);
            this.btnViewDocument.TabIndex = 6;
            this.btnViewDocument.Text = "View Document...";
            this.btnViewDocument.Click += new System.EventHandler(this.btnViewDocument_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.Location = new System.Drawing.Point(119, 61);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnScan
            // 
            this.btnScan.BackColor = System.Drawing.SystemColors.Control;
            this.btnScan.Location = new System.Drawing.Point(10, 61);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(99, 23);
            this.btnScan.TabIndex = 2;
            this.btnScan.Text = "Scan Document...";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // lblScannedDocuments
            // 
            this.lblScannedDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblScannedDocuments.Location = new System.Drawing.Point(10, 39);
            this.lblScannedDocuments.Name = "lblScannedDocuments";
            this.lblScannedDocuments.Size = new System.Drawing.Size(121, 14);
            this.lblScannedDocuments.TabIndex = 1;
            this.lblScannedDocuments.Text = "Scanned Documents";
            // 
            // lblInstructionalMsg
            // 
            this.lblInstructionalMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblInstructionalMsg.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(204)), ((System.Byte)(0)), ((System.Byte)(0)));
            this.lblInstructionalMsg.Location = new System.Drawing.Point(10, 10);
            this.lblInstructionalMsg.Name = "lblInstructionalMsg";
            this.lblInstructionalMsg.Size = new System.Drawing.Size(800, 19);
            this.lblInstructionalMsg.TabIndex = 0;
            // 
            // ListOfDocumentsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelDocuments);
            this.Name = "ListOfDocumentsView";
            this.Size = new System.Drawing.Size(828, 406);
            this.Enter += new System.EventHandler(this.ListOfDocumentsView_Enter);
            this.panelDocuments.ResumeLayout(false);
            this.panelNoDocuments.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        private VIWEBFeatureManager VIWEBFeatureManager { get; set; }
        #endregion

        #region Construction and Finalization
        public ListOfDocumentsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
            base.EnableThemesOn(this);
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

        private Container components = null;

        private Label lblInstructionalMsg;
        private Label lblScannedDocuments;
        private Label lblNoDocuments;
        private Label lblControlClick;

        private LoggingButton btnScan;
        private LoggingButton btnRefresh;
        private LoggingButton btnViewDocument;

        private ListView lvDocumentList;

        private ColumnHeader chType;
        private ColumnHeader chDate;
        private ColumnHeader chheader;

        private Panel panelDocuments;
        private Panel panelNoDocuments;

        private string i_DocumentId = String.Empty;

        private ArrayList i_Docs = new ArrayList();

        private CheckBox chkAllDocuments;

        #endregion

        #region Constants

        private static readonly ICollection<string> CASH_DOCUMENT_TYPES = new[] { "CASHP", "PATRECPT" };

        #endregion

    }

    [Serializable]
    public class Sorter : IComparer
    {
        // Initialize the variables to default
        public bool bAscending = false;

        // Using the Compare function of IComparer
        public int Compare(object x, object y)
        {
            // Cast the objects to ListViewItems
            ListViewItem lvi1 = (ListViewItem)x;
            ListViewItem lvi2 = (ListViewItem)y;


            string lvi1String1 = lvi1.SubItems[2].Text.Substring(6, 4) + lvi1.SubItems[2].Text.Substring(0, 2)
                + lvi1.SubItems[2].Text.Substring(3, 2);
            string lvi1String2 = lvi1.SubItems[1].Text;
            string lvi2String1 = lvi2.SubItems[2].Text.Substring(6, 4) + lvi2.SubItems[2].Text.Substring(0, 2)
                + lvi2.SubItems[2].Text.Substring(3, 2);
            string lvi2String2 = lvi2.SubItems[1].Text;

            int r1 = String.Compare(lvi1String1, lvi2String1);
            int r2 = String.Compare(lvi1String2, lvi2String2);

            if (r1 == 0 && r2 == 0)
                return 0;

            else if (r1 == 0 && r2 < 0)
                return -1;

            else if (r1 == 0 && r2 > 0)
                return 1;

            else if (r1 > 0 && r2 == 0)
                return -1;

            else if (r1 < 0 && r2 == 0)
                return 1;

            else if (r1 > 0 && r2 > 0)
                return 1;

            else if (r1 > 0 && r2 < 0)
                return -1;

            return 1;


        }
    }

}
