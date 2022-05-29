using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.ShortRegistration.InsuranceViews.MSP2;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// MSP2Dialog - the form launched when the MSP button is selected.  This form holds a WizardContainer instance
    /// which comprises this wizard
    /// </summary>
    [Serializable]
	public class MSP2Dialog : TimeOutFormView
	{
        #region Events
        
        public event EventHandler MspButtonClickEvent;

        #endregion

        #region Event Handlers

        private void summaryPage_MSPCancelled(object sender, EventArgs e)
        {
            this.Model_Account.MedicareSecondaryPayor = this.savedMedicareSecondaryPayor;
        }

        /// <summary>
        /// wizardContainer_OnNavigation - a navigate event occurred on the a page in the container; determine
        /// if the checkbox to display/not display the Welcome page is visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardContainer_OnNavigation(object sender, EventArgs e)
        {
            WizardPage aPage = ((LooseArgs)e).Context as WizardPage;

            if( aPage != null )
            {
                this.CheckDontShowCheckBox( aPage );
            }           

            aPage.UpdateView();
        }

        /// <summary>
        /// cbDontShowAgain_CheckedChanged - the value for the Don't show the Welcome page checkbox changed;
        /// persist to user storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDontShowAgain_CheckedChanged(object sender, EventArgs e)
        {
            if( this.cbDontShowAgain.Checked )
            {
                this.userPreference.ShowMSPWelcomeScreen = false;
            }
            else
            {
                this.userPreference.ShowMSPWelcomeScreen = true;
            }
            
            this.userPreference.Save( this.userPreference );            
        }

        /// <summary>
        /// MSP2Dialog_Load - instantiate and load up the wizard pages 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MSP2Dialog_Load(object sender, EventArgs e)
        {
            // instantiate the pages for the container

            this.welcomePage                            = new WelcomePage( "WelcomePage", this.wizardContainer );
            this.welcomePage.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.specialProgramsPage                    = new SpecialProgramsPage( "SpecialProgramsPage", this.wizardContainer, this.Model_Account );
            this.specialProgramsPage.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.liabilityInsurerPage                   = new LiabilityInsurerPage( "LiabilityInsurerPage", this.wizardContainer, this.Model_Account );
            this.liabilityInsurerPage.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.medicareEntitlementPage                = new MedicareEntitlementPage( "MedicareEntitlementPage", this.wizardContainer, this.Model_Account );
            this.medicareEntitlementPage.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.ageEntitlementPage1                    = new AgeEntitlementPage1( "AgeEntitlementPage1", this.wizardContainer, this.Model_Account );
            this.ageEntitlementPage1.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            shortAgeEntitlementPage1 = new ShortAgeEntitlementPage1( "ShortAgeEntitlementPage1", wizardContainer, Model_Account );
            shortAgeEntitlementPage1.MSPCancelled += summaryPage_MSPCancelled;

            this.ageEntitlementPage2                    = new AgeEntitlementPage2( "AgeEntitlementPage2", this.wizardContainer, this.Model_Account );
            this.ageEntitlementPage2.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.disabilityEntitlementPage1             = new DisabilityEntitlementPage1( "DisabilityEntitlementPage1", this.wizardContainer, this.Model_Account );
            this.disabilityEntitlementPage1.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            shortDisabilityEntitlementPage1 = new ShortDisabilityEntitlementPage1( "ShortDisabilityEntitlementPage1", this.wizardContainer, this.Model_Account );
            shortDisabilityEntitlementPage1.MSPCancelled += summaryPage_MSPCancelled;

            this.disabilityEntitlementPage2             = new DisabilityEntitlementPage2( "DisabilityEntitlementPage2", this.wizardContainer, this.Model_Account );
            this.disabilityEntitlementPage2.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.disabilityEntitlementPage3             = new DisabilityEntitlementPage3( "DisabilityEntitlementPage3", this.wizardContainer, this.Model_Account );
            this.disabilityEntitlementPage3.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.esrdEntitlementPage1                   = new ESRDEntitlementPage1( "ESRDEntitlementPage1", this.wizardContainer, this.Model_Account );
            this.esrdEntitlementPage1.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.esrdEntitlementPage2                   = new ESRDEntitlementPage2( "ESRDEntitlementPage2", this.wizardContainer, this.Model_Account );
            this.esrdEntitlementPage2.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            this.summaryPage                            = new SummaryPage( "SummaryPage", this.wizardContainer, this.Model_Account );
            this.summaryPage.MSPCancelled +=new EventHandler(summaryPage_MSPCancelled);

            // add them to the collection of pages

            this.wizardContainer.AddWizardPage( welcomePage );       
            this.wizardContainer.AddWizardPage( specialProgramsPage );
            this.wizardContainer.AddWizardPage( liabilityInsurerPage );
            this.wizardContainer.AddWizardPage( medicareEntitlementPage );
            this.wizardContainer.AddWizardPage( ageEntitlementPage1  );
            wizardContainer.AddWizardPage( shortAgeEntitlementPage1 );
            this.wizardContainer.AddWizardPage( ageEntitlementPage2 );
            this.wizardContainer.AddWizardPage( disabilityEntitlementPage1  );
            wizardContainer.AddWizardPage( shortDisabilityEntitlementPage1 );
            this.wizardContainer.AddWizardPage( disabilityEntitlementPage2  );
            this.wizardContainer.AddWizardPage( disabilityEntitlementPage3  );
            this.wizardContainer.AddWizardPage( esrdEntitlementPage1  );
            this.wizardContainer.AddWizardPage( esrdEntitlementPage2  );
            this.wizardContainer.AddWizardPage( summaryPage  );            

            // add the links for each page

            this.welcomePage.AddButtons();
            this.specialProgramsPage.AddButtons();
            this.liabilityInsurerPage.AddButtons();
            this.medicareEntitlementPage.AddButtons();
            this.ageEntitlementPage1.AddButtons();
            shortAgeEntitlementPage1.AddButtons();
            this.ageEntitlementPage2.AddButtons();
            this.disabilityEntitlementPage1.AddButtons();
            shortDisabilityEntitlementPage1.AddButtons();
            this.disabilityEntitlementPage2.AddButtons();
            this.disabilityEntitlementPage3.AddButtons();
            this.esrdEntitlementPage1.AddButtons();
            this.esrdEntitlementPage2.AddButtons();
            this.summaryPage.AddButtons();

            // and kick it off!

            if( this.SummaryOnly )
            {
                this.wizardContainer.Start( this.summaryPage.PageName );
            }
            else
            {
                if( userPreference.ShowMSPWelcomeScreen )
                {
                    this.wizardContainer.Start( this.welcomePage.PageName );
                }
                else
                {
                    this.wizardContainer.Start( this.specialProgramsPage.PageName );
                }
            }

            this.Cursor = Cursors.Default;

            if( this.Model_Account != null
                && this.Model_Account.MedicareSecondaryPayor != null )
            {
                this.savedMedicareSecondaryPayor = this.Model_Account.MedicareSecondaryPayor.DeepCopy() 
                    as MedicareSecondaryPayor;
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// UpdateModel - invoke UpdateModel on the WizardContainer (which, in turn, invokes UpdateModel on each page)
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();

            this.wizardContainer.UpdateModel();
        }

        /// <summary>
        /// GetPrimaryCoverage - common method called by multiple pages to retrieve the primary insurance
        /// </summary>
        /// <returns></returns>
        public Coverage GetPrimaryCoverage()
        {
            Coverage primaryCoverage = null;
            ICollection coverageCollection = Model_Account.Insurance.Coverages;

            if( coverageCollection == null )
            {
                return null;
            }

            foreach( Coverage coverage in coverageCollection )
            {
                if( coverage == null )
                {
                    continue;
                }
                else if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    primaryCoverage = coverage;
                    break;
                }
            }
            
            return primaryCoverage;
        }

        /// <summary>
        /// GetSecondaryCoverage - common method called by multiple pages to retrieve the primary insurance
        /// </summary>
        /// <returns></returns>
        public Coverage GetSecondaryCoverage()
        {
            Coverage secondaryCoverage = null;
            ICollection coverageCollection = Model_Account.Insurance.Coverages;

            if( coverageCollection == null )
            {
                return null;
            }

            foreach( Coverage coverage in coverageCollection )
            {
                if( coverage == null )
                {
                    continue;
                }
                else if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    secondaryCoverage = coverage;
                    break;
                }
            }
            
            return secondaryCoverage;
        }

        /// <summary>
        /// RaiseTabSelectedEvent - a button was clicked which allows the user to abandon the wizard and
        /// edit info for the patient or guarantor... raise the event to open the requested tab on AccountView
        /// </summary>
        /// <param name="index"></param>
        public void RaiseTabSelectedEvent( int index )
        {
            this.wizardContainer.ResetPages();

            if( MspButtonClickEvent != null )
            {
                MspButtonClickEvent( this, new LooseArgs( index ) );
            }
        }

        /// <summary>
        /// CheckDontShowCheckBox - based on user storage, the user has opted not to show the welcome page
        /// </summary>
        /// <param name="aPage"></param>
        private void CheckDontShowCheckBox( WizardPage aPage )
        {      
            if( aPage != null )
            {
                if( aPage.GetType() == typeof( WelcomePage ) )
                {
                    if( !aPage.Enabled )
                    {
                        this.cbDontShowAgain.Visible        = false;
                    }
                    else
                    {
                        if( !this.userPreference.ShowMSPWelcomeScreen )
                        {
                            this.cbDontShowAgain.Checked    = true;
                            this.cbDontShowAgain.Visible    = true;
                        }
                        else
                        {
                            this.cbDontShowAgain.Checked    = false;
                            this.cbDontShowAgain.Visible    = true;
                        }
                    }

                    if( aPage.MyWizardLinks.LoggingLinkButtonsList.Count == 0 )
                    {
                        aPage.MyWizardLinks.TabStop         = false;
                    }
                    else
                    {
                        aPage.MyWizardLinks.TabStop         = true;
                    }
                }
                else 
                {
                    this.cbDontShowAgain.Visible            = false;                    
                }           
            }
             
        }

        #endregion

        #region Properties

        private Account Model_Account 
        {
            get
            {
                return this.Model as Account;
            }
            set
            {
                this.Model = value;
            }
        }

        public bool SummaryOnly
        {
            private get
            {
                return i_SummaryOnly;
            }
            set
            {
                i_SummaryOnly = value;
            }
        }

        #endregion

        #region Private Methods

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wizardContainer = new PatientAccess.UI.CommonControls.Wizard.WizardContainer();
            this.cbDontShowAgain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // wizardContainer
            // 
            this.wizardContainer.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.wizardContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardContainer.Location = new System.Drawing.Point(0, 0);
            this.wizardContainer.Model = null;
            this.wizardContainer.Name = "wizardContainer";
            this.wizardContainer.Size = new System.Drawing.Size(734, 653);
            this.wizardContainer.TabIndex = 0;
            this.wizardContainer.OnNavigation += new System.EventHandler(this.wizardContainer_OnNavigation);
            // 
            // cbDontShowAgain
            // 
            this.cbDontShowAgain.Location = new System.Drawing.Point(7, 624);
            this.cbDontShowAgain.Name = "cbDontShowAgain";
            this.cbDontShowAgain.Size = new System.Drawing.Size(177, 24);
            this.cbDontShowAgain.TabIndex = 3;
            this.cbDontShowAgain.Text = "Don\'t show this screen again";
            this.cbDontShowAgain.CheckedChanged += new System.EventHandler(this.cbDontShowAgain_CheckedChanged);
            // 
            // MSP2Dialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(734, 653);
            this.Controls.Add(this.cbDontShowAgain);
            this.Controls.Add(this.wizardContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MSP2Dialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medicare Secondary Payor";
            this.Load += new System.EventHandler(this.MSP2Dialog_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties

        private UserPreference userPreference
        {
            get
            {
                if( this.i_userPreference == null )
                {
                    this.i_userPreference = UserPreference.Load();
                }

                return i_userPreference;
            }
        }

        #endregion

        #region Construction and Finalization

        public MSP2Dialog()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public MSP2Dialog( Account anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            this.Model_Account = anAccount;
            
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

        public WizardContainer      wizardContainer;
        private UserPreference                                              i_userPreference;

        private WelcomePage                                            welcomePage;
        private SpecialProgramsPage                                    specialProgramsPage;
        private LiabilityInsurerPage                                   liabilityInsurerPage;
        private MedicareEntitlementPage                                medicareEntitlementPage;
        private AgeEntitlementPage1                                    ageEntitlementPage1;
        private AgeEntitlementPage2                                    ageEntitlementPage2;
        private DisabilityEntitlementPage1                             disabilityEntitlementPage1;
        private DisabilityEntitlementPage2                             disabilityEntitlementPage2;
        private DisabilityEntitlementPage3                             disabilityEntitlementPage3;
        private ESRDEntitlementPage1                                   esrdEntitlementPage1;
        private ESRDEntitlementPage2                                   esrdEntitlementPage2;
        private SummaryPage                                            summaryPage;

        private ShortAgeEntitlementPage1                                shortAgeEntitlementPage1;
        private ShortDisabilityEntitlementPage1                         shortDisabilityEntitlementPage1;

        private CheckBox                               cbDontShowAgain;   
   
        private bool                                                        i_SummaryOnly = false;

        private MedicareSecondaryPayor                                      savedMedicareSecondaryPayor = new MedicareSecondaryPayor();

        #endregion

        #region Constants
        #endregion
    }
}

