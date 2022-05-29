using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for NonPurgedAccountDetails.
	/// </summary>
	public class NonPurgedShortAccountView : TimeOutFormView
	{
		#region Events

		private void closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Event Handler
        
		#endregion

		#region Methods
		
		public override void UpdateView()
		{
		    AccountDetails = Model as Account;
			if( AccountDetails != null )
			{
				if( AccountDetails.Patient != null )
				{
					nonPurgedAccountDetailsHeader1.Model = AccountDetails;
					nonPurgedAccountDetailsHeader1.UpdateView();
				}
				nonPurgedAccountDetails1.Model = AccountDetails;
				nonPurgedAccountDetails1.UpdateView();
			}
		}

		#endregion

		#region Methods
		#endregion

		#region Private Properties

		private Account AccountDetails
		{
			get
			{
				return i_Account;
			}
			set 
			{
				i_Account = value;
			}
		}
		#endregion

		#region Construction And Finalization

		public NonPurgedShortAccountView()
		{
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(NonPurgedShortAccountView));
            this.patientDetailsHeaderPanel = new System.Windows.Forms.Panel();
            this.nonPurgedAccountDetailsHeader1 = new PatientAccess.UI.HistoricalAccountViews.NonPurgedAccountDetailsHeader();
            this.nonPurgedAccountDetailsPanel = new System.Windows.Forms.Panel();
            this.nonPurgedAccountDetails1 = new PatientAccess.UI.HistoricalAccountViews.NonPurgedShortAccountDetails();
            this.closeButton = new LoggingButton();
            this.patientDetailsHeaderPanel.SuspendLayout();
            this.nonPurgedAccountDetailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patientDetailsHeaderPanel
            // 
            this.patientDetailsHeaderPanel.Controls.Add(this.nonPurgedAccountDetailsHeader1);
            this.patientDetailsHeaderPanel.Location = new System.Drawing.Point(7, 7);
            this.patientDetailsHeaderPanel.Name = "patientDetailsHeaderPanel";
            this.patientDetailsHeaderPanel.Size = new System.Drawing.Size(810, 53);
            this.patientDetailsHeaderPanel.TabIndex = 0;
            // 
            // nonPurgedAccountDetailsHeader1
            // 
            this.nonPurgedAccountDetailsHeader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nonPurgedAccountDetailsHeader1.Location = new System.Drawing.Point(0, 0);
            this.nonPurgedAccountDetailsHeader1.Model = null;
            this.nonPurgedAccountDetailsHeader1.Name = "nonPurgedAccountDetailsHeader1";
            this.nonPurgedAccountDetailsHeader1.Size = new System.Drawing.Size(810, 53);
            this.nonPurgedAccountDetailsHeader1.TabIndex = 0;
            // 
            // nonPurgedAccountDetailsPanel
            // 
            this.nonPurgedAccountDetailsPanel.AutoScroll = true;
            this.nonPurgedAccountDetailsPanel.Controls.Add(this.nonPurgedAccountDetails1);
            this.nonPurgedAccountDetailsPanel.Location = new System.Drawing.Point(8, 70);
            this.nonPurgedAccountDetailsPanel.Name = "nonPurgedAccountDetailsPanel";
            this.nonPurgedAccountDetailsPanel.Size = new System.Drawing.Size(810, 386);
            this.nonPurgedAccountDetailsPanel.TabIndex = 1;
            // 
            // nonPurgedAccountDetails1
            // 
            this.nonPurgedAccountDetails1.AutoScroll = true;
            this.nonPurgedAccountDetails1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nonPurgedAccountDetails1.Location = new System.Drawing.Point(0, 0);
            this.nonPurgedAccountDetails1.Model = null;
            this.nonPurgedAccountDetails1.Name = "nonPurgedAccountDetails1";
            this.nonPurgedAccountDetails1.Size = new System.Drawing.Size(810, 386);
            this.nonPurgedAccountDetails1.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(727, 470);
            this.closeButton.Name = "closeButton";
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "&Close";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // NonPurgedAccountView
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(830, 503);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.nonPurgedAccountDetailsPanel);
            this.Controls.Add(this.patientDetailsHeaderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NonPurgedShortAccountView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Account Details";
            this.patientDetailsHeaderPanel.ResumeLayout(false);
            this.nonPurgedAccountDetailsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		#region Data Elements
        
		private Container components = null;
		private Panel patientDetailsHeaderPanel;
		private NonPurgedAccountDetailsHeader nonPurgedAccountDetailsHeader1;
		private NonPurgedShortAccountDetails nonPurgedAccountDetails1;
		private LoggingButton closeButton;
		private Panel nonPurgedAccountDetailsPanel;

		private Account i_Account;

		#endregion

		#region Constants
        
		#endregion
	}
}
