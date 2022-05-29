using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for SearchField.
    /// </summary>
    public class SearchField : ControlView
    {
        #region Events
        public event EventHandler PerformSearch;
        #endregion

        #region Event Handlers
        private void button_Search_Click(object sender, EventArgs e)
        {
            OnPerformSearch();
        }

        private void button_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyData == Keys.Enter )
            {
                OnPerformSearch();
            }
        }
        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployerName(textBox_SearchText);
        }
        #endregion

        #region Methods
        public virtual void OnPerformSearch() 
        {
            if (PerformSearch != null)
            {
                PerformSearch(this, EventArgs.Empty);
            }
        }

        public void Activate()
        {
            this.FindForm().ActiveControl = textBox_SearchText;
        }
        #endregion

        #region Properties
        public string SearchText
        {
            get
            {
                //return textBox_SearchText.Text;
                return textBox_SearchText.UnMaskedText;
            }
            set
            {
                //textBox_SearchText.Text = value;
                textBox_SearchText.UnMaskedText = value;
            }
        }

        public int MaxFieldLength
        {
            get
            {
                return textBox_SearchText.MaxLength;
            }
            set
            {
                textBox_SearchText.MaxLength = value;
            }
        }

        public string Mask
        {
            get
            {
                return textBox_SearchText.Mask;
            }
            set
            {
                textBox_SearchText.Mask = value;
            }
        }

        public string ValidationExpression
        {
            get
            {
                return textBox_SearchText.ValidationExpression;
            }
            set
            {
                textBox_SearchText.ValidationExpression = value;
            }
        }

        public string KeyPressExpression
        {
            get
            {
                return textBox_SearchText.KeyPressExpression;
            }
            set
            {
                textBox_SearchText.KeyPressExpression = value;
            }
        }

        public new bool Enabled
        {
            get
            {
                return textBox_SearchText.Enabled && button_Search.Enabled;
            }
            set
            {
                textBox_SearchText.Enabled = button_Search.Enabled = value;
            }
        }

        public bool TextBoxEnabled
        {
            get
            {
                return textBox_SearchText.Enabled;
            }
            set
            {
                textBox_SearchText.Enabled = value;
            }
        }

        public Button Button
        {
            get
            {
                return button_Search;
            }
        }
        #endregion

        #region Private Methods
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Search = new LoggingButton();
            this.textBox_SearchText = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.SuspendLayout();
            // 
            // button_Search
            // 
            this.button_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Search.Location = new System.Drawing.Point(218, 0);
            this.button_Search.Name = "button_Search";
            this.button_Search.TabIndex = 4;
            this.button_Search.Text = "Sear&ch";
            this.button_Search.Click += new System.EventHandler(this.button_Search_Click);
            this.button_Search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button_Search_KeyDown);
            // 
            // textBox_SearchText
            // 
            this.textBox_SearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_SearchText.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SearchText.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textBox_SearchText.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.textBox_SearchText.Location = new System.Drawing.Point(1, 1);
            this.textBox_SearchText.Mask = "";
            this.textBox_SearchText.Name = "textBox_SearchText";
            this.textBox_SearchText.Size = new System.Drawing.Size(215, 20);
            this.textBox_SearchText.TabIndex = 3;
            this.textBox_SearchText.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            // 
            // SearchField
            // 
            this.AcceptButton = this.button_Search;
            this.Controls.Add(this.textBox_SearchText);
            this.Controls.Add(this.button_Search);
            this.Name = "SearchField";
            this.Size = new System.Drawing.Size(294, 23);
            this.ResumeLayout(false);

        }
        #endregion
        
        #endregion

        #region Private Properties
        #endregion

        #region Constructors and Finalization
        public SearchField()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();
        }
        #endregion

        #region Data Elements
        private Container components = null;
        public MaskedEditTextBox textBox_SearchText;
        public LoggingButton button_Search;
        #endregion

        #region Constants
        #endregion

    }
}
