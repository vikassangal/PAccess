using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.EmploymentViews
{
    /// <summary>
    /// Summary description for SelectAnEmployer.
    /// </summary>
    public class SelectAnEmployer : ControlView
    {
        #region Events
        
        public event EventHandler SelectedEmployerChanged;
        
        #endregion

        #region Event Handlers

        private void SelectAnEmployer_Load(object sender, EventArgs e)
        {
            this.panelNoItemsFound.Visible = false;
        }

        private void searchField_PerformSearch(object sender, EventArgs e)
        {
            OnPerformSearch();
        }

        private void buttonNewEmployer_Click(object sender, EventArgs e)
        {
            OnNewEmployer();
        }

        private void listView_Employers_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSelectedEmployerFromList();
            this.listView_Employers.Focus();
        }
        #endregion

        #region Methods

        private void OnPerformSearch()
        {
            SearchForEmployers(searchField.SearchText);
        }

        private void OnNewEmployer()
        {
            FormNewEmployer formNewEmployer = new FormNewEmployer();
            formNewEmployer.Activity = this.Activity;
            formNewEmployer.Owner = this.FindForm();
            this.FindForm().Hide();

            try
            {
                formNewEmployer.ShowDialog();

                searchField.SearchText = formNewEmployer.EmployerName;
                OnPerformSearch();

                if (formNewEmployer.DialogResult == DialogResult.OK &&
                    this.FindForm().Visible == false)
                {
                    SelectEmployerByCode(formNewEmployer.NewEmployer.EmployerCode);
                    SelectedEmployer = formNewEmployer.NewEmployer;
                    this.FindForm().DialogResult = DialogResult.OK;
                }
            }
            finally
            {
                formNewEmployer.Dispose();
            }

        }

        /// <summary>
        /// Update Employer listview with data.
        /// </summary>
        public override void UpdateView()
        {
            searchField.Activate();
        }

        protected virtual void OnSelectedEmployerChanged(EventArgs e)
        {
            if (SelectedEmployerChanged != null)
            {
                SelectedEmployerChanged(this, e);
            }
        }
        #endregion

        #region Properties
        public Employer SelectedEmployer
        {
            get
            {
                return i_SelectedEmployer;
            }
            set
            {
                if (value != i_SelectedEmployer)
                {
                    i_SelectedEmployer = value;
                    OnSelectedEmployerChanged(EventArgs.Empty);
                }
            }
        }

        public Activity Activity
        {
            private get
            {
                return i_activity;
            }
            set
            {
                i_activity = value;
            }
        }

        public bool EmployersFound
        {
            get
            {
                return listView_Employers.Items.Count > 0;
            }
        }
        #endregion

        #region Private Methods
        private void SearchForEmployers(string searchString)
        {
            listView_Employers.BeginUpdate();
            this.FindForm().Cursor = Cursors.WaitCursor;
            try
            {
                listView_Employers.Items.Clear();
                if (searchString.Length > 0)
                {
                    IEmployerBroker eb = BrokerFactory.BrokerOfType<IEmployerBroker>();

                    Facility facility = User.GetCurrent().Facility;

                    SortedList employers = eb.AllEmployersWith(facility.Oid, searchString);
                    foreach (EmployerProxy ep in employers.Values)
                    {
                        ListViewItem item = new ListViewItem();

                        item.Tag = ep;

                        item.Text = "";
                        item.SubItems.Add(ep.Name);
                        item.SubItems.Add(ep.NationalId);

                        listView_Employers.Items.Add(item);
                    }
                    if (listView_Employers.Items.Count > 0)
                    {
                        listView_Employers.Items[0].Selected = true;
                        this.FindForm().ActiveControl = listView_Employers;
                        SetPanelNoItemsFoundVisibility(false);
                    }
                    else
                    {
                        SetPanelNoItemsFoundVisibility(true);
                        GetSelectedEmployerFromList();
                    }
                }
                else
                {
                    SelectedEmployer = new Employer();
                }
            }
            catch (RemotingTimeoutException)
            {
                MessageBox.Show(UIErrorMessages.EMPLOYER_SEARCH_TIMEOUT_MSG);
            }
            finally
            {
                listView_Employers.EndUpdate();
                this.FindForm().Cursor = Cursors.Default;
            }
        }

        private void SetPanelNoItemsFoundVisibility(bool status)
        {
            this.panelNoItemsFound.Visible = status;
            this.panel3.Visible = !status;
        }

        private void GetSelectedEmployerFromList()
        {
            if (listView_Employers.SelectedItems.Count > 0)
            {
                SelectedEmployer = ResolveItemEmployer(listView_Employers.SelectedItems[0]);
                BreadCrumbLogger.GetInstance.Log(SelectedEmployer.Name + " employer selected");
            }
            else
            {
                SelectedEmployer = new Employer();
            }
        }

        private Employer ResolveItemEmployer(ListViewItem item)
        {
            if (item.Tag is EmployerProxy)
            {
                EmployerProxy employerProxy = (EmployerProxy)item.Tag;
                item.Tag = employerProxy.AsEmployer();
            }
            return (Employer)item.Tag;
        }

        private void SelectEmployerByCode(long employerCode)
        {
            for (int idx = 0; idx < listView_Employers.Items.Count; idx++)
            {
                Employer employer = ResolveItemEmployer(listView_Employers.Items[idx]);
                if (employer.EmployerCode == employerCode)
                {
                    listView_Employers.Items[idx].Selected = true;
                    listView_Employers.EnsureVisible(idx);
                    break;
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchField = new PatientAccess.UI.CommonControls.SearchField();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonNewEmployer = new LoggingButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listView_Employers = new System.Windows.Forms.ListView();
            this.columnHeader_Dummy1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEmployerName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNationalID = new System.Windows.Forms.ColumnHeader();
            this.panelNoItemsFound = new System.Windows.Forms.Panel();
            this.labelNoItemsFound = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelNoItemsFound.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.searchField);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 30);
            this.panel1.TabIndex = 1;
            // 
            // searchField
            // 
            this.searchField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.searchField.KeyPressExpression = "^.*";
            this.searchField.Location = new System.Drawing.Point(9, 7);
            this.searchField.Mask = "";
            this.searchField.MaxFieldLength = 25;
            this.searchField.Model = null;
            this.searchField.Name = "searchField";
            this.searchField.SearchText = "";
            this.searchField.Size = new System.Drawing.Size(373, 24);
            this.searchField.TabIndex = 1;
            this.searchField.TextBoxEnabled = true;
            this.searchField.ValidationExpression = "^.*";
            this.searchField.PerformSearch += new System.EventHandler(this.searchField_PerformSearch);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonNewEmployer);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(392, 30);
            this.panel2.TabIndex = 2;
            // 
            // buttonNewEmployer
            // 
            this.buttonNewEmployer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewEmployer.Location = new System.Drawing.Point(276, 4);
            this.buttonNewEmployer.Name = "buttonNewEmployer";
            this.buttonNewEmployer.Size = new System.Drawing.Size(105, 23);
            this.buttonNewEmployer.TabIndex = 2;
            this.buttonNewEmployer.Text = "&New Employer...";
            this.buttonNewEmployer.Click += new System.EventHandler(this.buttonNewEmployer_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select an employer, or create one if needed.";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listView_Employers);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.DockPadding.Bottom = 2;
            this.panel3.DockPadding.Left = 10;
            this.panel3.DockPadding.Right = 10;
            this.panel3.DockPadding.Top = 5;
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(392, 160);
            this.panel3.TabIndex = 3;
            // 
            // listView_Employers
            // 
            this.listView_Employers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                 this.columnHeader_Dummy1,
                                                                                                 this.columnHeaderEmployerName,
                                                                                                 this.columnHeaderNationalID});
            this.listView_Employers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Employers.FullRowSelect = true;
            this.listView_Employers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_Employers.HideSelection = false;
            this.listView_Employers.Location = new System.Drawing.Point(10, 5);
            this.listView_Employers.MultiSelect = false;
            this.listView_Employers.Name = "listView_Employers";
            this.listView_Employers.Size = new System.Drawing.Size(372, 153);
            this.listView_Employers.TabIndex = 3;
            this.listView_Employers.View = System.Windows.Forms.View.Details;
            this.listView_Employers.SelectedIndexChanged += new System.EventHandler(this.listView_Employers_SelectedIndexChanged);
            // 
            // columnHeader_Dummy1
            // 
            this.columnHeader_Dummy1.Width = 0;
            // 
            // columnHeaderEmployerName
            // 
            this.columnHeaderEmployerName.Text = "Employer";
            this.columnHeaderEmployerName.Width = 276;
            // 
            // columnHeaderNationalID
            // 
            this.columnHeaderNationalID.Text = "National ID";
            this.columnHeaderNationalID.Width = 70;
            // 
            // panelNoItemsFound
            // 
            this.panelNoItemsFound.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNoItemsFound.Controls.Add(this.labelNoItemsFound);
            this.panelNoItemsFound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNoItemsFound.DockPadding.All = 10;
            this.panelNoItemsFound.Location = new System.Drawing.Point(0, 60);
            this.panelNoItemsFound.Name = "panelNoItemsFound";
            this.panelNoItemsFound.Size = new System.Drawing.Size(392, 160);
            this.panelNoItemsFound.TabIndex = 4;
            this.panelNoItemsFound.Visible = true;
            // 
            // labelNoItemsFound
            // 
            this.labelNoItemsFound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNoItemsFound.Location = new System.Drawing.Point(10, 10);
            this.labelNoItemsFound.Name = "labelNoItemsFound";
            this.labelNoItemsFound.Size = new System.Drawing.Size(370, 138);
            this.labelNoItemsFound.TabIndex = 0;
            this.labelNoItemsFound.Text = "No items found";
            // 
            // SelectAnEmployer
            // 
            this.Load += new EventHandler(SelectAnEmployer_Load);
            this.Controls.Add(this.panelNoItemsFound);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SelectAnEmployer";
            this.Size = new System.Drawing.Size(392, 220);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panelNoItemsFound.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties

        #endregion

        #region Constructors and Finalization
        public SelectAnEmployer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            searchField.MaxFieldLength = 25;

            SelectedEmployer = new Employer();
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
        private Panel panel1;
        private Panel panel2;
        private Label label2;
        private LoggingButton buttonNewEmployer;
        private Panel panel3;
        private ListView listView_Employers;
        private ColumnHeader columnHeaderEmployerName;
        private ColumnHeader columnHeaderNationalID;
        private Container components = null;
        private ColumnHeader columnHeader_Dummy1;
        private SearchField searchField;

        private Activity i_activity;
        private Employer i_SelectedEmployer = new Employer();
        #endregion

        #region Constants
        private const string NEW_EMPLOYER_CAPTION = "Warning for Create a New Employer";

        #endregion
        private Panel panelNoItemsFound;
        private Label labelNoItemsFound;
    }
}
