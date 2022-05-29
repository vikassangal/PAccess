using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for SummaryPageView.
    /// </summary>
    public class SummaryPageView : ControlView
    {
        #region Event Handlers
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {   // Disable listItem selections
            ListView lv = sender as ListView;
            ListView.SelectedListViewItemCollection collection = lv.SelectedItems;
            if( collection.Count > 0 )
            {   // First item in collection is the selected index for single selection listbox
                ListViewItem item = collection[ 0 ];
                if( item.Selected == true )
                {
                    item.Selected = false;
                }
            }
        }

        private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if( e.Index > -1 )
            {
                ContributingCondition cc = (ContributingCondition) listBox.Items[e.Index];
                SizeF sizef = e.Graphics.MeasureString( cc.Question, listBox.Font );
                e.ItemHeight = (int) sizef.Height + 2;

                if( (int) sizef.Width > columnQuestion.Width )
                {
                    e.ItemHeight += listBox.Font.Height + 2;
                }
            }
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if( e.Index > -1 )
            {
                ContributingCondition cc = listBox.Items[e.Index] as ContributingCondition;
                SizeF sizef = e.Graphics.MeasureString( cc.Question, Font );

                if( (e.State & DrawItemState.Focus) == 0 )
                {    
                    if( (int) sizef.Width > columnQuestion.Width )
                    {   // Split the question text into 2 lines
                        stringBuilder.Append( cc.Question );
    
                        while( (int) sizef.Width > columnQuestion.Width )
                        {   // Whittle some text off the end until it fits the column
                            int bLen = stringBuilder.Length;
                            stringBuilder.Remove( bLen - 1, 1 );
                            sizef = e.Graphics.MeasureString( stringBuilder.ToString(), Font );
                        }
                        // If we chopped it in the middle of a word, move left to a blank
                        while( Char.IsWhiteSpace( stringBuilder[ stringBuilder.Length - 1] ) == false )
                        {
                            int bLen = stringBuilder.Length;
                            stringBuilder.Remove( bLen - 1, 1 );
                            bLen = stringBuilder.Length;
                        }
                        // Trim leading & trailing white space
                        string questionPart1 = stringBuilder.ToString().Trim();
                        string questionPart2 = cc.Question.Substring( stringBuilder.Length, cc.Question.Length - stringBuilder.Length ).Trim();
                        stringBuilder.Length = 0;

                        e.Graphics.DrawString( questionPart1, e.Font, new SolidBrush( SystemColors.WindowText ),
                                               columnScreen.Width + frameOffset, e.Bounds.Top + 1 );

                        e.Graphics.DrawString( questionPart2, e.Font, new SolidBrush( SystemColors.WindowText ),
                                               columnScreen.Width + frameOffset,
                                               e.Bounds.Top + 2 + listBox.Font.Height );
                    }
                    else
                    {
                        e.Graphics.DrawString( cc.Question, e.Font, new SolidBrush( SystemColors.WindowText ),
                                               columnScreen.Width + frameOffset, e.Bounds.Top + 1 );
                    }

                    e.Graphics.DrawString( cc.Section, e.Font, new SolidBrush( SystemColors.WindowText ),
                                           frameOffset, e.Bounds.Top + 2 );
                    // Draw vertical lines to simulate the detail view of a ListView
                    e.Graphics.DrawLine( linePen, columnScreen.Width-1, e.Bounds.Top, columnScreen.Width - 1, e.Bounds.Bottom );

                    e.Graphics.DrawLine( linePen, columnScreen.Width + columnQuestion.Width - 2, e.Bounds.Top,
                                         columnScreen.Width + columnQuestion.Width - 2, e.Bounds.Bottom );

                    e.Graphics.DrawString( cc.Answer, e.Font, new SolidBrush( SystemColors.WindowText ),
                                           columnScreen.Width + columnQuestion.Width + frameOffset, e.Bounds.Top + 1 );

                    // Draw a horizontal line below the data
                    e.Graphics.DrawLine( linePen, 0, e.Bounds.Bottom, e.Bounds.Width, e.Bounds.Bottom );
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            FormChanged       = false;
            lblResult.Text    = String.Empty;
            lblComplaint.Text = String.Empty;

            // Display primary & secondary payors.
            foreach( Coverage coverage in Model_Account.Insurance.Coverages )
            {
                if( coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    lblPrimaryPayor.Text = coverage.InsurancePlan.PlanName;
                }
                else if( coverage.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID )
                {
                    lblSecondaryPayor.Text = coverage.InsurancePlan.PlanName;
                }
            }
            if( lblPrimaryPayor.Text.Equals( String.Empty ) )
            {
                lblPrimaryPayor.Text = "NONE";
            }
            if( lblSecondaryPayor.Text.Equals( String.Empty ) )
            {
                lblSecondaryPayor.Text = "NONE";
            }
            if( Model_Account.Patient.MaritalStatus != null )
            {
                lblMaritalStatus.Text = Model_Account.Patient.MaritalStatus.Description;
            }
            if( Model_Account.Patient.Sex != null )
            {
                lblAge.Text = Model_Account.Patient.Age();
            }
            lblComplaint.Text = Model_Account.Diagnosis.ChiefComplaint;


            if( parentForm.ShowingSummary )
            {
                Model_Account.MedicareSecondaryPayor.MSPVersion = VERSION_1;
                msp = Model_Account.MedicareSecondaryPayor.MakeRecommendation();
            }
            else
            {
                Model_MedicareSecondaryPayor.MSPVersion = VERSION_1;
                msp = Model_MedicareSecondaryPayor.MakeRecommendation();
            }

            if( msp.IsMedicareRecommended )
            {
                lblResult.Text = yesResult;
            }
            else
            {
                lblResult.Text = noResult;
            }
            // Display the questionaire questions that led to Medicare result
            PopulateConditionListView( msp );
        }
        #endregion

        #region Private Methods
        private void PopulateConditionListView( MSPRecommendation msp )
        {
            foreach( ContributingCondition cc in msp.ContributingConditions )
            {
                listBox.Items.Add( cc );
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private MedicareSecondaryPayor Model_MedicareSecondaryPayor
        {
            get
            {
                return (MedicareSecondaryPayor) this.Model;
            }
        }

        [Browsable(false)]
        public Account Model_Account
        {
            private get
            {
                return (Account) this.i_account;
            }
            set
            {
                i_account = value;
            }
        }

        [Browsable(false)]
        public bool FormChanged
        {
            get
            {
                return formWasChanged;
            }
            set
            {
                formWasChanged = value;
            }
        }

        [Browsable(false)]
        public int Response
        {
            get
            {
                return response;
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStaticIntro = new System.Windows.Forms.Label();
            this.lblStaticPayor = new System.Windows.Forms.Label();
            this.lblPrimaryPayor = new System.Windows.Forms.Label();
            this.lblStaticSecondaryPayor = new System.Windows.Forms.Label();
            this.lblSecondaryPayor = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.listView = new PatientAccess.UI.CommonControls.LockingListView();
            this.columnScreen = new System.Windows.Forms.ColumnHeader();
            this.columnQuestion = new System.Windows.Forms.ColumnHeader();
            this.columnResponse = new System.Windows.Forms.ColumnHeader();
            this.grpComplaint = new System.Windows.Forms.GroupBox();
            this.lblComplaint = new System.Windows.Forms.Label();
            this.grpPayor = new System.Windows.Forms.GroupBox();
            this.grpDemographics = new System.Windows.Forms.GroupBox();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.lblStaticStatus = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.grpComplaint.SuspendLayout();
            this.grpPayor.SuspendLayout();
            this.grpDemographics.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(16, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "MSP Summary";
            // 
            // lblStaticIntro
            // 
            this.lblStaticIntro.Location = new System.Drawing.Point(16, 64);
            this.lblStaticIntro.Name = "lblStaticIntro";
            this.lblStaticIntro.Size = new System.Drawing.Size(608, 48);
            this.lblStaticIntro.TabIndex = 0;
            this.lblStaticIntro.Text = @"The following assumptions are derived from responses to questions asked during the MSP Form Analysis wizard.  This summary report aids in determining payor(s) that may or may not be primary to Medicare.  Please review the summary information and make adjustments to primary and secondary payors on the Insurance screen as needed.";
            // 
            // lblStaticPayor
            // 
            this.lblStaticPayor.Location = new System.Drawing.Point(8, 18);
            this.lblStaticPayor.Name = "lblStaticPayor";
            this.lblStaticPayor.Size = new System.Drawing.Size(123, 23);
            this.lblStaticPayor.TabIndex = 0;
            this.lblStaticPayor.Text = "Primary payor selected:";
            // 
            // lblPrimaryPayor
            // 
            this.lblPrimaryPayor.Location = new System.Drawing.Point(144, 18);
            this.lblPrimaryPayor.Name = "lblPrimaryPayor";
            this.lblPrimaryPayor.Size = new System.Drawing.Size(470, 23);
            this.lblPrimaryPayor.TabIndex = 0;
            // 
            // lblStaticSecondaryPayor
            // 
            this.lblStaticSecondaryPayor.Location = new System.Drawing.Point(8, 44);
            this.lblStaticSecondaryPayor.Name = "lblStaticSecondaryPayor";
            this.lblStaticSecondaryPayor.Size = new System.Drawing.Size(138, 23);
            this.lblStaticSecondaryPayor.TabIndex = 0;
            this.lblStaticSecondaryPayor.Text = "Secondary payor selected:";
            // 
            // lblSecondaryPayor
            // 
            this.lblSecondaryPayor.Location = new System.Drawing.Point(144, 44);
            this.lblSecondaryPayor.Name = "lblSecondaryPayor";
            this.lblSecondaryPayor.Size = new System.Drawing.Size(470, 23);
            this.lblSecondaryPayor.TabIndex = 0;
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(16, 275);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(645, 23);
            this.lblResult.TabIndex = 0;
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnScreen,
                                                                                       this.columnQuestion,
                                                                                       this.columnResponse});
            this.listView.Location = new System.Drawing.Point(16, 304);
            this.listView.LockColumnSize = true;
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(645, 20);
            this.listView.TabIndex = 0;
            this.listView.TabStop = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnScreen
            // 
            this.columnScreen.Text = "Section";
            this.columnScreen.Width = 135;
            // 
            // columnQuestion
            // 
            this.columnQuestion.Text = "Question";
            this.columnQuestion.Width = 325;
            // 
            // columnResponse
            // 
            this.columnResponse.Text = "Response";
            this.columnResponse.Width = 160;
            // 
            // grpComplaint
            // 
            this.grpComplaint.Controls.Add(this.lblComplaint);
            this.grpComplaint.Location = new System.Drawing.Point(16, 447);
            this.grpComplaint.Name = "grpComplaint";
            this.grpComplaint.Size = new System.Drawing.Size(645, 56);
            this.grpComplaint.TabIndex = 0;
            this.grpComplaint.TabStop = false;
            this.grpComplaint.Text = "Chief Complaint";
            // 
            // lblComplaint
            // 
            this.lblComplaint.Location = new System.Drawing.Point(8, 25);
            this.lblComplaint.Name = "lblComplaint";
            this.lblComplaint.Size = new System.Drawing.Size(595, 23);
            this.lblComplaint.TabIndex = 0;
            // 
            // grpPayor
            // 
            this.grpPayor.Controls.Add(this.lblSecondaryPayor);
            this.grpPayor.Controls.Add(this.lblStaticPayor);
            this.grpPayor.Controls.Add(this.lblPrimaryPayor);
            this.grpPayor.Controls.Add(this.lblStaticSecondaryPayor);
            this.grpPayor.Location = new System.Drawing.Point(16, 113);
            this.grpPayor.Name = "grpPayor";
            this.grpPayor.Size = new System.Drawing.Size(645, 72);
            this.grpPayor.TabIndex = 0;
            this.grpPayor.TabStop = false;
            this.grpPayor.Text = "Payor selected";
            // 
            // grpDemographics
            // 
            this.grpDemographics.Controls.Add(this.lblAge);
            this.grpDemographics.Controls.Add(this.lblMaritalStatus);
            this.grpDemographics.Controls.Add(this.lblStaticAge);
            this.grpDemographics.Controls.Add(this.lblStaticStatus);
            this.grpDemographics.Location = new System.Drawing.Point(16, 194);
            this.grpDemographics.Name = "grpDemographics";
            this.grpDemographics.Size = new System.Drawing.Size(645, 72);
            this.grpDemographics.TabIndex = 0;
            this.grpDemographics.TabStop = false;
            this.grpDemographics.Text = "Demographic information";
            // 
            // lblAge
            // 
            this.lblAge.Location = new System.Drawing.Point(81, 44);
            this.lblAge.Name = "lblAge";
            this.lblAge.TabIndex = 0;
            // 
            // lblMaritalStatus
            // 
            this.lblMaritalStatus.Location = new System.Drawing.Point(81, 18);
            this.lblMaritalStatus.Name = "lblMaritalStatus";
            this.lblMaritalStatus.Size = new System.Drawing.Size(200, 23);
            this.lblMaritalStatus.TabIndex = 0;
            // 
            // lblStaticAge
            // 
            this.lblStaticAge.Location = new System.Drawing.Point(8, 44);
            this.lblStaticAge.Name = "lblStaticAge";
            this.lblStaticAge.Size = new System.Drawing.Size(75, 23);
            this.lblStaticAge.TabIndex = 0;
            this.lblStaticAge.Text = "Age:";
            // 
            // lblStaticStatus
            // 
            this.lblStaticStatus.Location = new System.Drawing.Point(8, 18);
            this.lblStaticStatus.Name = "lblStaticStatus";
            this.lblStaticStatus.Size = new System.Drawing.Size(75, 23);
            this.lblStaticStatus.TabIndex = 0;
            this.lblStaticStatus.Text = "Marital status:";
            // 
            // listBox
            // 
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox.Location = new System.Drawing.Point(16, 322);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox.Size = new System.Drawing.Size(645, 109);
            this.listBox.TabIndex = 1;
            this.listBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            // 
            // SummaryPageView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.grpDemographics);
            this.Controls.Add(this.grpPayor);
            this.Controls.Add(this.grpComplaint);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblStaticIntro);
            this.Controls.Add(this.lblTitle);
            this.Name = "SummaryPageView";
            this.Size = new System.Drawing.Size(680, 520);
            this.grpComplaint.ResumeLayout(false);
            this.grpPayor.ResumeLayout(false);
            this.grpDemographics.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public SummaryPageView( MSPDialog form )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            parentForm = form;
            frameOffset = listBox.Font.Height / 2;
            stringBuilder = new StringBuilder();
            blackPen = new Pen( SystemColors.ControlDark );
            linePen = new Pen( SystemColors.ControlLight );
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
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

        private ColumnHeader           columnScreen;
        private ColumnHeader           columnQuestion;
        private ColumnHeader           columnResponse;

        private GroupBox               grpComplaint;
        private GroupBox               grpPayor;
        private GroupBox               grpDemographics;

        private Label                  lblComplaint;
        private Label                  lblTitle;
        private Label                  lblStaticIntro;
        private Label                  lblStaticPayor;
        private Label                  lblPrimaryPayor;
        private Label                  lblStaticSecondaryPayor;
        private Label                  lblSecondaryPayor;
        private Label                  lblResult;
        private Label                  lblStaticStatus;
        private Label                  lblStaticAge;
        private Label                  lblMaritalStatus;
        private Label                  lblAge;

        private ListBox                listBox;

        private LockingListView listView;

        private Pen                                         blackPen;
        private Pen                                         linePen;
        private StringBuilder                               stringBuilder;
        private MSPRecommendation                           msp;
        private Account                                     i_account;
        private MSPDialog                                   parentForm;

        private static bool                                  formWasChanged;
        
        private int                                         response = -1;
        private int                                         frameOffset;
        private const int                                   VERSION_1 = 1;

        private string noResult  = "Based on the following questions and responses, Medicare is not the primary payor:";
        private string yesResult = "Based on the following questions and responses, Medicare is the primary payor:";
        #endregion
    }
}
