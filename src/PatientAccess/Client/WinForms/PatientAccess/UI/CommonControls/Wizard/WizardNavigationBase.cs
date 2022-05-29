using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CommonControls.Wizard
{
    [Serializable]
    public class WizardNavigationBase : ControlView, IWizardNavigationBase
    {
        #region Event Handlers

        private void WizardNavigationBase_Load(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Methods

        public virtual void AddNavigation(string navName, string pageName)
        {
            if( !WizardNavigations.Contains( navName ) )
            {
                WizardPage aPage = this.MyWizardPage.MyWizardContainer.GetWizardPage( pageName );

                if( aPage != null )
                {
                    WizardNavigations.Add( navName, aPage );
                }                
            }
        }

        public virtual void AddNavigation(string navName, WizardPage aPage)
        {
            if( !WizardNavigations.Contains( navName ) )
            {
                WizardNavigations.Add( navName, aPage );
            }
        }

        public virtual void AddNavigation(string navName, FunctionDelegate functionDelegate)
        {
            if( !WizardNavigations.Contains( navName ) )
            {
                WizardNavigations.Add( navName, functionDelegate );
            }
        }

        public virtual void RemoveNavigation(string navName)
        {  
            if( WizardNavigations.Contains( navName ) )
            {                          
                WizardNavigations.Remove( navName );
            }
        }

        public virtual void UpdateNavigation(string navName, WizardPage aPage)
        {
            if( aPage == null )
            {
                return;
            }

            if( WizardNavigations.Contains( navName ) )
            {
                WizardNavigations.Remove( navName );                
            }

            WizardNavigations.Add( navName, aPage );
        }

        public WizardPage GetNavigationPage( string navName )
        {
            if( WizardNavigations.Contains( navName ) )
            {
                return WizardNavigations[navName] as WizardPage;
            }
            else
                return null;
        }

        public virtual void UpdateNavigation( string navName, string pageName )
        {
            if( this.MyWizardPage == null
                || this.MyWizardPage.MyWizardContainer == null )
            {
                return;
            }

            if( WizardNavigations.Contains( navName ) )
            {
                WizardNavigations.Remove( navName );
            }

            WizardPage aPage = this.MyWizardPage.MyWizardContainer.GetWizardPage( pageName );

            if( aPage != null )
            { 
                WizardNavigations.Add( navName, aPage );
            }
        }

        public virtual void UpdateNavigation( string navName, FunctionDelegate functionDelegate )
        {
            if( this.MyWizardPage == null
                || this.MyWizardPage.MyWizardContainer == null )
            {
                return;
            }

            if( WizardNavigations.Contains( navName ) )
            {
                WizardNavigations.Remove( navName );                
            }

            WizardNavigations.Add( navName, functionDelegate );
        }

        public void Navigate( string navName )
        {
            if( !this.i_WizardNavigations.Contains( navName) )
            {
                throw new Exception("Wizard link not found");
            }

            if( this.i_WizardNavigations[ navName ].GetType().BaseType == typeof(WizardPage) )
            {
                WizardPage aPage = (WizardPage)this.i_WizardNavigations[ navName ];

                if( aPage == null)
                {
                    throw new Exception("Wizard page not found");
                }

                if( aPage.PageName == string.Empty )
                {
                    throw new Exception("Wizard page name not found");
                }

                aPage.Enabled = true;
                this.MyWizardPage.MyWizardContainer.Navigate( aPage.PageName );
            }
            else if ( this.i_WizardNavigations[ navName ].GetType() == typeof(FunctionDelegate) )
            {
                this.Invoke( (FunctionDelegate)this.i_WizardNavigations[ navName ] );
            }
            
        }

        #endregion

        #region Properties

        public WizardPage MyWizardPage
        {
            get
            {
                return i_MyWizardPage;
            }
            set
            {
                i_MyWizardPage = value;
            }
        }

        private Hashtable WizardNavigations
        {
            get
            {
                return i_WizardNavigations;
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
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.pnlBorder.Location = new System.Drawing.Point(0, 0);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(684, 35);
            this.pnlBorder.TabIndex = 0;
            // 
            // WizardNavigationBase
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.pnlBorder);
            this.Name = "WizardNavigationBase";
            this.Size = new System.Drawing.Size(684, 35);
            this.Load += new System.EventHandler(this.WizardNavigationBase_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WizardNavigationBase()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
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

        private IContainer                components = null;

        private WizardPage                                    i_MyWizardPage = null;
        private Hashtable                                     i_WizardNavigations = new Hashtable();

        protected Panel                    pnlBorder = null;

        #endregion

        #region Constants
        #endregion

    }
}

