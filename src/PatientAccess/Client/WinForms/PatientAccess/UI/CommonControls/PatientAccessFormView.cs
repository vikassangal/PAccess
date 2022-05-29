using System.ComponentModel;

namespace PatientAccess.UI.CommonControls 
{
	/// <summary>
	/// Summary description for PatientAccessFormView.
	/// </summary>
	public class PatientAccessFormView : TimeOutFormView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public PatientAccessFormView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PatientAccessFormView));
            // 
            // PatientAccessFormView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(292, 246);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PatientAccessFormView";
            this.Text = "PatientAccessFormView";

        }
		#endregion
	}
}
