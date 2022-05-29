using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using log4net;

namespace PatientAccess.UI.FinancialCounselingViews.CreditReportViews
{
	/// <summary>
	/// Summary description for CreditReportView.
	/// </summary>
	//TODO: Create XML summary comment for CreditReportView
	[Serializable]
	public class CreditReportView : ControlView
	{
		#region Event Handlers
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			this.PopulateCreditReport();
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{   

		}
		#endregion

		#region Properties
		public new Guarantor Model
		{
			private get
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
		private void PopulateCreditReport()
		{
			this.lblSsn.Text = this.Model.SocialSecurityNumber.AsFormattedString();
			this.lblName.Text = this.Model.FormattedName;
			//this.tbCreditReport.Text = this.Model.CreditReport.Report;

            webBrowser1.Navigate("about:blank", null, null, null);

            // Wait for the control to be initialized and ready.
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete ||
                    webBrowser1.IsBusy)
            {
                Application.DoEvents();
            }

            HtmlDocument htmlDoc = webBrowser1.Document;

            htmlDoc.Write("<style type='text/css'>");
            htmlDoc.Write("body{font-family:Arial Narrow;font-size:smaller;}");
            htmlDoc.Write(".header{color:White;background-color:Gray;font-weight:bold;}");
            htmlDoc.Write(".section{cellspacing:0;cellpadding:0;width: 100%;border-color:Gray; border-width:1px; border-style:solid;}");
            htmlDoc.Write(".label{font-weight:bold;}");
            htmlDoc.Write(".subheading{background-color:Silver;border-color:Gray;border-style:solid;border-width:1px;}");
            htmlDoc.Write(".subiteml{border-color:Gray;border-bottom-style:solid;border-left-style:solid;border-width:1px;}");
            htmlDoc.Write(".subitemr{border-color:Gray;border-bottom-style:solid;border-right-style:solid;border-width:1px;}");
            htmlDoc.Write(".subitem{border-color:Gray;border-bottom-style:solid;border-width:1px;}");
            htmlDoc.Write("</style>");
            htmlDoc.Write("<body></body>");

            if( this.Model.CreditReport == null || this.Model.CreditReport.Report == "" )
            {
                if( this.Model.DataValidationTicket != null )
                {

                    IDataValidationBroker gBroker =
                        BrokerFactory.BrokerOfType<IDataValidationBroker>();

                    CreditValidationResponse response = gBroker.GetCreditValidationResponse(this.Model.DataValidationTicket.TicketId,
                        User.GetCurrent().SecurityUser.UPN, User.GetCurrent().Facility.Code );

                    if( response != null )
                    {
                        this.Model.CreditReport = response.ResponseCreditReport;
                    }
                }

            }

            if( this.Model.CreditReport != null )
            {
                htmlDoc.Body.InnerHtml = this.Model.CreditReport.Report;
            }
            else
            {
                htmlDoc.Body.InnerHtml = String.Empty;
            }
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CreditReportView));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblSsn = new System.Windows.Forms.Label();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 11);
            this.label1.TabIndex = 0;
            this.label1.Text = "Guarantor Name:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "SSN:";
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(111, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(302, 16);
            this.lblName.TabIndex = 0;
            // 
            // lblSsn
            // 
            this.lblSsn.Location = new System.Drawing.Point(47, 37);
            this.lblSsn.Name = "lblSsn";
            this.lblSsn.Size = new System.Drawing.Size(118, 13);
            this.lblSsn.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(10, 60);
            this.webBrowser1.Size = new System.Drawing.Size(858, 426);
            this.webBrowser1.TabIndex = 1;
            // 
            // CreditReportView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSsn);
            this.Name = "CreditReportView";
            this.Size = new System.Drawing.Size(876, 498);
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CreditReportView()
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

            try
            {
                this.webBrowser1.Dispose();
            }
            catch (Exception ex)
            {
                c_log.Error("Failed to Dispose WebBrowser control", ex);
            }
        }
		#endregion

		#region Data Elements
		private Container components = null;
		private Label label1;
		private Label label2;
		private Label lblName;
		private Label lblSsn;
        private WebBrowser webBrowser1;

        private static readonly ILog c_log = LogManager.GetLogger(typeof(CreditReportView));
        #endregion

		#region Constants
		#endregion
	}
}
