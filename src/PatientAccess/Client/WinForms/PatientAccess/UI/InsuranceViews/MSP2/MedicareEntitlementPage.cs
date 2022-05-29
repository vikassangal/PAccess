using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.ShortRegistration.InsuranceViews.MSP2;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// MedicareEntitlementPage - determine which entitlement the patient has
    /// </summary>
   
    [Serializable]
    public class MedicareEntitlementPage : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// medicareEntitlementGroup1_KeyUp - the user tabbed; check if there are no radios selected... if not, 
        /// default to ESRD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void medicareEntitlementGroup1_KeyUp(object sender, KeyEventArgs e)
        {
            if( this.noRadioChecked )
            {
                this.noRadioChecked = false;
                this.medicareEntitlementGroup1.rbQ1ESRD.Checked = true;
                this.medicareEntitlementGroup1.rbQ1ESRD.Focus();
            }
        }

        /// <summary>
        /// pnlWizardPageBody_Enter - setup for the tab event (see medicareEntitlementGroup1_KeyUp above)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlWizardPageBody_Enter(object sender, EventArgs e)
        {
            if( !this.medicareEntitlementGroup1.rbQ1Age.Checked 
                && !this.medicareEntitlementGroup1.rbQ1Disability.Checked 
                && !this.medicareEntitlementGroup1.rbQ1ESRD.Checked )
            {
                this.noRadioChecked = true;
            }
        }

        /// <summary>
        /// medicareEntitlementGroup1_RadioChanged - a radio was selected; determine if we can navigate, 
        /// and reset any previously selected entitlement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void medicareEntitlementGroup1_RadioChanged(object sender, EventArgs e)
        {
            this.CanPageNavigate();      

            if( this.medicareEntitlementGroup1.rbQ1ESRD.Checked )
            {                                                
                if( this.previousEntitlementPage != string.Empty )
                {                    
                    if( this.previousEntitlementPage == GetDisabilityEntitlementPageName() )
                    {                        
                        this.resetDisabilityPages();
                    }
                    else if( this.previousEntitlementPage == GetAgeEntitlementPageName() )
                    {
                        this.resetAgePages();
                    }
                }

                if( this.previousEntitlementPage != string.Empty
                    && this.previousEntitlementPage != "ESRDEntitlementPage1")
                {
                    if( Model_Account != null
                        && Model_Account.MedicareSecondaryPayor != null) 
                    {
                        this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = new ESRDEntitlement();
                    }                    
                }

                this.previousEntitlementPage = "ESRDEntitlementPage1";

            }
            else if( this.medicareEntitlementGroup1.rbQ1Disability.Checked )
            {                      
                if( this.previousEntitlementPage != string.Empty )
                {                    
                    if( this.previousEntitlementPage == "ESRDEntitlementPage1" )
                    {
                        this.resetESRDPages();
                    }
                    else if( this.previousEntitlementPage == GetAgeEntitlementPageName() )
                    {
                        this.resetAgePages();
                    }
                }

                if( this.previousEntitlementPage != string.Empty
                    && this.previousEntitlementPage != GetDisabilityEntitlementPageName() )
                {
                    if( Model_Account != null
                        && Model_Account.MedicareSecondaryPayor != null)
                    {
                        this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement =new DisabilityEntitlement();
                    }                    
                }

                 this.previousEntitlementPage = GetDisabilityEntitlementPageName();
                
            }
            else if( this.medicareEntitlementGroup1.rbQ1Age.Checked )
            {            
                if( this.previousEntitlementPage != string.Empty )
                {                                        
                    if( this.previousEntitlementPage == "ESRDEntitlementPage1" )
                    {
                        this.resetESRDPages();
                    }
                    else if( this.previousEntitlementPage == GetDisabilityEntitlementPageName() )
                    {
                        this.resetDisabilityPages();
                    }
                }
    
                if( this.previousEntitlementPage != string.Empty
                    && this.previousEntitlementPage != GetAgeEntitlementPageName() )
                {
                    if( Model_Account != null
                        && Model_Account.MedicareSecondaryPayor != null ) 
                    {
                        this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = new AgeEntitlement();
                    }
                }            
    
                this.previousEntitlementPage = GetAgeEntitlementPageName();
            }             
           
            this.MyWizardLinks.SetPanel();
        }

        private bool IsShortRegAccount
        {
            get
            {
                return Model_Account.IsShortRegistered ||
                       AccountView.IsShortRegAccount; 
            }
        }

        private string GetAgeEntitlementPageName()
        {
            if ( IsShortRegAccount )
            {
                return "ShortAgeEntitlementPage1";
            }
            
            return "AgeEntitlementPage1";
        }

        private IAgeEntitlementPage1View GetAgeEntitlementPage()
        {
            IAgeEntitlementPage1View aPage1;
            if ( IsShortRegAccount )
            {
                aPage1 = MyWizardContainer.GetWizardPage( "ShortAgeEntitlementPage1" ) as ShortAgeEntitlementPage1;
            }
            else
            {
                aPage1 = MyWizardContainer.GetWizardPage( "AgeEntitlementPage1" ) as AgeEntitlementPage1;
            }
            return aPage1;
        }

        private string GetDisabilityEntitlementPageName()
        {
            if ( IsShortRegAccount )
            {
                return "ShortDisabilityEntitlementPage1";
            }

            return "DisabilityEntitlementPage1";
        }

        private IDisabilityEntitlementPage1View GetDisabilityEntitlementPage()
        {
            IDisabilityEntitlementPage1View aPage1;
            if ( IsShortRegAccount )
            {
                aPage1 = MyWizardContainer.GetWizardPage( "ShortAgeEntitlementPage1" ) as ShortDisabilityEntitlementPage1;
            }
            else
            {
                aPage1 = MyWizardContainer.GetWizardPage( "AgeEntitlementPage1" ) as DisabilityEntitlementPage1;
            }
            return aPage1;
        }


        /// <summary>
        /// resetAgePages - for each of the Age pages, invoke the ResetPage method to re-initialize the page
        /// </summary>
        private void resetAgePages()
        {
            IAgeEntitlementPage1View aPage1 = GetAgeEntitlementPage();

            if ( aPage1 != null )
            {
                aPage1.ResetPage();
                aPage1.DisablePage();
            }

            AgeEntitlementPage2 aPage2 = this.MyWizardContainer.GetWizardPage( "AgeEntitlementPage2" )
                as AgeEntitlementPage2;
                        
            if( aPage2 != null )
            {
                aPage2.ResetPage();
                aPage2.Enabled = false;
            }
        }

        /// <summary>
        /// resetESRDPages - for each of the ESRD pages, invoke the ResetPage method to re-initialize the page
        /// </summary>
        private void resetESRDPages()
        {
            ESRDEntitlementPage1 ePage1 = this.MyWizardContainer.GetWizardPage( "ESRDEntitlementPage1" )
                as ESRDEntitlementPage1;
                        
            if( ePage1 != null )
            {
                ePage1.ResetPage();
                ePage1.Enabled = false;
            }

            ESRDEntitlementPage2 ePage2 = this.MyWizardContainer.GetWizardPage( "ESRDEntitlementPage2" )
                as ESRDEntitlementPage2;
                        
            if( ePage2 != null )
            {
                ePage2.ResetPage();
                ePage2.Enabled = false;
            }
        }

        /// <summary>
        /// resetDisabilityPages - for each of the Disability pages, invoke the ResetPage method to re-initialize the page
        /// </summary>
        private void resetDisabilityPages()
        {
            IDisabilityEntitlementPage1View dPage1 = GetDisabilityEntitlementPage();
                        
            if( dPage1 != null )
            {
                dPage1.ResetPage();
                dPage1.DisablePage();
            }

            DisabilityEntitlementPage2 dPage2 = this.MyWizardContainer.GetWizardPage( "DisabilityEntitlementPage2" )
                as DisabilityEntitlementPage2;
                        
            if( dPage2 != null )
            {
                dPage2.ResetPage();
                dPage2.Enabled = false;                            
            }

            DisabilityEntitlementPage3 dPage3 = this.MyWizardContainer.GetWizardPage( "DisabilityEntitlementPage3" )
                as DisabilityEntitlementPage3;
                        
            if( dPage3 != null )
            {
                dPage3.ResetPage();
                dPage3.Enabled = false;                            
            }
        }

        /// <summary>
        /// MedicareEntitlementPage_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MedicareEntitlementPage_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled )
            {
                this.UpdateView();
            }
        }

        /// <summary>
        /// MedicareEntitlementPage_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MedicareEntitlementPage_Load(object sender, EventArgs e)
        {
            this.LinkName                           = "Medicare Entitlement";
            this.MyWizardMessages.Message1          = "Medicare Entitlement";            
            this.MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1         = 8.25;
            this.MyWizardMessages.FontStyle1        = FontStyle.Bold;

            this.MyWizardMessages.Message2          = "Choose only one entitlement option.";

            this.MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize2         = 8.25;

            this.MyWizardMessages.ShowMessages();

            this.lbl1WhatBold.Text      = "Choose Age entitlement if:";
            this.lbl1What.Text          = 
                "The patient's entitlement by age began before he or she became entitled to Medicare by ESRD\r\n\r\n" +
                "OR \r\n\r\n" +
                "The patient was previously entitled by disability and is now 65 or older";
            this.lbl2WhatBold.Text      = "Choose Disability entitlement if:";
            this.lbl2What.Text          = 
                "The patient's entitlement by disability began before he or she became entitled to Medicare by ESRD\r\n\r\n" +
                "AND\r\n\r\n" +
                "The patient is not yet 65";
            this.lbl3WhatBold.Text      =
                "Choose ESRD entitlement if:";
            this.lbl3What.Text          =
                "The patient's ESRD entitlement began before age entitlement OR disability entitlement";        
        }

        /// <summary>
        /// Cancel - is the delegate for the Cancel button click event
        /// </summary>
        private void Cancel()
        {
            this.MyWizardContainer.Cancel();

            if( this.MSPCancelled != null )
            {
                this.MSPCancelled(this, null);
            }
        }

        #endregion

        #region Methods

        public override void ResetPage()
        {
            base.ResetPage ();

            this.medicareEntitlementGroup1.rbQ1Age.Checked          = false;
            this.medicareEntitlementGroup1.rbQ1Disability.Checked   = false;
            this.medicareEntitlementGroup1.rbQ1ESRD.Checked         = false;
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        private bool CheckForSummary()
        {
            bool rc = false;

            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if( this.CanNavigate )
            {
                if( this.EntitlementCanNavigate() )
                {
                    rc = true;
                    this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                    this.MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
                }
            }

            this.HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// EntitlementCanNavigate - called from Welcome, SpecialPrograms, or LiabilityInsurer page - determines if
        /// the Summary button can be enabled.
        /// </summary>
        /// <returns></returns>
        public bool EntitlementCanNavigate()
        {
            bool rc = false;

            if( this.medicareEntitlementGroup1.rbQ1ESRD.Checked )
            {  
                ESRDEntitlementPage1 page1 = this.MyWizardContainer.GetWizardPage( "ESRDEntitlementPage1" )
                    as ESRDEntitlementPage1;

                if( page1 != null )                   
                {
                    if( page1.CheckForSummary() )
                    {
                        rc = true;
                    }                    
                }
            }
            else if ( this.medicareEntitlementGroup1.rbQ1Disability.Checked )
            {
                IDisabilityEntitlementPage1View page1 = GetDisabilityEntitlementPage();
                        
                if( page1 != null )
                {
                    if( page1.CheckForSummary() )                        
                    {
                        rc = true;
                    }                    
                }
            }
            else if( this.medicareEntitlementGroup1.rbQ1Age.Checked )
            {
                IAgeEntitlementPage1View page1 = GetAgeEntitlementPage();

                if( page1 != null )
                {
                    if( page1.CheckForSummary() )
                    {
                        rc = true;
                    }                    
                }
            }

            return rc;
        }
        
        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {
            bool canNav = false;

            this.MyWizardButtons.UpdateNavigation( "&Next >", string.Empty );
            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if( this.medicareEntitlementGroup1.rbQ1ESRD.Checked )
            {                               
                SummaryPage sPage = this.MyWizardContainer.GetWizardPage( "SummaryPage" ) as SummaryPage;

                if( sPage != null )
                {
                    sPage.Enabled = false;
                    sPage.MyWizardButtons.UpdateNavigation( "< &Back", "ESRDEntitlementPage2" );
                }

                canNav = true;
                this.MyWizardButtons.UpdateNavigation( "&Next >", "ESRDEntitlementPage1" );
                this.MyWizardButtons.SetAcceptButton( "&Next >" );
            }
            else if ( this.medicareEntitlementGroup1.rbQ1Disability.Checked )
            {                                
                SummaryPage sPage = this.MyWizardContainer.GetWizardPage( "SummaryPage" ) as SummaryPage;

                if( sPage != null )
                {
                    sPage.Enabled = false;
                    sPage.MyWizardButtons.UpdateNavigation( "< &Back", "DisabilityEntitlementPage3" );
                }

                canNav = true;
                this.MyWizardButtons.UpdateNavigation( "&Next >", GetDisabilityEntitlementPageName() );
                this.MyWizardButtons.SetAcceptButton( "&Next >" );
            }
            else if( this.medicareEntitlementGroup1.rbQ1Age.Checked )
            {                
                SummaryPage sPage = this.MyWizardContainer.GetWizardPage( "SummaryPage" ) as SummaryPage;

                if( sPage != null )
                {
                    sPage.Enabled = false;
                    sPage.MyWizardButtons.UpdateNavigation( "< &Back", "AgeEntitlementPage2" );
                }

                canNav = true;
                this.MyWizardButtons.UpdateNavigation( "&Next >", GetAgeEntitlementPageName() );
                this.MyWizardButtons.SetAcceptButton( "&Next >" );
            }

            this.CanNavigate = canNav;

            this.CheckForSummary();

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
                blnLoaded       = true;                
            }    

            if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement != null )
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement != null )
                {
                    if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                        == typeof( AgeEntitlement) )
                    {
                        this.medicareEntitlementGroup1.rbQ1Age.Checked = true;
                    }
                    else if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                        == typeof( DisabilityEntitlement) )
                    {
                        this.medicareEntitlementGroup1.rbQ1Disability.Checked = true;
                    }
                    else if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                        == typeof( ESRDEntitlement) )
                    {
                        this.medicareEntitlementGroup1.rbQ1ESRD.Checked = true;
                    }

                }
                else
                {
                    if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.EntitlementType != null )
                    {
                        if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.EntitlementType == typeof(AgeEntitlement) )
                        {
                            this.medicareEntitlementGroup1.rbQ1Age.Checked = true;
                        }
                        else if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.EntitlementType == typeof(DisabilityEntitlement) )
                        {
                            this.medicareEntitlementGroup1.rbQ1Disability.Checked = true;
                        }
                        else if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.EntitlementType == typeof(ESRDEntitlement) )
                        {
                            this.medicareEntitlementGroup1.rbQ1ESRD.Checked = true;
                        }
                    }                    
                }
            }
        
            this.CanPageNavigate();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();

            if( this.medicareEntitlementGroup1.rbQ1ESRD.Checked )
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null
                    || this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                    != typeof(ESRDEntitlement) )
                {
                    this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = new ESRDEntitlement();
                }                
            }
            else if( this.medicareEntitlementGroup1.rbQ1Disability.Checked )
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null
                    || this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                    != typeof(DisabilityEntitlement) )
                {
                    this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = new DisabilityEntitlement();
                }                
            }
            else if( this.medicareEntitlementGroup1.rbQ1Age.Checked )
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null
                    || this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() 
                    != typeof(AgeEntitlement) )
                {
                    this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = new AgeEntitlement();
                }                
            }
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );

            WizardPage aPage = this.MyWizardButtons.GetNavigationPage( "< &Back" );

            if( aPage == null
                || ( aPage != null
                    && aPage.GetType() != typeof(SpecialProgramsPage) ) )
            {
                this.MyWizardButtons.AddNavigation( "< &Back", "LiabilityInsurerPage" );
            }
            
            this.MyWizardButtons.AddNavigation( "&Next >", string.Empty );            
            this.MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            this.MyWizardButtons.SetPanel();
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQuestion1 = new System.Windows.Forms.Panel();
            this.medicareEntitlementGroup1 = new PatientAccess.UI.InsuranceViews.MSP2.MedicareEntitlementGroup();
            this.lbl1WhatBold = new System.Windows.Forms.Label();
            this.lblWhatShouldIChoose = new System.Windows.Forms.Label();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.pnlDividerQ2 = new System.Windows.Forms.Panel();
            this.gbQ1Info = new System.Windows.Forms.GroupBox();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.lbl1What = new System.Windows.Forms.Label();
            this.lbl2WhatBold = new System.Windows.Forms.Label();
            this.lbl2What = new System.Windows.Forms.Label();
            this.lbl3WhatBold = new System.Windows.Forms.Label();
            this.lbl3What = new System.Windows.Forms.Label();
            this.pnlWizardPageBody.SuspendLayout();
            this.pnlQuestion1.SuspendLayout();
            this.gbQ1Info.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.pnlDivider1);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion1);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Enter += new System.EventHandler(this.pnlWizardPageBody_Enter);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlDivider1, 0);
            // 
            // pnlQuestion1
            // 
            this.pnlQuestion1.Controls.Add(this.medicareEntitlementGroup1);
            this.pnlQuestion1.Controls.Add(this.lblWhatShouldIChoose);
            this.pnlQuestion1.Controls.Add(this.lblQuestion1);
            this.pnlQuestion1.Controls.Add(this.pnlDividerQ2);
            this.pnlQuestion1.Controls.Add(this.gbQ1Info);
            this.pnlQuestion1.Location = new System.Drawing.Point(8, 63);
            this.pnlQuestion1.Name = "pnlQuestion1";
            this.pnlQuestion1.Size = new System.Drawing.Size(666, 414);
            this.pnlQuestion1.TabIndex = 1;
            // 
            // medicareEntitlementGroup1
            // 
            this.medicareEntitlementGroup1.Location = new System.Drawing.Point(231, -7);
            this.medicareEntitlementGroup1.Name = "medicareEntitlementGroup1";
            this.medicareEntitlementGroup1.Size = new System.Drawing.Size(422, 35);
            this.medicareEntitlementGroup1.TabIndex = 1;
            this.medicareEntitlementGroup1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.medicareEntitlementGroup1_KeyUp);
            this.medicareEntitlementGroup1.RadioChanged += new System.EventHandler(this.medicareEntitlementGroup1_RadioChanged);
            // 
            // lbl1WhatBold
            // 
            this.lbl1WhatBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lbl1WhatBold.Location = new System.Drawing.Point(38, 57);
            this.lbl1WhatBold.Name = "lbl1WhatBold";
            this.lbl1WhatBold.Size = new System.Drawing.Size(526, 14);
            this.lbl1WhatBold.TabIndex = 0;
            // 
            // lblWhatShouldIChoose
            // 
            this.lblWhatShouldIChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblWhatShouldIChoose.Location = new System.Drawing.Point(56, 64);
            this.lblWhatShouldIChoose.Name = "lblWhatShouldIChoose";
            this.lblWhatShouldIChoose.Size = new System.Drawing.Size(459, 15);
            this.lblWhatShouldIChoose.TabIndex = 0;
            this.lblWhatShouldIChoose.Text = "What should I choose if the patient has dual entitlement?";
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point(8, 3);
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size(223, 16);
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1. Are you entitled to Medicare based on:";
            // 
            // pnlDividerQ2
            // 
            this.pnlDividerQ2.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ2.Location = new System.Drawing.Point(5, 405);
            this.pnlDividerQ2.Name = "pnlDividerQ2";
            this.pnlDividerQ2.Size = new System.Drawing.Size(656, 2);
            this.pnlDividerQ2.TabIndex = 0;
            // 
            // gbQ1Info
            // 
            this.gbQ1Info.Controls.Add(this.lbl3What);
            this.gbQ1Info.Controls.Add(this.lbl3WhatBold);
            this.gbQ1Info.Controls.Add(this.lbl2What);
            this.gbQ1Info.Controls.Add(this.lbl2WhatBold);
            this.gbQ1Info.Controls.Add(this.lbl1What);
            this.gbQ1Info.Controls.Add(this.lbl1WhatBold);
            this.gbQ1Info.Location = new System.Drawing.Point(36, 39);
            this.gbQ1Info.Name = "gbQ1Info";
            this.gbQ1Info.Size = new System.Drawing.Size(593, 337);
            this.gbQ1Info.TabIndex = 0;
            this.gbQ1Info.TabStop = false;
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point(13, 54);
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size(656, 2);
            this.pnlDivider1.TabIndex = 0;
            // 
            // lbl1What
            // 
            this.lbl1What.Location = new System.Drawing.Point(38, 79);
            this.lbl1What.Name = "lbl1What";
            this.lbl1What.Size = new System.Drawing.Size(526, 66);
            this.lbl1What.TabIndex = 2;
            // 
            // lbl2WhatBold
            // 
            this.lbl2WhatBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lbl2WhatBold.Location = new System.Drawing.Point(38, 164);
            this.lbl2WhatBold.Name = "lbl2WhatBold";
            this.lbl2WhatBold.Size = new System.Drawing.Size(526, 14);
            this.lbl2WhatBold.TabIndex = 3;
            // 
            // lbl2What
            // 
            this.lbl2What.Location = new System.Drawing.Point(38, 188);
            this.lbl2What.Name = "lbl2What";
            this.lbl2What.Size = new System.Drawing.Size(526, 70);
            this.lbl2What.TabIndex = 4;
            // 
            // lbl3WhatBold
            // 
            this.lbl3WhatBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lbl3WhatBold.Location = new System.Drawing.Point(38, 276);
            this.lbl3WhatBold.Name = "lbl3WhatBold";
            this.lbl3WhatBold.Size = new System.Drawing.Size(526, 14);
            this.lbl3WhatBold.TabIndex = 5;
            // 
            // lbl3What
            // 
            this.lbl3What.Location = new System.Drawing.Point(38, 297);
            this.lbl3What.Name = "lbl3What";
            this.lbl3What.Size = new System.Drawing.Size(526, 14);
            this.lbl3What.TabIndex = 6;
            // 
            // MedicareEntitlementPage
            // 
            this.Name = "MedicareEntitlementPage";
            this.EnabledChanged += new System.EventHandler(this.MedicareEntitlementPage_EnabledChanged);
            this.Load += new System.EventHandler(this.MedicareEntitlementPage_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.pnlQuestion1.ResumeLayout(false);
            this.gbQ1Info.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public MedicareEntitlementPage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public MedicareEntitlementPage( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public MedicareEntitlementPage( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public MedicareEntitlementPage( string pageName, WizardContainer wizardContainer, Account anAccount )
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
        
        private IContainer                                components = null;

        private Panel                                      pnlDividerQ2;
        private Panel                                      pnlDivider1;
        private Panel                                      pnlQuestion1;

        private GroupBox                                   gbQ1Info;

        private Label                                      lblQuestion1;
        private Label                                      lblWhatShouldIChoose;

        private MedicareEntitlementGroup   medicareEntitlementGroup1;       

        private string                                                          previousEntitlementPage = string.Empty;

        private bool                                                            noRadioChecked = false;
        private Label lbl3WhatBold;
        private Label lbl3What;
        private Label lbl2WhatBold;
        private Label lbl2What;
        private Label lbl1WhatBold;
        private Label lbl1What;
        private bool                                                            blnLoaded = false;

        #endregion        

        #region Constants
        #endregion
    }
}

