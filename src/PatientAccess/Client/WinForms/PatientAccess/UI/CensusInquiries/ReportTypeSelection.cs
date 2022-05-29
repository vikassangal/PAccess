using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
	/// <summary>
	/// Summary description for ReportTypeSelection.
	/// </summary>
	public class ReportTypeSelection : TimeOutFormView
	{
        #region Events
        public event EventHandler ReportTypeSelected;
        #endregion

        #region EventHandlers
        
        private void ReportTypeSelection_Load(object sender, EventArgs e)
        {
            departmentRadioButton.Checked = true;
            continueButton.Focus();
        }
        private void continueButton_Click(object sender, EventArgs e)
        {
            this.Close();
            if( informationDeskRadioButton.Checked )
            {
                selection = INFO_DESK;
            }
            else
            {
                selection = DEPARTMENT;
            }
            if( ReportTypeSelected != null )
            {
                ReportTypeSelected( this, new LooseArgs( selection ) );
            }
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion      

        #region Construction and Finalization
		public ReportTypeSelection()
		{			
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.reportSelectionPanel = new System.Windows.Forms.Panel();
            this.reportTypeSelectionLabel = new System.Windows.Forms.Label();
            this.departmentRadioButton = new System.Windows.Forms.RadioButton();
            this.informationDeskRadioButton = new System.Windows.Forms.RadioButton();
            this.departmentLabel = new System.Windows.Forms.Label();
            this.InfoDeskLabel = new System.Windows.Forms.Label();
            this.continueButton = new LoggingButton();
            this.cancelButton = new LoggingButton();
            this.reportSelectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportSelectionPanel
            // 
            this.reportSelectionPanel.BackColor = System.Drawing.Color.White;
            this.reportSelectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.reportSelectionPanel.Controls.Add(this.reportTypeSelectionLabel);
            this.reportSelectionPanel.Controls.Add(this.departmentRadioButton);
            this.reportSelectionPanel.Controls.Add(this.informationDeskRadioButton);
            this.reportSelectionPanel.Controls.Add(this.departmentLabel);
            this.reportSelectionPanel.Controls.Add(this.InfoDeskLabel);
            this.reportSelectionPanel.Location = new System.Drawing.Point(7, 7);
            this.reportSelectionPanel.Name = "reportSelectionPanel";
            this.reportSelectionPanel.Size = new System.Drawing.Size(489, 193);
            this.reportSelectionPanel.TabIndex = 0;
            // 
            // reportTypeSelectionLabel
            // 
            this.reportTypeSelectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.reportTypeSelectionLabel.Location = new System.Drawing.Point(24, 19);
            this.reportTypeSelectionLabel.Name = "reportTypeSelectionLabel";
            this.reportTypeSelectionLabel.Size = new System.Drawing.Size(272, 23);
            this.reportTypeSelectionLabel.TabIndex = 0;
            this.reportTypeSelectionLabel.Text = "Select the type of report you wish to print:";
            this.reportTypeSelectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // departmentRadioButton
            // 
            this.departmentRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.departmentRadioButton.Location = new System.Drawing.Point(20, 44);
            this.departmentRadioButton.Name = "departmentRadioButton";
            this.departmentRadioButton.TabIndex = 0;
            this.departmentRadioButton.TabStop = true;
            this.departmentRadioButton.Text = "Department";
            // 
            // informationDeskRadioButton
            // 
            this.informationDeskRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.informationDeskRadioButton.Location = new System.Drawing.Point(20, 95);
            this.informationDeskRadioButton.Name = "informationDeskRadioButton";
            this.informationDeskRadioButton.Size = new System.Drawing.Size(128, 24);
            this.informationDeskRadioButton.TabIndex = 1;
            this.informationDeskRadioButton.TabStop = true;
            this.informationDeskRadioButton.Text = "Information Desk";
            // 
            // departmentLabel
            // 
            this.departmentLabel.Location = new System.Drawing.Point(32, 69);
            this.departmentLabel.Name = "departmentLabel";
            this.departmentLabel.Size = new System.Drawing.Size(256, 23);
            this.departmentLabel.TabIndex = 3;
            this.departmentLabel.Text = "A full version that displays all patient information.";
            // 
            // InfoDeskLabel
            // 
            this.InfoDeskLabel.Location = new System.Drawing.Point(32, 120);
            this.InfoDeskLabel.Name = "InfoDeskLabel";
            this.InfoDeskLabel.Size = new System.Drawing.Size(384, 32);
            this.InfoDeskLabel.TabIndex = 4;
            this.InfoDeskLabel.Text = "A limited version that displays only location, patient name, patient type, and at" +
                "tending physician information.";
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(304, 207);
            this.continueButton.Name = "continueButton";
            this.continueButton.TabIndex = 2;
            this.continueButton.Text = "Contin&ue";
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(384, 207);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ReportTypeSelection
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(504, 237);
            this.Controls.Add(this.reportSelectionPanel);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportTypeSelection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Report Type";
            this.Load += new System.EventHandler(this.ReportTypeSelection_Load);
            this.reportSelectionPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Private Properties
        #endregion

        #region Data Elements
        private string selection;
        private Panel reportSelectionPanel;
        private Label reportTypeSelectionLabel;
        private RadioButton departmentRadioButton;
        private RadioButton informationDeskRadioButton;
        private Label departmentLabel;
        private Label InfoDeskLabel;
        private LoggingButton continueButton;
        private LoggingButton cancelButton;		
        private Container components = null;
        #endregion

        #region Constants
        private const string DEPARTMENT = "Department",
            INFO_DESK = "Information Desk";
        #endregion

    }
}
