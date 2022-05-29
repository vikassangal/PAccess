using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    [Serializable]
	public class CheckBoxYesNoGroup : UserControl
	{
        #region Events

        public event EventHandler RadioChanged;

        #endregion
        
        #region Event Handlers

        /// <summary>
        /// CheckBoxYesNoGroup_GotFocus - default to the Yes radio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxYesNoGroup_GotFocus(object sender, EventArgs e)
        {
            if(! this.rbYes.Checked && !this.rbNo.Checked)
            {
                this.rbYes.Checked = true;
                this.rbYes.Focus();
            }
        }

        /// <summary>
        /// Fire the changed event for the radio button 
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
            this.pnlGroup = new System.Windows.Forms.Panel();
            this.rbYes = new System.Windows.Forms.RadioButton();
            this.rbNo = new System.Windows.Forms.RadioButton();
            this.pnlGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlGroup
            // 
            this.pnlGroup.TabStop = true;
            this.pnlGroup.Controls.Add(this.rbNo);
            this.pnlGroup.Controls.Add(this.rbYes);
            this.pnlGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGroup.Location = new System.Drawing.Point(0, 0);
            this.pnlGroup.Name = "pnlGroup";
            this.pnlGroup.Size = new System.Drawing.Size(125, 35);
            this.pnlGroup.TabIndex = 1;            
            // 
            // rbYes
            // 
            this.TabStop = true;
            this.rbYes.Location = new System.Drawing.Point(7, 7);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(46, 24);
            this.rbYes.TabIndex = 1;
            this.rbYes.Text = "Yes";
            this.rbYes.CheckedChanged +=new EventHandler(radio_CheckedChanged);
            // 
            // rbNo
            // 
            this.TabStop = true;
            this.rbNo.Location = new System.Drawing.Point(69, 7);
            this.rbNo.Name = "rbNo";
            this.rbNo.Size = new System.Drawing.Size(46, 24);
            this.rbNo.TabIndex = 2;
            this.rbNo.Text = "No";
            this.rbNo.CheckedChanged +=new EventHandler(radio_CheckedChanged);
            // 
            // CheckBoxYesNoGroup
            // 
            this.GotFocus +=new EventHandler(CheckBoxYesNoGroup_GotFocus);
            this.Controls.Add(this.pnlGroup);
            this.Name = "CheckBoxYesNoGroup";
            this.Size = new System.Drawing.Size(125, 35);
            this.pnlGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.Enabled = true;
            this.TabStop = true;

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CheckBoxYesNoGroup()
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
        
        private Container             components = null;

        public Panel                   pnlGroup;

        public RadioButton             rbYes;
        public RadioButton             rbNo;

        #endregion

        #region Constants
        #endregion

    }
}
