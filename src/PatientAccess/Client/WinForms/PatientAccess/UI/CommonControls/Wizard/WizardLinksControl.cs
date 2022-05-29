using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using log4net;

namespace PatientAccess.UI.CommonControls.Wizard
{
    /// <summary>
    /// WizardLinksControl - a 'link bar' for a wizard page; controls navigation to
    /// previously visited wizard pages.  Inherit from this class if you wish to change the size, placement, etc.
    /// </summary>
    [Serializable]
    public class WizardLinksControl : WizardNavigationBase
    {
        #region Event Handlers


        private void aLoggingLinkButton_GotFocus(object sender, EventArgs e)
        {
            LoggingLinkButton lb = sender as LoggingLinkButton;

            lb.Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold );

            this.Cursor = Cursors.Hand;
        }

        private void aLoggingLinkButton_LostFocus(object sender, EventArgs e)
        {
            LoggingLinkButton lb = sender as LoggingLinkButton;

            lb.Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular );

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// link_Clicked - handle the click event; call the base class Navigate method.
        /// </summary>
        /// <param name="sender">The LoggingLinkButton clicked</param>
        /// <param name="e">null</param>
        private void link_Clicked(object sender, EventArgs e)
        {
            string navName = ((LoggingLinkButton)sender).Text;

            base.Navigate( navName );
        }

        #endregion

        #region Methods

        /// <summary>
        /// SetPanel - determines the placement of buttons based on the containing control size, the length of text
        /// for the button description, and minimum button size.
        /// </summary>
        
        public void SetPanel()
        {
            this.LoggingLinkButtonCollection.Clear();
            this.LoggingLinkButtons.Clear();
            this.pnlBorder.Controls.Clear();

            int prevLabelLength = 0;

            if( !this.DesignMode )
            {
                if ( (this.MyWizardPage != null)
                    && (this.MyWizardPage.MyWizardLinks != null ))
                {
                    base.Size = this.MyWizardPage.MyWizardLinks.Size;
                    base.pnlBorder.Size = this.MyWizardPage.MyWizardLinks.Size;

                    if (this.MyWizardPage.Dock == DockStyle.Fill)
                    {
                        // resize this panel

                        this.pnlBorder.Width = this.MyWizardPage.MyWizardContainer.Width - 20;
                        this.Width = this.pnlBorder.Width;

                        this.Location = new Point(10, this.MyWizardPage.MyWizardContainer.Top + 10);
                    }
                }
            }  
            
            if( this.MyWizardPage != null
                && this.MyWizardPage.MyWizardContainer != null
                && this.MyWizardPage.MyWizardContainer.SortedPages != null
                && this.MyWizardPage.MyWizardContainer.SortedPages.Count > 0)
            {
                ArrayList pages = this.MyWizardPage.MyWizardContainer.SortedPages;

                foreach( WizardPage page in pages  )
                {
                    if( page != null
                        && page.Enabled 
                        && page.ShowLink )
                    {
                        this.AddNavigation( page.LinkName, page );
                    }
                }

                foreach( LoggingLinkButton label in this.LoggingLinkButtons )
                {
                    int xposition = getStartingXLocation();

                    int debug1 = label.Width;
                    int debug2 = label.Height;

                    if( pnlBorder.Controls.Count == 0 )
                    {
                        label.SetBounds( 0, heightPad, label.Width, label.Height );
                    }
                    else
                    {
                        label.SetBounds( xposition + (prevLabelLength + widthPad), heightPad, label.Width, label.Height );
                    }
                
                    this.pnlBorder.Controls.Add( label );
                    label.Visible = true;
                    label.BringToFront();

                    prevLabelLength = label.Width - widthPad;
                }
            }
        }


        /// <summary>
        /// AddNavigation - add an a page to link to (based on name) for a link in the collection
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="pageName"></param>
        public override void AddNavigation( string navName, string pageName )
        {
            if( pageName != null && pageName != string.Empty )
            {
                base.AddNavigation( navName, pageName );
            }

            this.addLink( navName );
        }

        /// <summary>
        /// AddNavigation - add an a page to link to for a link in the collection
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="aPage"></param>
        public override void AddNavigation( string navName, WizardPage aPage )
        {
            if( aPage != null )
            {
                base.AddNavigation( navName, aPage );
            }

            this.addLink( navName );
        }

        /// <summary>
        /// RemoveNavigation - remove the link associated with a link label
        /// </summary>
        /// <param name="navName"></param>
        
