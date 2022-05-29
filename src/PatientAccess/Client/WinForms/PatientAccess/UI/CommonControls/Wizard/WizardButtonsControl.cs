using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls.Wizard
{
    /// <summary>
    /// WizardButtonsControl - a 'button bar' for a wizard page; controls navigation to
    /// Next, Prev, Summary pages, etc.  Inherit from this class if you wish to change the size, placement, etc.
    /// </summary>
    
    [Serializable]
	public class WizardButtonsControl : WizardNavigationBase
	{
        #region Event Handlers

        /// <summary>
        /// button_Clicked - handle the click event; call the base class Navigate method.
        /// </summary>
        /// <param name="sender">The calling button</param>
        /// <param name="e">null</param>
        private void button_Clicked(object sender, EventArgs e)
        {
            string navName = ((LoggingButton)sender).Text;

            if( this.buttonCollection[ navName ] != null )
            {
                base.Navigate( navName );
            }            
        }

        #endregion

        #region Methods

        /// <summary>
        /// SetPanel - determines the placement of buttons based on the containing control size, the length of text
        /// for the button description, and minimum button size.  Sets tab index from left to right in the 90 - 100 range.
        /// </summary>
        
        public void SetPanel()
        {
            this.pnlBorder.Controls.Clear();

            if( !this.DesignMode )
            {
                base.Size = this.MyWizardPage.MyWizardButtons.Size;
                base.pnlBorder.Size = this.MyWizardPage.MyWizardButtons.Size;

                if( this.MyWizardPage.Dock == DockStyle.Fill )
                {
                    this.pnlBorder.Width = this.MyWizardPage.MyWizardContainer.Width - 20;
                    this.Width = this.pnlBorder.Width;

                    this.Location = new Point( 10, this.MyWizardPage.MyWizardContainer.Height - this.pnlBorder.Height );
                }
            }               

            ArrayList reversedList = this.buttons;
            reversedList.Reverse();

            int tabIndex = 100;

            foreach( LoggingButton button in reversedList )
            {
                            
                button.Width = measureDisplayStringWidth(this.MyGraphics, button.Text, button.Font);

                if( button.Width < 75 )
                {
                    button.Width = 75;
                }

                int xposition = getStartingXLocation();

                if( xposition == this.pnlBorder.Width )
                {
                    button.TabIndex = 1;
                    button.SetBounds( xposition - button.Width, heightPad, button.Width, button.Height );                   
                }
                else
                {
                     button.SetBounds( xposition - (button.Width + widthPad), heightPad, button.Width, button.Height );   
                }
                
                this.pnlBorder.Controls.Add( button );
                button.BackColor = Color.White;
                button.Visible = true;
                button.BringToFront();
                button.TabIndex = tabIndex;

                tabIndex--;
            }

            EnableThemesOn( this );
        }

        /// <summary>
        /// SetAcceptButton - allows the setting of the AcceptButton property for a button in the collection.
        /// </summary>
        /// <param name="navName"></param>
        
        public void SetAcceptButton( string navName )
        {
            if( this.buttonCollection.Contains( navName ) )
            {
                LoggingButton aButton = (LoggingButton)this.buttonCollection[ navName ];

                this.AcceptButton = aButton;
            }
        }

        /// <summary>
        /// SetDialogResult - allows the setting of the DialogResult property for a button in the collection.
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="dr"></param>

        public void SetDialogResult( string navName, DialogResult dr )
        {
            if( this.buttonCollection.ContainsKey(navName) )
            {
                LoggingButton aButton = this.buttonCollection[ navName ] as LoggingButton;

                aButton.DialogResult = dr;
            }
        }

        /// <summary>
        /// AddNavigation - add an event handler for a button in the collection
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="functionDelegate"></param>
        
        public override void AddNavigation( string navName, FunctionDelegate functionDelegate )
        {
            if( functionDelegate != null )
            {
                base.AddNavigation( navName, functionDelegate );
            }

            LoggingButton aButton = new LoggingButton();
           
            aButton.Name = navName;
            aButton.Text = navName;
            aButton.Click +=new EventHandler(button_Clicked);

            this.buttonCollection.Add( navName, aButton );  
            this.buttons.Add( aButton );

            aButton.Visible = true;
            aButton.BringToFront();

            if( functionDelegate == null )
            {
                aButton.Enabled = false;
            }
        }

        /// <summary>
        /// AddNavigation - add a page to link to (based on page name) for a button in the collection
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="pageName"></param>
        
        public override void AddNavigation( string navName, string pageName )
        {
            if( pageName != null && pageName != string.Empty )
            {
                base.AddNavigation( navName, pageName );
            }          

            LoggingButton aButton = this.addButton( navName );

            if( pageName == null || pageName == string.Empty )
            {
                aButton.Enabled = false;
            }
        }

        /// <summary>
        /// AddNavigation - add a page to link to for a button in the collection
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="aPage"></param>
        public override void AddNavigation( string navName, WizardPage aPage )
        {
            if( aPage != null )
            {
                base.AddNavigation( navName, aPage );
            }          

            LoggingButton aButton = this.addButton( navName );

            if( aPage == null )
            {
                aButton.Enabled = false;
            }
        }

        /// <summary>
        /// RemoveNavigation - remove the link associated with a button
        /// </summary>
        /// <param name="navName"></param>
        
        public override void RemoveNavigation( string navName )
        {
            base.RemoveNavigation( navName );

            this.pnlBorder.Controls.Remove( (LoggingButton)this.buttonCollection[ navName ] );
            this.buttonCollection.Remove( navName );

            foreach( LoggingButton button in this.buttons  )
            {  
                if( button.Name == navName )
                {
                    this.buttons.Remove( button );
                    break;
                }
            }            
        }

        /// <summary>
        /// UpdateNavigation - updates the button 'action' associated with the button name
        // and enables the button
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="aPage"></param>
        
        public override void UpdateNavigation( string navName, WizardPage aPage )
        {
            base.UpdateNavigation( navName, aPage );
            
            if( navName != null                
                && aPage != null )
            {
                this.EnableNavigation( navName );
            }    
            else
            {
                this.DisableNavigation( navName );
            }
        }

        // and enables the button
        /// <summary>
        /// UpdateNavigation - updates the button 'action' associated with the button name
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="pageName"></param>
        public override void UpdateNavigation( string navName, string pageName )
        {
            base.UpdateNavigation( navName, pageName );

            if( pageName != null
                && pageName != string.Empty )
            {
                this.EnableNavigation( navName );
            }    
            else
            {
                this.DisableNavigation( navName );
            }
        }

        /// <summary>
        /// UpdateNavigation - updates the button's event handler for a button in the collectin; enables the button
        /// </summary>
        /// <param name="navName"></param>
        /// <param name="functionDelegate"></param>
        public override void UpdateNavigation( string navName, FunctionDelegate functionDelegate )
        {
            base.UpdateNavigation( navName, functionDelegate );
            
            if( functionDelegate != null )
            {
                this.EnableNavigation( navName );
            }   
            else
            {
                this.DisableNavigation( navName );
            }
        }

        /// <summary>
        /// EnableNavigation - enables a button 
        /// </summary>
        /// <param name="navName"></param>
        private void EnableNavigation( string navName )
        {
            LoggingButton aButton = this.buttonCollection[ navName ] as LoggingButton;

            if( aButton != null )
            {
                aButton.Enabled = true;
            } 
        }

        /// <summary>
        /// DisableNavigation - disables a button 
        /// </summary>
        /// <param name="navName"></param>

        public void DisableNavigation( string navName )
        {
            LoggingButton aButton = this.buttonCollection[ navName ] as LoggingButton;

            if( aButton != null )
            {
                aButton.Enabled = false;
            } 
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// addButton - add a button to the collection.
        /// </summary>
        /// <param name="navName"></param>
        /// <returns></returns>
        
        private LoggingButton addButton( string navName )
        {
            LoggingButton aButton = new LoggingButton();
           
            aButton.Name = navName;
            aButton.Text = navName;
            aButton.Click +=new EventHandler(button_Clicked);

            if( this.buttonCollection.Contains( navName ) )
            {
                this.RemoveNavigation( navName );
            }

            this.buttonCollection.Add( navName, aButton );  
            this.buttons.Add( aButton );

            aButton.Enabled = true;
            aButton.Visible = true;
            aButton.BringToFront();

            return aButton;
        }

        /// <summary>
        /// measureDisplayStringWidth - calculates the width of a string based on the text length
        /// and the font; used for manually drawing listbox items
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        
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
            int x = this.pnlBorder.Width;
			            
            foreach( LoggingButton button in this.pnlBorder.Controls )
            {
                if( button.Location.X < x )
                {
                    x = button.Location.X;
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
            this.pnlBorder.Size = new System.Drawing.Size(684, 35);
            // 
            // WizardButtonsControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Name = "WizardButtonsControl";
            this.Size = new System.Drawing.Size(684, 35);
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

        #endregion

        #region Construction and Finalization


        public WizardButtonsControl( WizardPage aWizardPage )
        {
            base.MyWizardPage = aWizardPage;

            InitializeComponent();
        }

        public WizardButtonsControl()
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

        private IContainer                    components = null;

        private Hashtable                                           buttonCollection = new Hashtable();
        private ArrayList                                           buttons = new ArrayList();
        private Hashtable                                           commands = new Hashtable();
        private int                                                 widthPad = 7;
        private int                                                 heightPad = 2;

        private Graphics                                            graphics;

        #endregion

        #region Constants
        #endregion

    }
}

