using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.FindBedViews
{
    /// <summary>
    /// Summary description for FindBedForm.
    /// </summary>
    public class FindBedForm : TimeOutFormView
    {       
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.findABedView = new PatientAccess.UI.FindBedViews.FindABedView();
            this.SuspendLayout();
            // 
            // findABedView
            // 
            this.findABedView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.findABedView.Cursor = System.Windows.Forms.Cursors.Default;
            this.findABedView.Location = new System.Drawing.Point(0, 0);
            this.findABedView.Model = null;
            this.findABedView.Name = "findABedView";
            this.findABedView.NewLocation = null;
            this.findABedView.Size = new System.Drawing.Size(905, 465);
            this.findABedView.TabIndex = 0;
            this.findABedView.CloseDialog += new System.EventHandler(this.findABedView_CloseDialog);
            // 
            // FindBedForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(905, 465);
            this.Controls.Add(this.findABedView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindBedForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Bed to Assign";
            this.ResumeLayout(false);

        }

        #endregion

        #region Events

        #endregion

        #region Event Handlers
              
        private void findABedView_CloseDialog( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region Methods
        
        public DialogResult ShowDialog( Account account )
        {
            findABedView.Model = account;
            return base.ShowDialog();
        }
        
        #endregion

        #region Properties

        public Location NewLocation
        {
            get
            {
                return findABedView.NewLocation;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public FindBedForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion
        
        # region Data Elements

        private Container components = null;
        private FindABedView findABedView;

        # endregion

        # region Constants

        # endregion

    }
}
