using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for AssignPhysicianView.
	/// </summary>
	//TODO: Create XML summary comment for AssignPhysicianView
	[Serializable]
	public class AssignPhysicianView : ControlView
	{
		#region Event Handlers
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
			this.gbPhysicians = new System.Windows.Forms.GroupBox();
			this.lvPhysicians = new System.Windows.Forms.ListView();
			this.label1 = new System.Windows.Forms.Label();
			this.maskedEditTextBox1 = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.btnVerify = new LoggingButton();
			this.btnClearChecked = new LoggingButton();
			this.chChecked = new System.Windows.Forms.ColumnHeader();
			this.hcRel = new System.Windows.Forms.ColumnHeader();
			this.chNo = new System.Windows.Forms.ColumnHeader();
			this.chName = new System.Windows.Forms.ColumnHeader();
			this.chCopyFrom = new System.Windows.Forms.ColumnHeader();
			this.chActions = new System.Windows.Forms.ColumnHeader();
			this.gbPhysicians.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbPhysicians
			// 
			this.gbPhysicians.Controls.Add(this.lvPhysicians);
			this.gbPhysicians.Location = new System.Drawing.Point(4, 1);
			this.gbPhysicians.Name = "gbPhysicians";
			this.gbPhysicians.Size = new System.Drawing.Size(586, 164);
			this.gbPhysicians.TabIndex = 0;
			this.gbPhysicians.TabStop = false;
			this.gbPhysicians.Text = "Physicians";
			// 
			// lvPhysicians
			// 
			this.lvPhysicians.CheckBoxes = true;
			this.lvPhysicians.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.chChecked,
																						   this.hcRel,
																						   this.chNo,
																						   this.chName,
																						   this.chCopyFrom,
																						   this.chActions});
			this.lvPhysicians.GridLines = true;
			this.lvPhysicians.Location = new System.Drawing.Point(11, 20);
			this.lvPhysicians.Name = "lvPhysicians";
			this.lvPhysicians.Size = new System.Drawing.Size(564, 135);
			this.lvPhysicians.TabIndex = 0;
			this.lvPhysicians.View = System.Windows.Forms.View.Details;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 182);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Find checked by number:";
			// 
			// maskedEditTextBox1
			// 
			this.maskedEditTextBox1.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.maskedEditTextBox1.Location = new System.Drawing.Point(144, 178);
			this.maskedEditTextBox1.Mask = "";
			this.maskedEditTextBox1.Name = "maskedEditTextBox1";
			this.maskedEditTextBox1.Size = new System.Drawing.Size(55, 20);
			this.maskedEditTextBox1.TabIndex = 2;
			// 
			// btnVerify
			// 
			this.btnVerify.BackColor = System.Drawing.SystemColors.Control;
			this.btnVerify.Location = new System.Drawing.Point(213, 175);
			this.btnVerify.Name = "btnVerify";
			this.btnVerify.TabIndex = 3;
			this.btnVerify.Text = "Verify";
			// 
			// btnClearChecked
			// 
			this.btnClearChecked.BackColor = System.Drawing.SystemColors.Control;
			this.btnClearChecked.Location = new System.Drawing.Point(301, 175);
			this.btnClearChecked.Name = "btnClearChecked";
			this.btnClearChecked.TabIndex = 4;
			this.btnClearChecked.Text = "Clear checked";
			// 
			// chChecked
			// 
			this.chChecked.Text = "";
			this.chChecked.Width = 25;
			// 
			// hcRel
			// 
			this.hcRel.Text = "Rel";
			// 
			// chNo
			// 
			this.chNo.Text = "No.";
			// 
			// chName
			// 
			this.chName.Text = "Name";
			this.chName.Width = 250;
			// 
			// chCopyFrom
			// 
			this.chCopyFrom.Text = "Copy from";
			this.chCopyFrom.Width = 75;
			// 
			// chActions
			// 
			this.chActions.Text = "Actions";
			this.chActions.Width = 90;
			// 
			// AssignPhysicianView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.btnClearChecked);
			this.Controls.Add(this.btnVerify);
			this.Controls.Add(this.maskedEditTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gbPhysicians);
			this.Name = "AssignPhysicianView";
			this.Size = new System.Drawing.Size(595, 208);
			this.gbPhysicians.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public AssignPhysicianView()
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

		private GroupBox gbPhysicians;
		private ListView lvPhysicians;
		private Label label1;
		private MaskedEditTextBox maskedEditTextBox1;
		private LoggingButton btnVerify;
		private LoggingButton btnClearChecked;
		private ColumnHeader chChecked;
		private ColumnHeader hcRel;
		private ColumnHeader chNo;
		private ColumnHeader chName;
		private ColumnHeader chCopyFrom;
		private ColumnHeader chActions;

		#region Data Elements
		private Container components = null;
		#endregion

		#region Constants
		#endregion
	}
}
