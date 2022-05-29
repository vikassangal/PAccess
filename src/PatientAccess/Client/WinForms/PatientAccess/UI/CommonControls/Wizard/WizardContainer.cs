using System;
using System.Collections;
using System.ComponentModel;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CommonControls.Wizard
{
    /// <summary>
    /// WizardContainer - is a container classes that holds the collection of WizardPages that comprise a wizard.
    /// Inherit from this class if you wish to change the size, placement, etc.
    /// </summary>
    
    [Serializable]
    public class WizardContainer : ControlView
    {    
        #region Events

        public event EventHandler OnNavigation;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public void Cancel()
        {
            this.ResetPages();
            this.ParentForm.Dispose();
        }

        /// <summary>
        /// GetWizardPage - return the specified page based on page name
        /// </summary>
        /// <param name="pageName">page name</param>
        /// <returns></returns>
        
        public WizardPage GetWizardPage( string pageName )
        {
            if( this.wizardPages.Contains( pageName ) )
            {
                return this.wizardPages[ pageName ] as WizardPage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Start - kick off the wizard by displaying the first page
        /// </summary>
        /// <param name="pageName"></param>
        public void Start( string pageName )
        {
            if( this.wizardPages.Contains( pageName ) )
            {
                this.Navigate( pageName );
            }
        }

        /// <summary>
        /// Finish - end the wizard by updating the domain and closing
        /// </summary>
        public void Finish()
        {
            //this.UpdateModel();

            this.Dispose();
        }

        /// <summary>
        /// UpdateModel - for each WizardPage in the collection, invoke it's UpdateModel method to update the domain.
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();

            // call UpdateModel on each page

            foreach( WizardPage page in this.wizardPages.Values  )
            {
                if( page.Enabled )
                {
                    page.UpdateModel();
                }
            }
        }

        /// <summary>
        /// UpdateView - Update this view
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView ();
        }

        /// <summary>
        /// AddWizardPage - add a page of the collection
        /// </summary>
        /// <param name="aPage"></param>
        public void AddWizardPage( WizardPage aPage )
        {            
            if( aPage == null )
            {
                return;
            }

            if( !this.wizardPages.Contains( aPage.PageName ) )
            {
                this.wizardPages.Add( aPage.PageName, aPage ); 
            }

            if( !this.sortedPages.Contains( aPage ) )
            {
                this.sortedPages.Add( aPage );
            }

            aPage.Enabled = false;

            this.Controls.Add( aPage );

        }

        /// <summary>
        /// Navigate - hide a previoulsy displayed page and show the requested page.  This method will typically
        /// be called from either the WizardLinksControl or the WizardButtonsControl.  The page is also enabled.
        /// </summary>
        /// <param name="pageName"></param>
        public void Navigate( string pageName )
        {
            if( currentPage == null
                || pageName != this.currentPage.PageName )
            {
                if( !this.wizardPages.Contains( pageName ) )
                {
                    return;
                }

                if( currentPage != null )
                {
                    currentPage.SendToBack();
                    currentPage.Hide();
                }                

                currentPage = (WizardPage)this.wizardPages[ pageName ];

                currentPage.Enabled = true;

                currentPage.Visible = true;
                currentPage.BringToFront();

                if( this.GetCurrentPage() != null )
                {
                    this.GetCurrentPage().IsCurrentPage = false;
                }
                
                currentPage.IsCurrentPage = true;
                currentPage.MyWizardLinks.SetPanel();
                currentPage.Show();

                if( this.OnNavigation != null )
                {
                    this.OnNavigation( this, new LooseArgs( currentPage ) );
                }
            }
        }

        public void ResetPages()
        {
            foreach( DictionaryEntry de in this.wizardPages  )
            {
                WizardPage aPage = de.Value as WizardPage;

                if( aPage.Enabled )
                {
                    aPage.ResetPage();
                }
            }
        }
        
        #endregion

        #region Properties

        public ArrayList SortedPages
        {
            get
            {
                return this.sortedPages;
            }
        }

        #endregion

        #region Private Methods

        private WizardPage GetCurrentPage()
        {
            if( this.wizardPages != null
                && this.wizardPages.Count > 0)
            {
                foreach( WizardPage aPage in this.wizardPages.Values  )
                {
                    if( aPage.IsCurrentPage )
                    {
                        return aPage;
                    }
                }
            }

            return null;
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // WizardContainer
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Name = "WizardContainer";
            this.Size = new System.Drawing.Size(970, 590);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WizardContainer()
        {
            base.EnableThemesOn( this );

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
        
        private IContainer            components = null;

        /// <summary>
        /// WizardPages - a collection of pages for this wizard.  Key = WizardPage.Name, Value = WizardPage
        /// </summary>
        private IDictionary                                 wizardPages = new Hashtable();
        private ArrayList                                   sortedPages = new ArrayList();
        private WizardPage                                  currentPage = null;

        #endregion

        #region Constants
        #endregion
          
    }
}

