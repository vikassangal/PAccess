using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for AKAView.
    /// </summary>
    public class AKAView  : ControlView
    {
        #region Events
        public event EventHandler TextEntryEvent;
        #endregion

        #region Event Handlers
        private void mtbLastName_TextChanged( object sender, EventArgs e )
        {
            RaiseTextEntryEvent();
        }

        private void mtbFirstName_TextChanged( object sender, EventArgs e )
        {
            RaiseTextEntryEvent();
        }

        #endregion

        #region Methods
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

        public override void UpdateModel()
        {
            Name aName = new Name( this.mtbFirstName.Text, this.mtbLastName.Text, string.Empty );
            //if ShowAKA checked then it is NOT Confidential.
            //By design PA User cannot change value for ShowAKA.
            //ShowAKA is always disabled. In case you add new AKA - showAKA is checked by default and disabled.
            aName.IsConfidential =  !(this.checkBoxShowAka.Checked);
            aName.Timestamp = DateTime.Now.Date;

            this.Model = aName;
        }

        public override void UpdateView()
        {
           this.mtbLastName.Text = this.Model_Name.LastName;
           this.mtbFirstName.Text = this.Model_Name.FirstName;
           //if it is Confidential then do NOT show AKA (Show AKA should be UNchecked):
           this.checkBoxShowAka.Checked = !(this.Model_Name.IsConfidential);
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );
        }

        #endregion

        #region Properties
        public Name Model_Name
        {
            get
            {
                return ( Name )this.Model;
            }
        }

        public string FirstName
        {
            get
            {
                return mtbFirstName.Text;
            }
        }

        public string LastName
        {
            get
            {
                return mtbLastName.Text;
            }
        }
        #endregion

        #region Private Methods
        private void RaiseTextEntryEvent()
        {
            if( TextEntryEvent != null )
            {
               TextEntryEvent( null,null );
            }
        }
 
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblLastName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.checkBoxShowAka = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblLastName
            // 
            this.lblLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblLastName.Location = new System.Drawing.Point(8, 23);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(64, 16);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last name:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            
            this.mtbLastName.Location = new System.Drawing.Point(80, 21);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(232, 20);
            this.mtbLastName.TabIndex = 1;
            
            this.mtbLastName.TextChanged += new System.EventHandler(this.mtbLastName_TextChanged);
            // 
            // lblFirstName
            // 
            this.lblFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblFirstName.Location = new System.Drawing.Point(328, 23);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(72, 16);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First name:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            
            this.mtbFirstName.Location = new System.Drawing.Point(400, 21);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 15;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(168, 20);
            this.mtbFirstName.TabIndex = 2;
           
            this.mtbFirstName.TextChanged += new System.EventHandler(this.mtbFirstName_TextChanged);
            // 
            // checkBoxShowAka
            // 
            this.checkBoxShowAka.Checked = true;
            //according design ShowAKA should always be disabled:
            this.checkBoxShowAka.Enabled = false;
            this.checkBoxShowAka.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowAka.Location = new System.Drawing.Point(8, 72);
            this.checkBoxShowAka.Name = "checkBoxShowAka";
            this.checkBoxShowAka.Size = new System.Drawing.Size(360, 24);
            this.checkBoxShowAka.TabIndex = 3;
            this.checkBoxShowAka.Text = "Show AKA (Show this name in search results and patient account)";
            // 
            // AKAView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.checkBoxShowAka);
            this.Controls.Add(this.mtbFirstName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.mtbLastName);
            this.Controls.Add(this.lblLastName);
            this.Name = "AKAView";
            this.Size = new System.Drawing.Size(580, 112);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AKAView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();
        }

        #endregion

        #region Data Elements
        private Label lblLastName;
        private Label lblFirstName;
        private CheckBox checkBoxShowAka;
        private Container components = null;

        private MaskedEditTextBox            mtbFirstName;
        private MaskedEditTextBox            mtbLastName;
        #endregion

        #region Constants
        #endregion
    }
}
