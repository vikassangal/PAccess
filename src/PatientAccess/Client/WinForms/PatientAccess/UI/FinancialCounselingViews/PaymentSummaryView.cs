using System;
using System.ComponentModel;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for PaymentSummaryView.
	/// </summary>
	//TODO: Create XML summary comment for PaymentSummaryView
	[Serializable]
	public class PaymentSummaryView : ControlView
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
			components = new System.ComponentModel.Container();
		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PaymentSummaryView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
		}

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
		#endregion

		#region Constants
		#endregion
	}
}
