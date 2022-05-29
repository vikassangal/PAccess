using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.WorklistViews
{
    [Serializable]
    public class IncompleteWorklistView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        protected override void Dispose( bool disposing )
        {
            Application.DoEvents();
            if( disposing )
            {
                if ( components != null ) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblIncompleteWorklist = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblIncompleteWorklist
            // 
            this.lblIncompleteWorklist.Location = new System.Drawing.Point(7, 12);
            this.lblIncompleteWorklist.Name = "lblIncompleteWorklist";
            this.lblIncompleteWorklist.Size = new System.Drawing.Size(576, 55);
            this.lblIncompleteWorklist.TabIndex = 0;
            this.lblIncompleteWorklist.Text = UIErrorMessages.DO_NOT_HAVE_PERMISSION;
            // 
            // IncompleteWorklistView
            // 
            this.Controls.Add(this.lblIncompleteWorklist);
            this.Name = "IncompleteWorklistView";
            this.Size = new System.Drawing.Size(594, 267);
            this.ResumeLayout(false);


        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public IncompleteWorklistView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label lblIncompleteWorklist;
        #endregion

        #region Constants
        #endregion
    }
}