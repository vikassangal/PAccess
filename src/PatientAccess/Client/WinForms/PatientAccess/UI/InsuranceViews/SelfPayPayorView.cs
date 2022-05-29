using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for SelfPayPayorView.
    /// </summary>
    public class SelfPayPayorView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
        }

        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            get
            {
                return (Coverage)this.Model;
            }
        }
        #endregion

        #region Private Methods

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.SuspendLayout();
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Plan Information";
            this.lineLabel.Location = new System.Drawing.Point(8, 1);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(864, 18);
            this.lineLabel.TabIndex = 3;
            this.lineLabel.TabStop = false;
            // 
            // SelfPayPayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lineLabel);
            this.Name = "SelfPayPayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public SelfPayPayorView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );
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
        private LineLabel   lineLabel;
        #endregion

        #region Constants
        #endregion
    }
}
