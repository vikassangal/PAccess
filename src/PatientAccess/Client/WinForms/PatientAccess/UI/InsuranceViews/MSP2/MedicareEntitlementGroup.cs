using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.InsuranceViews.MSP2
{    
    [Serializable]
	public class MedicareEntitlementGroup : UserControl
	{       
        #region Events

        public event EventHandler RadioChanged;

        #endregion

        #region Event Handlers     

        /// <summary>
        /// radio_CheckedChanged - a radio was selected, so fire the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if( this.RadioChanged != null )
            {
                this.RadioChanged(sender, e);
            }
        }

        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQ1RadioButtons = new System.Windows.Forms.Panel();           
            this.rbQ1Age = new System.Windows.Forms.RadioButton();
            this.rbQ1Disability = new System.Windows.Forms.RadioButton();
            this.rbQ1ESRD = new System.Windows.Forms.RadioButton();
            this.pnlQ1RadioButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlQ1RadioButtons
            // 
            this.pnlQ1RadioButtons.Controls.Add(this.rbQ1Age);
            this.pnlQ1RadioButtons.Controls.Add(this.rbQ1Disability);
            this.pnlQ1RadioButtons.Controls.Add(this.rbQ1ESRD);
            this.pnlQ1RadioButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQ1RadioButtons.Name = "pnlQ1RadioButtons";
            this.pnlQ1RadioButtons.Size = new System.Drawing.Size(422, 35);
            this.pnlQ1RadioButtons.TabIndex = 2;            
            // 
            // rbQ1Age
            // 
            this.rbQ1Age.Location = new System.Drawing.Point(370, 6);
            this.rbQ1Age.Name = "rbQ1Age";
            this.rbQ1Age.Size = new System.Drawing.Size(45, 24);
            this.rbQ1Age.TabIndex = 4;
            this.rbQ1Age.Text = "Age";
            this.rbQ1Age.CheckedChanged +=new EventHandler(radio_CheckedChanged);
            // 
            // rbQ1Disability
            // 
            this.rbQ1Disability.Location = new System.Drawing.Point(284, 6);
            this.rbQ1Disability.Name = "rbQ1Disability";
            this.rbQ1Disability.Size = new System.Drawing.Size(69, 24);
            this.rbQ1Disability.TabIndex = 3;
            this.rbQ1Disability.Text = "Disability";
            this.rbQ1Disability.CheckedChanged +=new EventHandler(radio_CheckedChanged);
            // 
            // rbQ1ESRD
            // 
            this.rbQ1ESRD.Location = new System.Drawing.Point(76, 6);
            this.rbQ1ESRD.Name = "rbQ1ESRD";
            this.rbQ1ESRD.Size = new System.Drawing.Size(195, 24);
            this.rbQ1ESRD.TabIndex = 2;
            this.rbQ1ESRD.Text = "End-Stage Renal Disease (ESRD)";
            this.rbQ1ESRD.CheckedChanged +=new EventHandler(radio_CheckedChanged);
            // 
            // MedicareEntitlementGroup
            // 
            this.Controls.Add(this.pnlQ1RadioButtons);
            this.Name = "MedicareEntitlementGroup";
            this.Size = new System.Drawing.Size(422, 35);
            this.pnlQ1RadioButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public MedicareEntitlementGroup()
        {
            // This call is required by the Windows.Forms Form Designer.
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
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements

        private Container                                 components = null;

        public Panel                                       pnlQ1RadioButtons;

        public RadioButton                                 rbQ1Age;
        public RadioButton                                 rbQ1Disability;
        public RadioButton                                 rbQ1ESRD;

        #endregion

        #region Constants
        #endregion
	}
}
