using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.GuarantorViews
{
	/// <summary>
	/// Summary description for GuarantorValidationAlerts.
	/// </summary>
	//TODO: Create XML summary comment for GuarantorValidationAlerts
	[Serializable]
	public class GuarantorValidationTabs : ControlView
	{
		#region Event Handlers
		#endregion

		#region Methods
		public override void UpdateView()
		{
			this.guarantorValidationAlerts1.Model = this.Model.CreditReport;
			this.guarantorValidationAlerts1.UpdateView();
		}
                          
		/// <summary>
		/// Puts data into the Model from the controls on the view.
		/// </summary>
		public override void UpdateModel()
		{   
			
		}
		#endregion

		#region Properties

	    private new Guarantor Model
		{
			get
			{
				return (Guarantor)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tcValidationActions = new System.Windows.Forms.TabControl();
			this.tpAlerts = new System.Windows.Forms.TabPage();
			this.guarantorValidationAlerts1 = new PatientAccess.UI.GuarantorViews.GuarantorValidationAlerts();
			this.tcValidationActions.SuspendLayout();
			this.tpAlerts.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcValidationActions
			// 
			this.tcValidationActions.Controls.Add(this.tpAlerts);
			this.tcValidationActions.Location = new System.Drawing.Point(0, 0);
			this.tcValidationActions.Name = "tcValidationActions";
			this.tcValidationActions.SelectedIndex = 0;
			this.tcValidationActions.Size = new System.Drawing.Size(728, 408);
			this.tcValidationActions.TabIndex = 0;
			// 
			// tpAlerts
			// 
			this.tpAlerts.BackColor = System.Drawing.Color.White;
			this.tpAlerts.Controls.Add(this.guarantorValidationAlerts1);
			this.tpAlerts.Location = new System.Drawing.Point(4, 22);
			this.tpAlerts.Name = "tpAlerts";
			this.tpAlerts.Size = new System.Drawing.Size(720, 382);
			this.tpAlerts.TabIndex = 0;
			this.tpAlerts.Text = "Alerts";
			// 
			// guarantorValidationAlerts1
			// 
			this.guarantorValidationAlerts1.BackColor = System.Drawing.Color.White;
			this.guarantorValidationAlerts1.Location = new System.Drawing.Point(8, 8);
			this.guarantorValidationAlerts1.Model = null;
			this.guarantorValidationAlerts1.Name = "guarantorValidationAlerts1";
			this.guarantorValidationAlerts1.Size = new System.Drawing.Size(456, 360);
			this.guarantorValidationAlerts1.TabIndex = 0;
			// 
			// GuarantorValidationTabs
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.tcValidationActions);
			this.Name = "GuarantorValidationTabs";
			this.Size = new System.Drawing.Size(728, 408);
			this.tcValidationActions.ResumeLayout(false);
			this.tpAlerts.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Construction and Finalization
		public GuarantorValidationTabs()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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
		private Container components = null;
		private TabControl tcValidationActions;
		private TabPage tpAlerts;
		private GuarantorValidationAlerts guarantorValidationAlerts1;
		#endregion
	}
}
