using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// SummaryPage - display the results of the wizard; this includes the recommendation and
    /// conditions (anwers to the questions on the wizard pages) that generated the recommendation.
    /// The recommendation is 'gathered' by invoking the Domain objects (SpecialPrograms, LiabilityInsurer,
    /// AgeEntitlement, ESRDEntitlement, DisabilityEntitlement) method MakeRecommendation()
    /// </summary>
    [Serializable]
    public class SummaryPage : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// listView_SelectedIndexChanged - reset the selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {   
            // Disable listItem selections
            
            ListView lv = sender as ListView;
            ListView.SelectedListViewItemCollection collection = lv.SelectedItems;
            
            if( collection.Count > 0 )
            {   
                // First item in collection is the selected index for single selection listbox
                
                ListViewItem item = collection[ 0 ];
                if( item.Selected == true )
                {
                    item.Selected = false;
                }
            }
        }

        /// <summary>
        /// measureDisplayStringWidth - determine the width of a string based on the specified font
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private int measureDisplayStringWidth(Graphics graphics, string text,
            Font font)
        {
            StringFormat format  = new StringFormat ();
            RectangleF   rect    = new RectangleF(0, 0,
                1000, 1000);
            CharacterRange[] ranges  = 
                                       {
                                           new CharacterRange(0, 
                                           text.Length) };
            Region[]         regions = new Region[1];

            format.SetMeasurableCharacterRanges (ranges);

            regions = graphics.MeasureCharacterRanges (text, font, rect, format);
            rect    = regions[0].GetBounds (graphics);

            return (int)(rect.Right + 20.0f);
        }

        /// <summary>
        /// measureDisplayStringHeight - determine the height of a string based on the specified font
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private int measureDisplayStringHeight(Graphics graphics, string text,
            Font font)
        {
            StringFormat format  = new StringFormat ();
            RectangleF   rect    = new RectangleF(0, 0,
                1000, 1000);
            CharacterRange[] ranges  = 
                                       {
                                           new CharacterRange(0, 
                                           text.Length) };
            Region[]         regions = new Region[1];

            format.SetMeasurableCharacterRanges (ranges);

            regions = graphics.MeasureCharacterRanges (text, font, rect, format);
            rect    = regions[0].GetBounds (graphics);

            return (int)(rect.Height);
        }

        /// <summary>
        /// listBox_MeasureItem - determine how big (i.e. number of rows high) a listbox item should be
        /// based on the length of text it is to contain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if( e.Index > -1 )
            {
                ContributingCondition cc = (ContributingCondition) listBox.Items[e.Index];

                int textWidth = this.measureDisplayStringWidth( e.Graphics, cc.Question, listBox.Font );
                int textHeight = this.measureDisplayStringHeight( e.Graphics, cc.Question, listBox.Font );
                string tQuestion = cc.Question;

                e.ItemHeight = textHeight + 2;

                if( columnQuestion.Width > 0 )
                {
                    int iRows = this.calcRows( e.Graphics, cc );

                    for( int i=1; i < iRows; i++ )
                    {
                        e.ItemHeight += textHeight + 2;
                    }
                }      
            }
        }

        /// <summary>
        /// listBox_DrawItem - draw the contents of the listbox cell; long lines are chopped into shorter
        /// lines that fit the cell width; this method works in tandem with listBox_MeasureItem so the text
        /// fits into the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            int iRowPlacement = 0;

            if( e.Index > -1 )
            {
                ContributingCondition cc = listBox.Items[e.Index] as ContributingCondition;

                int textWidth = this.measureDisplayStringWidth( e.Graphics, cc.Question, Font );
                int textHeight = this.measureDisplayStringHeight( e.Graphics, cc.Question, Font );

                if( (e.State & DrawItemState.Focus) == 0 )
                {    
                    if( textWidth > columnQuestion.Width )
                    {   // Split the question text into multiple lines                                              
    
                        int dwindlingSize = textWidth;
                        cc.Question = cc.Question + " ";
                        string workingQuestion =  cc.Question;

                        while( dwindlingSize > 0 )
                        {
                            stringBuilder = new StringBuilder();
                            stringBuilder.Append( workingQuestion );
                           
                            while( textWidth > columnQuestion.Width )
                            {   // Whittle some text off the end until it fits the column
                                int bLen = stringBuilder.Length;
                                stringBuilder.Remove( bLen - 1, 1 );
                                textWidth = this.measureDisplayStringWidth( e.Graphics, stringBuilder.ToString(), Font );
                            }
                            
                            // If we chopped it in the middle of a word, move left to a blank
                            while( Char.IsWhiteSpace( stringBuilder[ stringBuilder.Length - 1] ) == false )
                            {
                                int bLen = stringBuilder.Length;
                                stringBuilder.Remove( bLen - 1, 1 );
                                bLen = stringBuilder.Length;
                            }

                            e.Graphics.DrawString( stringBuilder.ToString(), e.Font, new SolidBrush( SystemColors.WindowText ),
                                columnSection.Width + frameOffset,
                                e.Bounds.Top + 1 + iRowPlacement );

                            if( workingQuestion.Length > stringBuilder.Length )
                            {
                                string tempQuestion = workingQuestion.Substring( stringBuilder.Length, workingQuestion.Length - stringBuilder.Length );
                                workingQuestion = tempQuestion;
                                dwindlingSize = workingQuestion.Length;
                            }
                            else
                            {
                                dwindlingSize   = 0;
                            }  
                            
                            textWidth = this.measureDisplayStringWidth( e.Graphics, workingQuestion, Font );
                            iRowPlacement += listBox.Font.Height;
                        }                       
                    }
                    else
                    {
                        e.Graphics.DrawString( cc.Question, e.Font, new SolidBrush( SystemColors.WindowText ),
                            columnSection.Width + frameOffset, e.Bounds.Top + 1 );
                    }

                    e.Graphics.DrawString( cc.Section, e.Font, new SolidBrush( SystemColors.WindowText ),
                        frameOffset, e.Bounds.Top + 1 );

                    // Draw vertical lines to simulate the detail view of a ListView
                    e.Graphics.DrawLine( linePen, columnSection.Width-1, e.Bounds.Top, columnSection.Width - 1, e.Bounds.Bottom );

                    e.Graphics.DrawLine( linePen, columnSection.Width + columnQuestion.Width - 2, e.Bounds.Top,
                        columnSection.Width + columnQuestion.Width - 2, e.Bounds.Bottom );

                    e.Graphics.DrawString( cc.Answer, e.Font, new SolidBrush( SystemColors.WindowText ),
                        columnSection.Width + columnQuestion.Width + frameOffset, e.Bounds.Top + 1 );

                    // Draw a horizontal line below the data
                    e.Graphics.DrawLine( linePen, 0, e.Bounds.Bottom, e.Bounds.Width, e.Bounds.Bottom );
                }
            }
        }

        /// <summary>
        /// calcRows - determine how many rows the text will require
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        private int calcRows( Graphics graphics, ContributingCondition cc )
        {
            int textWidth       = this.measureDisplayStringWidth( graphics, cc.Question, Font );
            int textHeight      = this.measureDisplayStringHeight( graphics, cc.Question, Font );

            int iRowPlacement   = 0;
            int iRows           = 0;

           
            if( textWidth > columnQuestion.Width )
            {   // Split the question text into multiple lines                                              
    
                int dwindlingSize = textWidth;
                cc.Question = cc.Question + " ";
                string workingQuestion =  cc.Question;

                while( dwindlingSize > 0 )
                {
                    stringBuilder = new StringBuilder();
                    stringBuilder.Append( workingQuestion );
                           
                    while( textWidth > columnQuestion.Width )
                    {   // Whittle some text off the end until it fits the column
                        int bLen = stringBuilder.Length;
                        stringBuilder.Remove( bLen - 1, 1 );
                        textWidth = this.measureDisplayStringWidth( graphics, stringBuilder.ToString(), Font );
                    }
                            
                    // If we chopped it in the middle of a word, move left to a blank
                    while( Char.IsWhiteSpace( stringBuilder[ stringBuilder.Length - 1] ) == false )
                    {
                        int bLen = stringBuilder.Length;
                        stringBuilder.Remove( bLen - 1, 1 );
                        bLen = stringBuilder.Length;
                    }                    

                    if( workingQuestion.Length > stringBuilder.Length )
                    {
                        string tempQuestion = workingQuestion.Substring( stringBuilder.Length, workingQuestion.Length - stringBuilder.Length );
                        workingQuestion = tempQuestion;
                        dwindlingSize = workingQuestion.Length;
                    }
                    else
                    {
                        dwindlingSize   = 0;
                    }  
                            
                    textWidth = this.measureDisplayStringWidth( graphics, workingQuestion, Font );
                    iRowPlacement += listBox.Font.Height;
                    iRows++;
                }                       
            }
            else
            {
                iRows++;
            }

            return iRows;
        }
    
        /// <summary>
        /// Finish - the finish button event handler
        /// </summary>

        private void Finish()
        {
            this.Model_Account.MedicareSecondaryPayor.HasBeenCompleted = true;
            this.MyWizardContainer.Finish();
        }

        /// <summary>
        /// SummaryPage_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SummaryPage_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled )
            {
                this.UpdateView();
            } 
        }
        /// <summary>
        /// SummaryPage_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SummaryPage_Load(object sender, EventArgs e)
        {
            this.LinkName                           = "Summary";
            this.MyWizardMessages.Message1          = "MSP Summary";            
            this.MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1         = 8.25;
            this.MyWizardMessages.FontStyle1        = FontStyle.Bold;

            this.MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize2         = 8.25;

            this.MyWizardMessages.ShowMessages();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel - handle the Cancel button click
        /// </summary>
        private void Cancel()
        {
            this.MyWizardContainer.Cancel();

            if( this.MSPCancelled != null )
            {
                this.MSPCancelled(this, null);
            }
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {
            bool canNav = true;

            this.CanNavigate = canNav;

            return canNav;
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {           
            base.UpdateView();           

            if( !blnLoaded )
            {
                blnLoaded           = true;

                parentForm          = this.ParentForm as MSP2Dialog ;
                frameOffset         = listBox.Font.Height / 2;
                stringBuilder       = new StringBuilder();
                blackPen            = new Pen( SystemColors.ControlDark );
                linePen             = new Pen( SystemColors.ControlLight );
            }         
            
            if( !blnUpdated )
            {                
                ((MSP2Dialog)this.ParentForm).UpdateModel();
                blnUpdated = true;
            }

            lblResult.Text          = String.Empty;
            lblComplaint.Text       = String.Empty;

            this.listView.Items.Clear();
            this.listBox.Items.Clear();

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

            Model_Account.MedicareSecondaryPayor.MSPVersion = VERSION_2;

            msp = Model_Account.MedicareSecondaryPayor.MakeRecommendation();

            if( msp.IsMedicareRecommended )
            {
                lblResult.Text = yesResult;
            }
            else
            {
                lblResult.Text = noResult;
            }
            // Display the questionaire questions that led to Medicare result
            this.populateConditionListView( msp );

            this.CanPageNavigate();

            this.blnUpdated = false;

            this.MyWizardButtons.SetAcceptButton( "Fini&sh" );
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );
            this.MyWizardButtons.AddNavigation( "< &Back", string.Empty );
            this.MyWizardButtons.AddNavigation( "&Next >", "" );            
            
            this.MyWizardButtons.AddNavigation( "Fini&sh", new FunctionDelegate( this.Finish ) );
            this.MyWizardButtons.SetDialogResult( "Fini&sh", DialogResult.OK );
            this.MyWizardButtons.SetAcceptButton( "Fini&sh" );
            
            this.MyWizardButtons.SetPanel();
        }

        #endregion

        #region Properties

        public int Response
        {
            get
            {
                return i_Response;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// populateConditionListView - build out the conditions based on the answers provided during 
        /// execution of the wizard (note, the conditions are based on the Domain - not the wizard page components;
        /// this is done by the ContributingConditions method on the MSPRecommendation)
        /// </summary>
        /// <param name="msp"></param>
        private void populateConditionListView( MSPRecommendation msp )
        {
            foreach( ContributingCondition cc in msp.ContributingConditions )
            {
                listBox.Items.Add( cc );
            }
        }
        public void DisablePage()
        {
            Enabled = false;
        }
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox = new System.Windows.Forms.ListBox();
            this.grpDemographics = new System.Windows.Forms.GroupBox();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblMaritalStatus = new System.Windows.Forms.Label();
            this.lblStaticAge = new System.Windows.Forms.Label();
            this.lblStaticStatus = new System.Windows.Forms.Label();
            this.grpPayor = new System.Windows.Forms.GroupBox();
            this.lblSecondaryPayor = new System.Windows.Forms.Label();
            this.lblStaticPayor = new System.Windows.Forms.Label();
            this.lblPrimaryPayor = new System.Windows.Forms.Label();
            this.lblStaticSecondaryPayor = new System.Windows.Forms.Label();
            this.grpComplaint = new System.Windows.Forms.GroupBox();
            this.lblComplaint = new System.Windows.Forms.Label();
            this.listView = new PatientAccess.UI.CommonControls.LockingListView();
            this.columnSection = new System.Windows.Forms.ColumnHeader();
            this.columnQuestion = new System.Windows.Forms.ColumnHeader();
            this.columnResponse = new System.Windows.Forms.ColumnHeader();
            this.lblStaticIntro = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.pnlWizardPageBody.SuspendLayout();
            this.grpDemographics.SuspendLayout();
            this.grpPayor.SuspendLayout();
            this.grpComplaint.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.lblResult);
            this.pnlWizardPageBody.Controls.Add(this.listBox);
            this.pnlWizardPageBody.Controls.Add(this.grpDemographics);
            this.pnlWizardPageBody.Controls.Add(this.grpPayor);
            this.pnlWizardPageBody.Controls.Add(this.grpComplaint);
            this.pnlWizardPageBody.Controls.Add(this.listView);
            this.pnlWizardPageBody.Controls.Add(this.lblStaticIntro);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblStaticIntro, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.listView, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.grpComplaint, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.grpPayor, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.grpDemographics, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.listBox, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblResult, 0);
            // 
            // listBox
            // 
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox.Location = new System.Drawing.Point(9, 307);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox.Size = new System.Drawing.Size(666, 109);
            this.listBox.TabIndex = 0;
            this.listBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox_MeasureItem);
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            // 
            // grpDemographics
            // 
            this.grpDemographics.Controls.Add(this.lblAge);
            this.grpDemographics.Controls.Add(this.lblMaritalStatus);
            this.grpDemographics.Controls.Add(this.lblStaticAge);
            this.grpDemographics.Controls.Add(this.lblStaticStatus);
            this.grpDemographics.Location = new System.Drawing.Point(9, 179);
            this.grpDemographics.Name = "grpDemographics";
            this.grpDemographics.Size = new System.Drawing.Size(666, 72);
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
            // grpPayor
            // 
            this.grpPayor.Controls.Add(this.lblSecondaryPayor);
            this.grpPayor.Controls.Add(this.lblStaticPayor);
            this.grpPayor.Controls.Add(this.lblPrimaryPayor);
            this.grpPayor.Controls.Add(this.lblStaticSecondaryPayor);
            this.grpPayor.Location = new System.Drawing.Point(8, 98);
            this.grpPayor.Name = "grpPayor";
            this.grpPayor.Size = new System.Drawing.Size(666, 72);
            this.grpPayor.TabIndex = 0;
            this.grpPayor.TabStop = false;
            this.grpPayor.Text = "Payor selected";
            // 
            // lblSecondaryPayor
            // 
            this.lblSecondaryPayor.Location = new System.Drawing.Point(144, 44);
            this.lblSecondaryPayor.Name = "lblSecondaryPayor";
            this.lblSecondaryPayor.Size = new System.Drawing.Size(470, 23);
            this.lblSecondaryPayor.TabIndex = 0;
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
            // grpComplaint
            // 
            this.grpComplaint.Controls.Add(this.lblComplaint);
            this.grpComplaint.Location = new System.Drawing.Point(9, 432);
            this.grpComplaint.Name = "grpComplaint";
            this.grpComplaint.Size = new System.Drawing.Size(666, 56);
            this.grpComplaint.TabIndex = 0;
            this.grpComplaint.TabStop = false;
            this.grpComplaint.Text = "Chief Complaint";
            // 
            // lblComplaint
            // 
            this.lblComplaint.Location = new System.Drawing.Point(12, 18);
            this.lblComplaint.Name = "lblComplaint";
            this.lblComplaint.Size = new System.Drawing.Size(638, 23);
            this.lblComplaint.TabIndex = 0;
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnSection,
                                                                                       this.columnQuestion,
                                                                                       this.columnResponse});
            this.listView.Location = new System.Drawing.Point(9, 289);
            this.listView.LockColumnSize = true;
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(666, 20);
            this.listView.TabIndex = 0;
            this.listView.TabStop = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnSection
            // 
            this.columnSection.Text = "Section";
            this.columnSection.Width = 135;
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
            // lblStaticIntro
            // 
            this.lblStaticIntro.Location = new System.Drawing.Point(8, 44);
            this.lblStaticIntro.Name = "lblStaticIntro";
            this.lblStaticIntro.Size = new System.Drawing.Size(666, 48);
            this.lblStaticIntro.TabIndex = 0;
            this.lblStaticIntro.Text = @"The following assumptions are derived from responses to questions asked during the MSP Form Analysis wizard.  This summary report aids in determining payor(s) that may or may not be primary to Medicare.  Please review the summary information and make adjustments to primary and secondary payors on the Insurance screen as needed.";
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(17, 257);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(650, 23);
            this.lblResult.TabIndex = 0;
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SummaryPage
            // 
            this.Name = "SummaryPage";
            this.EnabledChanged += new System.EventHandler(this.SummaryPage_EnabledChanged);
            this.Load += new System.EventHandler(this.SummaryPage_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.grpDemographics.ResumeLayout(false);
            this.grpPayor.ResumeLayout(false);
            this.grpComplaint.ResumeLayout(false);
            this.ResumeLayout(false);

            base.pnlWizardPageBody.TabIndex = 0;
            base.pnlWizardPageBody.TabStop = false;

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public SummaryPage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public SummaryPage( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public SummaryPage( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public SummaryPage( string pageName, WizardContainer wizardContainer, Account anAccount )
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

        private ListBox                                listBox;
        
        private LockingListView             listView;

        private GroupBox                               grpDemographics;
        private GroupBox                               grpPayor;
        private GroupBox                               grpComplaint;
        private Label                                  lblAge;
        private Label                                  lblMaritalStatus;
        private Label                                  lblStaticAge;
        private Label                                  lblStaticStatus;
        private Label                                  lblSecondaryPayor;
        private Label                                  lblStaticPayor;
        private Label                                  lblPrimaryPayor;
        private Label                                  lblStaticSecondaryPayor;        
        private Label                                  lblComplaint;
        private Label                                  lblStaticIntro;
        private Label                                  lblResult;
        
        private ColumnHeader                           columnSection;
        private ColumnHeader                           columnQuestion;
        private ColumnHeader                           columnResponse;

        private bool                                                        blnLoaded = false;
        private bool                                                        blnUpdated = false;

        private int                                                         i_Response = -1;
        private int                                                         frameOffset;

        private Pen                                                         blackPen;
        private Pen                                                         linePen;

        private MSP2Dialog                                                  parentForm;
        
        private MSPRecommendation                                           msp;

        private StringBuilder                                               stringBuilder;
        #endregion

        #region Constants

        private const int                                                   VERSION_2 = 2;

        private const string                                                noResult  
            = "Based on the following questions and responses, Medicare is not the primary payor:";

        private const string                                                yesResult 
            = "Based on the following questions and responses, Medicare is the primary payor:";

        #endregion
    }
}