        public override void RemoveNavigation( string navName )
        {
            base.RemoveNavigation( navName );

            this.pnlBorder.Controls.Remove( (LoggingLinkButton)this.LoggingLinkButtonCollection[ navName ] );
            this.LoggingLinkButtonCollection.Remove( navName );

            foreach( LoggingLinkButton label in this.LoggingLinkButtons  )
            {                
                if( label.Name == navName )
                {
                    this.LoggingLinkButtons.Remove( label );
                    break;
                }
            }  
        }

        /// <summary>
        /// UpdateNavigation - update the page associated with a link
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="aPage"></param>
        
        public override void UpdateNavigation( string navName, WizardPage aPage )
        {
            base.UpdateNavigation( navName, aPage );
        }

        /// <summary>
        /// UpdateNavigation - update the page (based on name) associated with a link
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="pageName"></param>
        public override void UpdateNavigation( string navName, string pageName )
        {
            base.UpdateNavigation( navName, pageName );
        }

        /// <summary>
        /// UpdateNavigation - Update the event handler associated with a link
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="functionDelegate"></param>
        public override void UpdateNavigation( string navName, FunctionDelegate functionDelegate )
        {
            base.UpdateNavigation( navName, functionDelegate );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods


        /// <summary>
        /// addLink - adds a link to the collection.
        /// </summary>
        /// <param name="navName"></param>
        private void addLink( string navName )
        {
            this.tabIndex++;

            LoggingLinkButton aLoggingLinkButton = new LoggingLinkButton();
           
            aLoggingLinkButton.Name = navName;
            aLoggingLinkButton.Text = navName;
            aLoggingLinkButton.TabIndex = tabIndex;
            aLoggingLinkButton.FlatStyle = FlatStyle.Flat;
            aLoggingLinkButton.ForeColor = Color.Blue;
            
            aLoggingLinkButton.Click +=new EventHandler(link_Clicked);
            aLoggingLinkButton.GotFocus +=new EventHandler(aLoggingLinkButton_GotFocus);
            aLoggingLinkButton.LostFocus +=new EventHandler(aLoggingLinkButton_LostFocus);
                       
            aLoggingLinkButton.Width = measureDisplayStringWidth(this.MyGraphics, aLoggingLinkButton.Text, aLoggingLinkButton.Font);

            if( this.LoggingLinkButtonCollection.Contains( navName ) )
            {
                this.RemoveNavigation( navName );
            }

            this.LoggingLinkButtonCollection.Add( navName, aLoggingLinkButton );
            this.LoggingLinkButtons.Add( aLoggingLinkButton );

            aLoggingLinkButton.Visible = true;
            aLoggingLinkButton.BringToFront();
        }

        /// <summary>
        /// measureDisplayStringWidth - calculates the width of a string based on the text length
        /// and the font; used for manually drawing listbox items
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        /// 
        static private int measureDisplayStringWidth(Graphics graphics, string text,
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
        /// getStartingXLocation - determines the X coordinate of a button on the button control; used to 
        /// dynamically 'place' buttons in the button collection
        /// </summary>
        /// <returns></returns>
        
        private int getStartingXLocation()
        {
            int x = 0;
			
            foreach( LoggingLinkButton label in this.pnlBorder.Controls )
            {
                int debug = label.Location.X;

                if( label.Location.X > x )
                {
                    x = label.Location.X;
                }				
            }

            return x;
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // pnlBorder
            // 
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(684, 25);
            // 
            // WizardLinksControl
            //            
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Name = "WizardLinksControl";
            this.Size = new System.Drawing.Size(684, 27);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        private Graphics MyGraphics
        {
            get
            {
                if( this.graphics == null )
                {
                    Label lbl = new Label();
                    Graphics g = lbl.CreateGraphics();
                    graphics = g;

                }

                return this.graphics;
            }
        }

        public ArrayList LoggingLinkButtonsList
        {
            get
            {
                return this.LoggingLinkButtons;
            }
        }
        #endregion

        #region Construction and Finalization

        public WizardLinksControl()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public WizardLinksControl( WizardPage aWizardPage )
        {
            base.MyWizardPage = aWizardPage;

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

        private IContainer                    components = null;

        private Hashtable                                           LoggingLinkButtonCollection = new Hashtable();
        private ArrayList                                           LoggingLinkButtons = new ArrayList();
        private int                                                 widthPad = 3;
        private int                                                 heightPad = 8;
        private int                                                 tabIndex = 1;

        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( WizardLinksControl ) );

        private Graphics                                            graphics;

        #endregion

        #region Constants
        #endregion

    }
}

