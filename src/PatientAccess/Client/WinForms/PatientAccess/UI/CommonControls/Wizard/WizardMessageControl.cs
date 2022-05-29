using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CommonControls.Wizard
{
    /// <summary>
    /// WizardMessageControl - a control to display heading, text, error conditions, etc for a wizard page. 
    /// Inherit from this control if you wish to change the size, placement, etc.
    /// </summary>
    [Serializable]
	public class WizardMessageControl : ControlView
	{
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// ShowMessages - show the header message and the secondary message.
        /// </summary>
        public void ShowMessages()
        {
            this.applyFonts();
            this.lblMessage1.Text = this.Message1;
            this.lblMessage2.Text = this.Message2;
        }

        #endregion

        #region Properties

        public WizardPage MyWizardPage
        {
            get
            {
                return this.i_MyWizardPage;
            }
            set
            {
                this.i_MyWizardPage = value;
            }
        }

        public string Message1
        {
            private get
            {
                return i_Message1;
            }
            set
            {
                i_Message1 = value;
            }
        }

        public Color TextColor1
        {
            get
            {
                return i_TextColor1;
            }
            set
            {
                i_TextColor1 = value;
            }
        }

        public string TextFont1
        {
            get
            {
                return i_TextFont1;
            }
            set
            {
                i_TextFont1 = value;

                if( this.i_TextSize1 > 0 )
                {
                    this.i_SystemFont1 = new Font( value, (float)this.i_TextSize1 );
                }
            }
        }

        public FontStyle FontStyle1
        {
            get
            {
                return i_FontStyle1;
            }
            set
            {
                i_FontStyle1 = value;
            }
        }

        public double TextSize1
        {
            get
            {
                return i_TextSize1;
            }
            set
            {
                i_TextSize1 = value;

                if( this.i_TextFont1.Trim() != string.Empty )
                {
                    this.i_SystemFont1 = new Font( this.i_TextFont1, (float)value );
                }
            }
        }

        public string Message2
        {
            private get
            {
                return i_Message2;
            }
            set
            {
                i_Message2 = value;
            }
        }

        public Color TextColor2
        {
            get
            {
                return i_TextColor2;
            }
            set
            {
                i_TextColor2 = value;
            }
        }

        public string TextFont2
        {
            get
            {
                return i_TextFont2;
            }
            set
            {
                i_TextFont2 = value;

                if( this.i_TextSize2 > 0 )
                {
                    this.i_SystemFont2 = new Font( value, (float)this.i_TextSize2 );
                }
            }
        }

        public FontStyle FontStyle2
        {
            get
            {
                return i_FontStyle2;
            }
            set
            {
                i_FontStyle2 = value;
            }
        }

        public double TextSize2
        {
            get
            {
                return i_TextSize2;
            }
            set
            {
                i_TextSize2 = value;

                if( this.i_TextFont2.Trim() != string.Empty )
                {
                    this.i_SystemFont2 = new Font( this.i_TextFont2, (float)value );
                }
            }
        }

        #endregion

        #region Private Methods

        private void applyFont1()
        {
            this.lblMessage1.Font        = this.i_SystemFont1;
            this.lblMessage1.ForeColor   = this.i_TextColor1;

            if( this.i_FontStyle1 == FontStyle.Bold )
            {
                this.lblMessage1.Font = new Font( this.i_TextFont1, (float)this.i_TextSize1, FontStyle.Bold );
            }
        }

        private void applyFont2()
        {
            this.lblMessage2.Font        = this.i_SystemFont2;
            this.lblMessage2.ForeColor   = this.i_TextColor2;

            if( this.i_FontStyle2 == FontStyle.Bold )
            {
                this.lblMessage2.Font = new Font( this.i_TextFont2, (float)this.i_TextSize2, FontStyle.Bold );
            }
        }

        private void applyFonts()
        {
            this.applyFont1();
            this.applyFont2();
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage1 = new System.Windows.Forms.Label();
            this.lblMessage2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMessage1
            // 
            this.lblMessage1.Location = new System.Drawing.Point(9, 0);
            this.lblMessage1.Name = "lblMessage1";
            this.lblMessage1.Size = new System.Drawing.Size(668, 17);
            this.lblMessage1.TabIndex = 0;
            // 
            // lblMessage2
            // 
            this.lblMessage2.Location = new System.Drawing.Point(9, 23);
            this.lblMessage2.Name = "lblMessage2";
            this.lblMessage2.Size = new System.Drawing.Size(669, 18);
            this.lblMessage2.TabIndex = 1;
            // 
            // WizardMessageControl
            // 
            this.Controls.Add(this.lblMessage2);
            this.Controls.Add(this.lblMessage1);
            this.Name = "WizardMessageControl";
            this.Size = new System.Drawing.Size(680, 40);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WizardMessageControl()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public WizardMessageControl( WizardPage aPage )
        {
            this.MyWizardPage = aPage;

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

        private Label                          lblMessage1;
        private Label                          lblMessage2;

        private string                                              i_Message1;        
        private string                                              i_TextFont1     = "Microsoft Sans Serif";
        private Color                                i_TextColor1    = Color.Black;
        private Font                                 i_SystemFont1   = new Font("Microsoft Sans Serif", (float)8.25);        
        private FontStyle                            i_FontStyle1    = FontStyle.Regular;
        private double                                              i_TextSize1     = 8.25;

        private string                                              i_Message2;        
        private string                                              i_TextFont2     = "Microsoft Sans Serif";
        private Color                                i_TextColor2    = Color.Black;
        private Font                                 i_SystemFont2   = new Font("Microsoft Sans Serif", (float)8.25);        
        private FontStyle                            i_FontStyle2    = FontStyle.Regular;
        private double                                              i_TextSize2     = 8.25;

        private WizardPage                                          i_MyWizardPage;

        #endregion

        #region Constants
        #endregion

	}
}

